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

        #region common
        public ActionResult FillStudentsBatchwise(int BatchId)
        {
            var Students = from apl in db.Applied
                           join ap in db.Applications on apl.ApplicationId equals ap.ApplicationId
                           where (apl.BatchId == BatchId && apl.AdmissionStatus == true)
                           select new { ApplicationId = apl.ApplicationId, Name = ap.FullName};                          

            return Json(Students, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region oldReceipt
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
            if (Id != null)
            {


                var list = (from ap in db.Applications.AsEnumerable()
                            join b in db.Batches on ap.BatchId equals b.BatchId
                            join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                            join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                            join cr in db.Courses on ap.CourseId equals cr.CourseId
                            //join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId

                            where (ap.ApplicationId == Id)
                            
                            select new FeesViewPaymentDetailsVM
                            {
                                FeeReceiptNo = fr.FeeReceiptNo,
                                PaymentMode = fr.PaymentMode,
                                FeesType = fr.FeesType,
                                ReceiptDate = fr.ReceiptDate,
                                Name = ap.FirstName + " " + ap.LastName,
                                Course = cr.CourseName,
                                Batch = b.BatchCode,
                                StudentId = ap.ApplicationId,
                                ApplicationId = ap.ApplicationId,
                                ApplicationCode = ap.ApplicationCode,
                                FeePaid = fr.Amount,
                                FeeBal = sd.FeeBal,
                                AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                BatchStartDate = b.StartDate,
                                PaymentDate = fr.ReceiptDate


                            }).ToList();

                return new PdfActionResult(list);
            }
            return View("FeeReciept");
        }


        [HttpGet]
        public ActionResult ViewAllPaymentDetails(int? Id)
        {
            if (Id != null)
            {


                var list = (from ap in db.Applications.AsEnumerable()
                            join b in db.Batches on ap.BatchId equals b.BatchId
                            join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                            join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                            join cr in db.Courses on ap.CourseId equals cr.CourseId
                            //join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId

                            where (ap.BatchId == Id)
                            select new FeesViewPaymentDetailsVM
                            {
                                FeeReceiptNo = fr.FeeReceiptNo,
                                PaymentMode = fr.PaymentMode,
                                FeesType = fr.FeesType,
                                ReceiptDate = fr.ReceiptDate,
                                Name = ap.FirstName + " " + ap.LastName,
                                Course = cr.CourseName,
                                Batch = b.BatchCode,
                                StudentId = ap.ApplicationId,
                                ApplicationId = ap.ApplicationId,
                                ApplicationCode = ap.ApplicationCode,
                                FeePaid = fr.Amount,
                                FeeBal = sd.FeeBal,
                                AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                BatchStartDate = b.StartDate,
                                PaymentDate = fr.ReceiptDate

                            }).ToList();

                return new PdfActionResult(list);
            }
            return View("FeeReciept");
        }
        #endregion

        #region CoursePayment
        public ActionResult CoursePayments()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
           
            return View();
        }

        public ActionResult CoursePaymentsSearch(int? ApplicationId)
        {
            var list = from apd in db.Applied
                       join ap in db.Applications on apd.ApplicationId equals ap.ApplicationId
                       join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                       join b in db.Batches on apd.BatchId equals b.BatchId
                       where (apd.ApplicationId == ApplicationId)
                       select new FeesApplicantList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           //BatchCode = (b.StartDate).ToString("dd-MM-yyyy"),
                           Name = ap.FullName,
                           PaidAmount = sd.FeePaid.ToString(),
                           Email = ap.Email,
                           Cell = ap.CellNo,
                           BalanceAmount = sd.FeeBal.ToString()
                           //FeesType = fr.FeesType,
                           //PaymentMode = fr.PaymentMode,
                           //FeeReceiptNo = fr.FeeReceiptNo
                       };

            List<FeesApplicantList> obj = new List<FeesApplicantList>();
            obj.Add(list.FirstOrDefault());
            return PartialView("CoursePaymentsList",obj);
        }

        public async Task<ActionResult> CoursePaymentsMP(int? id)
        {
            var ap = await db.Applications.FindAsync(id);
            var sfd = await db.StudentFeeDetails.FirstOrDefaultAsync(x => x.ApplicationId == id);

            ScrutineeMakePaymentVM vm = new ScrutineeMakePaymentVM
            {
                ApplicationId = ap.ApplicationId,
                ApplicationCode = ap.ApplicationCode,
                Amount = sfd.FeeBal,
                FeesType = "CourseFee"
            };

            
            ViewBag.PaymentMode = DropdownData.PaymentMode();
            return PartialView("CoursePaymentsMP", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CoursePaymentsMP(ScrutineeMakePaymentVM obj)
        {
            var ap = db.Applications.Find(obj.ApplicationId);
            if (ap.IsPackage == false || ap.IsPackage == null)
            {

                StudentFeeDetail sfd = db.StudentFeeDetails.FirstOrDefault(x => x.ApplicationId == obj.ApplicationId);
                sfd.FeePaid = sfd.FeePaid + obj.Amount;
                sfd.FeeBal = sfd.FeeBal - obj.Amount;

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
                db.SaveChanges();

                  return Json(new { success = true });

            }
            return View();
        }
        #endregion

        #region PackagePayment

        #endregion

        #region PrintReceipts
        public ActionResult CoursePaymentPrintReceipt(int?  id )
         {  
           
            if (id != null)
            {
                var receiptid = db.FeeReceipts.Where(x => (x.ApplicationId == (int)id) && (x.PrintStatus == false)).Select(x => x.FeeReceiptId).FirstOrDefault();

                var list = (from ap in db.Applications.AsEnumerable()
                            join b in db.Batches on ap.BatchId equals b.BatchId
                            join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                            join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                            join cr in db.Courses on ap.CourseId equals cr.CourseId
                            //join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId

                            where (fr.FeeReceiptId == receiptid)
                            select new FeesViewPaymentDetailsVM
                            {
                                FeeReceiptNo = fr.FeeReceiptId.ToString(),//fr.FeeReceiptNo,
                                PaymentMode = fr.PaymentMode,
                                FeesType = fr.FeesType,
                                ReceiptDate = fr.ReceiptDate,
                                Name = ap.FirstName + " " + ap.LastName,
                                Course = cr.CourseName,
                                Batch = b.BatchCode,
                                StudentId = ap.ApplicationId,
                                ApplicationId = ap.ApplicationId,
                                ApplicationCode = ap.ApplicationCode,
                                FeePaid = fr.Amount,
                                FeeBal = sd.FeeBal,
                                AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                BatchStartDate = b.StartDate,
                                PaymentDate = DateTime.Now,
                                FeeReceiptId = fr.FeeReceiptId


                            }).ToList();
                foreach (var l in list)
                {
                    FeeReceipt fr = db.FeeReceipts.Find(l.FeeReceiptId);
                    fr.PrintStatus = true;
                    fr.ReceiptDate = DateTime.Now;
                    db.SaveChanges();

                }

                return new PdfActionResult(list);
            }

            return View("CoursePayments");
        }
        #endregion
        #region Scrutinee
        public ActionResult Scrutinee()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            return View();
        }
        public ActionResult FillStudentsForScrutinee(int BatchId)
        {
            var Students = from ap in db.Applications
                    
                    where (ap.BatchId == BatchId && ap.Scrutinee == true)
                    select new { ApplicationId = ap.ApplicationId, Name = ap.FullName};

            
            return Json(Students, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetListScrutineePayment(int ApplicationId)
        {
            Application ap = await db.Applications.FindAsync(ApplicationId);

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
                
                var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == ap.CourseId);

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
                    decimal tax ;
                    if (cf.GstPercentage > 0)
                        tax = (((decimal)cf.ActualFee / 100) * (decimal)cf.GstPercentage);
                    else
                        tax = 0;
                    var totalFee = (decimal)cf.ActualFee + tax;
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
            if (id != null)
            {


                var list = (from ap in db.Applications.AsEnumerable()
                            join b in db.Batches on ap.BatchId equals b.BatchId
                            join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                            join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                            join cr in db.Courses on ap.CourseId equals cr.CourseId
                            //join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId

                            where (ap.ApplicationId == id)
                            select new FeesViewPaymentDetailsVM
                            {
                                FeeReceiptNo = fr.FeeReceiptId.ToString(),//fr.FeeReceiptNo,
                                PaymentMode = fr.PaymentMode,
                                FeesType = fr.FeesType,
                                ReceiptDate = fr.ReceiptDate,
                                Name = ap.FirstName + " " + ap.LastName,
                                Course = cr.CourseName,
                                Batch = b.BatchCode,
                                StudentId = ap.ApplicationId,
                                ApplicationId = ap.ApplicationId,
                                ApplicationCode = ap.ApplicationCode,
                                FeePaid = fr.Amount,
                                FeeBal = sd.FeeBal,
                                AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                BatchStartDate = b.StartDate,
                                PaymentDate = DateTime.Now,
                                FeeReceiptId=fr.FeeReceiptId


                            }).ToList();
                foreach (var l in list)
                {
                    FeeReceipt fr = db.FeeReceipts.Find(l.FeeReceiptId);
                    fr.PrintStatus = true;
                    fr.ReceiptDate = DateTime.Now;
                    db.SaveChanges();
                  
                }

                return new PdfActionResult(list);
            }
          
            return View();
        }
        #endregion
    }
}