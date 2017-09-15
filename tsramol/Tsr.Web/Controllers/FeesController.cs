using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;
using Tsr.ToPdf;
using Tsr.Web.Common;

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

        #region Scrutinee
        public ActionResult Scrutinee()
        {
            var obj = new List<FeesScrutineeListVM>();
            return View(obj);
        }
        public async Task<ActionResult> GetListByApplicationCode(string ApplicationCode)
        {
            Application ap = await db.Applications.FirstOrDefaultAsync(x => x.ApplicationCode==ApplicationCode);

            var obj = new List<FeesScrutineeListVM>();
            if (ap.IsPackage == false || ap.IsPackage == null)
            {
                var t = db.Applied.FirstOrDefault(x => x.ApplicationId == ap.ApplicationId);
                string ps;
                bool mpf;
                if (t == null)
                { ps = "Pending"; mpf = true; }
                else
                { ps = "Success"; mpf = false; }

                FeesScrutineeListVM vm = new FeesScrutineeListVM
                {
                    ApplicationId = ap.ApplicationId,
                    ApplicationCode = ap.ApplicationCode,
                    PaymentStatus = ps,
                    MakePaymentFlag = mpf,
                    BatchName = Convert.ToDateTime(db.Batches.Find(ap.BatchId).StartDate).ToString("dd-MM-yyyy"),
                    CourseName = db.Courses.Find(ap.CourseId).CourseName,
                    Cell = ap.CellNo,
                    Email = ap.Email,
                    Name = ap.FirstName + " " + ap.MiddleName + " " + ap.LastName
                };
                obj.Add(vm);
            }
            return PartialView("ScrutineeList", obj.ToList());
        }

        public async Task<ActionResult> ScrutineeMakePayment(int? id)
        {
            var ap = await db.Applications.FindAsync(id);
            var c = await db.Courses.FindAsync(ap.CourseId);
            var cc = await db.CourseCategories.FindAsync(c.CategoryId);
            var cf = await db.CourseFees.FirstOrDefaultAsync(x => x.CourseId == ap.CourseId);

            ScrutineeMakePaymentVM vm = new ScrutineeMakePaymentVM
            {
                ApplicationId = ap.ApplicationId,
                ApplicationCode = ap.ApplicationCode
            };

            if (cc.CetRequired == true)
            {
                vm.Amount = cf.ApplicationFee;
                vm.FeesType = "ApplicationFee";
            }
            else
            {
                vm.FeesType = "CourseFee";
                if (cf.GstPercentage > 0)
                {                    
                    vm.totalFee = cf.ActualFee + ((cf.ActualFee / 100) * (decimal)cf.GstPercentage);
                    if (cf.MinBalance > 0)
                        vm.Amount = cf.MinBalance;
                    else
                        vm.Amount = cf.ActualFee + ((cf.ActualFee / 100) * (decimal)cf.GstPercentage);
                      
                }
                else
                {
                    if (cf.MinBalance > 0)
                        vm.Amount = cf.MinBalance;
                    else
                        vm.Amount = cf.ActualFee;
                }
            }
            ViewBag.PaymentMode = DropdownData.PaymentMode();
            return PartialView("ScrutineeMakePayment",vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ScrutineeMakePayment(ScrutineeMakePaymentVM obj)
        {
            var ap = await db.Applications.FindAsync(obj.ApplicationId);
            if (ap.IsPackage == false || ap.IsPackage == null)
            {
                var cc = await db.CourseCategories.FindAsync(ap.CategoryId);
                var c = await db.Courses.FindAsync(ap.CourseId);
                //var b = await db.Batches.FindAsync(ap.BatchId);
                var cf = await db.CourseFees.FindAsync(ap.CourseId);

                if (cc.CetRequired == true)
                {
                    //Applied
                    Applied nca = new Applied
                    {
                        AdmissionStatus = false,
                        ApplicationId = Convert.ToInt32(ap.ApplicationId),
                        BatchId = Convert.ToInt32(ap.BatchId),
                        CategoryId = (int)ap.CategoryId,
                        CourseId = (int)ap.CourseId
                    };
                    db.Applied.Add(nca);
                    await db.SaveChangesAsync();

                    //feeReceipt
                    FeeReceipt fr = new FeeReceipt
                    {
                        Amount = Convert.ToDecimal(obj.Amount),
                        ApplicationId = Convert.ToInt32(ap.ApplicationId),
                        PaymentMode = obj.PaymentMode,
                        PrintStatus = false,
                        FeesType = "ApplicationFee"
                    };
                    db.FeeReceipts.Add(fr);
                    await db.SaveChangesAsync();
                }
                else
                {
                    //NonCet
                    Batch bs = await db.Batches.FindAsync(ap.BatchId);
                    bs.BookedSeats = bs.BookedSeats + 1;
                    await db.SaveChangesAsync();

                    //Applied
                    Applied nca = new Applied
                    {
                        AdmissionStatus = true,
                        ApplicationId = Convert.ToInt32(ap.ApplicationId),
                        BatchId = Convert.ToInt32(ap.BatchId),
                        CategoryId = (int)ap.CategoryId,
                        CourseId = (int)ap.CourseId
                    };
                    db.Applied.Add(nca);
                    await db.SaveChangesAsync();

                    //feeReceipt
                    FeeReceipt fr = new FeeReceipt
                    {
                        Amount = Convert.ToDecimal(obj.Amount),
                        ApplicationId = Convert.ToInt32(ap.ApplicationId),
                        PaymentMode = obj.PaymentMode,
                        PrintStatus = false,
                        FeesType = "CourseFee"
                    };
                    db.FeeReceipts.Add(fr);
                    await db.SaveChangesAsync();

                    //Student Payment Details
                    var totalFee = (decimal)cf.ActualFee + (((decimal)cf.ActualFee / 100) * (decimal)cf.GstPercentage);
                    StudentFeeDetail sfd = new StudentFeeDetail
                    {
                        ApplicationId = Convert.ToInt32(ap.ApplicationId),
                        TotalFee = totalFee,
                        FeePaid = Convert.ToDecimal(obj.Amount),
                        FeeBal = totalFee - Convert.ToDecimal(obj.Amount),
                        BatchId = ap.BatchId
                    };
                    db.StudentFeeDetails.Add(sfd);
                    await db.SaveChangesAsync();

                    EmailModel em = new EmailModel
                    {
                        From = ConfigurationManager.AppSettings["admsmail"],
                        FromPass = ConfigurationManager.AppSettings["admsps"],
                        To = ap.Email,
                        Subject = "Your Admission Confirm",
                        Body = ap.FirstName + " " + ap.LastName + " " + obj.Amount + " " + c.CourseName
                    };

                    var res = await MessageService.sendEmail(em);
                }

                //return RedirectToAction("Scrutinee");
                return Json(new { success = true });
            }
            return View();
        }

        public ActionResult ScrutineePrintReceipt(int? id)
        {
            return View();
        }
        #endregion
    }
}