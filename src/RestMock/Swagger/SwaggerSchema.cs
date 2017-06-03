using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestMock.Swagger
{
    internal sealed class SwaggerSchema
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, SwaggerSchema> Properties { get; set; }

        [JsonProperty("items")]
        public SwaggerSchema Items { get; set; }
    }
}