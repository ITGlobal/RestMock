using System;
using System.Net.Http;
using Xunit;

namespace RestMock
{
    public class ErrorHandlerTests
    {
        [Fact]
        public void ErrorHandlerShouldNotBeCalledUponSuccess()
        {
            var invoked = new[] { false };
            var mock = RestMockBuilder.New();
            mock.Get("/foobar").Returns(_ => _.Write(200));
            mock.UseErrorHandler((method, url, exception) =>
            {
                invoked[0] = true;
            });
            
            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/foobar");
                client.SendAsync(request).GetAwaiter().GetResult();
            }

            Assert.False(invoked[0]);
        }

        [Theory]
        [InlineData("/foobar")]
        [InlineData("/foobar?x=1")]
        public void ErrorHandlerShouldBeCalledUponFailure(string urlToInvoke)
        {
            const string expectedErrorMessage = "MyErrorMessage";

            var invoked = new[] { false };
            string invokedMethod = null;
            string invokedUrl = null;
            string errorMessage = null;

            var mock = RestMockBuilder.New();
            mock.Get("/foobar").Returns(_ =>throw new Exception(expectedErrorMessage));
            mock.UseErrorHandler((method, url, exception) =>
            {
                invokedMethod = method;
                invokedUrl = url;
                errorMessage = exception.Message;
                invoked[0] = true;
            });

            using (var server = mock.Create())
            using (var client = server.CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, urlToInvoke);
                client.SendAsync(request).GetAwaiter().GetResult();
            }

            Assert.True(invoked[0]);
            Assert.Equal("GET", invokedMethod);
            Assert.Equal(urlToInvoke, invokedUrl);
            Assert.Equal(expectedErrorMessage, errorMessage);
        }
    }
}
