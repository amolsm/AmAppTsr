using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
            var C = db.Batches
                .Where(c => c.CourseId == CourseId && c.IsActive == true && c.OnlineBookingStatus == true)
                .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            return Json(Batches, JsonRequestBehavior.AllowGet); 
        }

        public JsonResult FillCet(int? BatchId)
        {
            var C = db.CetMasters
             .Where(x => (x.BatchId == BatchId && x.IsActive==true))
             .Select(x => new { CetMasterId = x.CetMasterId, Name = x.CetDate });
            var Cet = C.ToList().Select(x => new CetDropdown { CetMasterId = x.CetMasterId, CetCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
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
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Courses = new SelectList(a.ToList(), "CourseId", "CourseName");
           
            var obj = new List<AddmissionCetListVM>();
            return View(obj.ToList());
        }

        [HttpGet]
        public ActionResult GetCetScheduleList(int? BatchId)
        {
            var vm = from cm in db.CetMasters
                     join c in db.Courses on cm.CourseId equals c.CourseId
                     join b in db.Batches on cm.BatchId equals b.BatchId
                     where (b.IsActive == true && b.OnlineBookingStatus == true && b.BatchId==BatchId)
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
            if (vm.ToList().Count == 0)
            {
                var obj = new List<AddmissionCetListVM>();
                return PartialView("_CetScheduleList", obj.ToList()); }
            else { return PartialView("_CetScheduleList", vm.ToList()); }


           
        }
        public ActionResult CetCreate()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            var count = db.CetMasters.Count() + 1;
            var obj = new AddmissionCetCreateVM {
                CetCode = count.ToString().PadLeft(4, '0')
                        };
            return PartialView("CetCreate",obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CetCreate(AddmissionCetCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                //var extension = Path.GetExtension(obj.FileUpload.FileName);
                //var path = Path.Combine(Server.MapPath("~/Uploads/CetFile/"));
                //if (!Directory.Exists(path))
                //    Directory.CreateDirectory(path);
                //obj.FileUpload.SaveAs(path + obj.CetCode + extension);
                CetMaster cm = new CetMaster
                {
                    BatchId = obj.BatchId,
                    CetCode = obj.CetCode,
                    CetDate = obj.CetDate,
                    CetTime = obj.CetTime,
                    CourseId = obj.CourseId,
                    IsActive = obj.IsActive,
                    //StartDate = obj.StartDate,
                    FilePath= null,
                    Venue = obj.Venue
                };

                db.CetMasters.Add(cm);
                await db.SaveChangesAsync();

                //var sm = (from ap in db.Applications
                //          join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                //          where (ap.BatchId == obj.BatchId)
                //          select new
                //          {
                //              BatchId = (int)obj.BatchId,
                //              ApplicationId = ap.ApplicationId,
                //              CetMasterId = cm.CetMasterId,
                //              SelectStatus = false,
                //              Marks1 = 0,
                //              Marks2 = 0,
                //              Marks3 = 0,
                //              Marks4 = 0,
                //              Total = 0
                //          }).AsEnumerable().Select(x => new CetMark
                //                {
                //                  BatchId = x.BatchId,
                //                  ApplicationId = x.ApplicationId,
                //                  CetMasterId = x.CetMasterId,
                //                  SelectStatus = x.SelectStatus,
                //                  Marks1 = x.Marks1,
                //                  Marks2 = x.Marks2,
                //                  Marks3 = 0,
                //                  Marks4 = 0,
                //                  Total = 0
                //          }).ToList();
                //db.CetMarks.AddRange(sm);
                //await db.SaveChangesAsync();
               
                return Json(new { success = true });
            }
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("CetCreate", obj);
        }

        public async Task<ActionResult> CetEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CetMaster fp = await db.CetMasters.FindAsync(id);
            if (fp == null)
            {
                return HttpNotFound();
            }
            AddmissionCetCreateVM vm = new AddmissionCetCreateVM
            {
                BatchId =fp.BatchId,
                CetCode = fp.CetCode,
                CetDates = Convert.ToDateTime(fp.CetDate).ToString("yyyy-MM-dd"),
                CetId = fp.CetMasterId,
                CetTime = fp.CetTime,
                CourseId = fp.CourseId,
                IsActive = fp.IsActive,
                Venue = fp.Venue
            };
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            var b = db.Batches.Where(x => x.CourseId == fp.CourseId)
                    .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = b.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });

            ViewBag.Batches = new SelectList(Batches, "BatchId", "BatchCode");
            return PartialView("CetEdit", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CetEdit(AddmissionCetCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                CetMaster cm = new CetMaster
                {
                    BatchId = obj.BatchId,
                    CetCode = obj.CetCode,
                    CetDate = Convert.ToDateTime(obj.CetDates),
                    CetTime = obj.CetTime,
                    CourseId = obj.CourseId,
                    IsActive = obj.IsActive,
                    CetMasterId = (int)obj.CetId,
                    Venue = obj.Venue
                };

                db.Entry(cm).State = EntityState.Modified;
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
            return PartialView("CetEdit", obj);
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
            //ViewBag.Course = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
            var obj = new List<HallTicketListVM>();
            return View(obj);
        }
        public ActionResult GetApplicantForHalltickets(int? BatchId)
        {
            

            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join cr in db.Courses on ap.CourseId equals cr.CourseId
                       join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId
                       join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId
                       join cm in db.CetMasters on cr.CourseId equals cm.CourseId
                       where (b.BatchId == BatchId)
                       select new HallTicketListVM
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = ap.FirstName + " " + ap.LastName,
                           BatchId=b.BatchId,
                          
                           //PaidAmount = opi.amount,
                           Email = ap.Email,
                           Cell = ap.CellNo,
                           Select = false
                       };
            var ci = db.CetMasters.FirstOrDefault(x => x.BatchId == BatchId);
            if (ci != null)
            {
                ViewBag.Flag = "1";
            }
            else
            { ViewBag.Flag = "0"; }
             var C = db.CetMasters
            .Where(x => (x.BatchId == BatchId && x.IsActive == true))
            .Select(x => new { CetMasterId = x.CetMasterId, Name = x.CetDate });
            var Cet = C.ToList().Select(x => new CetDropdown { CetMasterId = x.CetMasterId, CetCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            ViewBag.CetMaster = new SelectList(Cet.ToList(), "CetMasterId", "CetCode");
            return PartialView("_HallticketApplicant", list.ToList());
        }
        public ActionResult Export(int? id)
        {

            var list = (from ap in db.Applications.AsEnumerable()
                        join b in db.Batches on ap.BatchId equals b.BatchId
                        join cr in db.Courses on ap.CourseId equals cr.CourseId
                        join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId
                        join cm in db.CetMasters on cr.CourseId equals cm.CourseId
                        where (cm.CetMasterId == id)
                        select new HallTicketListVM
                        {
                            CetMasterId = cm.CetMasterId,
                            ApplicationCode = ap.ApplicationCode,
                            ApplicationId = ap.ApplicationId,
                            CourseName = cr.CourseName,
                            BatchName = b.BatchCode,
                            CetDate = cm.CetDate,
                            CetTime = cm.CetTime,
                            Name = ap.FirstName + " " + ap.MiddleName + " " + ap.LastName,
                            Fathername = ap.MiddleName,
                            Mothername = ap.MotherName
                        });


            using (MemoryStream stream = new System.IO.MemoryStream())
            {

                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 36, 72, 108, 180);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);

                pdfDoc.Open();
                pdfDoc.AddTitle("Hall Ticket");
                PdfContentByte cb = writer.DirectContent;


                int count = 0;
                foreach (var app in list)
                {
                   
                    //var filename = app.ApplicationCode + ".pdf";
                    //var output = new FileStream(Path.Combine("c:\\myPDF\\", filename), FileMode.Create);
                    //var writer1 = PdfWriter.GetInstance(pdfDoc, output);

                    count++;

                    cb.Stroke();
                    iTextSharp.text.Font f = FontFactory.GetFont("Verdana", 50, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                    BaseFont bf_verdanabold = f.GetCalculatedBaseFont(false);
                    iTextSharp.text.Font f1 = FontFactory.GetFont("Verdana", 50, BaseColor.BLACK);
                    BaseFont bf_verdana = f1.GetCalculatedBaseFont(false);
                    iTextSharp.text.Font f2 = FontFactory.GetFont("Arial", 50, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                    BaseFont bf_arialbold = f2.GetCalculatedBaseFont(false);
                    iTextSharp.text.Font f3 = FontFactory.GetFont("Arial", 50, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    BaseFont bf_arial3 = f3.GetCalculatedBaseFont(false);


                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 13);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "SIR MOHAMED YUSUF SEAMEN WELFARE FOUNDATION", 300f, 785f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 11.5f);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "TRAINING SHIP RAHAMAN", 310f, 760f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 10);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "ENTRANCE EXAM October 2017", 313f, 730f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "EXAM ADMIT CARD", 320f, 710f, 0);
                    cb.EndText();


                    cb.Rectangle(36f, 450f, 410f, 250f);//Main box
                    cb.Stroke();
                    //cb.BeginText();
                    //cb.SetFontAndSize(bf_verdana, 10f);
                    //cb.ShowTextAligned(Element.ALIGN_LEFT, "Course Applied", 38f, 630f, 0);

                    //cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Date of Exam", 38f, 650f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Day", 185f, 650f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Time", 380f, 650f, 0);
                    cb.EndText();

                    cb.SetLineWidth(1.0f);
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(36f, 640f);
                    cb.LineTo(447f, 640f);
                    cb.Stroke();
                    // first horigentle line
                    cb.SetLineWidth(1.0f);
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(36f, 670f);
                    cb.LineTo(447f, 670f);
                    cb.Stroke();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 8f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT,app.CourseName == null ? "Course Applied : " : "Course Applied : " + app.CourseName, 38f, 675f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).ToString("dd-MM-yyyy"), 38f, 620f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, /*app.CourseName == null ? "" : app.CourseName*/ "Saturday", 185f, 620f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetTime == null ? "" : Convert.ToString(app.CetTime), 380f, 620f, 0);
                    cb.EndText();

                    cb.SetLineWidth(1.0f);
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(36f, 610f);
                    cb.LineTo(447f, 610f);
                    cb.Stroke();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Name of Examination Centre", 38f, 585f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 7.5f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "T. S. Rahaman, Nhava Tal, Panvel, Dist Raigad", 185f, 595f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 7.5f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Maharashtra 410206", 185f, 585f, 0);
                    cb.EndText();

                    cb.SetLineWidth(1.0f);
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(36f, 570f);
                    cb.LineTo(447f, 570f);
                    cb.Stroke();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Name of Candidate", 38f, 550f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 8.5f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.Name == null ? "" : app.Name, 185f, 550f, 0);
                    cb.EndText();

                    cb.SetLineWidth(1.0f);
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(36f, 540f);
                    cb.LineTo(447f, 540f);
                    cb.Stroke();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Father's Name", 38f, 525f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 8.5f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.Fathername == null ? "" : app.Fathername, 185f, 525f, 0);
                    cb.EndText();

                    cb.SetLineWidth(1.0f);
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(36f, 510f);
                    cb.LineTo(447f, 510f);
                    cb.Stroke();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Mother's Name", 38f, 490f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 8.5f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.Mothername == null ? "" : app.Mothername, 185f, 490f, 0);
                    cb.EndText();

                    cb.Rectangle(460f, 640f, 106f, 60f);//Hallticket box
                    cb.Stroke();
                    cb.BeginText();
                    cb.SetFontAndSize(bf_arialbold, 9f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Hall Ticket No", 465f, 675f, 0);
                    cb.EndText();
                    cb.BeginText();
                    cb.SetFontAndSize(bf_arialbold, 9f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.ApplicationCode == null ? "" : app.ApplicationCode, 465f, 650f, 0);
                    cb.EndText();

                    cb.Rectangle(460f, 510f, 106f, 120f);//Picture box
                    cb.Stroke();


                    cb.Rectangle(36f, 370f, 160f, 70f);// candidate signature box
                    cb.Stroke();
                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Candidates's Signature", 38f, 380f, 0);
                    cb.EndText();


                    cb.Rectangle(210f, 370f, 180f, 70f);//Hall Invigilator Singnature box
                    cb.Stroke();
                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Hall Invigilator Singnature with date", 215f, 380f, 0);
                    cb.EndText();

                    cb.Rectangle(405f, 370f, 160f, 70f);//Principle's Signature box
                    cb.Stroke();
                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Principle's Signature", 410f, 380f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Admission to Pre-Sea Course is subject to qualifying in entrance exam October 2017 and the", 38f, 350f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "eligiility Criteria as Per DG Shipping norms", 38f, 335f, 0);
                    cb.EndText();

                    cb.SetLineWidth(1.0f);                // Vertical line 
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(180f, 670f);
                    cb.LineTo(180f, 450f);
                    cb.Stroke();

                    //cb.SetLineWidth(1.0f);                // Vertical line 
                    //cb.SetColorStroke(BaseColor.BLACK);
                    //cb.MoveTo(295f, 670f);
                    //cb.LineTo(295f, 610f);
                    //cb.Stroke();

                    cb.SetLineWidth(1.0f);                // Vertical line 
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(375f, 670f);
                    cb.LineTo(375f, 610f);
                    cb.Stroke();

                    string imageURL = Server.MapPath("~/Img/avatar.png"); // Image 460f, 510f, 106f, 120f
                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
                    png.ScaleToFit(100f, 105f);
                    png.SpacingBefore = 1f;
                    png.SpacingAfter = 1f;
                    png.Alignment = Element.ALIGN_LEFT;
                    png.SetAbsolutePosition(465f, 515f);
                    pdfDoc.Add(png);

                    string imageURL1 = Server.MapPath("~/Img/signature.PNG"); // Image 
                    iTextSharp.text.Image png1 = iTextSharp.text.Image.GetInstance(imageURL1);
                    png1.ScaleToFit(135f, 50f);
                    png1.SpacingBefore = 1f;
                    png1.SpacingAfter = 1f;
                    png1.Alignment = Element.ALIGN_LEFT;
                    png1.SetAbsolutePosition(410f, 390f);
                    pdfDoc.Add(png1);

                    pdfDoc.NewPage();
                }

                pdfDoc.Close();

            
              
                //write to file
              
                return File(stream.ToArray(), "application/pdf");
                
            }




        }
        [HttpPost]
        public async Task<ActionResult> HallTickets(List<HallTicketListVM> ht, int? CetMasterId)
        {
            //FileContentResult file= Export(98);
            if (ht != null && CetMasterId != null)
            {
                foreach (var item in ht)
                {
                    if (item.Select == true)
                    {
                        CetMark c = new CetMark
                        {
                            CetMasterId = Convert.ToInt32(CetMasterId),
                            BatchId = item.BatchId,
                            ApplicationId = Convert.ToInt32(item.ApplicationId),
                            Marks1 = 0,
                            Marks2 = 0,
                            Marks3 = 0,
                            Marks4 = 0,
                            Total = 0,
                            SelectStatus = item.Select


                        };
                        db.CetMarks.Add(c);
                    }

                }

                await db.SaveChangesAsync();
            }
            //var path = Path.Combine(Server.MapPath("~/Uploads/HallTickets/"));
            //if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);

            //FileStream files = new FileStream(path, FileMode.Create, FileAccess.Write);
            //file.WriteTo(files);
            //files.Close();
            //stream.Close();
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            //ViewBag.Course = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
            var obj = new List<HallTicketListVM>();
            return View(obj);
        }

        #endregion


        #region AppliedList

        public ActionResult ViewApplicants()
        {
            var vm = from ap in db.Applied
                     join a in db.Applications on ap.ApplicationId equals a.ApplicationId
                     join cc in db.CourseCategories on ap.CategoryId equals cc.CourseCategoryId
                     join c in db.Courses on ap.CourseId equals c.CourseId
                     join b in db.Batches on ap.BatchId equals b.BatchId
                     //where (ap.AdmissionStatus == true)
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
            var list = from ap in db.Applied
                       join a in db.Applications on ap.ApplicationId equals a.ApplicationId
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       //join opi in db.OnlinePaymentInfos on ap.ApplicationId equals opi.ApplicationId

                       where (b.BatchId == BatchId)
                       select new ApplicationApplicantsList
                       {
                           ApplicationCode = a.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = a.FirstName + " " + a.LastName,
                           //PaidAmount = opi.amount,
                           Email = a.Email,
                           Cell = a.CellNo
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

        #region Interview Schedule

        public ActionResult InterviewSchedule()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Courses = new SelectList(a.ToList(), "CourseId", "CourseName");
            var schedulelist = new List<AdmissionInterviewScheduleVM>();
            return View(schedulelist.ToList());
        }
        [HttpGet]
        public ActionResult GetInterviewScheduleList(int? BatchId)
        {
            var schedulelist = from i in db.InterviewMasters.Where(x => x.BatchId == BatchId)
                               join b in db.Batches on i.BatchId equals b.BatchId
                               join c in db.Courses on i.CourseId equals c.CourseId
                               select new AdmissionInterviewScheduleVM
                               {
                                   InterviewMasterId = i.InterviewMasterId,
                                   InterviewCode=i.InterviewCode,
                                   BatchId=i.BatchId,
                                   CourseId=i.CourseId,
                                   Batch = b.StartDate.ToString(),
                                   Course = c.CourseName,
                                   InterviewDate =i.InterviewDate,
                                   InterviewTime=i.InterviewTime,
                                   Venue=i.Venue

                               };

          return PartialView("_InterviewScheduleList",schedulelist.ToList());
        }
      

        public ActionResult InterviewScheduleCreate()
        {
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            var count = db.InterviewMasters.Count() + 1;
            AdmissionInterviewScheduleVM im = new AdmissionInterviewScheduleVM
            {

                InterviewCode = count.ToString().PadLeft(4, '0'),
             
            };
            return PartialView("InterviewScheduleCreate", im);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewScheduleCreate(AdmissionInterviewScheduleVM obj)
        {
            if (ModelState.IsValid)
            {
                InterviewMaster i = new InterviewMaster
                {
                    InterviewCode = obj.InterviewCode,
                    BatchId = obj.BatchId,
                    CourseId = obj.CourseId,
                    InterviewDate = obj.InterviewDate,
                    InterviewTime = obj.InterviewTime,
                    Venue = obj.Venue
                };
                db.InterviewMasters.Add(i);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("InterviewScheduleCreate", obj);
        }

        public async Task<ActionResult> InterviewScheduleEdit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterviewMaster obj = await db.InterviewMasters.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            AdmissionInterviewScheduleVM vm = new AdmissionInterviewScheduleVM
            {
                InterviewMasterId = obj.InterviewMasterId,
                InterviewCode = obj.InterviewCode,
                BatchId = obj.BatchId,
                CourseId = obj.CourseId,
                InterviewDates = Convert.ToDateTime(obj.InterviewDate).ToString("yyyy-MM-dd"),
                InterviewTime = obj.InterviewTime,
                Venue = obj.Venue
            };
            var b = db.Batches.Where(x => x.CourseId == obj.CourseId)
                   .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = b.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });

            ViewBag.Batches = new SelectList(Batches, "BatchId", "BatchCode");
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("InterviewScheduleEdit", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewScheduleEdit(AdmissionInterviewScheduleVM obj)
        {
            if (ModelState.IsValid)
            {
                InterviewMaster i = new InterviewMaster
                {
                    InterviewMasterId = obj.InterviewMasterId,
                    InterviewCode = obj.InterviewCode,
                    BatchId = obj.BatchId,
                    CourseId = obj.CourseId,
                    InterviewDate = Convert.ToDateTime(obj.InterviewDates),
                    InterviewTime = obj.InterviewTime,
                    Venue = obj.Venue
                };
                db.Entry(i).State = EntityState.Modified;
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
            return PartialView("InterviewScheduleEdit", obj);
        }



        #endregion

        #region Medical Schedule

        public ActionResult MedicalSchedule()
        {
            ViewBag.Courses = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
            var schedulelist = new List<AdmissionMedicalScheduleVM>();
            return View(schedulelist.ToList());
        }
        [HttpGet]
        public ActionResult GetMedicalScheduleList(int? BatchId)
        {
            var schedulelist = from i in db.MedicalMasters.Where(x => x.BatchId == BatchId)
                               join c in db.Courses on i.CourseId equals c.CourseId
                               join b in db.Batches on i.BatchId equals b.BatchId
                               select new AdmissionMedicalScheduleVM
                               {
                                   MedicalMasterId=i.MedicalMasterId,
                                   MedicalCode=i.MedicalCode,
                                   Batch=b.StartDate.ToString(),
                                   Course=c.CourseName,
                                   CourseId=i.CourseId,
                                   MedicalDate=i.MedicalDate,
                                   MedicalFees=i.MedicalFees


                               };

            return PartialView("_MedicalScheduleList", schedulelist.ToList());
        }

        public ActionResult MedicalScheduleCreate()
        {
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");
            ViewBag.Course = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
            var count = db.MedicalMasters.Count() + 1;
            AdmissionMedicalScheduleVM im = new AdmissionMedicalScheduleVM
            {

                MedicalCode = count.ToString().PadLeft(4, '0'),

            };
            return PartialView("MedicalScheduleCreate", im);
          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MedicalScheduleCreate(AdmissionMedicalScheduleVM obj)
        {
            if (ModelState.IsValid)
            {
                MedicalMaster m = new MedicalMaster
                {
                
                    MedicalCode = obj.MedicalCode,
                    BatchId = obj.BatchId,
                    CourseId = obj.CourseId,
                    MedicalDate = obj.MedicalDate,
                    MedicalFees = obj.MedicalFees

                };
                db.MedicalMasters.Add(m);
                await db.SaveChangesAsync();
                return Json(new { success = true });
               
            }

            return PartialView("MedicalScheduleCreate", obj);
        }

        public async Task<ActionResult> MedicalScheduleEdit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalMaster obj = await db.MedicalMasters.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            AdmissionMedicalScheduleVM vm = new AdmissionMedicalScheduleVM
            {
                MedicalMasterId=obj.MedicalMasterId,
                MedicalCode = obj.MedicalCode,
                BatchId = obj.BatchId,
                CourseId = obj.CourseId,
                MedicalDates = Convert.ToDateTime(obj.MedicalDate).ToString("yyyy-MM-dd"),
                MedicalFees = obj.MedicalFees

              
            };
            var b = db.Batches.Where(x => x.CourseId == obj.CourseId)
               .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = b.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });

            ViewBag.Batches = new SelectList(Batches, "BatchId", "BatchCode");
            ViewBag.Course = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
            return PartialView("MedicalScheduleEdit", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MedicalScheduleEdit(AdmissionMedicalScheduleVM obj)
        {
            if (ModelState.IsValid)
            {
                MedicalMaster m = new MedicalMaster
                {
                    MedicalMasterId = obj.MedicalMasterId,
                    MedicalCode = obj.MedicalCode,
                    BatchId = obj.BatchId,
                    CourseId = obj.CourseId,
                    MedicalDate = Convert.ToDateTime(obj.MedicalDates),
                    MedicalFees = obj.MedicalFees

                };
                db.Entry(m).State = EntityState.Modified;
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
            return PartialView("MedicalScheduleEdit", obj);
        }
        #endregion


       
    }

}