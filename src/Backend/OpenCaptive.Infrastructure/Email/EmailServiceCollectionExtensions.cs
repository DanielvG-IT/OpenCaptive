using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Infrastructure.Email.Rendering;
using RazorLight;

namespace OpenCaptive.Infrastructure.Email;

public static class EmailServiceCollectionExtensions
{
  public static IServiceCollection AddOpenCaptiveEmail(this IServiceCollection services, IConfiguration configuration)
  {
    services
        .AddOptions<EmailOptions>()
        .Bind(configuration.GetSection(EmailOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services.AddSingleton<IRazorLightEngine>(_ =>
    {
      var assembly =
              typeof(RazorEmailTemplateRenderer).Assembly;

      return new RazorLightEngineBuilder()
              .UseEmbeddedResourcesProject(assembly, "OpenCaptive.Infrastructure.Email.Rendering.Templates")
              .UseMemoryCachingProvider()
              .Build();
    });

    services.AddScoped<ITransactionalEmailProvider, SmtpTransactionalEmailProvider>();
    services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();

    return services;
  }
}