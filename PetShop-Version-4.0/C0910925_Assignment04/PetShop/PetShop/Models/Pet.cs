using PetShop.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop.Models
{
    public class Pet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool isMale { get; set; }
        public string Breed { get; set; }

        [NonNegative] // Add NonNegativeAttribute to Age
        public int Age { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }
    }
}