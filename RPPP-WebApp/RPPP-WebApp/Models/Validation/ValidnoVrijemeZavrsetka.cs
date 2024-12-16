using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models.Validation
{
    public class ValidnoVrijemeZavrsetka : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var predavanje = validationContext.ObjectInstance as Predavanje;

            if (predavanje == null)
            {
                return new ValidationResult("Invalid object.");
            }

            if (predavanje.VrijemeZavrsetka <= predavanje.VrijemePocetka)
            {
                return new ValidationResult(ErrorMessage ?? "Vrijeme završetka mora biti nakon vremena početka.");
            }

            return ValidationResult.Success;
        }
    }
}