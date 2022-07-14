namespace AudioManette.Authorization
{
    public class AuthotizationConfig
    {
        public AuthotizationConfig(string clientId, string clientSecret, System.Uri redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string? Code { get; set; }
        public System.Uri RedirectUri { get; }
    }
}