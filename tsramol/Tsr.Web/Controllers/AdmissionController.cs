using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;

namespace Tsr.Web.Controllers
{
    public class AdmissionController : Controller
    {
        AppContext db = new AppContext();
        #region Common
        
        public JsonResult FillBatch(int? CourseId)
        {
            var C = db.Batches
                .Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true)
                .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            return Json(Batches, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FillBatchForCet(int? CourseId)
        {
            //var Batches = db.Batches.Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true);
            var C = from b in db.Batches
                          join ct in db.CetMasters on b.BatchId equals ct.BatchId
                          into cts
                          from ct in cts.DefaultIfEmpty()
                          where (ct == null && b.CourseId == CourseId && b.IsActive == true)
                          select new { BatchId =  b.BatchId, Name = b.StartDate };
            var Batches = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            return Json(Batches, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCet(int? BatchId)
        {
            var Cet = db.CetMasters.Where(b => b.BatchId == BatchId && b.IsActive == true);
            return Json(Cet, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult Index()
        {
            
            return View();
        }

        #region CET
        public ActionResult CETSchedule()
        {
            var vm = from cm in db.CetMasters
                     join c in db.Courses on cm.CourseId equals c.CourseId
                     join b in db.Batches on cm.BatchId equals b.BatchId
                     where (b.IsActive == true && b.OnlineBookingStatus == true)
                     select new AddmissionCetListVM
                     {
                         BatchCode = b.BatchCode,
                         CetCode = cm.CetCode,
                         CetDate = cm.CetDate,
                         CetId = cm.CetMasterId,
                         CetTime = cm.CetTime,
                         CourseCode = c.CourseCode,
                         CourseName = c.CourseName,
                         IsActive = cm.IsActive,
                         StartDate = b.StartDate,
                         Venue = cm.Venue
                     };
            return View(vm.ToList());
        }
        public ActionResult CetCreate()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("CetCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CetCreate(AddmissionCetCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                CetMaster cm = new CetMaster
                {
                    BatchId = obj.BatchId,
                    CetCode = obj.CetCode,
                    CetDate = obj.CetDate,
                    CetTime = obj.CetTime,
                    CourseId = obj.CourseId,
                    IsActive = obj.IsActive,
                    StartDate = obj.StartDate,
                    Venue = obj.Venue
                };

                db.CetMasters.Add(cm);
                await db.SaveChangesAsync();

                var sm = (from ap in db.Applications
                          join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                          where (ap.BatchId == obj.BatchId)
                          select new
                          {
                              BatchId = (int)obj.BatchId,
                              ApplicationId = ap.ApplicationId,
                              CetMasterId = cm.CetMasterId,
                              SelectStatus = false,
                              Marks1 = 0,
                              Marks2 = 0,
                              Marks3 = 0,
                              Marks4 = 0,
                              Total = 0
                          }).AsEnumerable().Select(x => new CetMark
                                {
                                  BatchId = x.BatchId,
                                  ApplicationId = x.ApplicationId,
                                  CetMasterId = x.CetMasterId,
                                  SelectStatus = x.SelectStatus,
                                  Marks1 = x.Marks1,
                                  Marks2 = x.Marks2,
                                  Marks3 = 0,
                                  Marks4 = 0,
                                  Total = 0
                          }).ToList();
                db.CetMarks.AddRange(sm);
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("CetCreate", obj);
        }
        #endregion

        #region Halltickets
        public ActionResult HallTickets()
        {
            //var obj = from cs in db.CetMasters 
            //          join b in db.Batches on cs.BatchId equals b.BatchId
            //          where (cs.IsActive == true)
            //          select 

            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<ApplicationApplicantsList>();
            return View(obj);
        }
        public ActionResult GetApplicantForHalltickets(int? CetMasterId)
        {
            var cm = db.CetMasters.FirstOrDefault(x => x.CetMasterId == CetMasterId);
            var BatchId = cm.BatchId;

            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join opi in db.OnlinePaymentInfos on ap.ApplicationId equals opi.ApplicationId

                       where (b.BatchId == BatchId)
                       select new ApplicationApplicantsList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = ap.FirstName + " " + ap.LastName,
                           PaidAmount = opi.amount,
                           Email = ap.Email,
                           Cell = ap.CellNo
                       };


            return PartialView("ApplicantsList", list.ToList());
        }
        #endregion

        #region AppliedList

        public ActionResult ViewApplicants()
        {
            var vm = from a in db.Applications
                     join cc in db.CourseCategories on a.CategoryId equals cc.CourseCategoryId
                     join c in db.Courses on a.CourseId equals c.CourseId
                     join b in db.Batches on a.BatchId equals b.BatchId
                     where (b.OnlineBookingStatus == true)
                     select new AdmissionViewApplicantsVM
                     {
                         ApplicantName = a.FirstName.ToString() + " " + a.MiddleName.ToString() + " " + a.LastName.ToString(),
                         ApplicationCode = a.ApplicationCode,
                         ApplicationId = a.ApplicationId,
                         BatchCode = b.BatchCode,
                         CategoryName = cc.CategoryName,
                         CourseName = c.CourseName
                     };

            return View(vm.ToList());
        }

        public ActionResult ViewApplied()
        {
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

                       where (b.BatchId == BatchId)
                       select new ApplicationApplicantsList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = ap.FirstName + " " + ap.LastName,
                           //PaidAmount = opi.amount,
                           Email = ap.Email,
                           Cell = ap.CellNo
                       };


            return PartialView("ApplicantsList", list.ToList());
        }

        #endregion

        #region EntranceMarks
        public ActionResult EntranceMarksEntry()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<EntranceMarksListVM>();

            return View(obj);
        }
        [HttpPost]
        public async Task<ActionResult> EntranceMarksEntry(List<EntranceMarksListVM> obj)
        {
            if (obj != null)
            { 
                foreach (var item in obj)
                {
                    CetMark cm = db.CetMarks.FirstOrDefault(x => x.ApplicationId == item.ApplicationId);
                    cm.Marks1 = item.Marks1;
                    cm.Marks2 = item.Marks2;
                    cm.Marks3 = item.Marks3;
                    cm.Marks4 = item.Marks4;
                    cm.Total = (item.Marks1 + item.Marks2 + item.Marks3 + item.Marks4);
                }
                await db.SaveChangesAsync();
            }

            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var v = new List<EntranceMarksListVM>();
            return View(v);
        }

        [HttpGet]
        public ActionResult GetListForMarksEntry(int? BatchId, int? CetMasterId)
        {
            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join ct in db.CetMasters on b.BatchId equals ct.BatchId
                       join sm in db.CetMarks on ap.ApplicationId equals sm.ApplicationId
                       //into sml 
                       //from sm in sml.DefaultIfEmpty()
                       where (b.BatchId == BatchId && ct.CetMasterId == CetMasterId)
                       select new EntranceMarksListVM
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           //BatchName = b.BatchCode,
                           Name = ap.FirstName + " " + ap.LastName,
                           Marks1 = sm.Marks1,
                           Marks2 = sm.Marks2,
                           Marks3 = sm.Marks3,
                           Marks4 = sm.Marks4
                       };


            return PartialView("EntranceMarksList", list.ToList());
        }

        #endregion

        #region TopStudents
        public ActionResult TopStudents()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<EntranceMarksListVM>();
            return View(obj);
        }
        [HttpPost]
        public async Task<ActionResult> TopStudents(List<EntranceMarksListVM> obj, int? InterviewMasterId)
        {
            if (obj != null && InterviewMasterId != null)
            {
                foreach (var item in obj)
                {
                    var ap = await db.Applications.FindAsync(item.ApplicationId);

                    CetInterview ci = new CetInterview
                    {
                        ApplicationId = item.ApplicationId,
                        BatchId = (int) ap.BatchId,
                        InterviewMasterId = (int)InterviewMasterId
                    };
                    db.CetInterviews.Add(ci);
                }
                await db.SaveChangesAsync();
            }

            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var v = new List<EntranceMarksListVM>();

            return View(v);
        }
        [HttpGet]
        public ActionResult GetListForTop(int? BatchId, int? CetMasterId, int? Count)
        {
            if (Count != null)
            {
                var list = from ap in db.Applications
                           join b in db.Batches on ap.BatchId equals b.BatchId
                           join ct in db.CetMasters on b.BatchId equals ct.BatchId
                           join sm in db.CetMarks on ap.ApplicationId equals sm.ApplicationId
                           //into sml 
                           //from sm in sml.DefaultIfEmpty()
                           where (b.BatchId == BatchId && ct.CetMasterId == CetMasterId)
                           orderby sm.Total descending
                           select new EntranceMarksListVM
                           {
                               ApplicationCode = ap.ApplicationCode,
                               ApplicationId = ap.ApplicationId,
                               //BatchName = b.BatchCode,
                               Name = ap.FirstName + " " + ap.LastName,
                               Marks1 = sm.Marks1,
                               Marks2 = sm.Marks2,
                               Marks3 = sm.Marks3,
                               Marks4 = sm.Marks4
                           };
                var c = (int)Count;
                var topList = list.Take(c);
                var ci = db.CetInterviews.FirstOrDefault(x => x.BatchId == BatchId);
                if (ci != null)
                    ViewBag.Flag = "1";
                else
                    ViewBag.Flag = "1";

                ViewBag.InterveiwMasters = new SelectList(db.InterviewMasters.ToList(), "InterviewMasterId", "InterviewCode");
                return PartialView("TopStudentsList", topList.ToList());
            }
            var obj = new List<EntranceMarksListVM>();
            return PartialView("TopStudentsList", obj.ToList());
        }

