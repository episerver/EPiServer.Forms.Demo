using EPiServer.Forms.Core.PostSubmissionActor;
using System;
using System.Net;
using System.Web;

namespace EPiServer.Forms.Demo.Implementation.Actors
{
    /// <summary>
    /// This very simple actor will perform on Form finalizing moment only. It has no UI for Editor.
    /// It will silently write a Cookie to Response.
    /// </summary>
    public class InvisibleActor : PostSubmissionActorBase
    {
        /// <summary>
        /// We want to send cookie to Visitor (by modifying the Response), so we need to set this property to true
        /// </summary>
        public override bool IsSyncedWithSubmissionProcess { get { return true; } }

        public override object Run(object input)
        {
            this.HttpResponseContext.Cookies.Add(new HttpCookie("InvisibleActor", DateTime.Now.Ticks.ToString()));
            return "";
        }
    }
}