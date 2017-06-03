using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestMock.Swagger
{
    internal sealed class SwaggerOperation
    {
        [JsonProperty("responses")]
        public Dictionary<string, SwaggerResponse> Responses { get; set; }
    }
}