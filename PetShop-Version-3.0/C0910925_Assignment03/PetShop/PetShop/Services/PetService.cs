using System;
using System.Collections.Generic; // Added for IEnumerable
using System.Linq; // Added for LINQ methods
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

        // New method to return all pets
        public IEnumerable<Pet> GetAllPets()
        {
            // Fetch and return all pets from the database
            return _context.Pets.ToList(); // Fetch all pets and convert to a list
        }

        // New method to return a single pet by ID
        public Pet GetPetById(int id)
        {
            // Find and return a pet by its ID
            return _context.Pets.Find(id); // Uses Find to retrieve the pet by ID
        }

        // New method to return pets by breed
        public IEnumerable<Pet> GetPetsByBreed(string breed)
        {
            // Fetch and return all pets that match the specified breed
            return _context.Pets
                .Where(p => p.Breed.Equals(breed, StringComparison.OrdinalIgnoreCase))
                .ToList(); // Fetch and convert to list, ignoring case for breed comparison
        }
    }
}
