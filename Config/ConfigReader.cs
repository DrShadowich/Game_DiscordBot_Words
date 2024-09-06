using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace DiscordBot.Config
{
    public class ConfigReader
    {
        public string Token { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public async Task ReadJSONAsync()
        {
            using StreamReader reader = new("config.json");
            string jsonString = await reader.ReadToEndAsync();
            JSONStruct json = JsonConvert.DeserializeObject<JSONStruct>(jsonString);

            Token = json.token;
            Prefix = json.prefix;
        }
    }

    internal struct JSONStruct
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
}