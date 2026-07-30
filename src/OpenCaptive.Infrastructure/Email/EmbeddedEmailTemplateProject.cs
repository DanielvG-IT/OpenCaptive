using System.Reflection;
using RazorLight.Razor;

namespace OpenCaptive.Infrastructure.Email;

/// <summary>
/// Embedded RazorLight project for the email templates.
/// <para>
/// Templates address each other with conventional path separators (for example
/// <c>Layout = "Shared/_Layout"</c> and <c>IncludeAsync("Shared/_Button")</c>), but embedded
/// resource names are dot-delimited. This project normalises the separator so templates can use
/// readable, idiomatic keys while still resolving to the correct manifest resource.
/// </para>
/// </summary>
public sealed class EmbeddedEmailTemplateProject(Assembly assembly)
    : EmbeddedRazorProject(assembly, TemplatesRootNamespace)
{
  public const string TemplatesRootNamespace = "OpenCaptive.Infrastructure.Email.Templates";

  public override Task<RazorLightProjectItem> GetItemAsync(string templateKey)
      => base.GetItemAsync(Normalize(templateKey));

  public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
      => base.GetImportsAsync(Normalize(templateKey));

  private static string Normalize(string templateKey) => templateKey.Replace('/', '.');
}
