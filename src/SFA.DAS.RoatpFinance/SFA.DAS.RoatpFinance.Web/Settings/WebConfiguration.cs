    using Newtonsoft.Json;

namespace SFA.DAS.RoatpFinance.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public string SessionRedisConnectionString { get; set; }

        [JsonRequired]
        public string SessionCachingDatabase { get; set; }

        [JsonRequired]
        public string DataProtectionKeysDatabase { get; set; }

        [JsonRequired]
        public ManagedIdentityApiAuthentication RoatpApplicationApiAuthentication { get; set; }

        [JsonRequired]
        public ManagedIdentityApiAuthentication QnaApiAuthentication { get; set; }

        [JsonRequired]
        public string EsfaAdminServicesBaseUrl { get; set; }
        public string DfESignInServiceHelpUrl { get; set; }
    }
}
