using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabConsoleDemo.Settings
{
    /// <summary>
    /// As this is a demo on GitHub, settings for each Azure service are stored in a local JSON file which is not committed to the repository.
    /// </summary> 
    internal interface ISettings<T> where T:struct
    {
        ///// <summary>
        ///// Parse configuration json file for given Azure service using the jsonConfigTemplate to validate it.
        ///// <paramref name="jsonConfigTemplatePath"> is the path to the template JSON file that contains the expected structure and keys for the configuration.</paramref>
        ///// <paramref name="jsonFilePath"> is the path to the JSON file that contains the actual configuration settings.</paramref>
        ///// </summary>
        //void ParseSettingsFromLocalJson<T>(string jsonFilePath, string jsonConfigTemplatePath);
    }
}
