using System.Dynamic;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Application.Email.Models;
using RazorLight;

namespace OpenCaptive.Infrastructure.Email;

public sealed class RazorEmailTemplateRenderer(
    IRazorLightEngine razorEngine,
    IOptions<EmailOptions> emailOptions) : IEmailTemplateRenderer
{
  private readonly IRazorLightEngine _razorEngine = razorEngine;
  private readonly EmailBrandingOptions _branding = emailOptions.Value.Branding;

  public async Task<TransactionalEmailBodies> RenderAsync<TModel>(EmailTemplate template, TModel model) where TModel : class
  {
    ArgumentNullException.ThrowIfNull(model);

    var templateName = template.ToString();

    // Branding is shared by every email (header, footer, plain-text signature) and lives in
    // the ViewBag rather than each model, so individual templates only own their own content.
    var htmlBody = await _razorEngine.CompileRenderAsync($"{templateName}/Html.cshtml", model, CreateBrandingViewBag());
    var textBody = await _razorEngine.CompileRenderAsync($"{templateName}/Text.cshtml", model, CreateBrandingViewBag());

    return new TransactionalEmailBodies(htmlBody.Trim(), textBody.Trim());
  }

  private ExpandoObject CreateBrandingViewBag()
  {
    var viewBag = new ExpandoObject();
    var values = (IDictionary<string, object?>)viewBag;

    values["BrandName"] = _branding.BrandName;
    values["WebsiteUrl"] = _branding.WebsiteUrl;
    values["SupportUrl"] = _branding.SupportUrl;

    return viewBag;
  }
}
