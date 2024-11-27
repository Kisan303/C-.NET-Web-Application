using System;
using System.Collections.Generic;
using System.Linq;
using PetShop.Models;
using PetShop.Interfaces;

namespace PetShop.Services
{
    public class PetService : IPetService
    {
        private ApplicationDbContext _context;

        public PetService()
        {
            _context = new ApplicationDbContext(); // Initialize a new instance of ApplicationDbContext
        }

        public PetService(ApplicationDbContext context)
        {
            _context = context; // Set the private variable to the provided context
        }

        public bool OldEnoughToAdopt(DateTime dateOfBirth)
        {
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;
            return age >= 21; // Return true if age is 21 or older
        }

        public IEnumerable<Pet> GetAllPets()
        {
            return _context.Pets.ToList(); // Fetch all pets and convert to a list
        }

        public Pet GetPetById(int id)
        {
            return _context.Pets.Find(id); // Fetch and return a pet by its ID
        }

        public IEnumerable<Pet> GetPetsByBreed(string breed)
        {
            return _context.Pets
                .Where(p => p.Breed.Equals(breed, StringComparison.OrdinalIgnoreCase))
                .ToList(); // Fetch and convert to list, ignoring case for breed comparison
        }
    }
}
