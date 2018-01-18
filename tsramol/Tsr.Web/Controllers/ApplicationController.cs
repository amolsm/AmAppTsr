using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;
using Tsr.Web.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.Text;
using System.Web.UI;
using Tsr.Web.Common;
using Fluentx.Mvc;
using System.Security.Cryptography;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Tsr.Web.Controllers
{
    public class ApplicationController : Controller
    {
        AppContext db = new AppContext();

        public ActionResult Policy()
        {
            string path = Server.MapPath("/Uploads/Policy.pdf");

            return File(path, "application/pdf");

        }
        // GET: Application
        #region Common
        public ActionResult FillCourse(int CategoryId)
        {
            var Courses = db.Courses
                .Where(c => c.CategoryId == CategoryId && c.IsActive == true);
            return Json(Courses, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillCourseAppl(int CategoryId)
        {
            var C = db.Courses
                .Where(c => c.CategoryId == CategoryId && c.IsActive == true);
            var cat = db.CourseCategories.Find(CategoryId);
            
            if (cat.CetRequired == true)
            {
              var  Courses = new { One = "True", Two = C };
                return Json(Courses, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Courses = new { One = "False", Two = C };
                return Json(Courses, JsonRequestBehavior.AllowGet);
            }
            
        }
        public ActionResult FillBatch(int CourseId)
        {
            var C = db.Batches
                .Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true && c.StartDate > DbFunctions.AddDays(DateTime.Now, 1) && c.CourseId ==(db.CourseFees.Where(x=>x.CourseId==CourseId).Select(m=>m.CourseId).FirstOrDefault()))
                .Select(x => new { BatchId = x.BatchId, Name = x.StartDate});

            var Courses = C.ToList().Select(x => new BatchDropdown {BatchId = x.BatchId, Name = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            ViewBag.Flag = "Test22";
            return Json(Courses, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillBatchAll(int CourseId)
        {
            var C = db.Batches
                .Where(c => c.CourseId == CourseId && c.IsActive == true)
                .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, Name = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });

            return Json(Batches, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsPackage(bool IsPackage)
        {
            if (IsPackage == true)
            {
                var Packages = db.packages.Where(x => x.IsActive == true).ToList();
                return Json(Packages, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string Packages = "Single";
                return Json(Packages, JsonRequestBehavior.AllowGet);
            }
            
        }
        
        public ActionResult CreateApplicationId(int CategoryId, int CourseId, int BatchId, string FirstName, string MiddleName, string LastName, string Email, string CellNo, DateTime? DateOfBirth)
        {
            var Courses = db.Batches.Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true);

            return Json(Courses, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillTerms(int CourseId)
        {
            //var Courses = db.Batches.Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true);
            var obj = from cd in db.CourseDocuments
                      join d in db.Documents on cd.DocumentId equals d.DocumentsListId
                      where cd.CourseId == CourseId
                      select new { d.DocumentName, d.DocumentsListId};
                      

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillTermsPackage(int PackageId)
        {
            //var Courses = db.Batches.Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true);
            var obj = from cd in db.CourseDocuments
                      join d in db.Documents on cd.DocumentId equals d.DocumentsListId
                      where cd.PackageId == PackageId
                      select new { d.DocumentName, d.DocumentsListId };


            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkAge(int CourseId, string DateOfBirth)
        {
            bool res;
            var c = db.Courses.Find(CourseId);
            var dob = Convert.ToDateTime(DateOfBirth);
            var age = DateTime.Now.Year - dob.Year;
            if (c.MinAge == 0 && c.MaxAge == 0)
            {
                res = true;
            }
            else if(c.MinAge<= age && c.MaxAge>= age)
            {
                res = true;
            }
            else { res = false; }
            
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getBatchRemainingSeats(int BatchId)
        {
            
            var b = db.Batches.Find(BatchId);
            var cc = db.CourseCategories.Find(b.CategoryId);
            string rem;
            if (cc.CetRequired == false)
            {
                var r = b.TotalSeats - b.ReserveSeats - b.BookedSeats;
                rem = r.ToString();
            }
            else
                rem = "null";
            return Json(rem, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult Index()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.IsPackage = DropdownData.CourseType();
            ApplicationIndexVM vm = new ApplicationIndexVM();
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ApplicationIndexVM obj)
        {
            if (obj.PackageId == 0)
            {
                ViewBag.InfoFlag = "NonPackage"; //for info flag

                var cc = db.CourseCategories.FirstOrDefault(x => x.CourseCategoryId == obj.CategoryId);
                bool isCet = (bool)cc.CetRequired;
                var c = db.Courses.FirstOrDefault(x => x.CourseId == obj.CourseId);
                var courseName = c.CourseName;

                ViewBag.Gender = Common.DropdownData.Gender();
                ViewBag.Meals = Common.DropdownData.Meals();
                ViewBag.YesNo = Common.DropdownData.YesNo();
                if (isCet == true)
                {
                    ApplicationCetVM vm = new ApplicationCetVM
                    {
                        CourseId = obj.CourseId,
                        CategoryId = obj.CategoryId,
                        BatchId = obj.BatchId,
                        CourseName = courseName
                    };

                    ViewBag.ShirtSize = Common.DropdownData.ShirtSize();
                    ViewBag.PantSize = Common.DropdownData.PantSize();
                    ViewBag.ShoeSize = Common.DropdownData.ShoeSize();

                    return View("CetApplication", vm);
                }
                else
                {
                    ApplicationNonCetVM vm = new ApplicationNonCetVM
                    {
                        CourseId = obj.CourseId,
                        CategoryId = obj.CategoryId,
                        BatchId = obj.BatchId,
                        CourseName = courseName,
                        PackageId = 0
                    };

                    var b = db.Batches.FirstOrDefault(x => x.BatchId == obj.BatchId);
                    int totSeat = (int)b.TotalSeats;
                    int reservSeat = (int)b.ReserveSeats;
                    int booked = (int)b.BookedSeats;
                    var remain = totSeat - (reservSeat + booked);
                    ViewBag.remain = remain;
                    if (remain > 0)
                    {
                        return View("NonCetApplication", vm);
                    }
                    else
                    {
                        ViewBag.BatchCode = obj.BatchCode;
                        return View("NonCetApplicationFull", vm);
                    }

                }
            }
            else
            {
                //For Package Course
                ApplicationNonCetVM vm = new ApplicationNonCetVM
                {
                    PackageId = obj.PackageId,
                    PackageBatchId = obj.PackageBatchId.ToList().Select(x => new PackageCourseBatches {
                        BatchId = x.BatchId,
                        CourseId = (int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).CourseId,
                        CourseName = db.Courses.FirstOrDefault(y => y.CourseId == db.Batches.FirstOrDefault(z => z.BatchId == x.BatchId).CourseId).CourseName,
                        RemainingSeats = ((int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).TotalSeats) - ((int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).ReserveSeats) - ((int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).BookedSeats)
                    }).ToList()
                };
                ViewBag.Gender = Common.DropdownData.Gender();
                ViewBag.Meals = Common.DropdownData.Meals();
                ViewBag.YesNo = Common.DropdownData.YesNo();
                ViewBag.InfoFlag = "Package";
                return View("NonCetApplication", vm);
            }
                
        }
        public ActionResult FillPackageBatches(int PackageId, int CategoryId)
        {
            var PackageBatches = from pc in db.PackageCourses
                                 join c in db.Courses on pc.CourseId equals c.CourseId
                                 where pc.PackageId == PackageId
                                 select new PackageCourseBatches
                                 {
                                     CourseId = c.CourseId,
                                     CourseName = c.CourseName,
                                     PackageId = pc.PackageId,
                                     BatchDropdowns = (db.Batches
                                         .Where(x => x.CourseId == c.CourseId && x.IsActive == true && x.OnlineBookingStatus == true && x.StartDate > DbFunctions.AddDays(DateTime.Now,1))
                                         .Select(x => new BatchDropdown { BatchId = x.BatchId, Name = x.StartDate.ToString() }))
                                         //.ToList()
                                         //.Select(p => new BatchDropdown { BatchId = p.BatchId, Name = Convert.ToDateTime(p.Name).ToString("dd-MM-yyyy") })
                                 };

            //var Courses = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, Name = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            var pb = PackageBatches.ToList().Select(x => new PackageCourseBatches
            {
                BatchDropdowns = x.BatchDropdowns.ToList().Select(y=>new BatchDropdown { BatchId = y.BatchId, Name = Convert.ToDateTime(y.Name).ToString("dd-MM-yyyy") }),
                CourseId = x.CourseId,
                CourseName = x.CourseName,
                PackageId = x.PackageId
            });
            ApplicationIndexVM vm = new ApplicationIndexVM
            {
                CategoryId = CategoryId,
                PackageBatchId = pb.ToList(),
                PackageId = PackageId
            };
            return PartialView("IndexPackageBatches", vm);
        }
        //public ActionResult NonCetApplication()
        //{
        //    ViewBag.Gender = DropdownData.Gender();
        //    ViewBag.Meals = DropdownData.Meals();
        //    ViewBag.YesNo = DropdownData.YesNo();
        //    return View();
        //}
        //public ActionResult CetApplication()
        //{
        //    ViewBag.Gender = DropdownData.Gender();
        //    ViewBag.ShirtSize = DropdownData.ShirtSize();
        //    ViewBag.PantSize = DropdownData.PantSize();
        //    ViewBag.ShoeSize = DropdownData.ShoeSize();
        //    ViewBag.Meals = DropdownData.Meals();
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CetApplication(ApplicationCetVM obj, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var b = db.Batches.FirstOrDefault(x => x.BatchId == obj.BatchId);
                var bc = b.BatchCode;
                var c = db.Courses.FirstOrDefault(x => x.CourseId == obj.CourseId);
                var cc = c.CourseCode;
                var n = db.Applications.Count(x=>x.BatchId == obj.BatchId);
                n = n + 1;

                var allowedExtensions = new[] {
                ".Jpg", ".png", ".jpg", "jpeg"
                  };
                //string fileName = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0');
                //string imgPath = "/Uploads/CetPhoto/" + fileName;
                //file.SaveAs(Server.MapPath(imgPath));
                //var ext = Path.GetExtension(file.FileName);
                var root = "/Uploads/CetPhoto/";
                var appcode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0');
                var files = file;
                var ext = Path.GetExtension(files.FileName);
                var fileName = appcode + ext;
                var path = Server.MapPath(root + fileName);
                files.SaveAs(path);
                var filepathname = root + fileName;

                Application ap = new Application
                {
                    ApplicationCode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4,'0'),
                    AnnualIncome = obj.AnnualIncome,
                    BatchId = obj.BatchId,
                    Caste = obj.Caste,
                    CategoryId = obj.CategoryId,
                    CellNo = obj.CellNo,
                    Citizenship = obj.Citizenship,
                    CourseId = obj.CourseId,
                    DateOfBirth = obj.DateOfBirth,
                    Email = obj.Email,
                    FatherEmail = obj.FatherEmail,
                    FatherOccupation = obj.FatherOccupation,
                    FirstName = obj.FirstName,
                    Gender = obj.FirstName,
                    GradAddress = obj.GradAddress,
                    GradCity = obj.GradCity,
                    GradCollegeName = obj.GradCollegeName,
                    GradPassAttempt = obj.GradPassAttempt,
                    GradPassingYear = obj.GradPassingYear,
                    GradPercentage = obj.GradPercentage,
                    GradPin = obj.GradPin,
                    GradState = obj.GradState,
                    GradSubjects = obj.GradSubjects,
                    GradUniversity = obj.GradUniversity,
                    GuardianAddress = obj.GuardianAddress,
                    GuardianCity = obj.GuardianCity,
                    GuardianContact = obj.GuardianContact,
                    GuardianEmail = obj.GuardianEmail,
                    GuardianName = obj.GuardianName,
                    GuardianPin = obj.GuardianPin,
                    GuardianRelation = obj.GuardianRelation,
                    GuardianState = obj.GuardianState,
                    Height = obj.Height,
                    IdentificationMark = obj.IdentificationMark,
                    InterAddress = obj.InterAddress,
                    InterBoard = obj.InterBoard,
                    InterChemistry = obj.InterChemistry,
                    InterCity = obj.InterCity,
                    InterMath = obj.InterMath,
                    InterEnglish = obj.InterEnglish,
                    InterPassingYear = obj.InterPassingYear,
                    InterPercentage = obj.InterPercentage,
                    InterPhysics = obj.InterPhysics,
                    InterPin = obj.InterPin,
                    InterRollNo = obj.InterRollNo,
                    InterSchoolName = obj.InterSchoolName,
                    InterState = obj.InterState,
                    LastName = obj.LastName,
                    MiddleName = obj.MiddleName,
                    MotherName = obj.MotherName,
                    PantSize = obj.PantSize,
                    PassportNo = obj.PassportNo,
                    PermenentAddress = obj.PermenentAddress,
                    PermenentCity = obj.PermenentCity,
                    PermenentContactNo = obj.PermenentContactNo,
                    PermenentPin = obj.PermenentPin,
                    PermenentState = obj.PermenentState,
                    PlaceOfBirth = obj.PlaceOfBirth,
                    PreferredMeal = obj.PreferredMeal,
                    PresentAddress = obj.PresentAddress,
                    PresentCity = obj.PresentCity,
                    PresentContactNo = obj.PresentContactNo,
                    PresentPin = obj.PresentPin,
                    PresentState = obj.PresentState,
                    Religion = obj.Religion,
                    SchoolAddress = obj.SchoolAddress,
                    SchoolBoard = obj.SchoolBoard,
                    SchoolCity = obj.SchoolCity,
                    SchoolEnglish = obj.SchoolEnglish,
                    SchoolMath = obj.SchoolMath,
                    SchoolName = obj.SchoolName,
                    SchoolPassingYear = obj.SchoolPassingYear,
                    SchoolPercentage = obj.SchoolPercentage,
                    SchoolPin = obj.SchoolPin,
                    SchoolScience = obj.SchoolScience,
                    SchoolState = obj.SchoolState,
                    ShirtSize = obj.ShirtSize,
                    ShoeSize = obj.ShoeSize,
                    Weight = obj.Weight,
                    FullName = obj.FullName,
                    FatherFullName = obj.FatherFullName
                };
                db.Applications.Add(ap);
                await db.SaveChangesAsync();

                var id = ap.ApplicationId;
                var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == ap.CourseId);
                //var c = db.Courses.FirstOrDefault(x => x.CourseId == ap.CourseId);
                var applicationFee = cf.ApplicationFee;
                
                ApplicationSumPayCetVM apsm = new ApplicationSumPayCetVM
                {
                    ApplicationId = ap.ApplicationId,
                    CategoryId = ap.CategoryId,
                    CourseId = ap.CourseId,
                    BatchId = ap.BatchId,
                    ApplicationCode = ap.ApplicationCode,
                    amount = applicationFee,
                    CellNo = ap.CellNo,
                    Email = ap.Email,
                    FirstName = ap.FirstName,
                    LastName = ap.LastName,
                    CourseName = c.CourseName,
                    BatchCode = bc,
                    udf1 = ap.BatchId.ToString(), //udf1 BatchId
                    udf2 = ap.ApplicationCode, //udf2 ApplicationCode  
                    udf3 = ap.ApplicationId.ToString(), //udf3 ApplicationID   
                    udf4 = "0" //Single Course Non Package          
                };

                ApplAmt aa = new ApplAmt()
                {
                    ApplicationId = ap.ApplicationId,
                    Amount = apsm.amount
                };
                db.ApplAmts.Add(aa);
                await db.SaveChangesAsync();
                MessageService ms = new MessageService();
                string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + obj.CourseName + "  Thanking you T.S.Rahaman";
                string mobileno = ap.CellNo;
                await ms.SendSmsAsync(msg, mobileno);
                return View("ApplicationSummaryPre", apsm);
            }
            

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NonCetApplication(ApplicationNonCetVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.PackageId == 0 || obj.PackageId == null)
                {
                    var b = db.Batches.FirstOrDefault(x => x.BatchId == obj.BatchId);
                    var bc = b.BatchCode;
                    var c = db.Courses.FirstOrDefault(x => x.CourseId == obj.CourseId);
                    var cc = c.CourseCode;
                    var n = db.Applications.Count(x => x.BatchId == obj.BatchId);
                    n = n + 1;
                    Application ap = new Application
                    {
                        IsPackage = false, //Single Course
                        ApplicationCode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0'),
                        BatchId = obj.BatchId,
                        CategoryId = obj.CategoryId,
                        CellNo = obj.CellNo,
                        CertOfCompetencyNo = obj.CertOfCompetencyNo,
                        CdcNo = obj.CdcNo,
                        Citizenship = obj.Citizenship,
                        CourseId = obj.CourseId,
                        DateOfBirth = obj.DateOfBirth,
                        Email = obj.Email,
                        FirstName = obj.FirstName,
                        Gender = obj.Gender,
                        InDosNo = obj.InDosNo,
                        LastName = obj.LastName,
                        MiddleName = obj.MiddleName,
                        PassportNo = obj.PassportNo,
                        PlaceOfBirth = obj.PlaceOfBirth,
                        GradeOfCompetencyNo = obj.GradeOfCompetencyNo,
                        CategoryOfCandidate = obj.CategoryOfCandidate,
                        ShippingCompany = obj.ShippingCompany,
                        RankOfCandidate = obj.RankOfCandidate,
                        CourseAttendedInTSR = obj.CourseAttendedInTSR,
                        FPFF_AFF_1995 = obj.FPFF_AFF_1995,
                        PermenentAddress = obj.PermenentAddress,
                        PermenentCity = obj.PermenentCity,
                        PermenentContactNo = obj.PermenentContactNo,
                        PermenentPin = obj.PermenentPin,
                        PermenentState = obj.PermenentState,
                        FullName = obj.FullName,
                        OldCertificateDate = obj.OldCertificateDate,
                        OldCertificateIssuedBy = obj.OldCertificateIssuedBy,
                        OldCertificateNo = obj.OldCertificateNo
                    };
                    db.Applications.Add(ap);
                    await db.SaveChangesAsync();

                    var id = ap.ApplicationId;
                    var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == ap.CourseId);
                    var actualFee = cf.ActualFee;
                    var minBal = cf.MinBalance;
                    var gstPercent = cf.GstPercentage;
                    decimal taxAmount;

                    if (gstPercent > 0)
                    {
                        taxAmount = ((decimal)actualFee / 100) * (decimal)gstPercent;
                    }
                    else { taxAmount = 0; }
                    decimal amount = 0;

                    if (actualFee > minBal && minBal > 1)
                    {
                        amount = (decimal)minBal + taxAmount;
                    }
                    else
                    {
                        amount = (decimal)actualFee;
                    }

                    var courseName = c.CourseName;

                    ApplicationSumPayNonCetVM apsm = new ApplicationSumPayNonCetVM
                    {
                        
                        ApplicationId = ap.ApplicationId,
                        CategoryId = ap.CategoryId,
                        CourseId = ap.CourseId,
                        BatchId = ap.BatchId,
                        ApplicationCode = ap.ApplicationCode,
                        amount = amount,
                        CellNo = ap.CellNo,
                        Email = ap.Email,
                        FirstName = ap.FirstName,
                        LastName = ap.LastName,
                        CourseName = courseName,
                        BatchCode = bc,
                        CourseFee = actualFee,
                        TaxAmount = taxAmount,
                        udf1 = ap.BatchId.ToString(), //udf1 BatchId
                        udf2 = ap.ApplicationCode, //udf2 ApplicationCode 
                        udf3 = ap.ApplicationId.ToString(), //udf3 ApplicationID     
                        udf4 = "0" //Single Course, NonPackage        
                    };
                    ApplAmt aa = new ApplAmt()
                    {
                        ApplicationId = ap.ApplicationId,
                        Amount = apsm.amount
                    };
                    db.ApplAmts.Add(aa);
                    await db.SaveChangesAsync();

                    EmailModel em = new EmailModel
                    {
                        From = ConfigurationManager.AppSettings["admsmail"],
                        FromPass = ConfigurationManager.AppSettings["admsps"],
                        To = obj.Email,
                        Subject = "Course Registration with TSR",
                        Body = "Dear " + ap.FirstName + " " + ap.LastName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                    };

                    var res = await MessageService.sendEmail(em);

                    MessageService ms = new MessageService();
                    string msg = "Dear " + ap.FirstName + " " + ap.LastName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                    string mobileno = ap.CellNo;
                    await ms.SendSmsAsync(msg, mobileno);

                    return View("ApplicationSummary", apsm);
                }
                else
                {
                    //Package
                    var bid = obj.PackageBatchId.FirstOrDefault().BatchId;
                    var b = db.Batches.FirstOrDefault(x => x.BatchId == bid);
                    var bc = b.BatchCode;
                    var c = db.Courses.FirstOrDefault(x => x.CourseId == b.CourseId);
                    var cc = c.CourseCode;
                    var n = db.Applications.Count(x => x.BatchId == obj.BatchId);
                    n = n + 1;
                    Application ap = new Application
                    {
                        IsPackage = true, //For Package
                        PackageId = obj.PackageId, //For Package
                        ApplicationCode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0'),
                        BatchId = obj.BatchId,
                        CategoryId = obj.CategoryId,
                        CellNo = obj.CellNo,
                        CertOfCompetencyNo = obj.CertOfCompetencyNo,
                        CdcNo = obj.CdcNo,
                        Citizenship = obj.Citizenship,
                        CourseId = obj.CourseId,
                        DateOfBirth = obj.DateOfBirth,
                        Email = obj.Email,
                        FirstName = obj.FirstName,
                        Gender = obj.FirstName,
                        InDosNo = obj.InDosNo,
                        LastName = obj.LastName,
                        MiddleName = obj.MiddleName,
                        PassportNo = obj.PassportNo,
                        PlaceOfBirth = obj.PlaceOfBirth,
                        GradeOfCompetencyNo = obj.GradeOfCompetencyNo,
                        CategoryOfCandidate = obj.CategoryOfCandidate,
                        ShippingCompany = obj.ShippingCompany,
                        RankOfCandidate = obj.RankOfCandidate,
                        CourseAttendedInTSR = obj.CourseAttendedInTSR,
                        FPFF_AFF_1995 = obj.FPFF_AFF_1995,
                        FullName = obj.FullName,
                        OldCertificateNo = obj.OldCertificateNo,
                        OldCertificateIssuedBy = obj.OldCertificateIssuedBy,
                        OldCertificateDate = obj.OldCertificateDate
                    };
                    db.Applications.Add(ap);
                    await db.SaveChangesAsync();

                    //Application Package Details and Fee Calcuation
                    decimal fee = 0, taxamount = 0, minBalance = 0;
                    foreach (var item in obj.PackageBatchId)
                    {
                        ApplicationPackageDetail apd = new ApplicationPackageDetail
                        {
                            ApplicationId = ap.ApplicationId,
                            BatchId = item.BatchId,
                            PackageId = (int)obj.PackageId,
                            ConfirmStatus = false,
                            CourseId = item.CourseId
                        };
                        db.ApplicationPackageDetails.Add(apd);

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
                    await db.SaveChangesAsync();

                    
                    ApplicationSumPayNonCetVM apsm = new ApplicationSumPayNonCetVM
                    {
                        ApplicationId = ap.ApplicationId,
                        CategoryId = ap.CategoryId,
                        CourseId = ap.CourseId,
                        BatchId = ap.BatchId,
                        ApplicationCode = ap.ApplicationCode,
                        amount = minBalance, //package Fee Min Balance
                        CellNo = ap.CellNo,
                        Email = ap.Email,
                        FirstName = ap.FirstName,
                        LastName = ap.LastName,
                        CourseName = db.packages.FirstOrDefault(x=>x.PackageId == ap.PackageId).PackageName, //PackageName
                        BatchCode = bc,
                        CourseFee = fee, //packageFee
                        TaxAmount = taxamount,
                        udf1 = ap.BatchId.ToString(), //udf1 BatchId
                        udf2 = ap.ApplicationCode, //udf2 ApplicationCode 
                        udf3 = ap.ApplicationId.ToString(), //udf3 ApplicationID 
                        udf4 = ap.PackageId.ToString() //udf4 PackageId if package             
                    };

                    ApplAmt aa = new ApplAmt()
                    {
                        ApplicationId = ap.ApplicationId,
                        Amount = apsm.amount
                    };
                    db.ApplAmts.Add(aa);
                    await db.SaveChangesAsync();

                    EmailModel em = new EmailModel
                    {
                        From = ConfigurationManager.AppSettings["admsmail"],
                        FromPass = ConfigurationManager.AppSettings["admsps"],
                        To = obj.Email,
                        Subject = "Course Registration with TSR",
                        Body = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + db.packages.Find(ap.PackageId).PackageName + "  Thanking you T.S.Rahaman"
                    };

                    var res = await MessageService.sendEmail(em);

                    MessageService ms = new MessageService();
                    string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + db.packages.Find(ap.PackageId).PackageName + "  Thanking you T.S.Rahaman";
                    string mobileno = ap.CellNo;
                    await ms.SendSmsAsync(msg, mobileno);

                    return View("ApplicationSummary", apsm);
                }
            }
            ViewBag.Gender = DropdownData.Gender();
            ViewBag.Meals = DropdownData.Meals();
            ViewBag.YesNo = DropdownData.YesNo();
            return View();
        }



        #region PayU
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePaymentNonCet(ApplicationSumPayNonCetVM obj)
        {
            

                string[] hashVarsSeq;
                string hash_string = string.Empty;

            Application app = db.Applications.Find(obj.ApplicationId);
            obj.txnid = app.TransactionId;

            ApplAmt aa = db.ApplAmts.FirstOrDefault(x => x.ApplicationId == obj.ApplicationId);
            obj.amount = aa.Amount;

                if (string.IsNullOrEmpty(obj.txnid)) // generating txnid
                {
                    Random rnd = new Random();
                    string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                    obj.txnid = strHash.ToString().Substring(0, 20);
                    txnid1 = obj.txnid;
                app.TransactionId = obj.txnid;
                db.SaveChanges();
                }

            //GetTxnStatus(obj.txnid);

            hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
            hash_string = "";
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var == "key")
                {
                    hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "txnid")
                {
                    hash_string = hash_string + txnid1;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "amount")
                {
                    hash_string = hash_string + Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "productinfo")
                {
                    hash_string = hash_string + obj.BatchCode.ToString();
                    hash_string = hash_string + '|';
                }
                else
                {

                    hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                    hash_string = hash_string + '|';
                }
            }

            hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

            hash1 = Generatehash512(hash_string).ToLower();         //generating hash
            obj.hash = hash1;

            string mkey = ConfigurationManager.AppSettings["MERCHANT_KEY"];
            string surl = ConfigurationManager.AppSettings["surl"];
            string furl = ConfigurationManager.AppSettings["furl"];
            if (!string.IsNullOrEmpty(hash1))
                {

                    Dictionary<string, object> rp = new Dictionary<string, object>();
                    rp.Add("hash", hash1);
                    rp.Add("key", mkey);
                    rp.Add("txnid", obj.txnid);
                    string amt = Convert.ToDecimal(obj.amount).ToString("g29");
                    rp.Add("amount", amt);
                    rp.Add("firstname", obj.FirstName);
                    
                    rp.Add("email", obj.Email);
                    rp.Add("phone", obj.CellNo);
                    rp.Add("productinfo", obj.BatchCode);
                    rp.Add("surl", surl);
                    rp.Add("furl", furl);
                    rp.Add("lastname", obj.LastName);
                    rp.Add("curl", obj.curl);
                    //rp.Add("address1", null);
                    //rp.Add("address2", null);
                    //rp.Add("city", null);
                    //rp.Add("state", null);
                    //rp.Add("country", null);
                    //rp.Add("zipcode", null);
                    rp.Add("udf1", obj.udf1);
                    rp.Add("udf2", obj.udf2);
                    rp.Add("udf3", obj.udf3);
                    rp.Add("udf4", obj.udf4);
                    rp.Add("udf5", obj.udf5);
                //rp.Add("pg", "");

                string payu = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                return this.RedirectAndPost(payu, rp);
                    //Or return new RedirectAndPostActionResult("http://TheUrlToPostDataTo", postData);
                }

                else
                {
                    //no hash

                }

            

           

            return View();
        }

        public ActionResult MakePaymentCet(ApplicationSumPayCetVM obj)
        {


            string[] hashVarsSeq;
            string hash_string = string.Empty;

            Application app = db.Applications.Find(obj.ApplicationId);
            obj.txnid = app.TransactionId;
            ApplAmt aa = db.ApplAmts.FirstOrDefault(x => x.ApplicationId == obj.ApplicationId);
            obj.amount = aa.Amount;
            if (string.IsNullOrEmpty(obj.txnid)) // generating txnid
            {
                Random rnd = new Random();
                string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                obj.txnid = strHash.ToString().Substring(0, 20);
                txnid1 = obj.txnid;
                app.TransactionId = obj.txnid;
                db.SaveChanges();
            }

            hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
            hash_string = "";
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var == "key")
                {
                    hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "txnid")
                {
                    hash_string = hash_string + txnid1;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "amount")
                {
                    hash_string = hash_string + Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "productinfo")
                {
                    hash_string = hash_string + obj.BatchCode.ToString();
                    hash_string = hash_string + '|';
                }
                else
                {

                    hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                    hash_string = hash_string + '|';
                }
            }

            hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

            hash1 = Generatehash512(hash_string).ToLower();         //generating hash
            obj.hash = hash1;

            string mkey = ConfigurationManager.AppSettings["MERCHANT_KEY"];
            string surl = ConfigurationManager.AppSettings["surl"];
            string furl = ConfigurationManager.AppSettings["furl"];
            if (!string.IsNullOrEmpty(hash1))
            {

                Dictionary<string, object> rp = new Dictionary<string, object>();
                rp.Add("hash", hash1);
                rp.Add("key", mkey);
                rp.Add("txnid", obj.txnid);
                string amt = Convert.ToDecimal(obj.amount).ToString("g29");
                rp.Add("amount", amt);
                rp.Add("firstname", obj.FirstName);

                rp.Add("email", obj.Email);
                rp.Add("phone", obj.CellNo);
                rp.Add("productinfo", obj.BatchCode);
                rp.Add("surl", surl);
                rp.Add("furl", furl);
                rp.Add("lastname", obj.LastName);
                rp.Add("curl", obj.curl);
                //rp.Add("address1", null);
                //rp.Add("address2", null);
                //rp.Add("city", null);
                //rp.Add("state", null);
                //rp.Add("country", null);
                //rp.Add("zipcode", null);
                rp.Add("udf1", obj.udf1);
                rp.Add("udf2", obj.udf2);
                rp.Add("udf3", obj.udf3);
                rp.Add("udf4", obj.udf4);
                rp.Add("udf5", obj.udf5);
                //rp.Add("pg", "");

                string payu = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                return this.RedirectAndPost(payu, rp);
                //Or return new RedirectAndPostActionResult("http://TheUrlToPostDataTo", postData);
            }

            else
            {
                //no hash

            }





            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PaymentSuccess(ApplicationPaymentSuccess obj)
        {
            if (obj.status == "success")
            {
                if (obj.udf4 == "0") //Single COurse, NonPackage
                {

                    //Course c, Batch b, Caategory cc CourseFee cf 
                    int bid = Convert.ToInt32(obj.udf1);
                    var b = db.Batches.FirstOrDefault(x => x.BatchId == bid);
                    var cid = b.CourseId;
                    var c = db.Courses.FirstOrDefault(x => x.CourseId == cid);
                    var ccId = c.CategoryId;
                    obj.CourseName = c.CourseName;
                    var cc = db.CourseCategories.FirstOrDefault(x => x.CourseCategoryId == ccId);
                    var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == cid);

                    OnlinePaymentInfo opi = new OnlinePaymentInfo
                    {
                        BatchId = bid,
                        amount = obj.amount,
                        ApplicationId = Convert.ToInt32(obj.udf3),
                        bank_ref_num = obj.bank_ref_num,
                        CategoryId = ccId,
                        CourseId = cid,
                        Hash = obj.Hash,
                        key = obj.key,
                        mihpayid = obj.mihpayid,
                        mode = obj.mode,
                        Productinfo = obj.Productinfo,
                        status = obj.status,
                        txnid = obj.txnid,
                        PaymentDate = DateTime.Now,
                        udf1 = obj.udf1,//BatchId
                        udf2 = obj.udf2,//ApplicationCode
                        udf3 = obj.udf3,//ApplicationId
                        udf4 = obj.udf4, //PackageId 0 if single
                        udf5 = obj.udf5
                    };

                    db.OnlinePaymentInfos.Add(opi);
                    await db.SaveChangesAsync();

                    var apcode = Convert.ToInt32(obj.udf3);
                    Application ap = await db.Applications.FindAsync(apcode);
                    Batch bs = await db.Batches.FindAsync(bid);
                    if (cc.CetRequired == true)
                    {
                        //Cet

                        //Applied
                        Applied nca = new Applied
                        {
                            AdmissionStatus = false,
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            BatchId = Convert.ToInt32(obj.udf1),
                            CategoryId = (int)ccId,
                            CourseId = (int)cid
                        };
                        db.Applied.Add(nca);
                        await db.SaveChangesAsync();

                        //feeReceipt
                        FeeReceipt fr = new FeeReceipt
                        {
                            Amount = Convert.ToDecimal(obj.amount),
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            PaymentMode = "Online",
                            PrintStatus = false,
                            FeesType = "ApplicationFee"
                        };
                        db.FeeReceipts.Add(fr);
                        await db.SaveChangesAsync();

                        EmailModel em = new EmailModel
                        {
                            From = ConfigurationManager.AppSettings["admsmail"],
                            FromPass = ConfigurationManager.AppSettings["admsps"],
                            To = obj.Email,
                            Subject = "Course Registration with TSR",
                            Body = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your Application has been reached for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                        };

                        var res = await MessageService.sendEmail(em);

                        MessageService ms = new MessageService();
                        string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application Fee "+ obj.amount + "recieved for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                        string mobileno = ap.CellNo;
                        await ms.SendSmsAsync(msg, mobileno);

                        return View("PaymentSuccessNC", obj);
                    }
                    else
                    {
                        //NonCet
                        
                        bs.BookedSeats = bs.BookedSeats + 1;
                        await db.SaveChangesAsync();

                        //Applied
                        Applied nca = new Applied
                        {
                            AdmissionStatus = true,
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            BatchId = Convert.ToInt32(obj.udf1),
                            CategoryId = (int)ccId,
                            CourseId = (int)cid
                        };
                        db.Applied.Add(nca);
                        await db.SaveChangesAsync();

                        //feeReceipt
                        FeeReceipt fr = new FeeReceipt
                        {
                            Amount = Convert.ToDecimal(obj.amount),
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            PaymentMode = "Online",
                            PrintStatus = false,
                            FeesType = "CourseFee"
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
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            TotalFee = totalFee,
                            FeePaid = Convert.ToDecimal(obj.amount),
                            FeeBal = totalFee - Convert.ToDecimal(obj.amount),
                            BatchId = bid
                        };
                        db.StudentFeeDetails.Add(sfd);
                        await db.SaveChangesAsync();

                        EmailModel em = new EmailModel
                        {
                            From = ConfigurationManager.AppSettings["admsmail"],
                            FromPass = ConfigurationManager.AppSettings["admsps"],
                            To = obj.Email,
                            Subject = "Course Registration with TSR",
                            Body = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                        };

                        var res = await MessageService.sendEmail(em);

                        MessageService ms = new MessageService();
                        string msg = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                        string mobileno = ap.CellNo;
                        await ms.SendSmsAsync(msg, mobileno);

                        return View("PaymentSuccessNC", obj);
                    }

                }
                else //Package
                {
                    OnlinePaymentInfo opi = new OnlinePaymentInfo
                    {
                        PackageId = Convert.ToInt32(obj.udf4),
                        IsPackage = true,
                        //BatchId = 0,
                        amount = obj.amount,
                        ApplicationId = Convert.ToInt32(obj.udf3),
                        bank_ref_num = obj.bank_ref_num,
                        //CategoryId = 0,
                        //CourseId = 0,
                        Hash = obj.Hash,
                        key = obj.key,
                        mihpayid = obj.mihpayid,
                        mode = obj.mode,
                        Productinfo = obj.Productinfo,
                        status = obj.status,
                        txnid = obj.txnid,
                        PaymentDate = DateTime.Now,
                        udf1 = obj.udf1,//BatchId
                        udf2 = obj.udf2,//ApplicationCode
                        udf3 = obj.udf3,//ApplicationId
                        udf4 = obj.udf4, //PackageId 0 if single
                        udf5 = obj.udf5
                    };

                    db.OnlinePaymentInfos.Add(opi);
                    await db.SaveChangesAsync();

                    //feeReceipt
                    FeeReceipt fr = new FeeReceipt
                    {
                        Amount = Convert.ToDecimal(obj.amount),
                        ApplicationId = Convert.ToInt32(obj.udf3),
                        PaymentMode = "Online",
                        PrintStatus = false,
                        FeesType = "PackageFee"
                    };
                    db.FeeReceipts.Add(fr);
                    await db.SaveChangesAsync();

                    var aps = db.ApplicationPackageDetails
                         .Where(x => x.ApplicationId.ToString() == obj.udf3)
                         .ToList();

                    decimal totalFee = 0;
                    foreach (var item in aps)
                    {
                        //fee
                        var cf = db.CourseFees.FirstOrDefault(x=>x.CourseId == item.CourseId);
                        totalFee = totalFee + (decimal)cf.PackageFee + (((decimal)cf.PackageFee / 100) * (decimal)cf.GstPercentage);
                        
                        //seatStock
                        Batch bs = await db.Batches.FindAsync(item.BatchId);
                        bs.BookedSeats = bs.BookedSeats + 1;
                        
                        //Applied
                        Applied nca = new Applied
                        {
                            AdmissionStatus = true,
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            BatchId = Convert.ToInt32(item.BatchId),
                            CategoryId =  (int) db.Courses.Find(item.CourseId).CategoryId,
                            CourseId = Convert.ToInt32(item.CourseId)
                        };

                        db.Applied.Add(nca);
                        await db.SaveChangesAsync();
                    }

                    //Student Payment Details                   
                    StudentFeeDetail sfd = new StudentFeeDetail
                    {
                        ApplicationId = Convert.ToInt32(obj.udf3),
                        TotalFee = totalFee,
                        FeePaid = Convert.ToDecimal(obj.amount),
                        FeeBal = totalFee - Convert.ToDecimal(obj.amount),
                        PackageId = Convert.ToInt32(obj.udf4)
                    };
                    db.StudentFeeDetails.Add(sfd);
                    await db.SaveChangesAsync();
                    //send mail
                    //Application ap = await db.Applications.FindAsync(obj.udf3);
                    EmailModel em = new EmailModel
                    {
                        From = ConfigurationManager.AppSettings["admsmail"],
                        FromPass = ConfigurationManager.AppSettings["admsps"],
                        To = obj.Email,
                        Subject = "Course Registration with TSR",
                        Body = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + db.packages.Find(obj.udf4).PackageName + "  Thanking you T.S.Rahaman"
                };

                    var res = await MessageService.sendEmail(em);

                    MessageService ms = new MessageService();
                    string msg = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + db.packages.Find(obj.udf4).PackageName + "  Thanking you T.S.Rahaman";
                    string mobileno = obj.Phone;
                    await ms.SendSmsAsync(msg, mobileno);

                    return View("PaymentSuccessNC", obj);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PaymentStatus(ApplicationPaymentSuccess obj)
        {
            decimal amt1 = Convert.ToDecimal(obj.amount);
            int appid = Convert.ToInt32(obj.udf3);

            var aa = await db.ApplAmts.FirstOrDefaultAsync(x => x.ApplicationId == appid);
            var app11 = await db.Applications.FindAsync(appid);
            var oinf = db.OnlinePaymentInfos.FirstOrDefault(x => x.ApplicationId == appid);
            if (app11.TransactionId == obj.txnid && oinf!=null)
            {
                return RedirectToAction("Index");
                //return redirectto
            }
            else
            {
               
                if (aa.Amount == amt1)
                {
                    if (obj.status == "success")
                    {
                        if (obj.udf4 == "0") //Single COurse, NonPackage
                        {

                            //Course c, Batch b, Caategory cc CourseFee cf 
                            int bid = Convert.ToInt32(obj.udf1);
                            var b = db.Batches.FirstOrDefault(x => x.BatchId == bid);
                            var cid = b.CourseId;
                            var c = db.Courses.FirstOrDefault(x => x.CourseId == cid);
                            var ccId = c.CategoryId;
                            obj.CourseName = c.CourseName;
                            var cc = db.CourseCategories.FirstOrDefault(x => x.CourseCategoryId == ccId);
                            var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == cid);

                            OnlinePaymentInfo opi = new OnlinePaymentInfo
                            {
                                BatchId = bid,
                                amount = obj.amount,
                                ApplicationId = Convert.ToInt32(obj.udf3),
                                bank_ref_num = obj.bank_ref_num,
                                CategoryId = ccId,
                                CourseId = cid,
                                Hash = obj.Hash,
                                key = obj.key,
                                mihpayid = obj.mihpayid,
                                mode = obj.mode,
                                Productinfo = obj.Productinfo,
                                status = obj.status,
                                txnid = obj.txnid,
                                PaymentDate = DateTime.Now,
                                udf1 = obj.udf1,//BatchId
                                udf2 = obj.udf2,//ApplicationCode
                                udf3 = obj.udf3,//ApplicationId
                                udf4 = obj.udf4, //PackageId 0 if single
                                udf5 = obj.udf5
                            };

                            db.OnlinePaymentInfos.Add(opi);
                            await db.SaveChangesAsync();

                            var apcode = Convert.ToInt32(obj.udf3);
                            Application ap = await db.Applications.FindAsync(apcode);
                            Batch bs = await db.Batches.FindAsync(bid);
                            if (cc.CetRequired == true)
                            {
                                //Cet

                                //Applied
                                Applied nca = new Applied
                                {
                                    AdmissionStatus = false,
                                    ApplicationId = Convert.ToInt32(obj.udf3),
                                    BatchId = Convert.ToInt32(obj.udf1),
                                    CategoryId = (int)ccId,
                                    CourseId = (int)cid
                                };
                                db.Applied.Add(nca);
                                await db.SaveChangesAsync();

                                //feeReceipt
                                FeeReceipt fr = new FeeReceipt
                                {
                                    Amount = Convert.ToDecimal(obj.amount),
                                    ApplicationId = Convert.ToInt32(obj.udf3),
                                    PaymentMode = "Online",
                                    PrintStatus = false,
                                    FeesType = "ApplicationFee",
                                    ReceiptDate = DateTime.Now
                                };
                                db.FeeReceipts.Add(fr);
                                await db.SaveChangesAsync();

                                EmailModel em = new EmailModel
                                {
                                    From = ConfigurationManager.AppSettings["admsmail"],
                                    FromPass = ConfigurationManager.AppSettings["admsps"],
                                    To = obj.Email,
                                    Subject = "Course Registration with TSR",
                                    Body = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your Application has been reached for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                                };

                                var res = await MessageService.sendEmail(em);

                                MessageService ms = new MessageService();
                                string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application Fee " + obj.amount + "recieved for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                                string mobileno = ap.CellNo;
                                await ms.SendSmsAsync(msg, mobileno);

                                return View("PaymentSuccessNC", obj);
                            }
                            else
                            {
                                //NonCet

                                bs.BookedSeats = bs.BookedSeats + 1;
                                await db.SaveChangesAsync();

                                //Applied
                                Applied nca = new Applied
                                {
                                    AdmissionStatus = true,
                                    ApplicationId = Convert.ToInt32(obj.udf3),
                                    BatchId = Convert.ToInt32(obj.udf1),
                                    CategoryId = (int)ccId,
                                    CourseId = (int)cid
                                };
                                db.Applied.Add(nca);
                                await db.SaveChangesAsync();

                                //feeReceipt
                                FeeReceipt fr = new FeeReceipt
                                {
                                    Amount = Convert.ToDecimal(obj.amount),
                                    ApplicationId = Convert.ToInt32(obj.udf3),
                                    PaymentMode = "Online",
                                    PrintStatus = false,
                                    FeesType = "CourseFee",
                                    ReceiptDate = DateTime.Now
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
                                    ApplicationId = Convert.ToInt32(obj.udf3),
                                    TotalFee = totalFee,
                                    FeePaid = Convert.ToDecimal(obj.amount),
                                    FeeBal = totalFee - Convert.ToDecimal(obj.amount),
                                    BatchId = bid
                                };
                                db.StudentFeeDetails.Add(sfd);
                                await db.SaveChangesAsync();

                                EmailModel em = new EmailModel
                                {
                                    From = ConfigurationManager.AppSettings["admsmail"],
                                    FromPass = ConfigurationManager.AppSettings["admsps"],
                                    To = obj.Email,
                                    Subject = "Course Registration with TSR",
                                    Body = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                                };

                                var res = await MessageService.sendEmail(em);

                                MessageService ms = new MessageService();
                                string msg = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(bs.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                                string mobileno = ap.CellNo;
                                await ms.SendSmsAsync(msg, mobileno);

                                return View("PaymentSuccessNC", obj);
                            }

                        } //single course success close
                        else //Package success start
                        {
                            OnlinePaymentInfo opi = new OnlinePaymentInfo
                            {
                                PackageId = Convert.ToInt32(obj.udf4),
                                IsPackage = true,
                                //BatchId = 0,
                                amount = obj.amount,
                                ApplicationId = Convert.ToInt32(obj.udf3),
                                bank_ref_num = obj.bank_ref_num,
                                //CategoryId = 0,
                                //CourseId = 0,
                                Hash = obj.Hash,
                                key = obj.key,
                                mihpayid = obj.mihpayid,
                                mode = obj.mode,
                                Productinfo = obj.Productinfo,
                                status = obj.status,
                                txnid = obj.txnid,
                                PaymentDate = DateTime.Now,
                                udf1 = obj.udf1,//BatchId
                                udf2 = obj.udf2,//ApplicationCode
                                udf3 = obj.udf3,//ApplicationId
                                udf4 = obj.udf4, //PackageId 0 if single
                                udf5 = obj.udf5
                            };

                            db.OnlinePaymentInfos.Add(opi);
                            await db.SaveChangesAsync();

                            //feeReceipt
                            FeeReceipt fr = new FeeReceipt
                            {
                                Amount = Convert.ToDecimal(obj.amount),
                                ApplicationId = Convert.ToInt32(obj.udf3),
                                PaymentMode = "Online",
                                PrintStatus = false,
                                FeesType = "PackageFee",
                                ReceiptDate = DateTime.Now
                            };
                            db.FeeReceipts.Add(fr);
                            await db.SaveChangesAsync();

                            var aps = db.ApplicationPackageDetails
                                 .Where(x => x.ApplicationId.ToString() == obj.udf3)
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
                                    ApplicationId = Convert.ToInt32(obj.udf3),
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
                                ApplicationId = Convert.ToInt32(obj.udf3),
                                TotalFee = totalFee,
                                FeePaid = Convert.ToDecimal(obj.amount),
                                FeeBal = totalFee - Convert.ToDecimal(obj.amount),
                                PackageId = Convert.ToInt32(obj.udf4)
                            };
                            db.StudentFeeDetails.Add(sfd);
                            await db.SaveChangesAsync();
                            //send mail
                            //Application ap1 = await db.Applications.FindAsync(obj.udf3);
                            int pid = Convert.ToInt32(obj.udf4);
                            var pname = db.packages.Find(pid).PackageName;
                            EmailModel em = new EmailModel
                            {
                                From = ConfigurationManager.AppSettings["admsmail"],
                                FromPass = ConfigurationManager.AppSettings["admsps"],
                                To = obj.Email,
                                Subject = "Course Registration with TSR",
                                Body = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + pname + "  Thanking you T.S.Rahaman"
                            };

                            var res = await MessageService.sendEmail(em);

                            MessageService ms = new MessageService();
                            string msg = "Dear " + obj.Firstname + " " + obj.Lastname + ", with the reference to your Enrolment ID " + obj.udf2 + " This is to confirm that your seat has been confirmed for " + pname + "  Thanking you T.S.Rahaman";
                            string mobileno = obj.Phone;
                            await ms.SendSmsAsync(msg, mobileno);

                            return View("PaymentSuccessNC", obj);
                        }
                    } //success payment close
                    else
                    { //fail payment
                        if (obj.udf4 == "0") //Single COurse, NonPackage
                        {
                            int bid = Convert.ToInt32(obj.udf1);
                            var b = db.Batches.FirstOrDefault(x => x.BatchId == bid);
                            var cid = b.CourseId;
                            var c = db.Courses.FirstOrDefault(x => x.CourseId == cid);
                            var ccId = c.CategoryId;
                            obj.CourseName = c.CourseName;
                            var cc = db.CourseCategories.FirstOrDefault(x => x.CourseCategoryId == ccId);
                            var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == cid);

                            OnlinePaymentInfo opi = new OnlinePaymentInfo
                            {
                                BatchId = bid,
                                amount = obj.amount,
                                ApplicationId = Convert.ToInt32(obj.udf3),
                                bank_ref_num = obj.bank_ref_num,
                                CategoryId = ccId,
                                CourseId = cid,
                                Hash = obj.Hash,
                                key = obj.key,
                                mihpayid = obj.mihpayid,
                                mode = obj.mode,
                                Productinfo = obj.Productinfo,
                                status = obj.status,
                                txnid = obj.txnid,
                                PaymentDate = DateTime.Now,
                                udf1 = obj.udf1,//BatchId
                                udf2 = obj.udf2,//ApplicationCode
                                udf3 = obj.udf3,//ApplicationId
                                udf4 = obj.udf4, //PackageId 0 if single
                                udf5 = obj.udf5
                            };

                            db.OnlinePaymentInfos.Add(opi);
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            OnlinePaymentInfo opi = new OnlinePaymentInfo
                            {
                                PackageId = Convert.ToInt32(obj.udf4),
                                IsPackage = true,
                                //BatchId = 0,
                                amount = obj.amount,
                                ApplicationId = Convert.ToInt32(obj.udf3),
                                bank_ref_num = obj.bank_ref_num,
                                //CategoryId = 0,
                                //CourseId = 0,
                                Hash = obj.Hash,
                                key = obj.key,
                                mihpayid = obj.mihpayid,
                                mode = obj.mode,
                                Productinfo = obj.Productinfo,
                                status = obj.status,
                                txnid = obj.txnid,
                                PaymentDate = DateTime.Now,
                                udf1 = obj.udf1,//BatchId
                                udf2 = obj.udf2,//ApplicationCode
                                udf3 = obj.udf3,//ApplicationId
                                udf4 = obj.udf4, //PackageId 0 if single
                                udf5 = obj.udf5
                            };

                            db.OnlinePaymentInfos.Add(opi);
                            await db.SaveChangesAsync();
                        }
                        return View("PaymentFailNC", obj);
                    }
                }
                else
                {
                    //AMOUNT TAMPERED
                    if (obj.udf4 == "0") //Single COurse, NonPackage Tampered
                    {
                        int bid = Convert.ToInt32(obj.udf1);
                        var b = db.Batches.FirstOrDefault(x => x.BatchId == bid);
                        var cid = b.CourseId;
                        var c = db.Courses.FirstOrDefault(x => x.CourseId == cid);
                        var ccId = c.CategoryId;
                        obj.CourseName = c.CourseName;
                        var cc = db.CourseCategories.FirstOrDefault(x => x.CourseCategoryId == ccId);
                        var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == cid);

                        OnlinePaymentInfo opi = new OnlinePaymentInfo
                        {
                            BatchId = bid,
                            amount = obj.amount,
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            bank_ref_num = obj.bank_ref_num,
                            CategoryId = ccId,
                            CourseId = cid,
                            Hash = obj.Hash,
                            key = obj.key,
                            mihpayid = obj.mihpayid,
                            mode = obj.mode,
                            Productinfo = obj.Productinfo,
                            status = "Tampered",//obj.status,
                            txnid = obj.txnid,
                            PaymentDate = DateTime.Now,
                            udf1 = obj.udf1,//BatchId
                            udf2 = obj.udf2,//ApplicationCode
                            udf3 = obj.udf3,//ApplicationId
                            udf4 = obj.udf4, //PackageId 0 if single
                            udf5 = obj.udf5
                        };

                        db.OnlinePaymentInfos.Add(opi);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        OnlinePaymentInfo opi = new OnlinePaymentInfo
                        {
                            PackageId = Convert.ToInt32(obj.udf4),
                            IsPackage = true,
                            //BatchId = 0,
                            amount = obj.amount,
                            ApplicationId = Convert.ToInt32(obj.udf3),
                            bank_ref_num = obj.bank_ref_num,
                            //CategoryId = 0,
                            //CourseId = 0,
                            Hash = obj.Hash,
                            key = obj.key,
                            mihpayid = obj.mihpayid,
                            mode = obj.mode,
                            Productinfo = obj.Productinfo,
                            status = "Tampered",//obj.status,
                            txnid = obj.txnid,
                            PaymentDate = DateTime.Now,
                            udf1 = obj.udf1,//BatchId
                            udf2 = obj.udf2,//ApplicationCode
                            udf3 = obj.udf3,//ApplicationId
                            udf4 = obj.udf4, //PackageId 0 if single
                            udf5 = obj.udf5
                        };

                        db.OnlinePaymentInfos.Add(opi);
                        await db.SaveChangesAsync();
                    }
                    return View("PaymentFailNC", obj);
                }

            }

            //return View();
        }

        private void GetTxnStatus( string txtid)
        {
            string Url = "https://info.payu.in/merchant/postservice?form=2";

            string method = "verify_payment";
            string salt = "TKLA3Zgf";
            string key = "nDmZR3";
            string var1 = txtid; //Transaction ID of the merchant

            string toHash = key + "|" + method + "|" + var1 + "|" + salt;

            string Hashed = Generatehash512(toHash);

            string postString = "key=" + "nDmZR3" +
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
            String status = (string)account.SelectToken("transaction_details." + var1 + ".status");


        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePaymentNonCet11(ApplicationSumPayNonCetVM obj)
        {
            try
            {

                string[] hashVarsSeq;
                string hash_string = string.Empty;


                if (string.IsNullOrEmpty(obj.txnid)) // generating txnid
                {
                    Random rnd = new Random();
                    string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                    obj.txnid = strHash.ToString().Substring(0, 20);
                    txnid1 = obj.txnid;

                }

                if (string.IsNullOrEmpty(obj.hash)) // generating hash value
                {
                    if (
                        string.IsNullOrEmpty(ConfigurationManager.AppSettings["MERCHANT_KEY"]) ||
                        string.IsNullOrEmpty(obj.txnid) ||
                        string.IsNullOrEmpty(obj.amount.ToString()) ||
                        string.IsNullOrEmpty(obj.FirstName) ||
                        string.IsNullOrEmpty(obj.Email) ||
                        string.IsNullOrEmpty(obj.CellNo) ||
                        string.IsNullOrEmpty(obj.BatchCode)
                        //string.IsNullOrEmpty(Request.Form["surl"]) ||
                        //string.IsNullOrEmpty(Request.Form["furl"])
                        )
                    {
                        //error

                        throw new Exception();
                    }

                    else
                    {

                        hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
                        hash_string = "";
                        foreach (string hash_var in hashVarsSeq)
                        {
                            if (hash_var == "key")
                            {
                                hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                                hash_string = hash_string + '|';
                            }
                            else if (hash_var == "txnid")
                            {
                                hash_string = hash_string + txnid1;
                                hash_string = hash_string + '|';
                            }
                            else if (hash_var == "amount")
                            {
                                hash_string = hash_string + Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                                hash_string = hash_string + '|';
                            }
                            else
                            {

                                hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                                hash_string = hash_string + '|';
                            }
                        }

                        hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

                        hash1 = Generatehash512(hash_string).ToLower();         //generating hash
                        obj.hash = hash1;
                        //action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";// setting URL

                    }


                }

                else if (!string.IsNullOrEmpty(obj.hash))
                {
                    hash1 = Request.Form["hash"];
                    //action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";

                }




                if (!string.IsNullOrEmpty(hash1))
                {

                    Dictionary<string, object> rp = new Dictionary<string, object>();
                    rp.Add("hash", hash1);
                    rp.Add("key", "gtKFFx");
                    rp.Add("txnid", obj.txnid);
                    string amt = Convert.ToDecimal(obj.amount).ToString("g29");
                    rp.Add("amount", amt);
                    rp.Add("firstname", obj.FirstName);

                    rp.Add("email", obj.Email);
                    rp.Add("phone", obj.CellNo);
                    rp.Add("productinfo", obj.BatchCode);
                    rp.Add("surl", "http://localhost:9071/Application/");
                    rp.Add("furl", "http://localhost:9071/");
                    rp.Add("lastname", obj.LastName);
                    //rp.Add("curl", "");
                    //rp.Add("address1", "");
                    //rp.Add("address2", "");
                    //rp.Add("city", "");
                    //rp.Add("state", "");
                    //rp.Add("country", "");
                    //rp.Add("zipcode", "");
                    //rp.Add("udf1", "");
                    //rp.Add("udf2", "");
                    //rp.Add("udf3", "");
                    //rp.Add("udf4", "");
                    //rp.Add("udf5", "");
                    //rp.Add("pg", "");

                    return this.RedirectAndPost("https://test.payu.in/_payment", rp);
                    //Or return new RedirectAndPostActionResult("http://TheUrlToPostDataTo", postData);
                }

                else
                {
                    //no hash

                }

            }

            catch (Exception ex)

            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


            return View();
        }


        #endregion

        #region checkStatus
        public ActionResult checkPaymentStatus()
        {

            return View();
        }
        #endregion

        public ActionResult ViewApplicants()
        {
            //var vm = from a in db.Applications
            //         join cc in db.CourseCategories on a.CategoryId equals cc.CourseCategoryId
            //         join c in db.Courses on a.CourseId equals c.CourseId
            //         join b in db.Batches on a.BatchId equals b.BatchId
            //         where (b.OnlineBookingStatus == true)
            //         select new AdmissionViewApplicantsVM
            //         {
            //             ApplicantName = a.FirstName.ToString() + " " + a.MiddleName.ToString() + " " + a.LastName.ToString(),
            //             ApplicationCode = a.ApplicationCode,
            //             ApplicationId = a.ApplicationId,
            //             BatchCode = b.BatchCode,
            //             CategoryName = cc.CategoryName,
            //             CourseName = c.CourseName
            //         };
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            var obj = new List<ApplicationApplicantsList>();

            return View(obj);
        }

        [HttpGet]
        public ActionResult GetApplicantList(int? BatchId)
        {
            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       //join opi in db.OnlinePaymentInfos on ap.ApplicationId equals opi.ApplicationId 
                       //into opis
                       //from opi in opis.DefaultIfEmpty()
                       join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                       into apllj
                       from apl in apllj.DefaultIfEmpty()
                       where (b.BatchId == BatchId)
                       select new ApplicationApplicantsList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = ap.FirstName + " " + ap.LastName,
                           PaymentStatus = (apl == null)? "Pending" : "Success",
                           Email = ap.Email,
                           Cell = ap.CellNo
                       };

            
            return PartialView("ApplicantsList", list.ToList());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export(string CategoryId, string CourseId, string BatchId)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                int BatchIds = 0;
                BatchIds = Convert.ToInt32(BatchId);
                var list = from ap in db.Applications
                           join b in db.Batches on ap.BatchId equals b.BatchId
                           join opi in db.OnlinePaymentInfos on ap.ApplicationId equals opi.ApplicationId
                           into opis
                           from opi in opis.DefaultIfEmpty()
                               //join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                           where (b.BatchId == BatchIds)
                           select new ApplicationApplicantsList
                           {
                               ApplicationCode = ap.ApplicationCode,
                               ApplicationId = ap.ApplicationId,
                               BatchName = b.BatchCode,
                               Name = ap.FirstName + " " + ap.LastName,
                               PaymentStatus = (opi == null) ? "Pending" : "Success",
                               Email = ap.Email,
                               Cell = ap.CellNo
                           };
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 36, 72, 108, 180);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.AddTitle("Applicants List");

                PdfPTable table = new PdfPTable(5);
                iTextSharp.text.Font f = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                table.TotalWidth = 525f;
                table.LockedWidth = true;
                Chunk beginning = new Chunk("Sir Mohammad Yusuf Seamen Welfare Foundation\n Training Ship Rahaman\n At Post Nhava, Tal. Panvel, Dist. Raigad, Maharashtra - 410206", f);
                PdfPCell header = new PdfPCell(new Phrase(beginning));
                header.Colspan = 5;
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                header.UseVariableBorders = true;
                header.BackgroundColor = BaseColor.WHITE;
                table.AddCell(header);
                Chunk main = new Chunk("Applicants List", f);
                PdfPCell mainheading = new PdfPCell(new Phrase(main));
                mainheading.BackgroundColor = BaseColor.WHITE;
                mainheading.HorizontalAlignment = Element.ALIGN_CENTER;
                mainheading.Colspan = 5;
                table.AddCell(mainheading);
                Chunk cthead1 = new Chunk("Application Code", f);
                PdfPCell thead1 = new PdfPCell(new Phrase(cthead1));
                thead1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead1);
                Chunk cthead2 = new Chunk("Name", f);
                PdfPCell thead2 = new PdfPCell(new Phrase(cthead2));
                thead2.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead2);
                Chunk cthead3 = new Chunk("Email", f);
                PdfPCell thead3 = new PdfPCell(new Phrase(cthead3));
                thead3.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead3);
                Chunk cthead4 = new Chunk("Cell", f);
                PdfPCell thead4 = new PdfPCell(new Phrase(cthead4));
                thead4.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead4);
                Chunk cthead5 = new Chunk("Payment Status", f);
                PdfPCell thead5 = new PdfPCell(new Phrase(cthead5));
                thead5.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead5);


                //actual width of table in points

                //fix the absolute width of the table
                table.LockedWidth = true;

                //relative col widths in proportions - 1/3 and 2/3
                float[] widths = new float[] { 100f, 100f, 100f, 100f, 100f };
                table.SetWidths(widths);
                table.HorizontalAlignment = 0;
                //leave a gap before and after the table
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;


                foreach (var s in list)
                {
                    table.AddCell(s.ApplicationCode.ToString());
                    table.AddCell(s.Name.ToString());
                    table.AddCell(s.Email.ToString());
                    table.AddCell(s.Cell.ToString());
                    table.AddCell(s.PaymentStatus.ToString());
                }

                Paragraph p = new Paragraph();
                p.IndentationLeft = 0;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                p.Add(table);
                pdfDoc.Add(p);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf");

            }
        }

        #region OfflineApplication
        public ActionResult OfflineApplication()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.IsPackage = DropdownData.CourseType();
            ApplicationIndexVM vm = new ApplicationIndexVM();
            return View(vm);
        }
        public ActionResult FillBatchOff(int CourseId)
        {
            var C = db.Batches
                .Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true && c.StartDate >= DbFunctions.AddDays(DateTime.Now,-1) && c.CourseId == (db.CourseFees.Where(x => x.CourseId == CourseId).Select(m => m.CourseId).FirstOrDefault()))
                .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Courses = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, Name = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            ViewBag.Flag = "Test22";
            return Json(Courses, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getBatchRemainingSeatsOff(int BatchId)
        {

            var b = db.Batches.Find(BatchId);
            var cc = db.CourseCategories.Find(b.CategoryId);
            string rem;
            if (cc.CetRequired == false)
            {
                var r = b.TotalSeats - b.BookedSeats;
                rem = r.ToString();
            }
            else
                rem = "null";
            return Json(rem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPackageBatchesOff(int PackageId, int CategoryId)
        {
            var PackageBatches = from pc in db.PackageCourses
                                 join c in db.Courses on pc.CourseId equals c.CourseId
                                 where pc.PackageId == PackageId
                                 select new PackageCourseBatches
                                 {
                                     CourseId = c.CourseId,
                                     CourseName = c.CourseName,
                                     PackageId = pc.PackageId,
                                     BatchDropdowns = (db.Batches
                                         .Where(x => x.CourseId == c.CourseId && x.IsActive == true && x.OnlineBookingStatus == true && x.StartDate >= DbFunctions.AddDays(DateTime.Now, -1))
                                         .Select(x => new BatchDropdown { BatchId = x.BatchId, Name = x.StartDate.ToString() }))
                                     //.ToList()
                                     //.Select(p => new BatchDropdown { BatchId = p.BatchId, Name = Convert.ToDateTime(p.Name).ToString("dd-MM-yyyy") })
                                 };

            //var Courses = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, Name = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            var pb = PackageBatches.ToList().Select(x => new PackageCourseBatches
            {
                BatchDropdowns = x.BatchDropdowns.ToList().Select(y => new BatchDropdown { BatchId = y.BatchId, Name = Convert.ToDateTime(y.Name).ToString("dd-MM-yyyy") }),
                CourseId = x.CourseId,
                CourseName = x.CourseName,
                PackageId = x.PackageId
            });
            ApplicationIndexVM vm = new ApplicationIndexVM
            {
                CategoryId = CategoryId,
                PackageBatchId = pb.ToList(),
                PackageId = PackageId
            };
            return PartialView("IndexPackageBatches", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfflineApplication(ApplicationIndexVM obj)
        {
            if (obj.PackageId == 0)
            {
                ViewBag.InfoFlag = "NonPackage"; //for info flag

                var cc = db.CourseCategories.FirstOrDefault(x => x.CourseCategoryId == obj.CategoryId);
                bool isCet = (bool)cc.CetRequired;
                var c = db.Courses.FirstOrDefault(x => x.CourseId == obj.CourseId);
                var courseName = c.CourseName;

                ViewBag.Gender = Common.DropdownData.Gender();
                ViewBag.Meals = Common.DropdownData.Meals();
                ViewBag.YesNo = Common.DropdownData.YesNo();
                if (isCet == true)
                {
                    ApplicationCetVM vm = new ApplicationCetVM
                    {
                        CourseId = obj.CourseId,
                        CategoryId = obj.CategoryId,
                        BatchId = obj.BatchId,
                        CourseName = courseName
                    };

                    ViewBag.ShirtSize = Common.DropdownData.ShirtSize();
                    ViewBag.PantSize = Common.DropdownData.PantSize();
                    ViewBag.ShoeSize = Common.DropdownData.ShoeSize();

                    return View("OfflineCetApplication", vm);
                }
                else
                {
                    ApplicationNonCetVM vm = new ApplicationNonCetVM
                    {
                        CourseId = obj.CourseId,
                        CategoryId = obj.CategoryId,
                        BatchId = obj.BatchId,
                        CourseName = courseName,
                        PackageId = 0
                    };

                    var b = db.Batches.FirstOrDefault(x => x.BatchId == obj.BatchId);
                    int totSeat = (int)b.TotalSeats;
                    //int reservSeat = (int)b.ReserveSeats;
                    int booked = (int)b.BookedSeats;
                    var remain = totSeat - (booked);
                    ViewBag.remain = remain;
                    if (remain > 0)
                    {
                        return View("OfflineNonCetApplication", vm);
                    }
                    else
                    {
                        ViewBag.BatchCode = obj.BatchCode;
                        return View("NonCetApplicationFull", vm);
                    }

                }
            }
            else
            {
                //For Package Course
                ApplicationNonCetVM vm = new ApplicationNonCetVM
                {
                    PackageId = obj.PackageId,
                    PackageBatchId = obj.PackageBatchId.ToList().Select(x => new PackageCourseBatches
                    {
                        BatchId = x.BatchId,
                        CourseId = (int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).CourseId,
                        CourseName = db.Courses.FirstOrDefault(y => y.CourseId == db.Batches.FirstOrDefault(z => z.BatchId == x.BatchId).CourseId).CourseName,
                        RemainingSeats = ((int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).TotalSeats) - ((int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).ReserveSeats) - ((int)db.Batches.FirstOrDefault(y => y.BatchId == x.BatchId).BookedSeats)
                    }).ToList()
                };
                ViewBag.Gender = Common.DropdownData.Gender();
                ViewBag.Meals = Common.DropdownData.Meals();
                ViewBag.YesNo = Common.DropdownData.YesNo();
                ViewBag.InfoFlag = "Package";
                return View("OfflineNonCetApplication", vm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OfflineCetApplication(ApplicationCetVM obj, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var b = db.Batches.FirstOrDefault(x => x.BatchId == obj.BatchId);
                var bc = b.BatchCode;
                var c = db.Courses.FirstOrDefault(x => x.CourseId == obj.CourseId);
                var cc = c.CourseCode;
                var n = db.Applications.Count(x => x.BatchId == obj.BatchId);
                n = n + 1;

                var allowedExtensions = new[] {
                ".Jpg", ".png", ".jpg", "jpeg"
                  };
                //string fileName = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0');
                //string imgPath = "/Uploads/CetPhoto/" + fileName;
                //file.SaveAs(Server.MapPath(imgPath));
                //var ext = Path.GetExtension(file.FileName);
                var root = "/Uploads/CetPhoto/";
                var appcode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0');
                var files = file;
                var ext = Path.GetExtension(files.FileName);
                var fileName = appcode + ext;
                var path = Server.MapPath(root + fileName);
                files.SaveAs(path);
                var filepathname = root + fileName;

                Application ap = new Application
                {
                    ApplicationCode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0'),
                    AnnualIncome = obj.AnnualIncome,
                    BatchId = obj.BatchId,
                    Caste = obj.Caste,
                    CategoryId = obj.CategoryId,
                    CellNo = obj.CellNo,
                    Citizenship = obj.Citizenship,
                    CourseId = obj.CourseId,
                    DateOfBirth = obj.DateOfBirth,
                    Email = obj.Email,
                    FatherEmail = obj.FatherEmail,
                    FatherOccupation = obj.FatherOccupation,
                    FirstName = obj.FirstName,
                    Gender = obj.FirstName,
                    GradAddress = obj.GradAddress,
                    GradCity = obj.GradCity,
                    GradCollegeName = obj.GradCollegeName,
                    GradPassAttempt = obj.GradPassAttempt,
                    GradPassingYear = obj.GradPassingYear,
                    GradPercentage = obj.GradPercentage,
                    GradPin = obj.GradPin,
                    GradState = obj.GradState,
                    GradSubjects = obj.GradSubjects,
                    GradUniversity = obj.GradUniversity,
                    GuardianAddress = obj.GuardianAddress,
                    GuardianCity = obj.GuardianCity,
                    GuardianContact = obj.GuardianContact,
                    GuardianEmail = obj.GuardianEmail,
                    GuardianName = obj.GuardianName,
                    GuardianPin = obj.GuardianPin,
                    GuardianRelation = obj.GuardianRelation,
                    GuardianState = obj.GuardianState,
                    Height = obj.Height,
                    IdentificationMark = obj.IdentificationMark,
                    InterAddress = obj.InterAddress,
                    InterBoard = obj.InterBoard,
                    InterChemistry = obj.InterChemistry,
                    InterCity = obj.InterCity,
                    InterMath = obj.InterMath,
                    InterEnglish = obj.InterEnglish,
                    InterPassingYear = obj.InterPassingYear,
                    InterPercentage = obj.InterPercentage,
                    InterPhysics = obj.InterPhysics,
                    InterPin = obj.InterPin,
                    InterRollNo = obj.InterRollNo,
                    InterSchoolName = obj.InterSchoolName,
                    InterState = obj.InterState,
                    LastName = obj.LastName,
                    MiddleName = obj.MiddleName,
                    MotherName = obj.MotherName,
                    PantSize = obj.PantSize,
                    PassportNo = obj.PassportNo,
                    PermenentAddress = obj.PermenentAddress,
                    PermenentCity = obj.PermenentCity,
                    PermenentContactNo = obj.PermenentContactNo,
                    PermenentPin = obj.PermenentPin,
                    PermenentState = obj.PermenentState,
                    PlaceOfBirth = obj.PlaceOfBirth,
                    PreferredMeal = obj.PreferredMeal,
                    PresentAddress = obj.PresentAddress,
                    PresentCity = obj.PresentCity,
                    PresentContactNo = obj.PresentContactNo,
                    PresentPin = obj.PresentPin,
                    PresentState = obj.PresentState,
                    Religion = obj.Religion,
                    SchoolAddress = obj.SchoolAddress,
                    SchoolBoard = obj.SchoolBoard,
                    SchoolCity = obj.SchoolCity,
                    SchoolEnglish = obj.SchoolEnglish,
                    SchoolMath = obj.SchoolMath,
                    SchoolName = obj.SchoolName,
                    SchoolPassingYear = obj.SchoolPassingYear,
                    SchoolPercentage = obj.SchoolPercentage,
                    SchoolPin = obj.SchoolPin,
                    SchoolScience = obj.SchoolScience,
                    SchoolState = obj.SchoolState,
                    ShirtSize = obj.ShirtSize,
                    ShoeSize = obj.ShoeSize,
                    Weight = obj.Weight,
                    FullName = obj.FullName,
                    FatherFullName = obj.FatherFullName
                };
                db.Applications.Add(ap);
                await db.SaveChangesAsync();

                var id = ap.ApplicationId;
                var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == ap.CourseId);
                //var c = db.Courses.FirstOrDefault(x => x.CourseId == ap.CourseId);
                var applicationFee = cf.ApplicationFee;

                ApplicationSumPayCetVM apsm = new ApplicationSumPayCetVM
                {
                    ApplicationId = ap.ApplicationId,
                    CategoryId = ap.CategoryId,
                    CourseId = ap.CourseId,
                    BatchId = ap.BatchId,
                    ApplicationCode = ap.ApplicationCode,
                    amount = applicationFee,
                    CellNo = ap.CellNo,
                    Email = ap.Email,
                    FirstName = ap.FirstName,
                    LastName = ap.LastName,
                    CourseName = c.CourseName,
                    BatchCode = bc,
                    udf1 = ap.BatchId.ToString(), //udf1 BatchId
                    udf2 = ap.ApplicationCode, //udf2 ApplicationCode  
                    udf3 = ap.ApplicationId.ToString(), //udf3 ApplicationID   
                    udf4 = "0" //Single Course Non Package          
                };

                MessageService ms = new MessageService();
                string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + obj.CourseName + "  Thanking you T.S.Rahaman";
                string mobileno = ap.CellNo;
                try
                {
                    await ms.SendSmsAsync(msg, mobileno);
                }
                catch (Exception) { }

                return View("OfflineApplicationSummaryPre", apsm);
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OfflineNonCetApplication(ApplicationNonCetVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.PackageId == 0 || obj.PackageId == null)
                {
                    var b = db.Batches.FirstOrDefault(x => x.BatchId == obj.BatchId);
                    var bc = b.BatchCode;
                    var c = db.Courses.FirstOrDefault(x => x.CourseId == obj.CourseId);
                    var cc = c.CourseCode;
                    var n = db.Applications.Count(x => x.BatchId == obj.BatchId);
                    n = n + 1;
                    Application ap = new Application
                    {
                        IsPackage = false, //Single Course
                        ApplicationCode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0'),
                        BatchId = obj.BatchId,
                        CategoryId = obj.CategoryId,
                        CellNo = obj.CellNo,
                        CertOfCompetencyNo = obj.CertOfCompetencyNo,
                        CdcNo = obj.CdcNo,
                        Citizenship = obj.Citizenship,
                        CourseId = obj.CourseId,
                        DateOfBirth = obj.DateOfBirth,
                        Email = obj.Email,
                        FirstName = obj.FirstName,
                        Gender = obj.FirstName,
                        InDosNo = obj.InDosNo,
                        LastName = obj.LastName,
                        MiddleName = obj.MiddleName,
                        PassportNo = obj.PassportNo,
                        PlaceOfBirth = obj.PlaceOfBirth,
                        GradeOfCompetencyNo = obj.GradeOfCompetencyNo,
                        CategoryOfCandidate = obj.CategoryOfCandidate,
                        ShippingCompany = obj.ShippingCompany,
                        RankOfCandidate = obj.RankOfCandidate,
                        CourseAttendedInTSR = obj.CourseAttendedInTSR,
                        FPFF_AFF_1995 = obj.FPFF_AFF_1995,
                        PermenentAddress = obj.PermenentAddress,
                        PermenentCity = obj.PermenentCity,
                        PermenentContactNo = obj.PermenentContactNo,
                        PermenentPin = obj.PermenentPin,
                        PermenentState = obj.PermenentState,
                        FullName = obj.FullName,
                        OldCertificateDate = obj.OldCertificateDate,
                        OldCertificateIssuedBy = obj.OldCertificateIssuedBy,
                        OldCertificateNo = obj.OldCertificateNo
                    };
                    db.Applications.Add(ap);
                    await db.SaveChangesAsync();

                    var id = ap.ApplicationId;
                    var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == ap.CourseId);
                    var actualFee = cf.ActualFee;
                    var minBal = cf.MinBalance;
                    var gstPercent = cf.GstPercentage;
                    decimal taxAmount;

                    if (gstPercent > 0)
                    {
                        taxAmount = ((decimal)actualFee / 100) * (decimal)gstPercent;
                    }
                    else { taxAmount = 0; }
                    decimal amount = 0;

                    if (actualFee > minBal && minBal > 1)
                    {
                        amount = (decimal)minBal + taxAmount;
                    }
                    else
                    {
                        amount = (decimal)actualFee;
                    }

                    var courseName = c.CourseName;

                    ApplicationSumPayNonCetVM apsm = new ApplicationSumPayNonCetVM
                    {

                        ApplicationId = ap.ApplicationId,
                        CategoryId = ap.CategoryId,
                        CourseId = ap.CourseId,
                        BatchId = ap.BatchId,
                        ApplicationCode = ap.ApplicationCode,
                        amount = amount,
                        CellNo = ap.CellNo,
                        Email = ap.Email,
                        FirstName = ap.FirstName,
                        LastName = ap.LastName,
                        CourseName = courseName,
                        BatchCode = bc,
                        CourseFee = actualFee,
                        TaxAmount = taxAmount,
                        udf1 = ap.BatchId.ToString(), //udf1 BatchId
                        udf2 = ap.ApplicationCode, //udf2 ApplicationCode 
                        udf3 = ap.ApplicationId.ToString(), //udf3 ApplicationID     
                        udf4 = "0" //Single Course, NonPackage        
                    };

                    MessageService ms = new MessageService();
                    string msg = "Dear " + ap.FirstName + " " + ap.LastName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + obj.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                    string mobileno = ap.CellNo;
                    await ms.SendSmsAsync(msg, mobileno);

                    return View("OfflineApplicationSummary", apsm);
                }
                else
                {
                    //Package
                    var bid = obj.PackageBatchId.FirstOrDefault().BatchId;
                    var b = db.Batches.FirstOrDefault(x => x.BatchId == bid);
                    var bc = b.BatchCode;
                    var c = db.Courses.FirstOrDefault(x => x.CourseId == b.CourseId);
                    var cc = c.CourseCode;
                    var n = db.Applications.Count(x => x.BatchId == obj.BatchId);
                    n = n + 1;
                    Application ap = new Application
                    {
                        IsPackage = true, //For Package
                        PackageId = obj.PackageId, //For Package
                        ApplicationCode = cc.ToString() + bc.ToString() + n.ToString().PadLeft(4, '0'),
                        BatchId = obj.BatchId,
                        CategoryId = obj.CategoryId,
                        CellNo = obj.CellNo,
                        CertOfCompetencyNo = obj.CertOfCompetencyNo,
                        CdcNo = obj.CdcNo,
                        Citizenship = obj.Citizenship,
                        CourseId = obj.CourseId,
                        DateOfBirth = obj.DateOfBirth,
                        Email = obj.Email,
                        FirstName = obj.FirstName,
                        Gender = obj.FirstName,
                        InDosNo = obj.InDosNo,
                        LastName = obj.LastName,
                        MiddleName = obj.MiddleName,
                        PassportNo = obj.PassportNo,
                        PlaceOfBirth = obj.PlaceOfBirth,
                        GradeOfCompetencyNo = obj.GradeOfCompetencyNo,
                        CategoryOfCandidate = obj.CategoryOfCandidate,
                        ShippingCompany = obj.ShippingCompany,
                        RankOfCandidate = obj.RankOfCandidate,
                        CourseAttendedInTSR = obj.CourseAttendedInTSR,
                        FPFF_AFF_1995 = obj.FPFF_AFF_1995,
                        FullName = obj.FullName,
                        OldCertificateDate = obj.OldCertificateDate,
                        OldCertificateIssuedBy = obj.OldCertificateIssuedBy,
                        OldCertificateNo = obj.OldCertificateNo
                    };
                    db.Applications.Add(ap);
                    await db.SaveChangesAsync();

                    //Application Package Details and Fee Calcuation
                    decimal fee = 0, taxamount = 0, minBalance = 0;
                    foreach (var item in obj.PackageBatchId)
                    {
                        ApplicationPackageDetail apd = new ApplicationPackageDetail
                        {
                            ApplicationId = ap.ApplicationId,
                            BatchId = item.BatchId,
                            PackageId = (int)obj.PackageId,
                            ConfirmStatus = false,
                            CourseId = item.CourseId
                        };
                        db.ApplicationPackageDetails.Add(apd);

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
                    await db.SaveChangesAsync();


                    ApplicationSumPayNonCetVM apsm = new ApplicationSumPayNonCetVM
                    {
                        ApplicationId = ap.ApplicationId,
                        CategoryId = ap.CategoryId,
                        CourseId = ap.CourseId,
                        BatchId = ap.BatchId,
                        ApplicationCode = ap.ApplicationCode,
                        amount = minBalance, //package Fee Min Balance
                        CellNo = ap.CellNo,
                        Email = ap.Email,
                        FirstName = ap.FirstName,
                        LastName = ap.LastName,
                        CourseName = db.packages.FirstOrDefault(x => x.PackageId == ap.PackageId).PackageName, //PackageName
                        BatchCode = bc,
                        CourseFee = fee, //packageFee
                        TaxAmount = taxamount,
                        udf1 = ap.BatchId.ToString(), //udf1 BatchId
                        udf2 = ap.ApplicationCode, //udf2 ApplicationCode 
                        udf3 = ap.ApplicationId.ToString(), //udf3 ApplicationID 
                        udf4 = ap.PackageId.ToString() //udf4 PackageId if package             
                    };

                    MessageService ms = new MessageService();
                    string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your Application has been submitted for " + db.packages.Find(ap.PackageId).PackageName + "  Thanking you T.S.Rahaman";
                    string mobileno = ap.CellNo;
                    await ms.SendSmsAsync(msg, mobileno);

                    return View("ApplicationSummary", apsm);
                }
            }
            ViewBag.Gender = DropdownData.Gender();
            ViewBag.Meals = DropdownData.Meals();
            ViewBag.YesNo = DropdownData.YesNo();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfflineMakePaymentNonCet(ApplicationSumPayNonCetVM obj)
        {


            string[] hashVarsSeq;
            string hash_string = string.Empty;


            if (string.IsNullOrEmpty(obj.txnid)) // generating txnid
            {
                Random rnd = new Random();
                string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                obj.txnid = strHash.ToString().Substring(0, 20);
                txnid1 = obj.txnid;

            }

            hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
            hash_string = "";
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var == "key")
                {
                    hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "txnid")
                {
                    hash_string = hash_string + txnid1;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "amount")
                {
                    hash_string = hash_string + Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "productinfo")
                {
                    hash_string = hash_string + obj.BatchCode.ToString();
                    hash_string = hash_string + '|';
                }
                else
                {

                    hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                    hash_string = hash_string + '|';
                }
            }

            hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

            hash1 = Generatehash512(hash_string).ToLower();         //generating hash
            obj.hash = hash1;

            string mkey = ConfigurationManager.AppSettings["MERCHANT_KEY"];
            string surl = ConfigurationManager.AppSettings["surlOffline"];
            string furl = ConfigurationManager.AppSettings["furlOffline"];
            if (!string.IsNullOrEmpty(hash1))
            {

                Dictionary<string, object> rp = new Dictionary<string, object>();
                rp.Add("hash", hash1);
                rp.Add("key", mkey);
                rp.Add("txnid", obj.txnid);
                string amt = Convert.ToDecimal(obj.amount).ToString("g29");
                rp.Add("amount", amt);
                rp.Add("firstname", obj.FirstName);

                rp.Add("email", obj.Email);
                rp.Add("phone", obj.CellNo);
                rp.Add("productinfo", obj.BatchCode);
                rp.Add("surl", surl);
                rp.Add("furl", furl);
                rp.Add("lastname", obj.LastName);
                rp.Add("curl", obj.curl);
                //rp.Add("address1", null);
                //rp.Add("address2", null);
                //rp.Add("city", null);
                //rp.Add("state", null);
                //rp.Add("country", null);
                //rp.Add("zipcode", null);
                rp.Add("udf1", obj.udf1);
                rp.Add("udf2", obj.udf2);
                rp.Add("udf3", obj.udf3);
                rp.Add("udf4", obj.udf4);
                rp.Add("udf5", obj.udf5);
                //rp.Add("pg", "");

                string payu = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                return this.RedirectAndPost(payu, rp);
                //Or return new RedirectAndPostActionResult("http://TheUrlToPostDataTo", postData);
            }

            else
            {
                //no hash

            }





            return View();
        }

        public ActionResult OfflineMakePaymentCet(ApplicationSumPayCetVM obj)
        {


            string[] hashVarsSeq;
            string hash_string = string.Empty;


            if (string.IsNullOrEmpty(obj.txnid)) // generating txnid
            {
                Random rnd = new Random();
                string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                obj.txnid = strHash.ToString().Substring(0, 20);
                txnid1 = obj.txnid;

            }

            hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
            hash_string = "";
            foreach (string hash_var in hashVarsSeq)
            {
                if (hash_var == "key")
                {
                    hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "txnid")
                {
                    hash_string = hash_string + txnid1;
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "amount")
                {
                    hash_string = hash_string + Convert.ToDecimal(Request.Form[hash_var]).ToString("g29");
                    hash_string = hash_string + '|';
                }
                else if (hash_var == "productinfo")
                {
                    hash_string = hash_string + obj.BatchCode.ToString();
                    hash_string = hash_string + '|';
                }
                else
                {

                    hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                    hash_string = hash_string + '|';
                }
            }

            hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

            hash1 = Generatehash512(hash_string).ToLower();         //generating hash
            obj.hash = hash1;

            string mkey = ConfigurationManager.AppSettings["MERCHANT_KEY"];
            string surl = ConfigurationManager.AppSettings["surlOffline"];
            string furl = ConfigurationManager.AppSettings["furlOffline"];
            if (!string.IsNullOrEmpty(hash1))
            {

                Dictionary<string, object> rp = new Dictionary<string, object>();
                rp.Add("hash", hash1);
                rp.Add("key", mkey);
                rp.Add("txnid", obj.txnid);
                string amt = Convert.ToDecimal(obj.amount).ToString("g29");
                rp.Add("amount", amt);
                rp.Add("firstname", obj.FirstName);

                rp.Add("email", obj.Email);
                rp.Add("phone", obj.CellNo);
                rp.Add("productinfo", obj.BatchCode);
                rp.Add("surl", surl);
                rp.Add("furl", furl);
                rp.Add("lastname", obj.LastName);
                rp.Add("curl", obj.curl);
                //rp.Add("address1", null);
                //rp.Add("address2", null);
                //rp.Add("city", null);
                //rp.Add("state", null);
                //rp.Add("country", null);
                //rp.Add("zipcode", null);
                rp.Add("udf1", obj.udf1);
                rp.Add("udf2", obj.udf2);
                rp.Add("udf3", obj.udf3);
                rp.Add("udf4", obj.udf4);
                rp.Add("udf5", obj.udf5);
                //rp.Add("pg", "");

                string payu = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                return this.RedirectAndPost(payu, rp);
                //Or return new RedirectAndPostActionResult("http://TheUrlToPostDataTo", postData);
            }

            else
            {
                //no hash

            }





            return View();
        }
        #endregion
    }
}