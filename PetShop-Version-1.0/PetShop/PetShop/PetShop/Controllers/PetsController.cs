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
    public class PetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pets
        public ActionResult Index()
        {
            var unadoptedPets = db.Pets.Where(p => p.Owner == null).ToList();
            return View(unadoptedPets);
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

        // GET: Pets/Adopt/5
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


        // POST: Pets/Adopt
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Adopt(int id)
        {
            var pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(userId);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var dateOfBirthClaim = user.Claims.FirstOrDefault(c => c.ClaimType == "DateOfBirth")?.ClaimValue;
            if (string.IsNullOrEmpty(dateOfBirthClaim) || !DateTime.TryParse(dateOfBirthClaim, out DateTime dateOfBirth))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Date of birth claim is missing or invalid.");
            }

            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Now.AddYears(-age)) age--;

            if (age < 18)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Only 18 years old or above older people can adopt a pet.");
            }

            if (pet.OwnerId == userId)
            {
                return RedirectToAction("MyPets");
            }

            pet.OwnerId = userId;
            db.Entry(pet).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MyPets");
        }

        // GET: Pets/Details/5
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

        // GET: Pets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,isMale,Breed")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                db.Pets.Add(pet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pet);
        }

        // GET: Pets/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,isMale,Breed")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        // GET: Pets/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pet pet = db.Pets.Find(id);
            db.Pets.Remove(pet);
            db.SaveChanges();
            return RedirectToAction("Index");
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
