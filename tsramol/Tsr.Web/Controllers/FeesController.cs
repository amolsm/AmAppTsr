using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;
using Tsr.ToPdf;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ViewPaymentDetails(FeeReceipt obj)
        {
            if (ModelState.IsValid)
            {
                db.Entry(obj).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                return Json(new { success = true });
            }
            return PartialView("ViewPaymentDetails", obj);
        }

        [HttpGet]
        public ActionResult ViewPaymentDetails(int? Id)
        {


            var list = (from ap in db.Applications.AsEnumerable()
                        join b in db.Batches on ap.BatchId equals b.BatchId
                        join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                        join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                        join cr in db.Courses on ap.CourseId equals cr.CourseId
                        where (ap.ApplicationId == Id)
                        select new FeesViewPaymentDetailsVM
                        {
                            FeeReceiptNo = fr.FeeReceiptNo,
                            PaymentMode = fr.PaymentMode,
                            FeesType = fr.FeesType,
                            ReceiptDate = fr.ReceiptDate,
                            Name = ap.FirstName + " " + ap.LastName,
                            Course = cr.CourseCode,
                            Batch = b.BatchCode,
                            StudentId = ap.ApplicationId,
                            ApplicationId = ap.ApplicationId,
                            FeePaid = sd.FeePaid,
                            FeeBal = sd.FeeBal,
                            AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(sd.FeePaid))


                        }).ToList();

            return new PdfActionResult(list);

        }
    }
}