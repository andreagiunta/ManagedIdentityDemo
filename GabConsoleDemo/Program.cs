// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.AI.Translation.Document;
using Azure.AI.Translation.Text;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using GabConsoleDemo.AzureClients;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.WebRequestMethods;

internal class Program
{
    private static async Task Main(string[] args)
    {

        try
        {
            //Call Cognitive services translation.
            Console.Clear();
            await Translate();
            //Enumerate blob from a container using Managed Identity
            Console.Clear();
            await EnumerateBlobs();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    static async Task Translate()
    {
        
        Console.WriteLine("Start translation");
        string input = "I would like to drive my car to the beach";
        Console.WriteLine($"Translating text:{input}");
        Console.WriteLine("Initialize CognitiveServiceClient");
        CognitiveServicesClient cognitiveServicesClient = new CognitiveServicesClient();
        var result = await cognitiveServicesClient.Translate(input);
        Console.WriteLine($"Translation result: {result}");
        Console.WriteLine("Press key to continue");
        Console.ReadKey();
    }

    static async Task EnumerateBlobs()
    {
        try
        {
            Console.WriteLine("Listing blobs using Managed Identity");
            Console.WriteLine("Initializing AzureStorageClient");
            StorageAccountClient _storageAccountClient = new StorageAccountClient();
            Console.WriteLine("Enumerating blobs");
            Console.WriteLine(await _storageAccountClient.EnumerateContainerBlobs());
            Console.WriteLine("Finished enumerating blobs");
            Console.WriteLine("Press key to continue");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}