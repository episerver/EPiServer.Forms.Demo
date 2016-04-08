using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.PostSubmissionActor;
using EPiServer.Forms.EditView;
using EPiServer.Framework.DataAnnotations;
using EPiServer.PlugIn;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EPiServer.Forms.Demo.Implementation.Actors
{
    /// <summary>
    /// </summary>
    public class ConfigurableActor : PostSubmissionActorBase, IUIPropertyCustomCollection
    {
        /// <inheritdoc />
        public override object Run(object input)
        {
            var ret = string.Empty;

            #region Inspect some important inputs of this Actor
            
            //this.FormIdentity   // info about which Form is submitting
            //this.HttpRequestContext // Http context, which perform the submission
            //this.Model  // model of the Actor (values from EditView)
            
            //this.SubmissionData   // actual submission data
            /// transform the submission data
            var transformedData = new Dictionary<string, object>();
            foreach (var submissionKv in this.SubmissionData.Data)
            {
                transformedData.Add("Demo_" + submissionKv.Key, submissionKv.Value + "_DemoValue");
            }

            //this.SubmissionFriendlyNameInfos  // field mappings to get friendly name of each field
            
            // get information from Editor UI of this Actor            
            var configs = Model as IEnumerable<ConfigurableActorModel>;
            if (configs == null || configs.Count() < 1)
            {
                return ret;
            }
            var username = GetConfigValue(configs, "username");
            var password = GetConfigValue(configs, "password");

            #endregion

            #region Execute main business of this actor
            
            // MAIN BUSINESS SHOULD BE HERE: use the usename, password, to send the transformedData to 3rd party server, or save to XML file
            
            #endregion

            return ret;
        }

        private string GetConfigValue(IEnumerable<ConfigurableActorModel> configs, string key)
        {
            var found = configs.Where(c => c.ConfigKey == key).FirstOrDefault();
            if (found != null)
            {
                return found.ConfigValue;
            }

            return string.Empty;
        }

        #region IUIPropertyCustomCollection Members

        /// <inheritdoc />
        public virtual Type PropertyType
        {
            get
            {
                return typeof(PropertyForDisplayingConfigurableActor);
            }
        }

        #endregion
    }




    /// <summary>
    /// Property definition for the Actor
    /// </summary>
    [EditorHint("ConfigurableActorPropertyHint")]
    [PropertyDefinitionTypePlugIn(DisplayName = "ConfigurableActorProp")]
    public class PropertyForDisplayingConfigurableActor : PropertyGenericList<ConfigurableActorModel> { }


    /// <summary>
    /// Editor descriptor class, for using Dojo widget CollectionEditor to render.
    /// Inherit from <see cref="CollectionEditorDescriptor"/>, it will be rendered as a grid UI.
    /// </summary>
    [EditorDescriptorRegistration(TargetType = typeof(IEnumerable<ConfigurableActorModel>), UIHint = "ConfigurableActorPropertyHint")]
    public class ConfigurableActorEditorDescriptor : CollectionEditorDescriptor<ConfigurableActorModel>
    {
        public ConfigurableActorEditorDescriptor()
        {
            ClientEditingClass = "epi-forms/contentediting/editors/CollectionEditor";
        }
    }




}