using OpenCaptive.Application.Email.Models;

namespace OpenCaptive.Application.Email.Contracts;

public interface IEmailTemplateRenderer
{
  Task<TransactionalEmailBodies> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default);
}