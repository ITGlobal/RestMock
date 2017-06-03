using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Xunit;

namespace RestMock
{
    public class ResponseEncodingTests
    {
        [Fact]
        public void DefaultContentTypeTest()
        {
            const string expectedContent = "qwertyuiop1234567890QWERTYUIOP";
            const string expectedContentType = "application/octet-stream";

            var mock = RestMockBuilder.New()
                .Get("/foobar")
                .Returns(_ => _.Write(200, expectedContent, encoding: Encoding.UTF8));

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/foobar");
                var response = client.SendAsync(request).Result;

                Assert.Equal(200, (int)response.StatusCode);

                var responseContent = response.Content.ReadAsStringAsync().Result;
                Assert.Equal(expectedContent, responseContent);
                
                Assert.Equal(expectedContentType, response.Content.Headers.ContentType.MediaType);
            }
        }

        [Fact]
        public void SpecificContentTypeTest()
        {
            const string expectedContent = "qwertyuiop1234567890QWERTYUIOP";
            const string expectedContentType = "foo/bar";

            var mock = RestMockBuilder.New()
                .Get("/foobar")
                .Returns(_ => _.Write(200, expectedContent, expectedContentType, Encoding.UTF8));

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/foobar");
                var response = client.SendAsync(request).Result;

                Assert.Equal(200, (int)response.StatusCode);

                var responseContent = response.Content.ReadAsStringAsync().Result;
                Assert.Equal(expectedContent, responseContent);
                
                Assert.Equal(expectedContentType, response.Content.Headers.ContentType.MediaType);
            }
        }

        [Fact]
        public void JsonContentTypeTest()
        {
            const string expectedContentType = "application/json";

            var mock = RestMockBuilder.New()
                .Get("/foobar")
                .ReturnsJson(_ => new { ok = true, msg = "foobar", count = 1024 });

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/foobar");
                var response = client.SendAsync(request).Result;

                Assert.Equal(200, (int)response.StatusCode);

                var responseContent = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(responseContent);

                Assert.NotNull(json.Property("ok"));
                Assert.NotNull(json.Property("msg"));
                Assert.NotNull(json.Property("count"));

                Assert.Equal(expectedContentType, response.Content.Headers.ContentType.MediaType);
            }
        }
    }
}
