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
        private readonly CancellationTokenSource _cancellationTokenSource
            = new CancellationTokenSource();
        private readonly ManualResetEventSlim _terminated
            = new ManualResetEventSlim();

        internal MockServer(IWebHost host, Uri listenUrl)
        {
            _host = host;
            ListenUrl = listenUrl;
            Task.Run(() =>
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
            });
        }

        /// <summary>
        ///     Server's URL
        /// </summary>
        [NotNull]
        public Uri ListenUrl { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _terminated.Wait();
            _cancellationTokenSource.Dispose();
            _host.Dispose();
        }
    }
}