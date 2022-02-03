using System.Net;
using System.Reflection;

namespace TMF.MagicLinks.API.DTO
{
    public class AzureADB2CResponse
    {
        public string Version { get; set; }
        public int Status { get; set; }
        public string UserMessage { get; set; }

        public AzureADB2CResponse(string message, HttpStatusCode status)
        {
            UserMessage = message;
            Status = (int)status;
            Version = Assembly.GetExecutingAssembly()
                              .GetName()
                              .Version
                              .ToString();
        }
    }
}
