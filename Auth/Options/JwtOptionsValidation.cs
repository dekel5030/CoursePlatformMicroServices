using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Common.Auth.Options;

public class JwtOptionsValidation : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            options, context, results, validateAllProperties: true);

        if (isValid)
            return ValidateOptionsResult.Success;

        var errors = results.Select(r => r.ErrorMessage!).ToArray();
        return ValidateOptionsResult.Fail(errors);
    }
}
