using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.AntiForgery
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        public const string DefaultHeaderName = "X-CSRF-Token";
        public const string DefaultInputName = "__RequestVerificationToken";
        public const string DefaultNotValidMessage = "The required anti-forgery input \"{0}\" is invalid.";

        public ValidateAntiForgeryTokenAttribute()
        {
            this.HeaderName = DefaultHeaderName;
            this.InputName = DefaultInputName;
            this.NotValidMessage = DefaultNotValidMessage;
        }

        public string HeaderName { get; set; }

        public string InputName { get; set; }

        public string NotValidMessage { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            var headers = actionContext.Request.Headers;
            if (!headers.Contains(HeaderName))
            {
                actionContext.ModelState.AddModelError(String.Empty, String.Format(CultureInfo.InvariantCulture, this.NotValidMessage, this.HeaderName));
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                return;
            }

            var cookie = headers.GetCookies().Select(c => c[AntiForgeryConfig.CookieName]).FirstOrDefault();
            var cookieToken = (cookie == null) ? default(string) : cookie.Value;
            var headerToken = headers.GetValues(HeaderName).FirstOrDefault();

            try
            {
                //Check for header token before input token.
                if (String.IsNullOrWhiteSpace(headerToken))
                    System.Web.Helpers.AntiForgery.Validate();
                else
                    System.Web.Helpers.AntiForgery.Validate(cookieToken, headerToken);//How many times can a token be re-used? Is there any sort of mechanism to prevent re-use?
            }
            catch (System.Web.Mvc.HttpAntiForgeryException ex)
            {
                actionContext.ModelState.AddModelError(String.Empty, ex);
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                return;
            }
        }
    }
}