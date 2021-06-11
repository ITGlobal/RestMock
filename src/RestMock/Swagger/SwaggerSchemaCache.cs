using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RestMock.Swagger
{
    internal sealed class SwaggerSchemaCache
    {
        private readonly Dictionary<string, JToken> _jsons = new();
        private readonly SwaggerDocument _document;

        public SwaggerSchemaCache(SwaggerDocument document)
        {
            _document = document;
        }
        
        public JToken GetGeneratedJson(SwaggerSchema schema)
        {
            if (schema.Ref != null)
            {
                return GetGeneratedJson(schema.Ref);
            }

            return GenerateExample(schema);
        }

        private JToken GetGeneratedJson(string reference)
        {
            if (!_jsons.TryGetValue(reference, out var json))
            {
                if (_document.Definitions.TryGetValue(reference, out var schema))
                {
                    json = GenerateExample(schema);
                    _jsons[reference] = json;
                }
            }

            return json;
        }

        private JToken GenerateExample(SwaggerSchema schema)
        {
            switch (schema.Type)
            {
                case "array":
                    {
                        var jArray = new JArray();
                        if (schema.Items != null)
                        {
                            var innerJson = GetGeneratedJson(schema.Items);
                            jArray.Add(innerJson.DeepClone());
                            jArray.Add(innerJson.DeepClone());
                            jArray.Add(innerJson.DeepClone());
                            jArray.Add(innerJson.DeepClone());
                            jArray.Add(innerJson.DeepClone());
                        }
                        return jArray;
                    }

                case "object":
                    {
                        var jObject = new JObject();
                        foreach (var property in schema.Properties)
                        {
                            var innerJson = property.Value != null
                                ? GetGeneratedJson(property.Value)
                                : JToken.FromObject(null);
                            jObject.Add(property.Key, innerJson.DeepClone());
                        }
                        return jObject;
                    }

                case "integer":
                    return new JValue(1024);

                case "number":
                    return new JValue(10.24);

                case "string":
                    return new JValue("foobar");

                case "boolean":
                    return new JValue(true);
            }

            return null;
        }
    }
}
