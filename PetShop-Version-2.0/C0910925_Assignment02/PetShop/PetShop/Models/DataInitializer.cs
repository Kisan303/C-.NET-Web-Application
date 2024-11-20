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
            // Create RoleManager and UserManager
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Admin Role Creation
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                roleManager.Create(role);
            }

            // Test Admin User Creation
            var adminUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                DateOfBirth = new DateTime(2000, 1, 1) // Ensure the user is over 18
            };

            string userPassword = "Pass@123"; // Use a strong password

            var existingUser = userManager.FindByName(adminUser.UserName);
            if (existingUser == null)
            {
                var result = userManager.Create(adminUser, userPassword);
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin"); // Assign Admin role
                }
            }

            // Create Sample Pets
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
                Owner = adminUser // Associate with the admin user
            };

            // Add Pets to the context
            context.Pets.Add(petWithoutOwner);
            context.Pets.Add(petWithOwner);

            // Save changes to the database
            context.SaveChanges();
        }
    }
}