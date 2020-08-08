namespace EDSc.Common.Services.Deployment.Util
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    public class JsonToDictionaryConfigConverter : IConfigConverter<string, Dictionary<string, string>>
    {
        public Dictionary<string, string> Convert(string jsonConfig)
        {
            var configDictionary = new Dictionary<string, string>();

            var rootNode = JObject.Parse(jsonConfig);

            foreach (var node in rootNode.Descendants())
            {
                if (!node.HasValues)
                {
                    configDictionary.Add(node.Path.Replace('.', '_'), node.ToString());
                }
            }

            return configDictionary;
        }
    }
}
