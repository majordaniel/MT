
using MTMiddleware.Shared.Models;
using MTMiddleware.Core.Helpers.Autofac;

namespace MTMiddleware.Core.Helpers.EmailSender.Interfaces
{
    public interface IEmailSenderService : IAutoDependencyCore
    {
        Task<Response<bool>> SendEmailAsync(EmailSettings settings);
    }
}
