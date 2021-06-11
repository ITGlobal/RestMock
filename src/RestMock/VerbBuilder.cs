using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace RestMock
{
    /// <summary>
    ///     Defines methods to configure a mocked response
    /// </summary>
    [PublicAPI]
    public sealed class VerbBuilder
    {

        private readonly Dictionary<string, ActionBuilder> _actions = new();

        private readonly RestMockBuilder _builder;
        private readonly string _verb;

        internal VerbBuilder(RestMockBuilder builder, string verb)
        {
            _builder = builder;
            _verb = verb;
        }

        /// <summary>
        ///     Gets a mock builder for specified URL
        /// </summary>
        [NotNull]
        public ActionBuilder Url([NotNull] string url)
        {
            if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

            if (!_actions.TryGetValue(url, out var builder))
            {
                builder = new ActionBuilder(_builder, _verb, url);
                _actions.Add(url, builder);
            }

            return builder;
        }

        internal void BuildRoutes(IRouteBuilder routes)
        {
            foreach (var action in _actions.Values)
            {
                action.BuildRoutes(routes);
            }
        }

    }
}
