namespace Kernel;

public sealed record ValidationError : Error
{
    public ValidationError(IEnumerable<Error> errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public IEnumerable<Error> Errors { get; }
}
