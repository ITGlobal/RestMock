using System.Net.Http;
using JetBrains.Annotations;

namespace RestMock
{
    /// <summary>
    ///     Extension methods for <see cref="MockServer"/>
    /// </summary>
    [PublicAPI]
    public static class MockServerExtensions
    {
        /// <summary>
        ///     Creates a pre-configured HTTP client for 
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        [NotNull]
        public static HttpClient CreateHttpClient([NotNull] this MockServer server)
        {
            var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            client.BaseAddress = server.ListenUrl;
            return client;
        }
    }
}