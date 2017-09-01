using System;

namespace RestMock
{
    /// <summary>
    ///     Error handler callback
    /// </summary>
    public delegate void ErrorHandler(string method, string url, Exception exception);
}