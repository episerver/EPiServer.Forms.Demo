using EPiServer.Forms.Core.Events;
using EPiServer.Forms.Core.Models;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System.Linq;

namespace EPiServer.Forms.Demo
{
    /// <summary>
    /// Listen to the Events emitting of Forms on server-side
    /// </summary>
    [InitializableModule]
    public class FormsEventsInitModule : IInitializableModule
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(FormsEventsInitModule));

        public void Initialize(InitializationEngine context)
        {
            // get the Forms Event Manager
            var formsEvents = ServiceLocator.Current.GetInstance<FormsEvents>();

            // listen to its events
            formsEvents.FormsStructureChange += OnStructureChange;
            formsEvents.FormsSubmitting += OnSubmitting1;
            formsEvents.FormsSubmitting += OnSubmitting2;

            formsEvents.FormsStepSubmitted += OnStepSubmit;
            formsEvents.FormsSubmissionFinalized += OnFormFinalized;
        }

        private void OnFormFinalized(object sender, FormsEventArgs e)
        {
            _logger.Critical("Form:{0}[{1}] is finalized",
                e.FormsContent.Name, e.FormsContent.ContentGuid);
        }

        private void OnStepSubmit(object sender, FormsEventArgs e)
        {
            _logger.Critical("Form:{0}[{1}] has a step submitted",
                e.FormsContent.Name, e.FormsContent.ContentGuid);
        }

        private void OnSubmitting1(object sender, FormsEventArgs e)
        {
            _logger.Critical("You are submitting Form:{0}[{1}]", e.FormsContent.Name, e.FormsContent.ContentGuid);
        }
        private void OnSubmitting2(object sender, FormsEventArgs args)
        {
            var valueOfFieldToCancel = "xxx";

            var e = args as FormsSubmittingEventArgs;
            var firstField = e.SubmissionData.Data.First();
            if (firstField.Value as string == valueOfFieldToCancel.ToString())
            {
                e.CancelAction = true;
                e.CancelReason = string.Format("AlloyMVC: firstElementValue={0} is too rude. Cancelled.", firstField.Value);
            }
            else
            {
                e.CancelReason = string.Format("AlloyMVC: firstElementValue={0} is OK", firstField.Value);
            }
        }

        private void OnStructureChange(object sender, FormsEventArgs e)
        {
            _logger.Critical("Form:{0}[{1}] has changed its structure",
               e.FormsContent.Name, e.FormsContent.ContentGuid);

            if (e.Data is FormStructure)
            {
                _logger.Critical("New Form structure: {0}", string.Join(",", (e.Data as FormStructure).AllFields));
            }
        }








        public void Uninitialize(InitializationEngine context) { }
        public void Preload(string[] parameters) { }
    }
}