        #endregion

        #region Interview
        public ActionResult Interview()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<AdmissionInterviewListVM>();
           // ViewBag.MedicalMaster = new SelectList(db.MedicalMasters.ToList(), "MedicalMasterId", "MedicalMasterCode");
            return View(obj);
        }
        [HttpPost]
        public async Task<ActionResult> Interview(List<AdmissionInterviewListVM> obj, int? MedicalMasterId)
        {
            if (obj != null && MedicalMasterId != null)
            {
                foreach (var item in obj)
                {
                    if (item.Select == true)
                    {
                        var ap = await db.Applications.FindAsync(item.ApplicationId);

                        CetMedical ci = new CetMedical
                        {
                            ApplicationId = item.ApplicationId,
                            BatchId = (int)ap.BatchId,
                            MedicalMasterId = (int)MedicalMasterId
                        };
                        db.CetMedicals.Add(ci);
                    }
                }
                await db.SaveChangesAsync();
            }
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var l = new List<AdmissionInterviewListVM>();
            return View(l);
        }
        [HttpGet]
        public ActionResult GetListForInterview(int? BatchId)
        {
            if (BatchId != null)
            {
                var list = from cis in db.CetInterviews
                           join ap in db.Applications on cis.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on cis.BatchId equals b.BatchId
                           join cim in db.InterviewMasters on cis.InterviewMasterId equals cim.InterviewMasterId
                           where (cis.BatchId == BatchId)
                           select new AdmissionInterviewListVM
                           {
                               ApplicationId = ap.ApplicationId,
                               ApplicationCode = ap.ApplicationCode,
                               Cell = ap.CellNo,
                               Email = ap.Email,
                               Name = ap.FirstName + " " + ap.LastName
                           };
                ViewBag.Flag = "1";
                ViewBag.MedicalMaster = new SelectList(db.MedicalMasters.ToList(), "MedicalMasterId", "MedicalCode");
                return PartialView("InterviewList", list.ToList());
            }
            var obj = new List<AdmissionInterviewListVM>();
            return PartialView("InterviewList", obj.ToList());
        }
        #endregion

