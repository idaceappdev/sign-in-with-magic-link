using System.IdentityModel.Tokens.Jwt;

namespace TMF.MagicLinks.API.Infrastructure
{
    public class IdTokenHintBuilder
    {
        private readonly IConfiguration _configuration;
        private readonly TokenSigningCertificateManager _tokenSigningCertificateManager;

        public IdTokenHintBuilder(IConfiguration configuration,
                                  TokenSigningCertificateManager tokenSigningCertificateManager)
        {
            _configuration = configuration;
            _tokenSigningCertificateManager = tokenSigningCertificateManager;
        }

        public string BuildIdToken(string userEmail)
        {
            string audience = _configuration
                              .GetSection("IdTokenHintBuilderConfiguration")["Audience"];
            double.TryParse(_configuration
                            .GetSection("IdTokenHintBuilderConfiguration")["TokenLifeTimeInMinutes"],
                                        out double tokenExpirationTime);

            string issuer = _configuration
                            .GetSection("IdTokenHintBuilderConfiguration")["Issuer"];

            // All parameters send to Azure AD B2C needs to be sent as claims:
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("email", userEmail, System.Security.Claims.ClaimValueTypes.String, issuer),
            };

            var signingCredential = _tokenSigningCertificateManager.GetSigningCredentials();

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(tokenExpirationTime),
                    signingCredential);

            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            return jwtHandler.WriteToken(token);
        }
    }
}
