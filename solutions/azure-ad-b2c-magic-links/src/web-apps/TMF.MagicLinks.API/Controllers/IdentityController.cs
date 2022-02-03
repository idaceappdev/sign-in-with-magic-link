using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using TMF.MagicLinks.API.DTO;
using TMF.MagicLinks.API.Infrastructure;

namespace TMF.MagicLinks.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly UserMagicLinkInvitationHandler _userMagicLinkInvitationHandler;

        public IdentityController(UserMagicLinkInvitationHandler userMagicLinkInvitationHandler)
        {
            _userMagicLinkInvitationHandler = userMagicLinkInvitationHandler;
        }

        [HttpPost(Name = "invite")]
        public async Task<IActionResult> PostAsync()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new AzureADB2CResponse("Request content is null",
                                                                                       HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new AzureADB2CResponse("Request content is empty",
                                                                                       HttpStatusCode.Conflict));
            }

            // Convert the input string into InputClaimsModel object
            UserInputClaimsForMagicLink userInputClaimsForMagicLink =
                                                JsonSerializer.Deserialize<UserInputClaimsForMagicLink>(input);

            if (userInputClaimsForMagicLink == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new AzureADB2CResponse("Can not deserialize input claims",
                                                                                       HttpStatusCode.Conflict));
            }

            // Check input email address 
            if (string.IsNullOrEmpty(userInputClaimsForMagicLink.Email))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new AzureADB2CResponse("Email address is empty",
                                                                                       HttpStatusCode.Conflict));
            }

            await _userMagicLinkInvitationHandler.SendEmailWithMagicLinkAsync(userInputClaimsForMagicLink);
            return Ok();
        }
    }
}