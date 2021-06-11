using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace RestMock
{
    /// <summary>
    ///     A mocked HTTP server
    /// </summary>
    [PublicAPI]
    public sealed class MockServer : IDisposable
    {

        private readonly IWebHost _host;
        private readonly Endpoint _endpoint;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ManualResetEventSlim _terminated = new();

        internal MockServer(IWebHost host, Endpoint endpoint)
        {
            _host = host;
            _endpoint = endpoint;

            Task.Run(
                () =>
                {
                    try
                    {
                        _host.Start();
                        _cancellationTokenSource.Token.WaitHandle.WaitOne();
                    }
                    finally
                    {
                        _terminated.Set();
                    }
                }
            );
        }

        /// <summary>
        ///     Server's URL
        /// </summary>
        [NotNull]
        public Uri ListenUrl => _endpoint.Uri;

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _terminated.Wait();
            _cancellationTokenSource.Dispose();
            _host.Dispose();

            Endpoint.ReleaseEndpoint(_endpoint);
        }

    }
}
