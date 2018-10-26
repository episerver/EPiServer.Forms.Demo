using EPiServer.Forms.Internal.Security;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Forms.Core;
using System.Web;
using System.Web.Mvc;

namespace EPiServer.Forms.Demo.Security
{
    /// <summary>
    /// Demo how to custom AntiForgeryService.
    /// </summary>
    [ServiceConfiguration(typeof(IAntiForgeryService))]
    public class CustomAntiForgeryService: AntiForgeryService
    {
        public override MvcHtmlString GenerateToken(HtmlHelper html, HttpContextBase httpContext, IFormContainerBlock formContainerBlock)
        {
            var defaultToken = base.GenerateToken(html, httpContext, formContainerBlock);
            var output = defaultToken.ToHtmlString();
            var guid = Guid.NewGuid();
            // remember the token in user session
            httpContext.Session[guid.ToString()] = true;

            output += $"<input type='hidden' name='SessionToken' value='{guid.ToString()}' />";
            return MvcHtmlString.Create(output);
        }
    }
}
