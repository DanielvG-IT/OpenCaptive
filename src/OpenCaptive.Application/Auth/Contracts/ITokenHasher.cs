namespace OpenCaptive.Application.Auth.Contracts;

public interface ITokenHasher
{
  string Hash(string value);
}
