
using MTMiddleware.Core.ExternalServices.Interfaces;
using MTMiddleware.Core.ViewModels;
using MTMiddleware.Data.Entities;
using MTMiddleware.Data.ViewModels;
using MTMiddleware.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary.Models;
using MTMiddleware.Core.Helpers.EmailSender.Interfaces;

namespace MTMiddleware.Core.ExternalServices
{
    public class MailerService : IMailerService
    {
        private readonly ILogger<MailerService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IEmailSenderService2 _emailSender;
        private readonly AppSettings _appSettings;

        public MailerService( ILogger<MailerService> logger, IOptions<AppSettings> appSettings, HttpClient client, IConfiguration config, IEmailSenderService2 emailSender)
        {
            _logger = logger;
            _httpClient = client;
            _config = config;
            _emailSender = emailSender;
            _appSettings = appSettings.Value;
        }

        public async Task SendAccountActivationMailAsync(ApplicationUser model, string code)
        {
            var link = $"{_appSettings.ClientURL}/reset/{code}";
        
            var MessageBody = $"Dear {model.FirstName}, <br><br>" +
                $"This is to inform you that your account has been created on the MTMiddleware portal.<br><br> " +
                $"Please <a href='{link}'>Click here</a> to setup your account.<br>" +
                $"Please contact us on +2347080626000-4 or email us on ccu@fbnquestmb.com if you require support.<br><br>" +
                $"Thank you.<br></br>" +
                $"<b><span lang='EN-GB' style='mso-ansi-language:EN-GB'>Customer Care</span><br>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'>Tel: +234(0) 708 062 6000-4  " +
                $"ccu@fbnquest.com" +
                $"</span><br />" +
                $"<b>FBNQuest Asset Management.</b>" +
                $"<p class='MsoNormal' style='margin-bottom:0in;margin-bottom:.0001pt'>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'><b>.<br>" +
                $"16-18, Keffi Street,<br>" +
                $"Off Awolowo Road,<br>" +
                $"S.W. Ikoyi, Lagos, Nigeria.<br>" +
                $"</span><a href='www.fbnquest.com'>www.fbnquest.com</a> </p>";

            var emailRequest = new EmailRequestViewModel()
            {
                MessageBody = MessageBody,
                Subject = "Account Creation on MTMiddleware Portal",
                To = model.Email,
                Entity = "AM"
            };

            var res = await _emailSender.SendEmailAsync(emailRequest);
        }

        public async Task SendRequestPasswordResetMailAsync(ApplicationUser model, string code)
        {
            var response = new Response<bool>();

            var link = $"{_appSettings.ClientURL}/reset-password/{code}";

            var MessageBody = $"Dear {model.FirstName}, <br><br>" +
                $"We recently received a request to reset your password.<br><br> " +
                $"Please <a href='{link}'>Click here</a> to finish resetting your password.<br>" +

                $"Please contact us on +2347080626000-4 or email us on ccu@fbnquestmb.com if you didn't request for a password reset.<br><br>" +
                $"Thank you.<br></br>" +
                $"<b><span lang='EN-GB' style='mso-ansi-language:EN-GB'>Customer Care</span><br>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'>Tel: +234(0) 708 062 6000-4  <br>" +
                $"ccu@fbnquest.com <br>" +
                $"<b>FBNQuest Asset Management.</b>" +
                $"16-18, Keffi Street,<br>" +
                $"Off Awolowo Road,<br>" +
                $"S.W. Ikoyi, Lagos, Nigeria.<br>" +
                $"</span><a href='www.fbnquest.com'>www.fbnquest.com</a>";

            var emailRequest = new EmailRequestViewModel()
            {
                MessageBody = MessageBody,
                Subject = "Password Reset on the MTMiddleware Portal",
                To = model.Email,
                Entity = "AM",
            };

            await _emailSender.SendEmailAsync(emailRequest);
        }

