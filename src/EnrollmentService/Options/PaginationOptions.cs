using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Options;

public class PaginationOptions
{
    public const string SectionName = "Pagination";

    [Range(1, 1000, ErrorMessage = $"{nameof(PaginationOptions)}: DefaultPageSize must be between 1 and 1000.")]
    public int DefaultPageSize { get; set; } = 10;

    [Range(1, 1000, ErrorMessage = $"{nameof(PaginationOptions)}: MaxPageSize must be between 1 and 1000.")]
    public int MaxPageSize { get; set; } = 100;
}
