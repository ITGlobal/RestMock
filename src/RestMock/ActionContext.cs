using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace RestMock
{
    /// <summary>
    ///     Defines properties and methods for a mocked HTTP request
    /// </summary>
    [PublicAPI]
    public sealed class ActionContext
    {
        private readonly HttpContext _context;

        internal ActionContext(HttpContext context, string verb, string url)
        {
            _context = context;
            Verb = verb;
            Url = url;
        }

        /// <summary>
        ///     Gets a mocked HTTP verb
        /// </summary>
        [NotNull]
        public string Verb { get; }

        /// <summary>
        ///     Gets a mocked URL
        /// </summary>
        [NotNull]
        public string Url { get; }

        /// <summary>
        ///     Gets an URL route parameter
        /// </summary>
        [CanBeNull]
        public string GetRouteValue([NotNull] string key) => _context.GetRouteValue(key)?.ToString();

        /// <summary>
        ///     Writes HTTP status code
        /// </summary>
        [NotNull]
        public ActionContext StatusCode(int statusCode)
        {
            _context.Response.StatusCode = statusCode;
            return this;
        }

        /// <summary>
        ///     Sets an HTTP response header
        /// </summary>
        [NotNull]
        public ActionContext Header([NotNull] string header, StringValues value)
        {
            _context.Response.Headers[header] = value;
            return this;
        }

        /// <summary>
        ///     Writes response content
        /// </summary>
        public void Write([NotNull] byte[] content, string contentType = null)
        {
            contentType = contentType ?? "application/octet-stream";

            Header("Content-Type", contentType);
            _context.Response.Body.Write(content, 0, content.Length);
        }
    }
}