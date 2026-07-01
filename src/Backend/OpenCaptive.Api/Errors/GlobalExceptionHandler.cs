using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpenCaptive.Api.Errors;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    // Serilog handles 'exception' as the first parameter to break down and index the stack trace properties
    logger.LogError(exception, "An unhandled exception occurred while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

    var problemDetails = new ProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = "Internal Server Error",
      Detail = "An unexpected error occurred on our end.",
      Instance = httpContext.Request.Path
    };

    // Returning true signals to the middleware pipeline that this exception is handled and formatting is complete.
    return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
    {
      HttpContext = httpContext,
      Exception = exception,
      ProblemDetails = problemDetails
    });
  }
}