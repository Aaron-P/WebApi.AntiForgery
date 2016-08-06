using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace WebApi.AntiForgery
{
    internal static partial class AntiForgery
    {
        internal static Tuple<HttpCookie, string> VerificationToken(HttpCookieCollection cookies, string cookieName = null)
        {
            cookieName = cookieName ?? AntiForgeryConfig.CookieName;
            var oldCookieToken = default(string);
            if (cookies != null)
            {
                if (cookies.AllKeys.Contains(cookieName))
                {
                    var oldCookie = cookies[cookieName];
                    if (oldCookie != null)
                        oldCookieToken = oldCookie.Value;
                }
            }

            var tokens = GetTokens(oldCookieToken);
            var cookie = tokens.Item1 != null && tokens.Item1 != oldCookieToken ? new HttpCookie(cookieName, tokens.Item1) { HttpOnly = true, Secure = Configuration.SecureCookie } : default(HttpCookie);//TODO: Move all config usage to the HttpHandler?
            return Tuple.Create(cookie, tokens.Item2);
        }

        private static Tuple<string, string> GetTokens(string oldCookieToken = null)
        {
            string cookieToken, formToken;
            System.Web.Helpers.AntiForgery.GetTokens(oldCookieToken, out cookieToken, out formToken);
            return Tuple.Create(cookieToken, formToken);
        }
    }
}