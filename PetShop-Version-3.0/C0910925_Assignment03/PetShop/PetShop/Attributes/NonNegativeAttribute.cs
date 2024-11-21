using System;
using System.ComponentModel.DataAnnotations;

namespace PetShop.Attributes
{
    public class NonNegativeAttribute : ValidationAttribute
    {
        public NonNegativeAttribute() : base("Number cannot be negative")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int intValue)
            {
                if (intValue < 0)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
