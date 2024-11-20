using System;
using PetShop.Models;

namespace PetShop.Services
{
    public class PetService
    {
        private ApplicationDbContext _context;

        // Constructor with no parameters
        public PetService()
        {
            _context = new ApplicationDbContext(); // Initialize a new instance of ApplicationDbContext
        }

        // Constructor that accepts an ApplicationDbContext parameter
        public PetService(ApplicationDbContext context)
        {
            _context = context; // Set the private variable to the provided context
        }

        // Function to determine if a DateTime is over 21 years old
        public bool OldEnoughToAdopt(DateTime dateOfBirth)
        {
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;
            return age >= 21; // Return true if age is 21 or older
        }
    }
}
