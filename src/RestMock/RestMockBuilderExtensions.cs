using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using RestMock.Swagger;

namespace RestMock
{
    /// <summary>
    ///     Extension methods for <see cref="RestMockBuilder"/>
    /// </summary>
    [PublicAPI]
    public static class RestMockBuilderExtensions
    {
        #region Xxx() methods

        /// <summary>
        ///     Gets a mock configuration builder for "GET" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Get([NotNull] this RestMockBuilder builder)
            => builder.Verb("GET");

        /// <summary>
        ///     Gets a mock configuration builder for "POST" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Post([NotNull] this RestMockBuilder builder)
            => builder.Verb("POST");

        /// <summary>
        ///     Gets a mock configuration builder for "PUT" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Put([NotNull] this RestMockBuilder builder)
            => builder.Verb("PUT");

        /// <summary>
        ///     Gets a mock configuration builder for "PATCH" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Patch([NotNull] this RestMockBuilder builder)
            => builder.Verb("PATCH");

        /// <summary>
        ///     Gets a mock configuration builder for "DELETE" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Delete([NotNull] this RestMockBuilder builder)
            => builder.Verb("DELETE");

        /// <summary>
        ///     Gets a mock configuration builder for "HEAD" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Head([NotNull] this RestMockBuilder builder)
            => builder.Verb("HEAD");

        /// <summary>
        ///     Gets a mock configuration builder for "OPTIONS" verb
        /// </summary>
        [NotNull]
        public static VerbBuilder Options([NotNull] this RestMockBuilder builder)
            => builder.Verb("OPTIONS");

        #endregion

        #region Xxx(string url) methods

        /// <summary>
        ///     Gets a mock response builder for "GET" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Get([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Get().Url(url);

        /// <summary>
        ///     Gets a mock response builder for "POST" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Post([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Post().Url(url);

        /// <summary>
        ///     Gets a mock response builder for "PUT" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Put([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Put().Url(url);

        /// <summary>
        ///     Gets a mock response builder for "PATCH" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Patch([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Patch().Url(url);

        /// <summary>
        ///     Gets a mock response builder for "DELETE" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Delete([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Delete().Url(url);

        /// <summary>
        ///     Gets a mock response builder for "HEAD" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Head([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Head().Url(url);

        /// <summary>
        ///     Gets a mock response builder for "OPTIONS" verb and specified URL
        /// </summary>
        [NotNull]
        public static ActionBuilder Options([NotNull] this RestMockBuilder builder, [NotNull] string url)
            => builder.Options().Url(url);

        #endregion

        #region Swagger support

        /// <summary>
        ///     Configures mocked server from JSON swagger document
        /// </summary>
        [NotNull]
        public static RestMockBuilder ImportSwaggerJson([NotNull] this RestMockBuilder builder, [NotNull] string swaggerJson)
        {
            SwaggerConfigurator.ConfigureFromSwagger(builder, swaggerJson);
            return builder;
        }

        #endregion

        #region ErrorHandler support

        /// <summary>
        ///     Adds a error handler callback to HTTP pipeline
        /// </summary>
        [NotNull]
        public static RestMockBuilder UseErrorHandler([NotNull] this RestMockBuilder builder, [NotNull] ErrorHandler handler)
        {
            return builder.UseMiddleware(new ErrorHandlerMiddleware(handler));
        }

        private sealed class ErrorHandlerMiddleware : IMiddleware
        {
            private readonly ErrorHandler _handler;

            public ErrorHandlerMiddleware(ErrorHandler handler)
            {
                _handler = handler;
            }

            public async Task InvokeAsync(HttpContext context, Func<Task> next)
            {
                try
                {
                    await next();
                }
                catch (Exception e)
                {
                    var url = $"{context.Request.Path}{context.Request.QueryString}";
                    _handler(context.Request.Method, url, e);
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(e.Message);
                }
            }
        }

        #endregion
    }
}
