using System.Net.Http;
using Xunit;

namespace RestMock
{
    public class BasicTests
    {
        [Theory]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("POST")]
        [InlineData("PATCH")]
        [InlineData("DELETE")]
        [InlineData("HEAD")]
        [InlineData("OPTIONS")]
        public void TestPlainUrlMethod(string verb)
        {
            const string url = "/foobar";

            var mock = RestMockBuilder.New()
                .Verb(verb)
                .Url(url)
                .Returns(200);

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod(verb), url);
                var response = client.SendAsync(request).Result;
                Assert.Equal(200, (int)response.StatusCode);
            }
        }
        
        [Theory]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("POST")]
        [InlineData("PATCH")]
        [InlineData("DELETE")]
        [InlineData("HEAD")]
        [InlineData("OPTIONS")]
        public void TestTemplateUrlMethod(string verb)
        {
            const string urlTemplate = "/foobar/{id}";

            string requestedId = null;

            var mock = RestMockBuilder.New()
                .Verb(verb)
                .Url(urlTemplate)
                .Returns(_ =>
                {
                    requestedId = _.GetRouteValue("id");
                    _.Write(200);
                });

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod(verb), "/foobar/qwerty");
                var response = client.SendAsync(request).Result;
                Assert.Equal(200, (int)response.StatusCode);
                Assert.Equal("qwerty", requestedId);
            }
        }
    }
}