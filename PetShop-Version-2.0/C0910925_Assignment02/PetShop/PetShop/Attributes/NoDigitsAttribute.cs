using System;
using System.ComponentModel.DataAnnotations;

namespace PetShop.Attributes
{
    public class NoDigitsAttribute : ValidationAttribute
    {
        public NoDigitsAttribute() : base("No numbers allowed!")
        {
        }

        // Change 'protected' to 'public'
        public override bool IsValid(object value)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue) && stringValue.IndexOfAny("0123456789".ToCharArray()) == -1; // Return true if the string has no digits
            }
            return false; // Return false if the value is not a string
        }
    }
}
