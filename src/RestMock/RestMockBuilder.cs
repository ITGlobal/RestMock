using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RestMock
{
    /// <summary>
    ///     Defines methods to configure and create a mocked HTTP server
    /// </summary>
    [PublicAPI]
    public sealed class RestMockBuilder
    {
        private readonly Dictionary<string, VerbBuilder> _verbs = new Dictionary<string, VerbBuilder>();

        private RestMockBuilder() { }

        /// <summary>
        ///     Gets a mock configuration builder for specified verb
        /// </summary>
        [NotNull]
        public static RestMockBuilder New() => new RestMockBuilder();

        /// <summary>
        ///     Gets a mock builder for specified verb
        /// </summary>
        [NotNull]
        public VerbBuilder Verb([NotNull] string verb)
        {
            verb = verb.ToUpperInvariant();

            if (!_verbs.TryGetValue(verb, out var builder))
            {
                builder = new VerbBuilder(this, verb);
                _verbs.Add(verb, builder);
            }

            return builder;
        }

        /// <summary>
        ///     Creates an instance of a mocked HTTP server
        /// </summary>
        [NotNull]
        public MockServer Create()
        {
            // TODO use smart detection
            const string url = "http://127.0.0.1:8000";

            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .CaptureStartupErrors(true)
                .Configure(app =>
                {
                    app.UseRouter(routes =>
                    {
                        foreach (var verb in _verbs.Values)
                        {
                            verb.BuildRoutes(routes);
                        }
                    });

                });
            var host = builder.Build();

            return new MockServer(host, new Uri(url, UriKind.Absolute));
        }
    }
}
