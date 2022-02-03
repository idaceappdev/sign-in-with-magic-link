using TMF.MagicLinks.API.DTO;

namespace TMF.MagicLinks.API.Infrastructure
{
    public class UserMagicLinkInvitationHandler
    {
        private readonly IdTokenHintBuilder _idTokenHintBuilder;
        private readonly MailDeliveryService _mailDeliveryService;
        private readonly IConfiguration _configuration;

        public UserMagicLinkInvitationHandler(IdTokenHintBuilder idTokenHintBuilder,
                                          MailDeliveryService mailDeliveryService,
                                          IConfiguration configuration)
        {
            _idTokenHintBuilder = idTokenHintBuilder;
            _mailDeliveryService = mailDeliveryService;
            _configuration = configuration;
        }

        public async Task SendEmailWithMagicLinkAsync(UserInputClaimsForMagicLink userInputClaimsForMagicLink)
        {
            var idTokenHint = _idTokenHintBuilder.BuildIdToken(userInputClaimsForMagicLink.Email);

            var redirectUrl = _configuration
                   .GetSection("UserMagicLinkInvitationConfiguration")["B2CRedirectUri"];

            var magicLinkLoginUrl = $"{redirectUrl}?id_token_hint={idTokenHint}";

            var emailContentWithMagicLink = new EmailContentWithMagicLink
            {
                LoginMagicLink = magicLinkLoginUrl,
                ToEmail = userInputClaimsForMagicLink.Email
            };

            await _mailDeliveryService.SendInvitationMessageAsync(emailContentWithMagicLink);
        }




        //IMPORTANT! This method will generate test magic link to redirect to jwt.ms site:
        public async Task SendEmailWithTestMagicLinkAsync(UserInputClaimsForMagicLink userInputClaimsForMagicLink)
        {
            var idTokenHint = _idTokenHintBuilder.BuildIdToken(userInputClaimsForMagicLink.Email);
            string nonce = Guid.NewGuid().ToString("n");

            var signInUrl = _configuration
                   .GetSection("UserMagicLinkInvitationConfiguration")["B2CSignInWithMagicLinkUrl"];
            var tenantName = _configuration
                   .GetSection("UserMagicLinkInvitationConfiguration")["B2CTenant"];
            var signInWithMagicLinkPolicy = _configuration
                   .GetSection("UserMagicLinkInvitationConfiguration")["B2CSignInWithMagicLinkPolicy"];
            var appClientId = _configuration
                   .GetSection("UserMagicLinkInvitationConfiguration")["B2CClientId"];
            var redirectUrl = "https://jwt.ms";

            var magicLinkLoginUrl = string.Format(signInUrl, tenantName,
                    signInWithMagicLinkPolicy,
                    appClientId,
                    Uri.EscapeDataString(redirectUrl),
                    nonce) + "&id_token_hint=" + idTokenHint;


            var emailContentWithMagicLink = new EmailContentWithMagicLink
            {
                LoginMagicLink = magicLinkLoginUrl,
                ToEmail = userInputClaimsForMagicLink.Email
            };

            await _mailDeliveryService.SendInvitationMessageAsync(emailContentWithMagicLink);
        }
    }
}
