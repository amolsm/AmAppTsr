using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tsr.Web.Controllers
{
    public class CertificationController : Controller
    {
        #region CheckList
        public ActionResult CheckList()
        {
            return View();
        }
        #endregion

        #region DesignCertificate
        public ActionResult DesignCertificate()
        {
            return View();
        }
        #endregion
    }
}