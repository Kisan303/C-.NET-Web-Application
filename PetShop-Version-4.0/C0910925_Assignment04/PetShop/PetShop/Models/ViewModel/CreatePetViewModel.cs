using System.Collections.Generic;
using System.Web.Mvc;

namespace PetShop.Models.ViewModel
{
    public class CreatePetViewModel

    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Breed { get; set; }
        public bool isMale { get; set; }
        public bool isFixed { get; set; }

        public List<SelectListItem> DropDownBreed { get; set; } // For the dropdown
    }
}
