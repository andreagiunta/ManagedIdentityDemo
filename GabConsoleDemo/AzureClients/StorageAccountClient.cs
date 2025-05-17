using GabConsoleDemo.Settings;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Core;
using Azure.Storage.Blobs.Models;
using Azure;
using static System.Reflection.Metadata.BlobBuilder;
using System.Text;

namespace GabConsoleDemo.AzureClients
{
    internal class StorageAccountClient : IAzureClient
    {
        private AzureStorageSettings _settings; 
        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;

        public StorageAccountClient()
        {
            this.LoadSettings();
            this.Connect();
        }

        public void Connect()
        {
            try
            {
                TokenCredential identity;
                // Check if Managed Identity Client ID is provided. If so it means we are using a specific assigned identity
                // Will be unused as demo runs only locally. But it showcase how the Managed Identity class could be used
                if (!string.IsNullOrEmpty(_settings.ManagedIdentityClientId))
                {
                    identity = new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(_settings.ManagedIdentityClientId));
                }
                else
                {
                    identity = new DefaultAzureCredential();
                    
                }
                _blobServiceClient = new BlobServiceClient(new Uri(_settings.StorageAccountEndpoint), identity);
                if (!string.IsNullOrEmpty(_settings.StorageAccountContainerName))
                {
                    _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.StorageAccountContainerName);
                }
            }
            catch (Exception)
            {
                // Let exception propagate. No need to implement for demo purpose
                throw;
            }
        }

        public void LoadSettings()
        {
            _settings = SettingsHelper<AzureStorageSettings>.Instance._settings;
            if (string.IsNullOrEmpty(_settings.StorageAccountEndpoint))
            {
                throw new ArgumentException("Storage account endpoint is not set.");
            }
            if(string.IsNullOrEmpty(_settings.StorageAccountContainerName))
            {
                throw new ArgumentException("Storage account container name is not set.");
            }
        }


        public async Task<string> EnumerateContainerBlobs()
        {
            try
            {
                StringBuilder _sb = new StringBuilder();
                var blobs = _blobContainerClient.GetBlobsAsync().AsPages();
                await foreach (Page<BlobItem> blobPage in blobs)
                    foreach (var blob in blobPage.Values)
                        _sb.AppendLine($"Found blob:{blob.Name}");
                return _sb.ToString();
            }
            catch
            {
                throw;
            }
        }
    }

}
