using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TMF.MagicLinks.WebApp.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class MagicLinksController : Controller
    {
        private readonly IConfiguration _configuration;
        public MagicLinksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("signin-oidc-link")]
        public IActionResult SignInLink([FromQuery] string id_token_hint)
        {
            var magic_link_auth = new AuthenticationProperties { RedirectUri = "/" };
            magic_link_auth.Items.Add("id_token_hint", id_token_hint);

            string magic_link_policy = _configuration.GetSection("AzureAdB2CConfiguration")["MagicLinksPolicyId"];
            return Challenge(magic_link_auth, magic_link_policy);
        }
    }
}
