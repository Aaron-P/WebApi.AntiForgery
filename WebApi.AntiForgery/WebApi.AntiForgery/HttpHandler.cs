using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;

namespace WebApi.AntiForgery
{
    internal static class HttpHandlerHelpers
    {
        internal static void Override(this NameValueCollection collection, NameValueCollection overrides)
        {
            overrides.AllKeys.ToList().ForEach(k => collection.Remove(k));
            collection.Add(overrides);
        }
    }

    internal class HttpHandler : Page, IHttpHandler//TODO: Do we care about using Page?
    {
        internal HttpHandler()
        {
        }

        new public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        new internal static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }

        new internal static HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        new internal static HttpResponse Response
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }

        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is an internal method.")]
        protected override void Render(HtmlTextWriter writer)
        {
            //html (IFRAME), json (AJAJ), xml (AJAX)?

            //var accept = new List<string>();
            //foreach (var type in Request.AcceptTypes)
            //{
            //    accept.Add(type.Split(new char[] { ';' })[0]);
            //}

            //if (accept.Contains("text/html", StringComparer.OrdinalIgnoreCase))
            //    ;
            //if (accept.Contains("application/json", StringComparer.OrdinalIgnoreCase) ||
            //    accept.Contains("text/json", StringComparer.OrdinalIgnoreCase))
            //    ;
            //if (accept.Contains("application/xml", StringComparer.OrdinalIgnoreCase) ||
            //    accept.Contains("text/xml", StringComparer.OrdinalIgnoreCase))
            //    ;

            //Referrer checking? Allowed origin list?
            //if (Request.UrlReferrer != null &&
            //    Request.UrlReferrer.Scheme != Request.Url.Scheme &&
            //    Request.UrlReferrer.Host != Request.Url.Host &&
            //    Request.UrlReferrer.Port != Request.Url.Port)
            //    throw new HttpException((int)HttpStatusCode.InternalServerError, Resource.TokenUnknownError);

            //Element name valid regex?
            //https://stackoverflow.com/questions/3158274/what-would-be-a-regex-for-valid-xml-names
            //https://stackoverflow.com/questions/2519845/how-to-check-if-string-is-a-valid-xml-element-name
            //<?xml version="1.0" encoding="UTF-8" standalone="yes" ?><!DOCTYPE VerificationToken [<!ELEMENT VerificationToken (#PCDATA)>]><VerificationToken><![CDATA[TOKEN VALUE]]></VerificationToken>
            //$($.parseXML("")).first('VerificationToken').text()

            //filler comment in xml
            //filler property in json?

            //Security confic section?

            //https://stackoverflow.com/questions/3446170/escape-string-for-use-in-javascript-regex

            Response.Headers.Override(new NameValueCollection {
                { "Content-Security-Policy", "default-src 'none'; form-action 'none'; frame-ancestors 'self'; sandbox allow-same-origin allow-scripts" },//allow-scripts is needed by our script to remove the iframe, not sure we want to keep doing that.
                { "Expires", "-1" },
                { "Pragma", "no-cache" },
                { "X-Frame-Options", "SAMEORIGIN" },
                { "X-Content-Type-Options", "nosniff" },
                { "X-XSS-Protection", "1; mode=block" },
                { "X-Robots-Tag", "noindex, nofollow" }
            });
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            Response.ContentType = "text/html";
            Response.ContentEncoding = Encoding.UTF8;

            if (Request.IsSecureConnection && Configuration.HstsMaxAge > 0)
                Response.AppendHeader("Strict-Transport-Security", String.Format(CultureInfo.InvariantCulture, "max-age={0}", Configuration.HstsMaxAge));

            //Random length random characters, may help against certain https attacks.
            var filler = String.Empty;
            if (Request.IsSecureConnection && Configuration.HttpsFiller)
            {
                var rand = new Random();//Faster method? We don't need secure random.
                var fill = new byte[rand.Next(32, 256)];//Good range?
                rand.NextBytes(fill);
                filler = Convert.ToBase64String(fill);
            }

            var tokens = AntiForgery.VerificationToken(Request.Cookies);//TODO: Configure cookie name? They may have changed it on their application.
            if (tokens.Item1 != default(HttpCookie))
                Response.SetCookie(tokens.Item1);

            if (String.IsNullOrEmpty(tokens.Item2))
                throw new HttpException((int)HttpStatusCode.InternalServerError, Resource.TokenUnknownError);

            //TODO: Output some random/random-length data with the response on secure connections? This can help against certain https attacks.
            Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(String.Format(CultureInfo.InvariantCulture, Resource.TokenTemplateHtml, HttpUtility.HtmlEncode(Configuration.Id), HttpUtility.HtmlEncode(tokens.Item2), HttpUtility.HtmlEncode(filler)));
            base.Render(writer);
        }
    }
}