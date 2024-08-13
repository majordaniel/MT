
using MTMiddleware.Core.Helpers.Autofac;

namespace MTMiddleware.Core.ExternalServices.Interfaces
{
    public interface IMailerService2 : IAutoDependencyCore
    {
        Task SendUserInvitationMailAsync(ApplicationUser model);
    }
}
