using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class UsernameValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null)
        {
            var username = value.ToString();
            if (username != null)
            {
                var regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{2,19}$");

                if (!regex.IsMatch(username))
                {
                    return new ValidationResult("Username must be alphanumeric, between 3 and 20 characters, and cannot start with a number.");
                }
            }
        }

        return ValidationResult.Success;
    }
}
