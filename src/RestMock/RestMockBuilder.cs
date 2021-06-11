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
        private readonly List<IMiddleware> _middlewares = new List<IMiddleware>();

        private RestMockBuilder()
        {
        }

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
        ///     Adds a middleware into HTTP pipeline
        /// </summary>
        [NotNull]
        public RestMockBuilder UseMiddleware([NotNull] IMiddleware middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }

        /// <summary>
        ///     Creates an instance of a mocked HTTP server
        /// </summary>
        [NotNull]
        public MockServer Create() => Create(Endpoint.GetEndpoint());

        /// <summary>
        ///     Creates an instance of a mocked HTTP server
        /// </summary>
        [NotNull]
        public MockServer Create(string endpoint) => Create(Endpoint.CreateEndpoint(endpoint));

        /// <summary>
        ///     Creates an instance of a mocked HTTP server
        /// </summary>
        [NotNull]
        private MockServer Create(Endpoint endpoint)
        {
            var builder = new WebHostBuilder()
                .UseKestrel(
                    options => { options.AllowSynchronousIO = true; }
                )
                .UseUrls(endpoint.Url)
                .ConfigureServices(services => { services.AddRouting(); })
                .CaptureStartupErrors(true)
                .Configure(
                    app =>
                    {
                        foreach (var m in _middlewares)
                        {
                            var middleware = m;
                            app.Use(
                                next => { return context => middleware.InvokeAsync(context, () => next(context)); }
                            );
                        }

                        app.UseRouter(
                            routes =>
                            {
                                foreach (var verb in _verbs.Values)
                                {
                                    verb.BuildRoutes(routes);
                                }
                            }
                        );
                    }
                );
            var host = builder.Build();

            return new MockServer(host, endpoint);
        }

    }
}
