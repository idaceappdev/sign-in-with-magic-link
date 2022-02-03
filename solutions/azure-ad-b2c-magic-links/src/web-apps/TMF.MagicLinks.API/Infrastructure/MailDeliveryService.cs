using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using TMF.MagicLinks.API.DTO;

namespace TMF.MagicLinks.API.Infrastructure
{
    public class MailDeliveryService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;

        public MailDeliveryService(ISendGridClient sendGridClient,
                                   IConfiguration configuration)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
        }

        public async Task SendInvitationMessageAsync(EmailContentWithMagicLink userMailInvitation)
        {
            string fromEmailAddress = _configuration
                  .GetSection("SendGridConfiguration")["FromEmail"];
            string toEmailAddress = userMailInvitation.ToEmail;
            string mailTemplateId = _configuration
                  .GetSection("SendGridConfiguration")["MailTemplateId"];



            var emailMessage = MailHelper.CreateSingleTemplateEmail(
                 new EmailAddress(fromEmailAddress),
                 new EmailAddress(toEmailAddress),
                 mailTemplateId,
                 new
                 {
                     magicLink = userMailInvitation.LoginMagicLink
                 });

            var response = await _sendGridClient.SendEmailAsync(emailMessage);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var responseContent = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid service returned status code {response.StatusCode}" +
                                    $" with response: {responseContent}");
            }
        }
    }
}