        public async Task SendSuccessfulPasswordResetMailAsync(ApplicationUser model)
        {
            var MessageBody = $"Dear {model.FirstName}, <br><br>" +
                $"Your Password was reset successfully.<br><br> " +
                $"Thank you.<br></br>" +
                $"<b><span lang='EN-GB' style='mso-ansi-language:EN-GB'>Customer Care</span><br>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'>Tel: +234(0) 708 062 6000-4  <br>" +
                $"ccu@fbnquest.com <br>" +
                $"<b>FBNQuest Asset Management.</b>" +
                $"16-18, Keffi Street,<br>" +
                $"Off Awolowo Road,<br>" +
                $"S.W. Ikoyi, Lagos, Nigeria.<br>" +
                $"</span><a href='www.fbnquest.com'>www.fbnquest.com</a>";

            var emailRequest = new EmailRequestViewModel()
            {
                MessageBody = MessageBody,
                Subject = "Password Reset on the MTMiddleware Portal",
                To = model.Email,
                Entity = "AM",
            };

            await _emailSender.SendEmailAsync(emailRequest);
        }


        public async Task SendUserInvitationToCustomerMailAsync(ApplicationUser model, string password)
        {
            var link = $"{_appSettings.ClientURL}/signin";

            var MessageBody = $"Dear {model.FirstName}, <br><br>" +
                $"This is to inform you that your account has been created on the MTMiddleware portal.<br><br> " +
                $"Please <a href='{link}'>Click here</a> to signin with your AD account.<br> with default password: {password}" +
                $"Please contact us on +2347080626000-4 or email us on ccu@fbnquestmb.com if you require support.<br><br>" +
                $"Thank you.<br></br>" +
                $"<b><span lang='EN-GB' style='mso-ansi-language:EN-GB'>Customer Care</span><br>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'>Tel: +234(0) 708 062 6000-4  " +
                $"ccu@fbnquest.com" +
                $"</span><br />" +
                $"<b>FBNQuest Asset Management.</b>" +
                $"<p class='MsoNormal' style='margin-bottom:0in;margin-bottom:.0001pt'>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'><b>.<br>" +
                $"16-18, Keffi Street,<br>" +
                $"Off Awolowo Road,<br>" +
                $"S.W. Ikoyi, Lagos, Nigeria.<br>" +
                $"</span><a href='www.fbnquest.com'>www.fbnquest.com</a> </p>";

            var emailRequest = new EmailRequestViewModel()
            {
                MessageBody = MessageBody,
                Subject = "Account Creation on MTMiddleware Portal",
                To = model.Email,
                Entity = "AM"
            };

            var res = await _emailSender.SendEmailAsync(emailRequest);
            //_emailSender.SendEmailRest(emailRequest.To,emailRequest.Subject, emailRequest.MessageBody);
            //var res = await _emailSender.SendEmailRest2(emailRequest.To, emailRequest.Subject, emailRequest.MessageBody);
        }
        public async Task SendUserInvitationMailAsync(ApplicationUser model)
        {
            var link = $"{_appSettings.ClientURL}/signin";

            var MessageBody = $"Dear {model.FirstName}, <br><br>" +
                $"This is to inform you that your account has been created on the MTMiddleware portal.<br><br> " +
                $"Please <a href='{link}'>Click here</a> to signin with your AD account.<br> with default password" +
                $"Please contact us on +2347080626000-4 or email us on ccu@fbnquestmb.com if you require support.<br><br>" +
                $"Thank you.<br></br>" +
                $"<b><span lang='EN-GB' style='mso-ansi-language:EN-GB'>Customer Care</span><br>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'>Tel: +234(0) 708 062 6000-4  " +
                $"ccu@fbnquest.com" +
                $"</span><br />" +
                $"<b>FBNQuest Asset Management.</b>" +
                $"<p class='MsoNormal' style='margin-bottom:0in;margin-bottom:.0001pt'>" +
                $"<span lang='EN-GB' style='mso-ansi-language:EN-GB'><b>.<br>" +
                $"16-18, Keffi Street,<br>" +
                $"Off Awolowo Road,<br>" +
                $"S.W. Ikoyi, Lagos, Nigeria.<br>" +
                $"</span><a href='www.fbnquest.com'>www.fbnquest.com</a> </p>";

            var emailRequest = new EmailRequestViewModel()
            {
                MessageBody = MessageBody,
                Subject = "Account Creation on MTMiddleware Portal",
                To = model.Email,
                Entity = "AM"
            };

            var res = await _emailSender.SendEmailAsync(emailRequest);
            //_emailSender.SendEmailRest(emailRequest.To,emailRequest.Subject, emailRequest.MessageBody);
            //var res = await _emailSender.SendEmailRest2(emailRequest.To, emailRequest.Subject, emailRequest.MessageBody);
        }
    }
}
