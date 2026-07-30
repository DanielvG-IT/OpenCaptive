using OpenCaptive.Application.Email.Models;

namespace OpenCaptive.Application.Email.Contracts;

public interface IEmailTemplateRenderer
{
  Task<TransactionalEmailBodies> RenderAsync<TModel>(EmailTemplate template, TModel model) where TModel : class;
}