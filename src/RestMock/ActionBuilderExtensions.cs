using System;
using JetBrains.Annotations;

namespace RestMock
{
    /// <summary>
    ///     Extension methods for <see cref="ActionBuilder"/>
    /// </summary>
    [PublicAPI]
    public static class ActionBuilderExtensions
    {
        /// <summary>
        ///     Configures empty mocked response with specified status code
        /// </summary>
        [NotNull]
        public static RestMockBuilder Returns([NotNull] this ActionBuilder builder, int statusCode)
            => builder.Returns(_ => _.Write(statusCode));

        /// <summary>
        ///     Configures empty mocked response with 200 OK status code
        /// </summary>
        [NotNull]
        public static RestMockBuilder ReturnsOk([NotNull] this ActionBuilder builder)
            => builder.Returns(200);

        /// <summary>
        ///     Configures JSON mocked response with specified status code
        /// </summary>
        [NotNull]
        public static RestMockBuilder ReturnsJson<T>(
            [NotNull] this ActionBuilder builder,
            int statusCode,
            Func<ActionContext, T> content)
            => builder.Returns(_ => _.WriteJson(statusCode, content(_)));

        /// <summary>
        ///     Configures JSON mocked response with specified status code
        /// </summary>
        [NotNull]
        public static RestMockBuilder ReturnsJson<T>(
            [NotNull] this ActionBuilder builder,
            int statusCode,
            T content)
            => builder.Returns(_ => _.WriteJson(statusCode, content));

        /// <summary>
        ///     Configures JSON mocked response with 200 OK status code
        /// </summary>
        [NotNull]
        public static RestMockBuilder ReturnsJson<T>(
            [NotNull] this ActionBuilder builder,
            Func<ActionContext, T> content)
            => builder.Returns(_ => _.WriteJson(200, content(_)));

        /// <summary>
        ///     Configures JSON mocked response with 200 OK status code
        /// </summary>
        [NotNull]
        public static RestMockBuilder ReturnsJson<T>(
            [NotNull] this ActionBuilder builder,
            T content) 
            => builder.ReturnsJson(200, content);
    }
}