using System;
using Azure.Identity;
using SFA.DAS.RoatpFinance.Web.Settings;

namespace SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpApplicationTokenService : IRoatpApplicationTokenService
    {
        private readonly IWebConfiguration _configuration;

        public RoatpApplicationTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(Uri baseUri)
        {
            if (baseUri != null && baseUri.IsLoopback)
                return string.Empty;

            var credential = new DefaultAzureCredential();
            var generateTokenTask = credential.GetTokenAsync(
                new Azure.Core.TokenRequestContext(new[] { _configuration.RoatpApplicationApiAuthentication.Identifier }));

            return generateTokenTask.GetAwaiter().GetResult().Token;
        }
    }
}
