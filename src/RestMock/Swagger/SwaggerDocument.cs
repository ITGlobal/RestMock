using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestMock.Swagger
{
    internal sealed class SwaggerDocument
    {
        [JsonProperty("paths")]
        public Dictionary<string, Dictionary<string, SwaggerOperation>> Paths { get; set; }

        [JsonProperty("definitions")]
        public Dictionary<string, SwaggerSchema> Definitions { get; set; }
    }
}