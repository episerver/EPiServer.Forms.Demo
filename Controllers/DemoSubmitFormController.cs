using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using EPiServer.Logging;
using System.Threading;
using EPiServer.Shell;
using EPiServer.Forms.Controllers;
using EPiServer.Forms.EditView.Internal;
using EPiServer.Forms.Configuration;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Core.Internal;
namespace EPiServer.Forms.Demo.Controllers
{
    /// <summary>
    /// In order to use this Controller, you have to
    /// * modify the Forms.config, coreController="/EPiServer.Forms/DemoSubmitForm"
    /// 
    /// * Register route to the Controller in Global.asax
    /// <code>
    /// protected override void RegisterRoutes(System.Web.Routing.RouteCollection routes) {
    ///    base.RegisterRoutes(routes);
    ///    RouteTable.Routes.MapRoute("DemoSubmitFormController", "demo/{controller}/{action}", 
    ///        new { controller = "demosubmitform" , action = "Index" });
    /// }
    /// </code>
    /// </summary>
    [ServiceConfiguration(typeof(IDataSubmitController))]
    public class DemoSubmitFormController : DataSubmitController
    {
        protected Injected<IEPiServerFormsImplementationConfig> _formConfig;
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(DemoSubmitFormController));
        public ActionResult Index()
        {
            return this.Content("This is a custom implementation Forms.DataSubmitController");
        }

        /// <summary>
        /// TECH NOTE:
        /// This function will read an ascx template from disk to generate the init script (JavaScript object) for each form.
        /// Views/FormContainerInitScript.ascx is template for generating init script for each Form.
        /// 
        /// You HAVE TO modify the path to your template.
        /// Examlple: If this controller is put to AlloyMVC, please put the FormContainerInitScript.ascx to AlloyMVC Views folder
        /// </summary>
        /// <param name="formGuid"></param>
        /// <param name="formLanguage"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override ActionResult GetFormInitScript(Guid formGuid, string formLanguage)
        {
            if (_formConfig.Service.WorkInNonJSMode)
            {
                return new EmptyResult();
            }

            var formContainer = new FormIdentity(formGuid, formLanguage).GetFormBlock();
            if (formContainer == null)
            {
                _logger.Debug("FAIL to init form: could not get Form with Guid = {0}.", formGuid);
                return JavaScript(string.Format("console.error('FAIL to GetFormInitScript: could not get Form with Guid = {0}');", formGuid));
            }

            _formBusinessService.Service.BuildFormModel(formContainer);
            var viewDataDic = new ViewDataDictionary<FormContainerBlock>(formContainer);
            var form = formContainer.Form;
            viewDataDic.Add("AllStepsAreNotLinked", _dataSubmissionService.Service.IsAllStepsAreNotLinked(form));
            viewDataDic.Add("FormHasNoStep_VirtualStepCreated", form.Steps.Count() > 0 && form.Steps.FirstOrDefault().SourceContent == null);
            viewDataDic.Add("FormHasNothing", form.Steps.Count() == 0);

            if (!string.IsNullOrEmpty(formLanguage))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(formLanguage);
            }

            var js = ControllerContext.RenderWebFormViewToString<FormContainerBlock>(Paths.ToResource(typeof(EPiServer.Forms.Controllers.DataSubmitController), "Views/FormContainerInitScript.ascx"), formContainer, viewDataDic);

            return JavaScript(js);
        }

    }
}
