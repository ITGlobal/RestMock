using System.Net.Http;
using Xunit;

namespace RestMock
{
    public class ResponseStatusCodeTests
    {
        [Theory]
        [InlineData(200)]
        [InlineData(201)]
        [InlineData(202)]
        [InlineData(203)]
        [InlineData(204)]
        [InlineData(300)]
        [InlineData(301)]
        [InlineData(302)]
        [InlineData(303)]
        [InlineData(304)]
        [InlineData(400)]
        [InlineData(401)]
        [InlineData(402)]
        [InlineData(403)]
        [InlineData(404)]
        [InlineData(405)]
        [InlineData(406)]
        [InlineData(407)]
        [InlineData(408)]
        [InlineData(409)]
        [InlineData(500)]
        [InlineData(501)]
        [InlineData(502)]
        public void ResponseStatusCodeTest(int statusCode)
        {
            var mock = RestMockBuilder.New()
                .Get("/foobar")
                .Returns(statusCode);

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/foobar");
                var response = client.SendAsync(request).Result;

                Assert.Equal(statusCode, (int)response.StatusCode);
            }
        }
    }
}