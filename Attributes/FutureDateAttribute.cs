using System.ComponentModel.DataAnnotations;

namespace NeatPath.Attributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            if (date < DateTime.UtcNow)
            {
                return new ValidationResult("Date must be in the future");
            }
            return ValidationResult.Success;
        }
    }
}