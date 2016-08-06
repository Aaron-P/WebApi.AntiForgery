using System;
using System.Reflection;
using System.Web;

namespace WebApi.AntiForgery
{
    internal static class Startup
    {
        static Startup()
        {
            Assembly = typeof(Startup).Assembly;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static Assembly Assembly { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is referenced by whatever application uses this library.")]
    internal class HttpHandlerFactory : IHttpHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            using (var handler = new HttpHandler())
                return handler;
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}