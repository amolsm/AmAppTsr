using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Models;
using Tsr.Infra;

namespace Tsr.Web.Controllers
{
    public class FeesController : Controller
    {
        AppContext db = new AppContext();
        public ActionResult FeeReciept()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            var obj = new List<FeesApplicantList>();
            return View(obj);
        }

        [HttpGet]
        public ActionResult GetApplicantList(int? BatchId)
        {
            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       //join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                       join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                       where (b.BatchId == BatchId)
                       select new FeesApplicantList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchCode = b.BatchCode,
                           Name = ap.FirstName + " " + ap.LastName,
                           PaidAmount = sd.FeePaid.ToString(),
                           Email = ap.Email,
                           Cell = ap.CellNo,
                           BalanceAmount = sd.FeeBal.ToString()
                           //FeesType = fr.FeesType,
                           //PaymentMode = fr.PaymentMode,
                           //FeeReceiptNo = fr.FeeReceiptNo
                       };


            return PartialView("ApplicantsList", list.ToList());
        }
    }
}