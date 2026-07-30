using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OpenCaptive.Infrastructure.Common.Options;

public static class OptionsServiceCollectionExtensions
{
  public static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>(this IServiceCollection services, IConfiguration configuration) where TOptions : class, IOptionsSection
  {
    return services
      .AddOptions<TOptions>()
      .Bind(configuration.GetSection(TOptions.SectionName))
      .ValidateDataAnnotations()
      .ValidateOnStart();
  }
}
