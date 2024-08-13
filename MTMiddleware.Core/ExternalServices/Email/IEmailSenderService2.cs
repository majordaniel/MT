
using MTMiddleware.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Core.ExternalServices.Interfaces
{
    public interface IEmailSenderService2
    {
        Task<bool> SendEmailAsync(EmailRequestViewModel request);

    }
}
