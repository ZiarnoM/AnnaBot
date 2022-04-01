using System;
using System.IO;
using System.Text.Json;

namespace Anna
{
    public class ConfigModel
    {
        public string server { get; set; }
        public string nick { get; set; }
        
        public string id { get; set; }
        
        public string[] channels { get; set; }
        
        public string[] commands { get; set; }
        
        public string connectionString { get; set; }
        
        public string redeployRepoLocation { get; set; }

        public static ConfigModel DeserializeData(string path)
        {
            var jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ConfigModel>(jsonString)!;
        }
    }
    
    
}