using Microsoft.Extensions.Logging;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Application.Email.Models;
using RazorLight;

namespace OpenCaptive.Infrastructure.Email.Rendering;

public sealed class RazorEmailTemplateRenderer(IRazorLightEngine razorEngine, ILogger<RazorEmailTemplateRenderer> logger) : IEmailTemplateRenderer
{
  private readonly IRazorLightEngine _razorEngine = razorEngine;
  private readonly ILogger<RazorEmailTemplateRenderer> _logger = logger;

  public async Task<TransactionalEmailBodies> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(templateName);
    ArgumentNullException.ThrowIfNull(model);

    try
    {

      var htmlTemplateKey = templateName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? templateName : $"{templateName}.cshtml";
      var htmlBody = await _razorEngine.CompileRenderAsync(htmlTemplateKey, model);

      var textTemplateKey = $"{htmlTemplateKey[..^".cshtml".Length]}.txt.cshtml";
      var textBody = HasEmbeddedTemplate(textTemplateKey) ? await _razorEngine.CompileRenderAsync(textTemplateKey, model) : null;

      return new TransactionalEmailBodies(htmlBody, textBody);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to compile or render Razor template: {TemplateName}", templateName);
      throw;
    }
  }

  private static bool HasEmbeddedTemplate(string templateKey)
  {
    const string resourceRoot = "OpenCaptive.Infrastructure.Email.Rendering.Templates.";
    var resourceName = $"{resourceRoot}{templateKey.Replace('/', '.').Replace('\\', '.')}";

    return typeof(RazorEmailTemplateRenderer).Assembly.GetManifestResourceNames().Contains(resourceName, StringComparer.Ordinal);
  }
}