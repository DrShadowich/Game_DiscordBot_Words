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

            Token = json.Token;
            Prefix = json.Prefix;
        }
    }

    internal struct JSONStruct
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}