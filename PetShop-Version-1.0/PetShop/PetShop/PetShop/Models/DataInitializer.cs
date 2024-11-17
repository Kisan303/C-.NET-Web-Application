using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PetShop.Models
{
    public class DataInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
       
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Admin Role
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                roleManager.Create(role);
            }

            //Test user
            var user = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            string userPWD = "Pass@123";

            var existingUser = userManager.FindByName(user.UserName);
            if (existingUser == null)
            {
                var result = userManager.Create(user, userPWD);
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            // Create-Pets
            var petWithoutOwner = new Pet
            {
                Name = "Buddy",
                isMale = true,
                Breed = "Golden Retriever"
            };

            var petWithOwner = new Pet
            {
                Name = "Charlie",
                isMale = false,
                Breed = "Beagle",
                Owner = user
            };

            context.Pets.Add(petWithoutOwner);
            context.Pets.Add(petWithOwner);

            context.SaveChanges();
        }
    }
}
