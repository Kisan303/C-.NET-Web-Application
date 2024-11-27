using System;
using System.Collections.Generic;
using PetShop.Models;

namespace PetShop.Interfaces
{
    public interface IPetService
    {
        bool OldEnoughToAdopt(DateTime dateOfBirth); // Method to check if a pet's age is above 21

        IEnumerable<Pet> GetAllPets(); // Method to get all pets

        Pet GetPetById(int id); // Method to get a pet by its ID

        IEnumerable<Pet> GetPetsByBreed(string breed); // Method to get pets by breed
    }
}
