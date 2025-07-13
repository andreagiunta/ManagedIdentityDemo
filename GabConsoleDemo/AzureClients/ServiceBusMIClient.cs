using Azure.Identity;
using Azure.Messaging.ServiceBus;
using GabConsoleDemo.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GabConsoleDemo.AzureClients
{
    internal class ServiceBusMIClient : IAzureClient
    {
        private ServiceBusSettings _settings = new ServiceBusSettings();
        private Azure.Messaging.ServiceBus.ServiceBusClient _serviceBusClient;

        public ServiceBusMIClient()
        {
            this.LoadSettings();
            this.Connect();
        }

        public void LoadSettings()
        {
            _settings = SettingsHelper<ServiceBusSettings>.Instance._settings;
            if (string.IsNullOrEmpty(_settings.FullyQualifiedNamespace))
            {
                throw new ArgumentException("Service Bus namespace is not set.");
            }
        }

        public void Connect()
        {
            _serviceBusClient = new Azure.Messaging.ServiceBus.ServiceBusClient(
                _settings.FullyQualifiedNamespace,
                //In this example, we are using DefaultAzureCredential which supports Managed Identity and we specify TenantId if needed.
                //This is useful in case your local Identity has access to different tenant to help DefaultAzure Credential resolve the correct identity.
                new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = this._settings.TenantId}));
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            if (_serviceBusClient == null)
                throw new InvalidOperationException("Service Bus client is not initialized.");

            ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);
            await sender.SendMessageAsync(new ServiceBusMessage(message));
        }

        public async Task<List<string>> ReceiveMessagesAsync(string queueName, int maxMessages = 10)
        {
            if (_serviceBusClient == null)
                throw new InvalidOperationException("Service Bus client is not initialized.");

            ServiceBusReceiver receiver = _serviceBusClient.CreateReceiver(queueName);
            var messages = await receiver.ReceiveMessagesAsync(maxMessages);
            List<string> result = new List<string>();
            foreach (var msg in messages)
            {
                result.Add(msg.Body.ToString());
                await receiver.CompleteMessageAsync(msg);
            }
            return result;
        }
    }
}
