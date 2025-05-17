using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using GabConsoleDemo.Settings;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GabConsoleDemo.AzureClients
{
    internal class CognitiveServicesClient : IAzureClient
    {
        private CognitiveServiceSettings _settings;
        private AccessToken token;

        public CognitiveServicesClient()
        {
            LoadSettings();
            Connect();
        }
        public void Connect()
        {
            try
            {
                Console.WriteLine("Attempting to connect to Azure Cognitive Services");
                TokenCredential identity;
                // Check if Managed Identity Client ID is provided. If so it means we are using a specific assigned identity
                // Will be unused as demo runs only locally. But it showcase how the Managed Identity class could be used
                if (!string.IsNullOrEmpty(_settings.ManagedIdentityClientId))
                {
                    identity = new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(_settings.ManagedIdentityClientId));
                }
                else
                {
                    identity = new DefaultAzureCredential(new DefaultAzureCredentialOptions()
                    {
                        TenantId = _settings.TenantId
                    });
                }
                string[] scopes = _settings.Scopes.Split(',');
                Console.WriteLine($"Scopes: {string.Join(",", scopes)}");
                Console.WriteLine("Acquiring token");
                token = identity.GetTokenAsync(new TokenRequestContext(scopes), CancellationToken.None).GetAwaiter().GetResult();
                Console.WriteLine($"Token acquired: { token.Token.Substring(0,10)}...");
            }
            catch (Exception)
            {
                // Let exception propagate. No need to implement for demo purpose
                throw;
            }
        }

        public void LoadSettings()
        {
            Console.WriteLine("Loading settings");
            _settings = SettingsHelper<CognitiveServiceSettings>.Instance._settings;
            if (string.IsNullOrEmpty(_settings.Endpoint))
            {
                throw new ArgumentException("Endpoint is not set.");
            }
            if (string.IsNullOrEmpty(_settings.Region))
            {
                throw new ArgumentException("Region name is not set.");
            }
            if (string.IsNullOrEmpty(_settings.Scopes))
            {
                throw new ArgumentException("Scopes are not set.");
            }
            if (string.IsNullOrEmpty(_settings.ResourceId))
            {
                throw new ArgumentException("Resource ID is not set.");
            }
        }

        public async Task<string> Translate(string input)
        {
            try
            {

                string route = "/translate?api-version=3.0&from=en&to=it";
                string textToTranslate = input;
                Console.WriteLine($"String to translate:{textToTranslate}");
                object[] body = new object[] { new { Text = textToTranslate } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {

                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(_settings.Endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", $"Bearer {token.Token}");
                    request.Headers.Add("Ocp-Apim-ResourceId", _settings.ResourceId);

                    request.Headers.Add("Ocp-Apim-Subscription-Region", _settings.Region);


                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }

        }
    }

}
