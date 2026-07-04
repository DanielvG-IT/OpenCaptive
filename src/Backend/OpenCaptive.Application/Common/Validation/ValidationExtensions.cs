using FluentValidation;
using OpenCaptive.Domain.Common;

namespace OpenCaptive.Application.Common.Validation;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> MustBeValidSlug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder
        .Must(Slugs.CheckSlug)
        .WithMessage("Slug must be lowercase alphanumeric words separated by single hyphens.");
  }
}
