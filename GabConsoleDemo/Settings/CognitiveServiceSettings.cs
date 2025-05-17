namespace GabConsoleDemo.Settings
{
    public struct CognitiveServiceSettings
    {
        public string Endpoint { get; set; }
        public string Scopes { get; set; }
        public string Region { get; set; }
        public string ManagedIdentityClientId{ get; set; }
        public string ResourceId { get; set; }
        public string TenantId { get; set; }
    }
}
