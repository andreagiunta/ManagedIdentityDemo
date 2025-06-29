using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs.Models;
using GabConsoleDemo.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace GabConsoleDemo.AzureClients
{
    internal class KeyVaultClient : IAzureClient
    {
        private KeyVaultSettings _settings = new KeyVaultSettings();

        private SecretClient _secretClient;
        private CertificateClient _certificateClient;
        public KeyVaultClient()
        {
            this.LoadSettings();
            this.Connect();
        }
        public void Connect()
        {
            //   Keyva
        }

        public void LoadSettings()
        {
            _settings = SettingsHelper<KeyVaultSettings>.Instance._settings;
            if (string.IsNullOrEmpty(_settings.KeyVaultEndpoint))
            {
                throw new ArgumentException("Keyvault endpoint is not set.");
            }
            _secretClient = new SecretClient(new Uri(_settings.KeyVaultEndpoint), new DefaultAzureCredential());
            _certificateClient = new CertificateClient(new Uri(_settings.KeyVaultEndpoint), new DefaultAzureCredential());
        }

        #region Secret Management
        public async Task<List<string>> ListSecrets()
        {
            if (_secretClient == null)
            {
                throw new InvalidOperationException("Secret client is not initialized.");
            }
            List<string> secretNames = new List<string>();
            var secrets = _secretClient.GetPropertiesOfSecretsAsync();
            await foreach (var secretPage in secrets.AsPages())
                foreach (var secret in secretPage.Values)
                {
                    Console.WriteLine($"Secret Name: {secret.Name}");
                    secretNames.Add(secret.Name);
                }
                return secretNames;
        }

        public async Task GetSecret(string secretName)
        {
            if (_secretClient == null)
            {
                throw new InvalidOperationException("Secret client is not initialized.");
            }
            var secret = await _secretClient.GetSecretAsync(secretName);
            Console.WriteLine($"Secret Name: {secret.Value.Name}, Secret Value: {secret.Value.Value}");
        }

        public async Task CreateSecret(string secretName, string secretValue)
        {
            if (_secretClient == null)
            {
                throw new InvalidOperationException("Secret client is not initialized.");
            }
            await _secretClient.SetSecretAsync(secretName, secretValue);
        }
        #endregion

        #region Certificate Management
        public async Task GetCertificate(string certificateName)
        {
            if (_certificateClient == null)
            {
                throw new InvalidOperationException("Certificate client is not initialized.");
            }
            var certificate = await _certificateClient.GetCertificateAsync(certificateName);
            Console.WriteLine($"Certificate Name: {certificate.Value.Name}, Certificate Size: {certificate.Value.Policy.KeySize}");
        }
        public async Task<List<string>> ListCertificates()
        {
            if (_certificateClient == null)
            {
                throw new InvalidOperationException("Certificate client is not initialized.");
            }
            List<string> certificateNames = new List<string>();
            var certificates = _certificateClient.GetPropertiesOfCertificatesAsync();
            await foreach (var certificatePages in certificates.AsPages())
                foreach (var certificate in certificatePages.Values)
                {
                    Console.WriteLine($"Certificate Name: {certificate.Name}");
                    certificateNames.Add(certificate.Name);
                }
            return certificateNames;
        }
        #endregion
    }

}
