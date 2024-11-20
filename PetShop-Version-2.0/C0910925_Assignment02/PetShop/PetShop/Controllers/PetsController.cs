using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PetShop.Models;

namespace PetShop.Controllers
{
    [Authorize] // Require authentication for all actions
    public class PetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pets
        [AllowAnonymous] // Allow anonymous users to access the Index action
        public ActionResult Index()
        {
            var unadoptedPets = db.Pets.Where(p => p.OwnerId == null).ToList();
            return View(unadoptedPets);
        }

        // [Authorize(Roles = "Admin")] // Only admins can access Create action
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,isMale,Breed,Age")] Pet pet) // Include Age here
        {
            if (ModelState.IsValid)
            {
                db.Pets.Add(pet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var pet = db.Pets.Find(id);
            if (pet == null) return HttpNotFound();
            return View(pet);
        }

        // POST: Pets/Edit/5
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,isMale,Breed,Age")] Pet pet) // Include Age here
        {
            if (ModelState.IsValid)
            {
                db.Entry(pet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        // [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var pet = db.Pets.Find(id);
            if (pet == null) return HttpNotFound();
            return View(pet);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        // [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pet pet = db.Pets.Find(id);
            db.Pets.Remove(pet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult MyPets()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User ID is null.");
            }

            var myPets = db.Pets.Where(p => p.OwnerId == userId).ToList();

            if (!myPets.Any())
            {
                ViewBag.Message = "You do not own any pets.";
            }

            return View(myPets);
        }

        [Authorize]
        public ActionResult Adopt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            if (pet.OwnerId == userId)
            {
                return RedirectToAction("MyPets");
            }

            return View(pet);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Adopt(int id)
        {
            var pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound("Pet not found.");
            }

            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                ModelState.AddModelError("", "You need to be logged in to adopt a pet.");
                return View(pet); // Rerender the pet view with error message
            }

            // Check if the user is old enough
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(userId);
            var dateOfBirthClaim = user.Claims.FirstOrDefault(c => c.ClaimType == "DateOfBirth")?.ClaimValue;

            if (string.IsNullOrEmpty(dateOfBirthClaim) || !DateTime.TryParse(dateOfBirthClaim, out DateTime dateOfBirth))
            {
                ModelState.AddModelError("", "Please provide your date of birth to adopt a pet.");
                return View(pet); // Show the error on the adoption page
            }

            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Now.AddYears(-age)) age--;

            if (age < 18)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Only users aged 18 or older can adopt a pet.");
            }

            // Assign the owner
            pet.OwnerId = userId;
            db.Entry(pet).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = $"Congratulations! You've adopted {pet.Name}.";
            return RedirectToAction("MyPets");
        }

        // GET: Pets/Details/5
        [Authorize] // Optional: Require authentication
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
