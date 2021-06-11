using System.Linq;
using Newtonsoft.Json;

namespace RestMock.Swagger
{
    internal static class SwaggerConfigurator
    {
        public static void ConfigureFromSwagger(RestMockBuilder builder, string swaggerJson)
        {
            var swagger = JsonConvert.DeserializeObject<SwaggerDocument>(swaggerJson);

            var schemas = new SwaggerSchemaCache(swagger);

            foreach (var (path, value) in swagger!.Paths)
            {
                foreach (var (verb, operation) in value)
                {
                    var (status, response) = operation.Responses
                        .Select(_ => (status: int.Parse(_.Key), response: _.Value))
                        .Where(_ => _.status >= 200 && _.status < 300)
                        .OrderBy(_ => _.status)
                        .FirstOrDefault();
                    if (response != null)
                    {
                        var actionBuilder = builder.Verb(verb).Url(path);

                        if (response.Schema == null)
                        {
                            actionBuilder.Returns(context => context.Write(status));
                        }
                        else
                        {
                            var json = schemas.GetGeneratedJson(response.Schema)?.ToString();
                            if (json != null)
                            {
                                actionBuilder.Returns(context => context.Write(status, json, "application/json"));
                            }
                            else
                            {
                                actionBuilder.Returns(context => context.Write(status));
                            }
                        }
                    }
                }
            }
        }
    }
}
