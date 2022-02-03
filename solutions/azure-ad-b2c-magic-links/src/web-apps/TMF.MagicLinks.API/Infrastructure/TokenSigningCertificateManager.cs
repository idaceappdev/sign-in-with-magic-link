using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace TMF.MagicLinks.API.Infrastructure
{
    public class TokenSigningCertificateManager
    {
        private readonly IConfiguration _configuration;

        public TokenSigningCertificateManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public X509SigningCredentials GetSigningCredentials()
        {
            string SigningCertThumbprint = _configuration
                                           .GetSection("TokenSigningCertificateManagerConfiguration")["CertificateThumbprint"];
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                        X509FindType.FindByThumbprint,
                                        SigningCertThumbprint,
                                        false);
            if (certCollection.Count > 0)
            {
                return new X509SigningCredentials(certCollection[0]);
            }

            throw new Exception("Certificate not found");
        }
    }
}
