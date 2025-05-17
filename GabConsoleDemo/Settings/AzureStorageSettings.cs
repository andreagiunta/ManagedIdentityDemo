namespace GabConsoleDemo.Settings
{
    public struct AzureStorageSettings
    {
        public string StorageAccountEndpoint { get; set; }
        public string StorageAccountContainerName { get; set; }
        public string ManagedIdentityClientId { get; set; }
    }
}
