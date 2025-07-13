namespace GabConsoleDemo.Settings
{
    public struct ServiceBusSettings
    {
        public string FullyQualifiedNamespace { get; set; }
        public string ManagedIdentityClientId { get; set; }
        public string TenantId { get; set; }
    }
}