using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Xunit;

namespace RestMock
{
    public class SwaggerTests
    {
        [Fact]
        public void ConfigureServerFromSwagger()
        {
            var swagger = GetSwaggerJson();

            var mock = RestMockBuilder.New();
            mock.ImportSwaggerJson(swagger);

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                int MakeRequest(string verb, string url)
                {
                    var request = new HttpRequestMessage(new HttpMethod(verb), url);
                    var response = client.SendAsync(request).Result;
                    return (int)response.StatusCode;
                }

                Assert.Equal(200, MakeRequest("GET", "/foo"));
                Assert.Equal(201, MakeRequest("POST", "/foo"));
                Assert.Equal(200, MakeRequest("GET", "/foo/123"));
                Assert.Equal(200, MakeRequest("PUT", "/foo/123"));
                Assert.Equal(200, MakeRequest("PATCH", "/foo/123"));
                Assert.Equal(204, MakeRequest("DELETE", "/foo/123"));
                Assert.Equal(200, MakeRequest("GET", "/bar"));
                Assert.Equal(404, MakeRequest("GET", "/bad/request"));
            }
        }

        private static string GetSwaggerJson()
        {
            var assembly = typeof(SwaggerTests).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{typeof(SwaggerTests).Namespace}.swagger.json"))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var json = reader.ReadToEnd();
                return json;
            }
        }
    }
}