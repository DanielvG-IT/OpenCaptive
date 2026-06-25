using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class EmailVerificationTokenProviderOptions : DataProtectionTokenProviderOptions;

public sealed class EmailVerificationTokenProvider<TUser>(
  IDataProtectionProvider dataProtectionProvider,
  IOptions<EmailVerificationTokenProviderOptions> options,
  ILogger<DataProtectorTokenProvider<TUser>> logger)
  : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger)
  where TUser : class;

public sealed class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions;

public sealed class PasswordResetTokenProvider<TUser>(
  IDataProtectionProvider dataProtectionProvider,
  IOptions<PasswordResetTokenProviderOptions> options,
  ILogger<DataProtectorTokenProvider<TUser>> logger)
  : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger)
  where TUser : class;
