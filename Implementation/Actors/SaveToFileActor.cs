using EPiServer.Forms.Core.PostSubmissionActor;
using EPiServer.Forms.Core.PostSubmissionActor.Internal;
using EPiServer.Forms.Helpers.Internal;
using System;
using System.IO;
using System.Linq;

namespace EPiServer.Forms.Demo.Implementation.Actors
{
    /// <summary>
    ///     Actor that save submission data to text file and will be run as synchronously before saving into DDS
    /// </summary>
    public class SaveToFileActor : PostSubmissionActorBase, ISyncOrderedSubmissionActor
    {
        /// <summary>
        ///     Ascennding Order of running actors. Set Order smaller 1000 to make actor run before saving data to storage.
        /// </summary>
        public int Order => 1;

        public override object Run(object input)
        {
            var submissionJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.SubmissionData.Data);
            /// Actor which wants to cancel the submission process should return this object by set CancelSubmit to true.
            /// And set ErrorMessage to show the reason for cancellation on UI.
            var result = new SubmissionActorResult { CancelSubmit = false, ErrorMessage = string.Empty };


            var formContainerBlock = this.FormIdentity.GetFormBlock();
            var allFormsElements = formContainerBlock.Form.Steps.SelectMany(st => st.Elements);

            // single choice element with two radio button values: Yes/No. The element name in edit mode is 'Email Confirmation'
            var emailConfirmationElement = allFormsElements.FirstOrDefault(x => x.SourceContent.Name == "Email Confirmation");
            if (emailConfirmationElement != null)
            {
                // get user selected value
                var confirmedValue = this.SubmissionData.Data[emailConfirmationElement.ElementName]?.ToString();

                // and do something with it
            }

            try
            {
                this.AppendText(submissionJson);
            }
            catch (Exception ex)
            {
                result.CancelSubmit = true;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private void AppendText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var filePath = "C:/Temp/App_Data/data.xml";
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(text);
            }
        }
    }
}
