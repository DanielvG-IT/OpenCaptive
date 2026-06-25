using System.Security.Cryptography;
using System.Text;
using OpenCaptive.Application.Auth.Contracts;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class Sha256TokenHasher : ITokenHasher
{
  public string Hash(string value)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(value);

    var bytes = Encoding.UTF8.GetBytes(value);
    var hash = SHA256.HashData(bytes);

    return Convert.ToHexString(hash);
  }
}