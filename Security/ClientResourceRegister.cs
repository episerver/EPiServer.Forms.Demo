using EPiServer.Framework.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Forms.Demo.Security
{
    /// <summary>
    /// Register client resource to add extra token into request header.
    /// </summary>
    [ClientResourceRegister]
    public class ClientResourceRegister : IClientResourceRegister
    {
        public void RegisterResources(IRequiredClientResourceList requiredResources, HttpContextBase context)
        {
            var script = @"
                        var originalGetAntiForgeryToken = epi.EPiServer.Forms.Extension.getAntiForgeryToken;
                        $.extend(true, epi.EPiServer.Forms, {
                            Extension: {
                                getAntiForgeryToken: function (workingFormInfo) {
                                    var token = originalGetAntiForgeryToken(workingFormInfo);
                                    var sessionToken = $('input[name=SessionToken]', workingFormInfo.$workingForm).val();
                                    $.extend(token, { sessionToken: sessionToken});

                                    return token;
                                }
                            }
                        });";
            requiredResources.RequireScriptInline(script, "AppendFormSessionToken.js", new List<string> { "EPiServerForms.js" }).AtFooter();
        }
    }
}
