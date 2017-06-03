using Newtonsoft.Json;

namespace RestMock.Swagger
{
    internal sealed class SwaggerResponse
    {
        [JsonProperty("schema")]
        public SwaggerSchema Schema { get; set; }
    }
}