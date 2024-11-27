using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PetShop.Models;
using PetShop.Models.ViewModel;
using PetShop.Services;

using PetShop.Interfaces; 


namespace PetShop.Controllers
{
    public class PetController : Controller
    {
        private readonly IPetService _petService;

        // Constructor now receives an IPetService through dependency injection
        public PetController(IPetService petService)
        {
            _petService = petService; // Save the injected service to a private variable
        }

        // Example action that uses the injected PetService
        public ActionResult Index()
        {
            var pets = _petService.GetAllPets();  // Call the service method to get pets
            return View(pets);
        }

        // Other actions for Create, Edit, etc., which use _petService
    }
}


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

        // GET: Pets/Create
        public ActionResult Create()
        {
            var viewModel = new CreatePetViewModel
            {
                DropDownBreed = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Labrador", Value = "Labrador" },
                    new SelectListItem { Text = "Poodle", Value = "Poodle" },
                    new SelectListItem { Text = "Bulldog", Value = "Bulldog" },
                    // Add more breeds as needed
                }
            };
            return View(viewModel);
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,isMale,Breed,Age")] CreatePetViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var pet = new Pet
                {
                    Name = viewModel.Name,
                    isMale = viewModel.isMale,
                    Breed = viewModel.Breed,
                    Age = viewModel.Age,
                    OwnerId = null // Ensure the owner is null for new pets
                };

                db.Pets.Add(pet);
                db.SaveChanges();
                return Json(new { text = "Success" }); // Return success JSON
            }

            // If ModelState is invalid, repopulate the dropdown
            viewModel.DropDownBreed = new List<SelectListItem>
            {
                new SelectListItem { Text = "Labrador", Value = "Labrador" },
                new SelectListItem { Text = "Poodle", Value = "Poodle" },
                new SelectListItem { Text = "Bulldog", Value = "Bulldog" },
                // Add more breeds as needed
            };
            return Json(new { text = "Failed: Please correct the form." }); // Return failure JSON
        }

        // GET: Pets/Edit/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var pet = db.Pets.Find(id);
            if (pet == null) return HttpNotFound();

            var viewModel = new EditPetViewModel
            {
                ID = pet.ID,
                Name = pet.Name,
                isMale = pet.isMale,
                Age = pet.Age,
                Breed = pet.Breed, // Set the current breed as the default
                DropDownBreed = db.Pets.Select(p => p.Breed).Distinct().Select(b => new SelectListItem
                {
                    Text = b,
                    Value = b,
                    Selected = b == pet.Breed // Set the selected item to the current pet's breed
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,isMale,Breed,Age")] EditPetViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var pet = db.Pets.Find(viewModel.ID);
                if (pet == null) return HttpNotFound();

                pet.Name = viewModel.Name;
                pet.isMale = viewModel.isMale;
                pet.Breed = viewModel.Breed;
                pet.Age = viewModel.Age;

                db.Entry(pet).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { text = "Success" }); // Return success JSON
            }

            // Repopulate dropdown if the ModelState is invalid
            viewModel.DropDownBreed = db.Pets.Select(p => p.Breed).Distinct().Select(b => new SelectListItem
            {
                Text = b,
                Value = b,
                Selected = b == viewModel.Breed // Maintain the selected breed
            }).ToList();

            return Json(new { text = "Failed: Please correct the form." }); // Return failure JSON
        }

        // GET: Pets/Delete/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var pet = db.Pets.Find(id);
            if (pet == null) return HttpNotFound();
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
                return View(pet); 
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


        [HttpGet] // Specify that this action will respond to GET requests
        public ActionResult GetPetsByBreed(string breed)
        {
            PetService petService = new PetService(); // Create a new instance of PetService
            var petsByBreed = petService.GetPetsByBreed(breed); // Call the method to get pets by breed

            // Return the partial view with the list of pets filtered by breed
            return PartialView("_PetListPartial", petsByBreed);
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
