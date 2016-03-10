using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Forms.Controllers;
using EPiServer.ServiceLocation;
using System.Web;
using System.Web.Mvc;

namespace EPiServer.Forms.Demo.Controllers
{
    /// <summary>
    /// Override the default DataSubmitController
    /// </summary>
    [ServiceConfiguration(typeof(IDataSubmitController))]
    public class DemoSubmitFormController : DataSubmitController
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
