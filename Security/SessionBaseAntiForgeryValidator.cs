using EPiServer.Forms.Internal.Security;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Forms.Demo.Security
{
    /// <summary>
    /// Anti-forgery validation base on session.
    /// </summary>
    [ServiceConfiguration(typeof(IAntiForgeryValidator))]
    public class SessionBaseAntiForgeryValidator : IAntiForgeryValidator
    {
        private const string SessionBaseTokenKey = "SessionToken";

        public int Order
        {
            get
            {
                return 50;
            }
        }

        public bool Validate(HttpContextBase httpContext)
        {
            var token = GetToken(httpContext);
            if (httpContext.Session[token] == null)
            {
                return false;
            }

            return true;
        }

        protected string GetToken(HttpContextBase httpContext)
        {
            var token = httpContext.Request.Headers[SessionBaseTokenKey];
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.Request.Form[SessionBaseTokenKey];
            }
            return token;
        }
    }
}
