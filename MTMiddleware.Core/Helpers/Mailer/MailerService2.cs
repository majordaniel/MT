
using AutoMapper.Configuration;
using Microsoft.Extensions.Options;
using MTMiddleware.Core.Helpers.EmailSender.Interfaces;
using MTMiddleware.Shared.Models;
using MTMiddleware.Core.ExternalServices.Interfaces;

namespace MTMiddleware.Core.ExternalServices
{
    public class MailerService2 : IMailerService2
    {
        private readonly ILogger<MailerService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IEmailSenderService _emailSender;
        private readonly AppSettings _appSettings;

        public MailerService2( ILogger<MailerService> logger, IOptions<AppSettings> appSettings, HttpClient client, IConfiguration config, IEmailSenderService emailSender)
        {
            _logger = logger;
            _httpClient = client;
            _config = config;
            _emailSender = emailSender;
            _appSettings = appSettings.Value;
        }
        public async Task SendUserInvitationMailAsync(ApplicationUser model)
        {
            var link = $"{_appSettings.ClientURL}/signin";

            string username = !string.IsNullOrEmpty(model.DisplayName) ? model.DisplayName : "User";

            var MessageBody = $"Dear {username}, <br><br>" +
                $"This is to inform you that your account has been created on the Virtual Account portal.<br><br> " +
                $"Please <a href='{link}'>Click here</a> to signin with your AD account.<br>" +
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

            var emailRequest = new EmailSettings()
            {
                MessageBody = MessageBody,
                Subject = "Account Creation on Virual Account Portal",
                To = model.Email,
                Entity = "AM"
            };

            var res = await _emailSender.SendEmailAsync(emailRequest);
        }
    }
}
