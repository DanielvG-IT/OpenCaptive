namespace OpenCaptive.Application.Auth;

public interface ITokenHasher
{
  string Hash(string value);
}