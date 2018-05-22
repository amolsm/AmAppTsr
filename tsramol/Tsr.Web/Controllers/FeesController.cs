using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
                        

                            where (ap.ApplicationId == Id)
                            
                            select new FeesViewPaymentDetailsVM
                            {
                                FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),
                                PaymentMode = fr.PaymentMode,
                                FeesType = fr.FeesType,
                                ReceiptDate = fr.ReceiptDate,
                                Name = ap.FirstName + " " + ap.LastName,
                                Course = cr.ShortName,
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
                           
                            where (ap.BatchId == Id)
                            select new FeesViewPaymentDetailsVM
                            {
                                FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),
                                PaymentMode = fr.PaymentMode,
                                FeesType = fr.FeesType,
                                ReceiptDate = fr.ReceiptDate,
                                Name = ap.FullName,
                                Course = cr.ShortName,
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
            ViewBag.Packages = new SelectList(db.packages.ToList(), "PackageId", "PackageName");
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
            if (ModelState.IsValid)
            {
                string loc = string.Empty;
                var fy = DateTime.Now.Month >= 4 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                if (User.IsInRole("Fees") || User.IsInRole("Admin"))
                    loc = "Nhava";
                else
                    loc = "Worli";

                var rno = Valid.CreateRecieptNo(loc, obj.PaymentMode, fy.ToString());

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
                        FeesType = "CourseFee",
                        ReceiptDate = DateTime.Now,
                        DDNo = obj.DdNo,
                        ChequeNo = obj.ChequeNo,
                        BankName = obj.BankName,
                        FeeReceiptNo = rno,
                        Fy = fy.ToString(),
                        Location = loc
                    };
                    db.FeeReceipts.Add(fr);
                    db.SaveChanges();


                    return Json(new { success = true });

                }
                else
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
                        FeesType = "CourseFee",
                        ReceiptDate = DateTime.Now,
                        DDNo = obj.DdNo,
                        ChequeNo = obj.ChequeNo,
                        BankName = obj.BankName,
                        FeeReceiptNo = rno,
                        Fy = fy.ToString(),
                        Location = loc
                    };
                    db.FeeReceipts.Add(fr);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }
            ViewBag.PaymentMode = DropdownData.PaymentMode();
            return PartialView("CoursePaymentsMP", obj);
        }
        #endregion

        #region PackagePayment

        #endregion

        #region PrintReceipts
        public ActionResult CoursePaymentPrintReceipt(int?  id )
         {

            if (id != null)
            {
                var app = db.Applications.Find(id);
                if (app.IsPackage == null || app.IsPackage == false)
                {
                    var list1 = (
                              
                                 from ap in db.Applications.AsEnumerable()
                                 join b in db.Batches on ap.BatchId equals b.BatchId
                                 join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                                 join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                                 into sdf
                                 from sd in sdf.DefaultIfEmpty()
                                 join cr in db.Courses on ap.CourseId equals cr.CourseId
                                 join gst in db.CourseFees on ap.CourseId equals gst.CourseId into pi4
                                 from gst in pi4.DefaultIfEmpty()

                                 where (ap.ApplicationId == id)

                                select new FeesViewPaymentDetailsVM
                                {
                                    // FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),//fr.FeeReceiptNo,
                                    FeeReceiptNo = fr.FeeReceiptNo,
                                    PaymentMode = fr.PaymentMode,
                                    FeesType = fr.FeesType,
                                    ReceiptDate = fr.ReceiptDate,
                                    Name = ap.FullName,
                                    Course = cr.ShortName,
                                    Batch = b.BatchCode,
                                    StudentId = ap.ApplicationId,
                                    ApplicationId = ap.ApplicationId,
                                    ApplicationCode = ap.ApplicationCode,
                                    FeePaid = fr.Amount,
                                    FeeBal = (sd == null) ? 0 : sd.FeeBal,
                                    AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                    BatchStartDate = b.StartDate,
                                    PaymentDate = DateTime.Now,
                                    FeeReceiptId = fr.FeeReceiptId,
                                    GST = (gst == null) ? 0 : Convert.ToDecimal(gst.GstPercentage)

                                }).ToList();

                    var list = list1.OrderByDescending(x => x.FeeReceiptId).Take(1).ToList();
                    foreach (var l in list)
                    {
                        FeeReceipt fr = db.FeeReceipts.Find(l.FeeReceiptId);
                        fr.PrintStatus = true;
                        //fr.ReceiptDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    list.AddRange(Enumerable.Repeat(0, 1).Select(
                        x => new FeesViewPaymentDetailsVM()
                        {
                            FeeReceiptNo = list[0].FeeReceiptNo,
                            PaymentMode = list[0].PaymentMode,
                            FeesType = list[0].FeesType,
                            ReceiptDate = list[0].ReceiptDate,
                            Name = list[0].Name,
                            Course = list[0].Course,
                            Batch = list[0].Batch,
                            StudentId = list[0].StudentId,
                            ApplicationId = list[0].ApplicationId,
                            ApplicationCode = list[0].ApplicationCode,
                            FeePaid = list[0].FeePaid,
                            FeeBal = list[0].FeeBal,
                            AmountInRs = list[0].AmountInRs,
                            BatchStartDate = list[0].BatchStartDate,
                            PaymentDate = list[0].PaymentDate,
                            FeeReceiptId = list[0].FeeReceiptId,
                            GST = list[0].GST
                        }));
                    return new PdfActionResult(list);
                }
                else
                {
                    var list1 = (from ap in db.Applications.AsEnumerable()
                                 join pk in db.packages on ap.PackageId equals pk.PackageId
                                 join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                                 join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                                 where (ap.ApplicationId == id)
                                 select new FeesViewPaymentDetailsVM
                                 {
                                     //FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),//fr.FeeReceiptNo,
                                     FeeReceiptNo = fr.FeeReceiptNo,
                                     PaymentMode = fr.PaymentMode,
                                     FeesType = fr.FeesType,
                                     ReceiptDate = fr.ReceiptDate,
                                     Name = ap.FirstName + " " + ap.LastName,
                                     Course = pk.PackageName + " (Package)",
                                     Batch = "",
                                     StudentId = ap.ApplicationId,
                                     ApplicationId = ap.ApplicationId,
                                     ApplicationCode = ap.ApplicationCode,
                                     FeePaid = fr.Amount,
                                     FeeBal = sd.FeeBal,
                                     AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                     BatchStartDate = null,
                                     PaymentDate = DateTime.Now,
                                     FeeReceiptId = fr.FeeReceiptId,
                                     GST = 0


                                }).ToList();
                    var list = list1.OrderByDescending(x => x.FeeReceiptId).Take(1).ToList();
                    foreach (var l in list)
                    {
                        FeeReceipt fr = db.FeeReceipts.Find(l.FeeReceiptId);
                        fr.PrintStatus = true;
                        //fr.ReceiptDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    list.AddRange(Enumerable.Repeat(0, 1).Select(
                      x => new FeesViewPaymentDetailsVM()
                      {
                          FeeReceiptNo = list[0].FeeReceiptNo,
                          PaymentMode = list[0].PaymentMode,
                          FeesType = list[0].FeesType,
                          ReceiptDate = list[0].ReceiptDate,
                          Name = list[0].Name,
                          Course = list[0].Course,
                          Batch = list[0].Batch,
                          StudentId = list[0].StudentId,
                          ApplicationId = list[0].ApplicationId,
                          ApplicationCode = list[0].ApplicationCode,
                          FeePaid = list[0].FeePaid,
                          FeeBal = list[0].FeeBal,
                          AmountInRs = list[0].AmountInRs,
                          BatchStartDate = list[0].BatchStartDate,
                          PaymentDate = list[0].PaymentDate,
                          FeeReceiptId = list[0].FeeReceiptId,
                          GST = list[0].GST
                      }));

                    return new PdfActionResult(list);
                }
            }
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            return View("CoursePayments");
        }
        #endregion

        #region Scrutinee
        public ActionResult Scrutinee()
        {
            
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.Packages = new SelectList(db.packages.ToList(), "PackageId", "PackageName");
            return View();
        }
        public ActionResult FillStudentsForScrutinee(int BatchId)
        {
            var Students = from ap in db.Applications
                    
                    where (ap.BatchId == BatchId && ap.Scrutinee == true)
                    select new { ApplicationId = ap.ApplicationId, Name = ap.FullName};

            
            return Json(Students, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillStudentsForPackageScrutinee(int PackageId)
        {
            var Students = from ap in db.Applications

                           where (ap.PackageId == PackageId && ap.Scrutinee == true)
                           select new { ApplicationId = ap.ApplicationId, Name = ap.FullName };


            return Json(Students, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetListScrutineePayment(int ApplicationId)
        {
            Application ap = await db.Applications.FindAsync(ApplicationId);

            var obj = new List<FeesScrutineeListVM>();
            
                var t = db.Applied.FirstOrDefault(x => x.ApplicationId == ap.ApplicationId);
                string ps;
                bool mpf;
                if (t == null)
                { ps = "Pending"; mpf = true; }
                else
                { ps = "Success"; mpf = false; }
            if (ap.IsPackage == false || ap.IsPackage == null)
            {
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
            else
            {
                if (mpf == true)
                {
                    var l = db.ApplicationPackageDetails.Where(x => x.ApplicationId == ap.ApplicationId).ToList();
                    foreach (var item in l)
                    {
                        var bz = await db.Batches.FindAsync(item.BatchId);
                        var bn = Convert.ToDateTime(bz.StartDate).ToString("dd-MM-yyyy");
                        var cn = db.Courses.Find(item.CourseId).CourseName;
                        FeesScrutineeListVM vm1 = new FeesScrutineeListVM
                        {
                            ApplicationId = ap.ApplicationId,
                            ApplicationCode = ap.ApplicationCode,
                            PaymentStatus = ps,
                            MakePaymentFlag = mpf,
                            BatchName = bn,
                            CourseName = cn ,
                            Cell = ap.CellNo,
                            Email = ap.Email,
                            Name = ap.FirstName + " " + ap.MiddleName + " " + ap.LastName
                        };
                        obj.Add(vm1);
                    }
                }
                else
                {
                    var l2 = db.Applied.Where(x => x.ApplicationId == ap.ApplicationId).ToList();
                    foreach (var item in l2)
                    {
                        FeesScrutineeListVM vm2 = new FeesScrutineeListVM
                        {
                            ApplicationId = ap.ApplicationId,
                            ApplicationCode = ap.ApplicationCode,
                            PaymentStatus = ps,
                            MakePaymentFlag = mpf,
                            BatchName = Convert.ToDateTime(db.Batches.Find(item.BatchId).StartDate).ToString("dd-MM-yyyy"),
                            CourseName = db.Courses.Find(item.CourseId).CourseName,
                            Cell = ap.CellNo,
                            Email = ap.Email,
                            Name = ap.FirstName + " " + ap.MiddleName + " " + ap.LastName
                        };
                        obj.Add(vm2);
                    }
                }
            }
            return PartialView("ScrutineeList", obj.ToList());
        }

        public async Task<ActionResult> ScrutineeMakePayment(int? id)
        {
            var ap = await db.Applications.FindAsync(id);
            ScrutineeMakePaymentVM vm = new ScrutineeMakePaymentVM
            {
                ApplicationId = ap.ApplicationId,
                ApplicationCode = ap.ApplicationCode
            };
            if (ap.IsPackage == false || ap.IsPackage == null)
            {
                var c = await db.Courses.FindAsync(ap.CourseId);
                var cc = await db.CourseCategories.FindAsync(c.CategoryId);
                var cf = await db.CourseFees.FirstOrDefaultAsync(x => x.CourseId == ap.CourseId);

               

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
            }
            else
            {
                //Application Package Details and Fee Calcuation
                decimal fee = 0, taxamount = 0, minBalance = 0;
                var l = db.ApplicationPackageDetails.Where(x => x.ApplicationId == ap.ApplicationId).ToList();
                foreach (var item in l)
                {
                    //Fee Calculations
                    var cf = await db.CourseFees.FirstOrDefaultAsync(x => x.CourseId == item.CourseId);
                    if (cf.GstPercentage == 0)
                    {
                        fee = fee + (decimal)cf.PackageFee;
                        if (cf.MinBalance == 0)
                            minBalance = minBalance + (decimal)cf.PackageFee;
                        else
                            minBalance = minBalance + (decimal)cf.MinBalance;
                    }
                    else
                    {
                        fee = fee + (decimal)cf.PackageFee + (((decimal)cf.PackageFee / 100) * (decimal)cf.GstPercentage);
                        taxamount = taxamount + (((decimal)cf.PackageFee / 100) * (decimal)cf.GstPercentage);
                        if (cf.MinBalance == 0)
                            minBalance = minBalance + (decimal)cf.PackageFee + (((decimal)cf.PackageFee / 100) * (decimal)cf.GstPercentage);
                        else
                            minBalance = minBalance + (decimal)cf.MinBalance;
                    }
                }
                vm.Amount = fee;
                vm.FeesType = "PackageFee";

            }
            ViewBag.PaymentMode = DropdownData.PaymentMode();
            ViewBag.PaymentLocation = DropdownData.PaymentLocation();
            return PartialView("ScrutineeMakePayment",vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ScrutineeMakePayment(ScrutineeMakePaymentVM obj)
        {
            if (ModelState.IsValid)
            {
                //Reciept No
                string loc = string.Empty;
                var fy = DateTime.Now.Month >= 4 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                if (User.IsInRole("Fees") || User.IsInRole("Admin"))
                    loc = "Nhava";
                else
                    loc = "Worli";

                var rno = Valid.CreateRecieptNo(loc, obj.PaymentMode, fy.ToString());
                var ap = await db.Applications.FindAsync(obj.ApplicationId);

                if (ap.IsPackage == false || ap.IsPackage == null)
                {
                    var cc = await db.CourseCategories.FindAsync(ap.CategoryId);
                    var c = await db.Courses.FindAsync(ap.CourseId);
                    var b = await db.Batches.FindAsync(ap.BatchId);

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
                        //await db.SaveChangesAsync();

                        //feeReceipt
                        FeeReceipt fr = new FeeReceipt
                        {
                            Amount = Convert.ToDecimal(obj.Amount),
                            ApplicationId = Convert.ToInt32(ap.ApplicationId),
                            PaymentMode = obj.PaymentMode,
                            PrintStatus = false,
                            FeesType = "ApplicationFee",
                            ReceiptDate = DateTime.Now,
                            DDNo = obj.DdNo,
                            ChequeNo = obj.ChequeNo,
                            BankName = obj.BankName,
                            FeeReceiptNo = rno,
                            Fy = fy.ToString(),
                            Location = loc
                        };
                        db.FeeReceipts.Add(fr);
                        await db.SaveChangesAsync();

                        EmailModel em = new EmailModel
                        {
                            From = ConfigurationManager.AppSettings["admsmail"],
                            FromPass = ConfigurationManager.AppSettings["admsps"],
                            To = ap.Email,
                            Subject = "Course Registration with TSR",
                            Body = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been reached for " + c.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                        };

                        var res = await MessageService.sendEmail(em);

                        MessageService ms = new MessageService();
                        string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application Fee " + obj.Amount + "recieved for " + c.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                        string mobileno = ap.CellNo;
                        await ms.SendSmsAsync(msg, mobileno);
                    }
                    else
                    {
                        //NonCet
                        Batch bs = await db.Batches.FindAsync(ap.BatchId);
                        bs.BookedSeats = bs.BookedSeats + 1;
                        //await db.SaveChangesAsync();

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
                        //await db.SaveChangesAsync();

                        //feeReceipt
                        FeeReceipt fr = new FeeReceipt
                        {
                            Amount = Convert.ToDecimal(obj.Amount),
                            ApplicationId = Convert.ToInt32(ap.ApplicationId),
                            PaymentMode = obj.PaymentMode,
                            PrintStatus = false,
                            FeesType = "CourseFee",
                            ReceiptDate = DateTime.Now,
                            DDNo = obj.DdNo,
                            ChequeNo = obj.ChequeNo,
                            BankName = obj.BankName,
                            FeeReceiptNo = rno,
                            Fy = fy.ToString(),
                            Location = loc
                        };
                        db.FeeReceipts.Add(fr);
                        await db.SaveChangesAsync();

                        //Student Payment Details
                        decimal tax;
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
                            Subject = "Course Registration with TSR",
                            Body = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your seat has been confirmed for " + c.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                        };

                        var res = await MessageService.sendEmail(em);

                        MessageService ms = new MessageService();
                        string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your seat has been confirmed for " + c.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                        string mobileno = ap.CellNo;
                        await ms.SendSmsAsync(msg, mobileno);
                    }

                    //return RedirectToAction("Scrutinee");
                    return Json(new { success = true });
                }
                else
                {
                    //Package
                    //feeReceipt
                    FeeReceipt fr = new FeeReceipt
                    {
                        Amount = Convert.ToDecimal(obj.Amount),
                        ApplicationId = Convert.ToInt32(obj.ApplicationId),
                        PaymentMode = obj.PaymentMode,
                        PrintStatus = false,
                        FeesType = "PackageFee",
                        ReceiptDate = DateTime.Now,
                        DDNo = obj.DdNo,
                        ChequeNo = obj.ChequeNo,
                        BankName = obj.BankName,
                        FeeReceiptNo = rno,
                        Fy = fy.ToString(),
                        Location = loc
                    };
                    db.FeeReceipts.Add(fr);
                    await db.SaveChangesAsync();

                    var aps = db.ApplicationPackageDetails
                         .Where(x => x.ApplicationId == obj.ApplicationId)
                         .ToList();

                    decimal totalFee = 0;
                    foreach (var item in aps)
                    {
                        //fee
                        var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == item.CourseId);
                        totalFee = totalFee + (decimal)cf.PackageFee + (((decimal)cf.PackageFee / 100) * (decimal)cf.GstPercentage);

                        //seatStock
                        Batch bs = await db.Batches.FindAsync(item.BatchId);
                        bs.BookedSeats = bs.BookedSeats + 1;

                        //Applied
                        Applied nca = new Applied
                        {
                            AdmissionStatus = true,
                            ApplicationId = obj.ApplicationId,
                            BatchId = Convert.ToInt32(item.BatchId),
                            CategoryId = (int)db.Courses.Find(item.CourseId).CategoryId,
                            CourseId = Convert.ToInt32(item.CourseId)
                        };

                        db.Applied.Add(nca);
                        await db.SaveChangesAsync();
                    }

                    //Student Payment Details                   
                    StudentFeeDetail sfd = new StudentFeeDetail
                    {
                        ApplicationId = obj.ApplicationId,
                        TotalFee = totalFee,
                        FeePaid = obj.Amount,
                        FeeBal = totalFee - obj.Amount,
                        PackageId = ap.PackageId
                    };
                    db.StudentFeeDetails.Add(sfd);
                    await db.SaveChangesAsync();

                    //send mail
                    //Application ap1 = await db.Applications.FindAsync(obj.ApplicationId);
                    EmailModel em = new EmailModel
                    {
                        From = ConfigurationManager.AppSettings["admsmail"],
                        FromPass = ConfigurationManager.AppSettings["admsps"],
                        To = ap.Email,
                        Subject = "Course Registration with TSR",
                        Body = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your seat has been confirmed for " + db.packages.Find(ap.PackageId).PackageName + "  Thanking you T.S.Rahaman"
                    };

                    var res = await MessageService.sendEmail(em);

                    MessageService ms = new MessageService();
                    string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your seat has been confirmed for " + db.packages.Find(ap.PackageId).PackageName + "  Thanking you T.S.Rahaman";
                    string mobileno = ap.CellNo;
                    await ms.SendSmsAsync(msg, mobileno);

                    return Json(new { success = true });
                }
            }
            ViewBag.PaymentMode = DropdownData.PaymentMode();
            ViewBag.PaymentLocation = DropdownData.PaymentLocation();
            return PartialView("ScrutineeMakePayment", obj);
            // return View();
        }

        public ActionResult ScrutineePrintReceipt(int? id)
        {
            if (id != null)
            {
                var app = db.Applications.Find(id);
                if (app.IsPackage == null || app.IsPackage==false)
                {
                    var list = (from ap in db.Applications.AsEnumerable()
                                join b in db.Batches on ap.BatchId equals b.BatchId
                                join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                                join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                                into sdf from sd in sdf.DefaultIfEmpty()
                                join cr in db.Courses on ap.CourseId equals cr.CourseId
                                join gst in db.CourseFees on ap.CourseId equals gst.CourseId into pi4
                                from gst in pi4.DefaultIfEmpty()

                                where (ap.ApplicationId == id)

                                select new FeesViewPaymentDetailsVM
                                {
                                    //FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),//fr.FeeReceiptNo,
                                    FeeReceiptNo = fr.FeeReceiptNo,
                                    PaymentMode = fr.PaymentMode,
                                    FeesType = fr.FeesType,
                                    ReceiptDate = fr.ReceiptDate,
                                    Name = ap.FullName,
                                    Course = cr.ShortName,
                                    Batch = b.BatchCode,
                                    StudentId = ap.ApplicationId,
                                    ApplicationId = ap.ApplicationId,
                                    ApplicationCode = ap.ApplicationCode,
                                    FeePaid = fr.Amount,
                                    FeeBal = (sd == null) ? 0 : sd.FeeBal,
                                    AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                    BatchStartDate = b.StartDate,
                                    PaymentDate = DateTime.Now,
                                    FeeReceiptId = fr.FeeReceiptId,
                                    GST = (gst == null) ? 0 : Convert.ToDecimal(gst.GstPercentage)


                                }).ToList();
                    foreach (var l in list)
                    {
                        FeeReceipt fr = db.FeeReceipts.Find(l.FeeReceiptId);
                        fr.PrintStatus = true;
                        //fr.ReceiptDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    list.AddRange(Enumerable.Repeat(0, 1).Select(
                        x => new FeesViewPaymentDetailsVM()
                        { FeeReceiptNo =list[0].FeeReceiptNo,
                          PaymentMode = list[0].PaymentMode,
                          FeesType = list[0].FeesType,
                          ReceiptDate = list[0].ReceiptDate,
                          Name =  list[0].Name,
                          Course = list[0].Course,
                          Batch = list[0].Batch,
                          StudentId = list[0].StudentId,
                          ApplicationId = list[0].ApplicationId,
                          ApplicationCode = list[0].ApplicationCode,
                          FeePaid = list[0].FeePaid,
                          FeeBal = list[0].FeeBal,
                          AmountInRs = list[0].AmountInRs,
                          BatchStartDate = list[0].BatchStartDate,
                          PaymentDate = list[0].PaymentDate,
                          FeeReceiptId = list[0].FeeReceiptId,
                          GST= list[0].GST
                        }));
                    return new PdfActionResult(list);
                }
                else
                {
                    var list = (from ap in db.Applications.AsEnumerable()
                                join pk in db.packages on ap.PackageId equals pk.PackageId
                                join fr in db.FeeReceipts on ap.ApplicationId equals fr.ApplicationId
                                join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId
                                where (ap.ApplicationId == id)
                                select new FeesViewPaymentDetailsVM
                                {
                                    //FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),//fr.FeeReceiptNo,
                                    FeeReceiptNo = fr.FeeReceiptNo,
                                    PaymentMode = fr.PaymentMode,
                                    FeesType = fr.FeesType,
                                    ReceiptDate = fr.ReceiptDate,
                                    Name = ap.FullName,
                                    Course = pk.PackageName + " (Package)",
                                    Batch = "",
                                    StudentId = ap.ApplicationId,
                                    ApplicationId = ap.ApplicationId,
                                    ApplicationCode = ap.ApplicationCode,
                                    FeePaid = fr.Amount,
                                    FeeBal = sd.FeeBal,
                                    AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64( fr.Amount)),
                                    BatchStartDate = null,
                                    PaymentDate = DateTime.Now,
                                    FeeReceiptId = fr.FeeReceiptId,
                                    GST=0


                                }).ToList();
                 
                    foreach (var l in list)
                    {
                        FeeReceipt fr = db.FeeReceipts.Find(l.FeeReceiptId);
                        fr.PrintStatus = true;
                        //fr.ReceiptDate = DateTime.Now;
                        db.SaveChanges();

                    }
                    list.AddRange(Enumerable.Repeat(0, 1).Select(
                      x => new FeesViewPaymentDetailsVM()
                      {
                          FeeReceiptNo = list[0].FeeReceiptNo,
                          PaymentMode = list[0].PaymentMode,
                          FeesType = list[0].FeesType,
                          ReceiptDate = list[0].ReceiptDate,
                          Name = list[0].Name,
                          Course = list[0].Course,
                          Batch = list[0].Batch,
                          StudentId = list[0].StudentId,
                          ApplicationId = list[0].ApplicationId,
                          ApplicationCode = list[0].ApplicationCode,
                          FeePaid = list[0].FeePaid,
                          FeeBal = list[0].FeeBal,
                          AmountInRs = list[0].AmountInRs,
                          BatchStartDate = list[0].BatchStartDate,
                          PaymentDate = list[0].PaymentDate,
                          FeeReceiptId = list[0].FeeReceiptId,
                          GST = list[0].GST
                      }));

                    return new PdfActionResult(list);
                }
            }
            return View();
        }
        #endregion

        #region checkStatus
        public ActionResult checkPaymentStatus()
        {
            ViewBag.Students = new SelectList(db.Applications.ToList(), "ApplicationId", "FullName");
            return View();
        }
        public ActionResult PaymentStatusSearch(int ApplicationId)
        {
            var obj = new List<PaymentStatusSearchVM>();
            PaymentStatusSearchVM vm = new PaymentStatusSearchVM();
            Application app = db.Applications.Find(ApplicationId);

            //string trxid = ;
            vm.Status = GetTxnStatus(app.TransactionId);
            if (app.IsPackage == true)
            {
                vm.ApplicationId = app.ApplicationId;
                vm.ApplicationCode = app.ApplicationCode;
                vm.CourseName = db.packages.Find(app.PackageId).PackageName;
                vm.Batch = "Package";
                vm.Email = app.Email;
                vm.Phone = app.CellNo;
                vm.FullName = app.FullName;
                
            }
            else
            {
                vm.ApplicationId = app.ApplicationId;
                vm.ApplicationCode = app.ApplicationCode;
                vm.CourseName = db.Courses.Find(app.CourseId).CourseName;
                vm.Batch = db.Batches.Find(app.BatchId).StartDate.ToString();
                vm.Email = app.Email;
                vm.Phone = app.CellNo;
                vm.FullName = app.FullName;
            }
            obj.Add(vm);
            return PartialView("PaymentStatusList", obj.ToList());
        }
        public string action1 = string.Empty;
        public string hash1 = string.Empty;
        public string txnid1 = string.Empty;

        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
            //return hashValue.ToString();

        }
        private string GetTxnStatus(string txtid)
        {
            string Url = ConfigurationManager.AppSettings["checkStatus"];

            string method = "verify_payment";
            string salt = ConfigurationManager.AppSettings["SALT"];
            string key = ConfigurationManager.AppSettings["MERCHANT_KEY"];
            string var1 = txtid; //Transaction ID of the merchant

            string toHash = key + "|" + method + "|" + var1 + "|" + salt;

            string Hashed = Generatehash512(toHash);

            string postString = "key=" + key +
                "&command=" + method +
                "&hash=" + Hashed +
                "&var1=" + var1;

            WebRequest myWebRequest = WebRequest.Create(Url);
            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";
            myWebRequest.Timeout = 180000;
            StreamWriter requestWriter = new StreamWriter(myWebRequest.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            StreamReader responseReader = new StreamReader(myWebRequest.GetResponse().GetResponseStream());
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(ReceiveStream, encode);

            string response = readStream.ReadToEnd();
            JObject account = JObject.Parse(response);
            string status = (string)account.SelectToken("transaction_details." + var1 + ".status");

            return status;
        }
        #endregion

        #region EditFeesReciept

#endregion
    }
}