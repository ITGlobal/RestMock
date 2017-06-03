using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace RestMock
{
    /// <summary>
    ///     Defines methods to configure a mocked response
    /// </summary>
    [PublicAPI]
    public sealed class ActionBuilder
    {
        private readonly RestMockBuilder _builder;
        private readonly string _verb;
        private readonly string _url;

        private Action<ActionContext> _handler;

        internal ActionBuilder(RestMockBuilder builder, string verb, string url)
        {
            _builder = builder;
            _verb = verb;
            _url = url;
        }

        /// <summary>
        ///     Configures a mocked HTTP response
        /// </summary>
        [NotNull]
        public RestMockBuilder Returns([NotNull] Action<ActionContext> handler)
        {
            _handler = handler;
            return _builder;
        }

        internal void BuildRoutes(IRouteBuilder routes)
        {
            if (_handler != null)
            {
                routes.MapVerb(_verb, _url, context =>
                {
                    _handler(new ActionContext(context, _verb, _url));
                    return Task.CompletedTask;
                });
            }
        }
    }
}