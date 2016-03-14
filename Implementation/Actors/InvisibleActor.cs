using EPiServer.Forms.Core.PostSubmissionActor;

namespace EPiServer.Forms.Demo.Implementation.Actors
{
    /// <summary>
    /// This very simple actor will perform on Form finalizing moment only. It has no UI for Editor.
    /// </summary>
    public class InvisibleActor : PostSubmissionActorBase
    {   
        public override object Run(object input)
        {
            return "";
        }
    }
}