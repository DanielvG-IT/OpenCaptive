using Microsoft.AspNetCore.Http.HttpResults;
using OpenCaptive.Application.Common;

namespace OpenCaptive.Api.Extensions;

public static class ErrorExtensions
{
  public static ProblemHttpResult ToProblem(this Error error) =>
    TypedResults.Problem(
      detail: error.Description,
      statusCode: error.Type.ToStatusCode(),
      title: error.Code
    );

  private static int ToStatusCode(this ErrorType type) => type switch
  {
    ErrorType.Validation => StatusCodes.Status400BadRequest,
    ErrorType.NotFound => StatusCodes.Status404NotFound,
    ErrorType.Conflict => StatusCodes.Status409Conflict,
    ErrorType.Forbidden => StatusCodes.Status403Forbidden,
    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
    ErrorType.Failure => StatusCodes.Status500InternalServerError,
    _ => StatusCodes.Status500InternalServerError,
  };
}
