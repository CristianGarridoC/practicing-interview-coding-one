using System.Net;
using ApplicationCore.Responses;
using FluentResults;
using FluentValidation.Results;

namespace ApplicationCore.Extensions;

public static class ResultExtensions
{
    public static ErrorResponse FormatValidationErrors(
        this ValidationResult validationResult,
        HttpStatusCode statusCode
    )
    {
        return new ErrorResponse
        {
            Message = "One or more validation errors occurred.",
            Errors = validationResult.ToDictionary(),
            StatusCode = (int)statusCode
        };
    }

    public static ErrorResponse FormatResultFailed(this IEnumerable<IError> errors, HttpStatusCode statusCode)
    {
        return new ErrorResponse
        {
            Message = errors.ElementAt(0).Message,
            Errors = new Dictionary<string, string[]>(),
            StatusCode = (int)statusCode
        };
    }
}