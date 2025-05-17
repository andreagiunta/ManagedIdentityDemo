namespace GabConsoleDemo.AzureClients
{
    internal interface IAzureClient
    {
        void Connect();
        void LoadSettings();
    }

}
