using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace RestMock
{
    /// <summary>
    ///     Extension methods for <see cref="ActionContext"/>
    /// </summary>
    [PublicAPI]
    public static class ActionContextExtensions
    {
        private static readonly byte[] EmptyArray = new byte[0];

        /// <summary>
        ///     Writes empty response with specified status code
        /// </summary>
        public static void Write([NotNull] this ActionContext context, int statusCode)
        {
            context.StatusCode(statusCode).Write(EmptyArray);
        }

        /// <summary>
        ///     Writes binary response with specified status code
        /// </summary>
        public static void Write(
            [NotNull] this ActionContext context,
            int statusCode,
            [NotNull] byte[] content,
            string contentType = null)
        {
            context.StatusCode(statusCode).Write(content, contentType);
        }

        /// <summary>
        ///     Writes string response with specified status code
        /// </summary>
        public static void Write(
            [NotNull] this ActionContext context,
            int statusCode,
            [NotNull] string content,
            string contentType = null,
            Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            contentType = contentType ?? "application/octet-stream";

#if NETSTANDARD1_6
            var mdHeader = MediaTypeHeaderValue.Parse(contentType);
#else
            var mdHeader = MediaTypeHeaderValue.Parse(new StringSegment(contentType));
#endif

            mdHeader.Charset = encoding.WebName;
            contentType = mdHeader.ToString();

            context.StatusCode(statusCode).Write(encoding.GetBytes(content), contentType);
        }

        /// <summary>
        ///     Writes JSON response with specified status code
        /// </summary>
        public static void WriteJson<T>(
            [NotNull] this ActionContext context,
            int statusCode,
            [NotNull] T content,
            string contentType = null)
        {
            contentType = contentType ?? "application/json";
            var json = JsonConvert.SerializeObject(content);
            context.Write(statusCode, json, contentType, Encoding.UTF8);
        }
    }
}
