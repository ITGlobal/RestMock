using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace RestMock
{
    /// <summary>
    ///     HTTP pipeline middleware
    /// </summary>
    public interface IMiddleware
    {
        /// <summary>
        ///     Handles a request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        [NotNull]
        Task InvokeAsync([NotNull] HttpContext context, [NotNull] Func<Task> next);
    }
}