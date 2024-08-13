
using MTMiddleware.Core.ViewModels;
using MTMiddleware.Data.Entities;
using MTMiddleware.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Interfaces
{
    public interface IMailerService
    {
        Task SendUserInvitationMailAsync(ApplicationUser model);
        Task SendUserInvitationToCustomerMailAsync(ApplicationUser model, string password);
        Task SendAccountActivationMailAsync(ApplicationUser model, string code);
        Task SendRequestPasswordResetMailAsync(ApplicationUser model, string code);
        Task SendSuccessfulPasswordResetMailAsync(ApplicationUser model);
    }
}
