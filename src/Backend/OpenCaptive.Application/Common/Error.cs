namespace OpenCaptive.Application.Common;

public enum ErrorType
{
  None = 0,
  Failure = 1,
  Validation = 2,
  NotFound = 3,
  Conflict = 4,
  Forbidden = 5,
  Unauthorized = 6,
}

public sealed record Error(string Code, string Description, ErrorType Type)
{
  public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

  public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);

  public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

  public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

  public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

  public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);

  public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);
}
