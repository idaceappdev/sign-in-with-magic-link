using System.Text.Json.Serialization;

namespace TMF.MagicLinks.API.DTO
{
    public class UserInputClaimsForMagicLink
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
}
