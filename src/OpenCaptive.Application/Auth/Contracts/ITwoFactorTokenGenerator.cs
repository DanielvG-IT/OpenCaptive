namespace OpenCaptive.Application.Auth.Contracts;

public interface ITwoFactorTokenGenerator
{
  string Generate(Guid userId);
  Task<Guid?> TryGetUserId(string token);
}