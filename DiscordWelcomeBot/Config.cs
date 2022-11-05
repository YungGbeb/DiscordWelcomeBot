using System.Collections.Generic;
using Newtonsoft.Json;

namespace config
{
    
    public class Config
    {
        private static Config config;

        public static Config Get()
        {
            if (config == null)
            {
                config = new Config();
                string cfg = System.IO.File.ReadAllText("config.ini");
                JsonConvert.PopulateObject(cfg, config);
            }
            return config;
        }

        private Config()
        {
            
        } 

        [JsonProperty("BotToken")]
        public string botToken { get; set; }

        [JsonProperty("MainChannelID")]
        public ulong channelId { get; set; }
        
        [JsonProperty("BotSelfID")]
        public ulong botId { get; set; }

        [JsonProperty("RuGreetings")]
        public List<string> rugreetings { get; set; }

        [JsonProperty("EnGreetings")]
        public List<string> engreetings { get; set; }

        [JsonProperty("Gifs")]
        public List<string> gifs { get; set; }
        
        [JsonProperty("DeleteAfter")]
        public int deleteAfter { get; set; }

        [JsonProperty("WithButtonDeleteAfter")]
        public int withButtonDeleteAfter { get; set; }

        [JsonProperty("EngChannelID")]
        public ulong engChannelID { get; set; }
    }

    
}