        #region MedicalTest
        public ActionResult MedicalTest()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<AdmissionMedicalListVM>();
            // ViewBag.MedicalMaster = new SelectList(db.MedicalMasters.ToList(), "MedicalMasterId", "MedicalMasterCode");
            return View(obj);
            
        }

        [HttpPost]
        public async Task<ActionResult> MedicalTest(List<AdmissionMedicalListVM> obj)
        {
            if (obj != null)
            {
                foreach (var item in obj)
                {
                    if (item.Select == true)
                    {
                        //var ap = await db.Applications.FindAsync(item.ApplicationId);

                        Applied apl = db.Applied.FirstOrDefault(x=>x.ApplicationId == item.ApplicationId);
                        apl.AdmissionStatus = true;                        
                    }
                }
                await db.SaveChangesAsync();
            }
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var l = new List<AdmissionMedicalListVM>();
            return View(l);
        }
        [HttpGet]
        public ActionResult GetListForMedical(int? BatchId)
        {
            if (BatchId != null)
            {
                var list = from mt in db.CetMedicals
                           join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on mt.BatchId equals b.BatchId
                           join mm in db.MedicalMasters on mt.MedicalMasterId equals mm.MedicalMasterId
                           where (mt.BatchId == BatchId)
                           select new AdmissionMedicalListVM
                           {
                               ApplicationId = ap.ApplicationId,
                               ApplicationCode = ap.ApplicationCode,
                               Cell = ap.CellNo,
                               Email = ap.Email,
                               Name = ap.FirstName + " " + ap.LastName
                           };
                ViewBag.Flag = "1";
                ViewBag.MedicalMaster = new SelectList(db.MedicalMasters.ToList(), "MedicalMasterId", "MedicalCode");
                //batchDetails
                //var bt = db.Batches.Find(BatchId);
                //ViewBag.TotalSeats = bt.TotalSeats;
                //ViewBag.Reserved = bt.ReserveSeats;
                //ViewBag.Booked = bt.BookedSeats; 
                return PartialView("MedicalList", list.ToList());
            }
            var obj = new List<AdmissionMedicalListVM>();
            return PartialView("MedicalList", obj.ToList());
        }
        #endregion

        #region ConfirmAdmissionList
        public ActionResult ConfirmAdmissions()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<AdmissionConfirmListVM>();
            return View(obj);
        }

        public ActionResult GetListConfirmedStudents(int? BatchId)
        {
            if (BatchId != null)
            {
                var list = from mt in db.Applied
                           join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on mt.BatchId equals b.BatchId
                           where (mt.BatchId == BatchId && mt.AdmissionStatus == true)
                           select new AdmissionConfirmListVM
                           {
                               ApplicationId = ap.ApplicationId,
                               ApplicationCode = ap.ApplicationCode,
                               Cell = ap.CellNo,
                               Email = ap.Email,
                               Name = ap.FirstName + " " + ap.LastName
                           };
                ViewBag.Flag = "1";
                
                //batchDetails
                //var bt = db.Batches.Find(BatchId);
                //ViewBag.TotalSeats = bt.TotalSeats;
                //ViewBag.Reserved = bt.ReserveSeats;
                //ViewBag.Booked = bt.BookedSeats; 
                return PartialView("ConfirmAdmissionsList", list.ToList());
            }
            var obj = new List<AdmissionConfirmListVM>();
            return PartialView("ConfirmAdmissionsList", obj.ToList());
        }
        #endregion

        #region RejectedList
        public ActionResult RejectedStudents()
        {
            return View();
        }

        #endregion
    }
}