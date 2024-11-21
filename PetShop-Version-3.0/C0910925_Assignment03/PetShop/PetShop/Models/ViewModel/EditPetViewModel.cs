using System.Collections.Generic;
using System.Web.Mvc;

namespace PetShop.Models.ViewModel
{
    public class EditPetViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool isMale { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public List<SelectListItem> DropDownBreed { get; set; }
    }
}
