using System.Net.Http.Json;
using Common.Errors;
using Common.Web.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ProblemDetailsFactory problemDetailsFactory, string? instance = null)
    {
        if (result.IsSuccess)
            throw new ArgumentException("Result must be a failure to convert to IActionResult.", nameof(result));

        var problem = problemDetailsFactory.Create(result.Error!, instance);

        return new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };
    }

    public static ProblemDetails ToProblemDetails<T>(
        this Result<T> result,
        ProblemDetailsFactory factory,
        string? instance = null)
    {
        return factory.Create(result.Error!, instance);
    }

    public static async Task<Result<T>> ToResultAsync<T>(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var value = await response.Content.ReadFromJsonAsync<T>();
            return Result<T>.Success(value!);
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        var error = new Error(
            MessageKey: problem?.Title ?? "Unexpected",
            HttpStatus: (int)response.StatusCode,
            IsPublic: true);

        return Result<T>.Failure(error);
    }
}
