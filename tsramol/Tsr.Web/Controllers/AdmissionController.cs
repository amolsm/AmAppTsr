using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;
using Tsr.ToPdf;
using Tsr.Web.Common;

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
                .Where(c => c.CourseId == CourseId && c.IsActive == true )
                .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            return Json(Batches, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCet(int? BatchId)
        {
            var C = db.CetMasters
             .Where(x => (x.BatchId == BatchId && x.IsActive == true))
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
        [HttpGet]
        public ActionResult CETSchedule()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Courses = new SelectList(a.ToList(), "CourseId", "CourseName");

            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var count = db.CetMasters.Count() + 1;
            var obj = new AddmissionCetCreateVM
            {
                CetCode = count.ToString().PadLeft(4, '0'),
                _AddmissionCetListVM = new List<AddmissionCetListVM>()
            };


            var Batches = new List<BatchDropdown>();

            ViewBag.Batches = new SelectList(Batches, "BatchId", "BatchCode");
            return View(obj);
        }

        [HttpGet]
        public ActionResult GetCetScheduleList(int? BatchId)
        {
            var vm = from cm in db.CetMasters
                     join c in db.Courses on cm.CourseId equals c.CourseId
                     join b in db.Batches on cm.BatchId equals b.BatchId
                     where (b.IsActive == true && b.OnlineBookingStatus == true && b.BatchId == BatchId)
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
                return PartialView("_CetScheduleList", obj.ToList());
            }
            else { return PartialView("_CetScheduleList", vm.ToList()); }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CETSchedule(AddmissionCetCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                string filepathname = string.Empty;
                if (obj.CetId == 0 || obj.CetId == null)
                {
                    if (Request.Files.Count > 0)
                    {
                        var root = "/Uploads/CetFile/";

                        var files = Request.Files[0];
                        var ext = Path.GetExtension(files.FileName);
                        var fileName = obj.CetCode + ext;
                        var path = Server.MapPath(root + fileName);
                        files.SaveAs(path);
                        filepathname = root + fileName;

                    }
                    CetMaster cm = new CetMaster
                    {
                        BatchId = obj.BatchId,
                        CetCode = obj.CetCode,
                        CetDate = Convert.ToDateTime(obj.CetDates),
                        CetTime = obj.CetTime,
                        CourseId = obj.CourseId,
                        IsActive = obj.IsActive,
                        //StartDate = obj.StartDate,
                        FilePath = filepathname,
                        Venue = obj.Venue
                    };

                    db.CetMasters.Add(cm);
                    db.SaveChanges();
                }
                else
                {
                    if (Request.Files.Count > 0)
                    {
                        var root = "/Uploads/CetFile/";

                        var files = Request.Files[0];
                        var ext = Path.GetExtension(files.FileName);
                        var fileName = obj.CetCode + ext;
                        var path = Server.MapPath(root + fileName);
                        files.SaveAs(path);
                        filepathname = root + fileName;

                    }
                    CetMaster cm = new CetMaster
                    {
                        BatchId = obj.BatchId,
                        CetCode = obj.CetCode,
                        CetDate = Convert.ToDateTime(obj.CetDates),
                        CetTime = obj.CetTime,
                        CourseId = obj.CourseId,
                        IsActive = obj.IsActive,
                        CetMasterId = (int)obj.CetId,
                        FilePath = filepathname,
                        Venue = obj.Venue
                    };

                    db.Entry(cm).State = EntityState.Modified;
                    db.SaveChanges();

                }


            }

            return RedirectToAction("CETSchedule");

        }

        public ActionResult CetEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CetMaster fp = db.CetMasters.Find(id);
            if (fp == null)
            {
                return HttpNotFound();
            }
            AddmissionCetCreateVM vm = new AddmissionCetCreateVM
            {
                BatchId = fp.BatchId,
                CetCode = fp.CetCode,
                CetDates = Convert.ToDateTime(fp.CetDate).ToString("yyyy-MM-dd"),
                CetId = fp.CetMasterId,
                CetTime = fp.CetTime,
                CourseId = fp.CourseId,
                IsActive = fp.IsActive,
                Venue = fp.Venue,
                _AddmissionCetListVM = new List<AddmissionCetListVM>()


            };
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            ViewBag.Courses = new SelectList(a.ToList(), "CourseId", "CourseName");
            var b = db.Batches.Where(x => x.CourseId == fp.CourseId)
                    .Select(x => new { BatchId = x.BatchId, Name = x.StartDate });

            var Batches = b.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });

            ViewBag.Batches = new SelectList(Batches, "BatchId", "BatchCode");
            return View("CETSchedule", vm);
        }

        #endregion

        #region Halltickets
        public ActionResult HallTickets()
        {
           

            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
           
            var obj = new List<HallTicketListVM>();
            return View(obj);
        }
        public ActionResult GetApplicantForHalltickets(int? BatchId)
        {


            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join cr in db.Courses on ap.CourseId equals cr.CourseId
                       join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId
                       join ct in db.CetMarks on ap.ApplicationId equals ct.ApplicationId into cts
                       from ct in cts.DefaultIfEmpty()
                       where (b.BatchId == BatchId && appl.AdmissionStatus == false && ct==null)
                       select new HallTicketListVM
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = ap.FullName,
                           BatchId = b.BatchId,
                           //Dob = Convert.ToDateTime(ap.DateOfBirth).ToString("dd-mm-yyyy"),
                           Dob = ap.DateOfBirth.ToString(),
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
                        join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId
                        where (b.BatchId == id && appl.AdmissionStatus == false)
                        join cm in db.CetMasters on b.BatchId equals cm.BatchId
                        select new HallTicketListVM
                        {
                            CetMasterId = cm.CetMasterId,
                            ApplicationCode = ap.ApplicationCode,
                            ApplicationId = ap.ApplicationId,
                            CourseName = cr.ShortName,
                            BatchName = b.BatchCode,
                            CetDate = cm.CetDate,
                            CetTime = (TimeSpan)cm.CetTime,
                            Name =ap.FullName,
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
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "ENTRANCE EXAM", 313f, 730f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "EXAM ADMIT CARD", 320f, 710f, 0);
                    cb.EndText();


                    cb.Rectangle(36f, 450f, 410f, 250f);//Main box
                    cb.Stroke();


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
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CourseName == null ? "Course Applied : " : "Course Applied : " + app.CourseName, 38f, 675f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).ToString("dd-MM-yyyy"), 38f, 620f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).DayOfWeek.ToString(), 185f, 620f, 0);
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
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Principal's Signature", 410f, 380f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Admission to Pre-Sea Course is subject to qualifying in entrance exam and the", 38f, 350f, 0);
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



                    cb.SetLineWidth(1.0f);                // Vertical line 
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(375f, 670f);
                    cb.LineTo(375f, 610f);
                    cb.Stroke();
                    string imageURL = string.Empty;
                   
                    string[] photos = Directory.GetFiles(Server.MapPath("~/Uploads/CetPhoto/"));
                    for (int i = 0; i < photos.Length; i++)
                    {
                        if ((Path.GetFileNameWithoutExtension(photos[i])) ==app.ApplicationCode)
                            imageURL = photos[i];
                      
                    }
                    if (imageURL == string.Empty) { imageURL = Server.MapPath("~/Uploads/CetPhoto/nophoto.jpg"); }
                   
                    // Image 460f, 510f, 106f, 120f
                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
                    png.ScaleToFit(100f, 110f);
                    png.SpacingBefore = 1f;
                    png.SpacingAfter = 1f;
                    
                    png.Alignment = Element.ALIGN_LEFT;
                    png.SetAbsolutePosition(465f, 520f);
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
        public ActionResult ExportEx(int? id)
        {
            var gv = new GridView();
            var list = (from ap in db.Applications.AsEnumerable()
                        join b in db.Batches on ap.BatchId equals b.BatchId
                        join cr in db.Courses on ap.CourseId equals cr.CourseId
                        join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId
                        where (b.BatchId == id && appl.AdmissionStatus == false)
                        join cm in db.CetMasters on b.BatchId equals cm.BatchId
                        
                        select new HallTicketListEx
                        {
                            Email = ap.Email,
                            ApplicationCode = ap.ApplicationCode,
                            Cell = ap.CellNo,
                            CourseName = cr.ShortName,
                            Batch = Convert.ToDateTime(b.StartDate).ToString("dd-mm-yyyy"),
                            CetDate = cm.CetDate,
                            CetTime = (TimeSpan)cm.CetTime,
                            Name = ap.FullName

                        });


            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=excelList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("HallTicketsNonPay");




        }
        [HttpPost]
        public async Task<ActionResult> HallTickets(List<HallTicketListVM> ht, int? CetMasterId)
        {
         
            if (ht != null && CetMasterId != null)
            {
                foreach (var item in ht)
                {
                    if (item.Select == true)
                    {
                        bool isp = false;
                        var hallticketpath = SavePdf(Convert.ToInt32(item.ApplicationId), (int)CetMasterId);


                        try
                        {
                            
                            isp = await SendHalltcketEmail(Convert.ToInt32(item.ApplicationId), Convert.ToInt32(CetMasterId), hallticketpath);
                        }
                        catch (Exception) { }

                        if (db.CetMarks.Any(u => (u.CetMasterId == (int)CetMasterId) && (u.BatchId == item.BatchId) && (u.ApplicationId == item.ApplicationId)))
                        {
                            var CetMarkIds = db.CetMarks.Where(u => (u.CetMasterId == (int)CetMasterId) && (u.BatchId == item.BatchId) && (u.ApplicationId == item.ApplicationId)).Select(x=>x.CetMarkId).ToList();
                            foreach (var cetmarkid in CetMarkIds)
                            {
                                CetMark c = new CetMark
                                {
                                    CetMarkId = cetmarkid,
                                    CetMasterId = Convert.ToInt32(CetMasterId),
                                    BatchId = item.BatchId,
                                    ApplicationId = Convert.ToInt32(item.ApplicationId),
                                    Marks1 = 0,
                                    Marks2 = 0,
                                    Marks3 = 0,
                                    Marks4 = 0,
                                    Total = 0,
                                    SelectStatus = item.Select,
                                    Hallticketpath = hallticketpath,
                                    IsPublish = isp



                                };
                                db.Entry(c).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else {
                            

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
                                SelectStatus = item.Select,
                                Hallticketpath = hallticketpath,

                                IsPublish = isp



                            };
                            db.CetMarks.Add(c);
                        }

                    }

                }

                await db.SaveChangesAsync();
                return RedirectToAction("HallTickets");
            }
          
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
           
            var obj = new List<HallTicketListVM>();
            return View(obj);
        }
        public string SavePdf(int applicationid, int cetmasterid)
        {
            string filpath = string.Empty;
            var cm = db.CetMasters.Find(cetmasterid);

            var list = (from ap in db.Applications.AsEnumerable()
                        join b in db.Batches on ap.BatchId equals b.BatchId
                        join cr in db.Courses on ap.CourseId equals cr.CourseId
                        //join op in db.OnlinePaymentInfos on ap.ApplicationId equals op.ApplicationId
                        //join cm in db.CetMasters on cr.CourseId equals cm.CourseId
                        where (ap.ApplicationId == applicationid)
                        select new HallTicketListVM
                        {
                            CetMasterId = cm.CetMasterId,
                            ApplicationCode = ap.ApplicationCode,
                            ApplicationId = ap.ApplicationId,
                            CourseName = cr.ShortName,
                            BatchName = b.BatchCode,
                            CetDate = cm.CetDate,
                            CetTime = (TimeSpan) cm.CetTime,
                            Name = ap.FullName,
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
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "ENTRANCE EXAM ", 313f, 730f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "EXAM ADMIT CARD", 320f, 710f, 0);
                    cb.EndText();


                    cb.Rectangle(36f, 450f, 410f, 250f);//Main box
                    cb.Stroke();


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
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CourseName == null ? "Course Applied : " : "Course Applied : " + app.CourseName, 38f, 675f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).ToString("dd-MM-yyyy"), 38f, 620f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).DayOfWeek.ToString(), 185f, 620f, 0);
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
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Principal's Signature", 410f, 380f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Admission to Pre-Sea Course is subject to qualifying in entrance exam  and the", 38f, 350f, 0);
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



                    cb.SetLineWidth(1.0f);                // Vertical line 
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(375f, 670f);
                    cb.LineTo(375f, 610f);
                    cb.Stroke();
                    string imageURL = string.Empty;
                    string imageExtn = string.Empty;
                    
                    string[] photos = Directory.GetFiles(Server.MapPath("~/Uploads/CetPhoto/"));
                    for (int i = 0; i < photos.Length; i++)
                    {
                        if ((Path.GetFileNameWithoutExtension(photos[i])) == app.ApplicationCode)
                        {
                            imageURL = photos[i];
                            //if ((Path.GetExtension(photos[i])) == app.ApplicationCode)
                            imageExtn = Path.GetExtension(photos[i]);
                        }
                    }
                    if (imageURL == string.Empty || (imageExtn != ".png" && imageExtn != ".jpg" && imageExtn != ".jpeg" && imageExtn != ".PNG" && imageExtn != ".JPG" && imageExtn != ".JPEG"))
                    { imageURL = Server.MapPath("~/Uploads/CetPhoto/nophoto.jpg"); }

                    // Image 460f, 510f, 106f, 120f
                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
                    png.ScaleToFit(100f, 110f);
                    png.SpacingBefore = 1f;
                    png.SpacingAfter = 1f;

                    png.Alignment = Element.ALIGN_LEFT;
                    png.SetAbsolutePosition(465f, 520f);
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

                var appcode = db.Applications.Where(x => x.ApplicationId == applicationid).Select(m => m.ApplicationCode).FirstOrDefault();
                var root = "/Uploads/Halltickets/" + appcode + ".pdf";
                var path = HttpContext.Server.MapPath(root);
                if ((System.IO.File.Exists(path)))
                {
                   
                        System.IO.File.Delete(path);
                    
                }
                byte[] content = stream.ToArray();
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.Write(content, 0, (int)content.Length);
                    file.Close();
                    file.Dispose();
                }
                stream.Dispose();
                filpath = root;


            }
            return filpath;
        }
        #endregion

        #region Halltickets without payment
        public ActionResult HallTicketsNonPay()
        {


            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            var obj = new List<HallTicketListVM>();
            return View(obj);
        }
        public ActionResult GetApplicantForHallticketsNonPay(int? BatchId)
        {


            var list = (from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join cr in db.Courses on ap.CourseId equals cr.CourseId
                       join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId into aps
                       from appl in aps.DefaultIfEmpty()
                       join ct in db.CetMarks on ap.ApplicationId equals ct.ApplicationId into cts
                       from ct in cts.DefaultIfEmpty()
                       where (b.BatchId == BatchId && appl==null && ct == null )
                       select new HallTicketListVM
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchName = b.BatchCode,
                           Name = ap.FullName,
                           BatchId = b.BatchId,
                          // Dob = Convert.ToDateTime(ap.DateOfBirth).ToString("dd-mm-yyyy"),
                           Dob = ap.DateOfBirth.ToString(),
                           Email = ap.Email,
                           Cell = ap.CellNo,
                           Select = false
                       }).ToList();
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

        public ActionResult ExportNonPay(int? id)
        {

            var list = (from ap in db.Applications.AsEnumerable()
                        join b in db.Batches on ap.BatchId equals b.BatchId
                        join cr in db.Courses on ap.CourseId equals cr.CourseId
                        join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId into aps
                        from appl in aps.DefaultIfEmpty()
                        where (b.BatchId == id && appl == null)
                        join cm in db.CetMasters on b.BatchId equals cm.BatchId
                        select new HallTicketListVM
                        {
                            CetMasterId = cm.CetMasterId,
                            ApplicationCode = ap.ApplicationCode,
                            ApplicationId = ap.ApplicationId,
                            CourseName = cr.ShortName,
                            BatchName = b.BatchCode,
                            CetDate = cm.CetDate,
                            CetTime = (TimeSpan)cm.CetTime,
                            Name = ap.FullName,
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
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "ENTRANCE EXAM", 313f, 730f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdana, 10);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "EXAM ADMIT CARD", 320f, 710f, 0);
                    cb.EndText();


                    cb.Rectangle(36f, 450f, 410f, 250f);//Main box
                    cb.Stroke();


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
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CourseName == null ? "Course Applied : " : "Course Applied : " + app.CourseName, 38f, 675f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).ToString("dd-MM-yyyy"), 38f, 620f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_arial3, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, app.CetDate == null ? "" : Convert.ToDateTime(app.CetDate).DayOfWeek.ToString(), 185f, 620f, 0);
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
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Principal's Signature", 410f, 380f, 0);
                    cb.EndText();

                    cb.BeginText();
                    cb.SetFontAndSize(bf_verdanabold, 10f);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Admission to Pre-Sea Course is subject to qualifying in entrance exam and the", 38f, 350f, 0);
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



                    cb.SetLineWidth(1.0f);                // Vertical line 
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.MoveTo(375f, 670f);
                    cb.LineTo(375f, 610f);
                    cb.Stroke();
                    string imageURL = string.Empty;

                    string[] photos = Directory.GetFiles(Server.MapPath("~/Uploads/CetPhoto/"));
                    for (int i = 0; i < photos.Length; i++)
                    {
                        if ((Path.GetFileNameWithoutExtension(photos[i])) == app.ApplicationCode)
                            imageURL = photos[i];

                    }
                    if (imageURL == string.Empty) { imageURL = Server.MapPath("~/Uploads/CetPhoto/nophoto.jpg"); }

                    // Image 460f, 510f, 106f, 120f
                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
                    png.ScaleToFit(100f, 110f);
                    png.SpacingBefore = 1f;
                    png.SpacingAfter = 1f;

                    png.Alignment = Element.ALIGN_LEFT;
                    png.SetAbsolutePosition(465f, 520f);
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
        public ActionResult ExportNonPayEx(int? id)
        {
            var gv = new GridView();
            var list = (from ap in db.Applications.AsEnumerable()
                        join b in db.Batches on ap.BatchId equals b.BatchId
                        join cr in db.Courses on ap.CourseId equals cr.CourseId
                        join appl in db.Applied on ap.ApplicationId equals appl.ApplicationId into aps
                        from appl in aps.DefaultIfEmpty()
                        where (b.BatchId == id && appl == null)
                        join cm in db.CetMasters on b.BatchId equals cm.BatchId
                        select new HallTicketListEx
                        {
                            Email = ap.Email,
                            ApplicationCode = ap.ApplicationCode,
                            Cell = ap.CellNo,
                            CourseName = cr.ShortName,
                            Batch = Convert.ToDateTime(b.StartDate).ToString("dd-mm-yyyy"),
                            CetDate = cm.CetDate,
                            CetTime = (TimeSpan)cm.CetTime,
                            Name = ap.FullName
                            
                        });

            
            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=excelList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("HallTicketsNonPay");




        }
        [HttpPost]
        public async Task<ActionResult> HallTicketsNonPay(List<HallTicketListVM> ht, int? CetMasterId)
        {
            //List<HallTicketListEx> ex = new List<HallTicketListEx>();
            if (ht != null && CetMasterId != null)
            {
                foreach (var item in ht)
                {
                    if (item.Select == true)
                    {

                        var hallticketpath = SavePdf(Convert.ToInt32(item.ApplicationId), (int)CetMasterId);
                        bool isp = false;
                        //try
                        //{
                            isp = await SendHalltcketEmail(Convert.ToInt32(item.ApplicationId), Convert.ToInt32(CetMasterId), hallticketpath);
                        //}
                        //catch (Exception) { }
                        if (db.CetMarks.Any(u => (u.CetMasterId == (int)CetMasterId) && (u.BatchId == item.BatchId) && (u.ApplicationId == item.ApplicationId)))
                        {
                            var CetMarkIds = db.CetMarks.Where(u => (u.CetMasterId == (int)CetMasterId) && (u.BatchId == item.BatchId) && (u.ApplicationId == item.ApplicationId)).Select(x => x.CetMarkId).ToList();
                            foreach (var cetmarkid in CetMarkIds)
                            {
                                CetMark c = new CetMark
                                {
                                    CetMarkId = cetmarkid,
                                    CetMasterId = Convert.ToInt32(CetMasterId),
                                    BatchId = item.BatchId,
                                    ApplicationId = Convert.ToInt32(item.ApplicationId),
                                    Marks1 = 0,
                                    Marks2 = 0,
                                    Marks3 = 0,
                                    Marks4 = 0,
                                    Total = 0,
                                    SelectStatus = item.Select,
                                    Hallticketpath = hallticketpath,
                                    IsPublish = isp



                                };
                                db.Entry(c).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
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
                                SelectStatus = item.Select,
                                Hallticketpath = hallticketpath,
                                IsPublish = isp



                            };
                            db.CetMarks.Add(c);
                        }

                        
                    }

                }

                await db.SaveChangesAsync();
                
                return RedirectToAction("HallTicketsNonPay");
            }

            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

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
                         ApplicantName = a.FullName,
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
                           Name = a.FullName,
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
                       join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                       //into sml 
                       //from sm in sml.DefaultIfEmpty()
                       where (b.BatchId == BatchId && ct.CetMasterId == CetMasterId)
                       select new EntranceMarksListVM
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           //BatchName = b.BatchCode,
                           Name = ap.FullName,
                           Marks1 = sm.Marks1,
                           Marks2 = sm.Marks2,
                           Marks3 = sm.Marks3,
                           Marks4 = sm.Marks4
                       };


            return PartialView("EntranceMarksList", list.ToList());
        }

        public ActionResult EntranceMarksEx(int? BatchId, int? CetMasterId)
        {
            var gv = new GridView();
            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join ct in db.CetMasters on b.BatchId equals ct.BatchId
                       join sm in db.CetMarks on ap.ApplicationId equals sm.ApplicationId
                       join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                       //into sml 
                       //from sm in sml.DefaultIfEmpty()
                       where (b.BatchId == BatchId && ct.CetMasterId == CetMasterId)
                       select new EntranceMarksListVM
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           //BatchName = b.BatchCode,
                           Name = ap.FullName,
                           Marks1 = sm.Marks1,
                           Marks2 = sm.Marks2,
                           Marks3 = sm.Marks3,
                           Marks4 = sm.Marks4,
                           Total = (sm.Marks1 + sm.Marks2 + sm.Marks3 + sm.Marks4)
                       };


            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=excelList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("EntranceMarksList");




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
                    var cim = await db.InterviewMasters.FindAsync(InterviewMasterId);
                    CetInterview ci = new CetInterview
                    {
                        ApplicationId = item.ApplicationId,
                        BatchId = (int)ap.BatchId,
                        InterviewMasterId = (int)InterviewMasterId
                    };
                    db.CetInterviews.Add(ci);
                    int Id = (int)InterviewMasterId;

                    EmailModel em = new EmailModel
                    {
                        From = ConfigurationManager.AppSettings["admsmail"],
                        FromPass = ConfigurationManager.AppSettings["admsps"],
                        To = ap.Email,
                        Subject = "Interview for Pre Sea Course",
                        Body = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " you are selected for the Interview Date " + Convert.ToDateTime(cim.InterviewDate).ToString("dd-MM-yyyy") + " Time " + cim.InterviewTime + "  Thanking you T.S.Rahaman",
                        File1 = HttpContext.Server.MapPath(db.InterviewMasters.Where(x => x.InterviewMasterId == Id).Select(x => x.FilePath).FirstOrDefault())
                        
                };

                    var res = await MessageService.sendAttachmentEmail(em);

                    MessageService ms = new MessageService();
                    string msg = "Dear " + ap.FullName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " you are selected for the Interview Date " + Convert.ToDateTime(cim.InterviewDate).ToString("dd-MM-yyyy") + " Time " + cim.InterviewTime + "  Thanking you T.S.Rahaman";
                    string mobileno = ap.CellNo;
                    await ms.SendSmsAsync(msg, mobileno);
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
                               Name = ap.FullName,
                               Marks1 = sm.Marks1,
                               Marks2 = sm.Marks2,
                               Marks3 = sm.Marks3,
                               Marks4 = sm.Marks4,
                               Total = sm.Marks1 + sm.Marks2 + sm.Marks3 + sm.Marks4
                           };
                var c = (int)Count;
                var topList = list.Take(c);
                var ci = db.CetInterviews.FirstOrDefault(x => x.BatchId == BatchId);
                if (ci != null)
                    ViewBag.Flag = "1";
                else
                    ViewBag.Flag = "1";
                var ims = db.InterviewMasters.Where(x=>x.BatchId == BatchId)
                  .Select(x => new { InterviewMasterId = x.InterviewMasterId, Name = x.InterviewDate });

                var InterveiwMasters = ims.ToList().Select(x => new InterviewDropdown { InterviewMasterId = x.InterviewMasterId, InterviewCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
                ViewBag.InterveiwMasters = new SelectList(InterveiwMasters.ToList(), "InterviewMasterId", "InterviewCode");
                return PartialView("TopStudentsList", topList.ToList());
            }
            var obj = new List<EntranceMarksListVM>();
            return PartialView("TopStudentsList", obj.ToList());
        }
        public ActionResult TopStudentEx(int? BatchId, int? CetMasterId, int? Count)
        {
            var gv = new GridView();
            var list = from ap in db.Applications
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       join ct in db.CetMasters on b.BatchId equals ct.BatchId
                       join sm in db.CetMarks on ap.ApplicationId equals sm.ApplicationId
                       //into sml 
                       //from sm in sml.DefaultIfEmpty()
                       where (b.BatchId == BatchId && ct.CetMasterId == CetMasterId)
                       orderby sm.Total descending
                       select new TopStudentEx
                       {
                           ApplicationCode = ap.ApplicationCode,
                           Email = ap.Email,
                           Name = ap.FullName,
                           Marks1 = sm.Marks1,
                           Marks2 = sm.Marks2,
                           Marks3 = sm.Marks3,
                           Marks4 = sm.Marks4,
                           Total = sm.Marks1 + sm.Marks2 + sm.Marks3 + sm.Marks4
                       };

            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=excelList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("EntranceMarksList");




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
                               Name = ap.FullName
                           };
                ViewBag.Flag = "1";
                var ims = db.MedicalMasters.Where(x => x.BatchId == BatchId)
                .Select(x => new { MedicalMasterId = x.MedicalMasterId, Name = x.MedicalDate });

                var MedicalMasters = ims.ToList().Select(x => new MedicalDropdown { MedicalMasterId = x.MedicalMasterId, MedicalCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });

                ViewBag.MedicalMaster = new SelectList(MedicalMasters.ToList(), "MedicalMasterId", "MedicalCode");
                return PartialView("InterviewList", list.ToList());
            }
            var obj = new List<AdmissionInterviewListVM>();
            return PartialView("InterviewList", obj.ToList());
        }
        public ActionResult InterviewEx(int? BatchId)
        {
            var gv = new GridView();
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
                           Name = ap.FullName
                       };

            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=excelList.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("EntranceMarksList");




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
                        var ap = await db.Applications.FindAsync(item.ApplicationId);
                        var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == ap.CourseId);
                        decimal tax;
                        if (cf.GstPercentage > 0)
                            tax = (((decimal)cf.ActualFee / 100) * (decimal)cf.GstPercentage);
                        else
                            tax = 0;
                        var totalFee = (decimal)cf.ActualFee + tax;

                        Applied apl = db.Applied.FirstOrDefault(x => x.ApplicationId == item.ApplicationId);
                        apl.AdmissionStatus = true;

                        StudentFeeDetail sfd = new StudentFeeDetail
                        {
                            ApplicationId = item.ApplicationId,
                            BatchId = ap.BatchId,
                            FeePaid = 0,
                            TotalFee = totalFee,
                            FeeBal = totalFee
                        };

                        db.StudentFeeDetails.Add(sfd);
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
                               Name = ap.FullName
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

            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            var obj = new List<AdmissionConfirmListVM>();
            return View(obj);
        }
       
        public ActionResult ConfirmAdmissionsExportToExcel(int? id)
        {
            var gv = new GridView();

            var list = from mt in db.Applied.AsEnumerable()
                       join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                       join b in db.Batches on mt.BatchId equals b.BatchId
                       join c in db.Courses on mt.CourseId equals c.CourseId
                       join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                       join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                       where (mt.BatchId == id && mt.AdmissionStatus == true)
                       select new AdmissionConfirmListExcelVM
                       {
                           AnnualIncome = ap.AnnualIncome,
                           ApplicationCode = ap.ApplicationCode,
                           StartDate = b.StartDate,
                           Caste = ap.Caste,
                           CategoryName = cc.CategoryName,
                           CategoryOfCandidate = ap.CategoryOfCandidate,
                           CdcNo = ap.CdcNo,
                           CellNo = ap.CellNo,
                           CertOfCompetencyNo = ap.CertOfCompetencyNo,
                           Citizenship = ap.Citizenship,
                           CourseAttendedInTSR = ap.CourseAttendedInTSR,
                           CourseName = c.CourseName,
                           DateOfBirth = ap.DateOfBirth,
                           Email = ap.Email,
                           FatherEmail = ap.FatherEmail,
                           FatherFullName = ap.FatherFullName,
                           FatherOccupation = ap.FatherOccupation,
                           FirstName = ap.FirstName,
                           FPFF_AFF_1995 = ap.FPFF_AFF_1995,
                           FullName = ap.FullName,
                           Gender = ap.Gender,
                           GradAddress = ap.GradAddress,
                           GradCity = ap.GradCity,
                           GradCollegeName = ap.GradCollegeName,
                           GradeOfCompetencyNo = ap.GradeOfCompetencyNo,
                           GradPassAttempt = ap.GradPassAttempt,
                           GradPassingYear = ap.GradPassingYear,
                           GradPercentage = ap.GradPercentage,
                           GradPin = ap.GradPin,
                           GradState = ap.GradState,
                           GradSubjects = ap.GradSubjects,
                           GradUniversity = ap.GradUniversity,
                           GuardianAddress = ap.GuardianAddress,
                           GuardianCity = ap.GuardianCity,
                           GuardianContact = ap.GuardianContact,
                           GuardianEmail = ap.GuardianEmail,
                           GuardianName = ap.GuardianName,
                           GuardianPin = ap.GuardianPin,
                           GuardianRelation = ap.GuardianRelation,
                           GuardianState = ap.GuardianState,
                           Height = ap.Height,
                           IdentificationMark = ap.IdentificationMark,
                           InDosNo = ap.InDosNo,
                           InterAddress = ap.InterAddress,
                           InterBoard = ap.InterBoard,
                           InterChemistry = ap.InterChemistry,
                           InterCity = ap.InterCity,
                           InterEnglish = ap.InterEnglish,
                           InterMath = ap.InterMath,
                           InterPassingYear = ap.InterPassingYear,
                            InterPercentage = ap.InterPercentage,
                            InterPhysics = ap.InterPhysics,
                            InterPin = ap.InterPin,
                            InterRollNo = ap.InterRollNo,
                            InterSchoolName = ap.InterSchoolName,
                            InterState = ap.InterState,
                            LastName = ap.LastName,
                            MiddleName = ap.MiddleName,
                            MotherName = ap.MotherName,
                            PantSize = ap.PantSize,
                            PassportNo = ap.PassportNo,
                            PermenentAddress = ap.PermenentAddress,
                            PermenentCity = ap.PermenentCity,
                            PermenentContactNo = ap.PermenentContactNo,
                            PermenentPin = ap.PermenentPin,
                            PermenentState = ap.PermenentState,
                            PlaceOfBirth = ap.PlaceOfBirth,
                            PreferredMeal = ap.PreferredMeal,
                            PresentAddress = ap.PresentAddress,
                            PresentCity = ap.PresentCity,
                            PresentContactNo = ap.PresentContactNo,
                            PresentPin = ap.PresentPin,
                            PresentState = ap.PresentState,
                            RankOfCandidate = ap.RankOfCandidate,
                            Religion = ap.Religion,
                            SchoolAddress = ap.SchoolAddress,
                            SchoolBoard = ap.SchoolBoard,
                            SchoolCity = ap.SchoolCity,
                            SchoolEnglish = ap.SchoolEnglish,
                            SchoolMath = ap.SchoolMath,
                            SchoolName = ap.SchoolName,
                            SchoolPassingYear = ap.SchoolPassingYear,
                            SchoolPercentage = ap.SchoolPercentage,
                            SchoolPin = ap.SchoolPin,
                            SchoolScience = ap.SchoolScience,
                            SchoolState = ap.SchoolState,
                            ShippingCompany = ap.ShippingCompany,
                            ShirtSize = ap.ShirtSize,
                            ShoeSize = ap.ShoeSize,
                            Weight = ap.Weight
                       };

            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ConfirmAdmissions.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Index");
        }

        public ActionResult ConfirmAdmissionsDGExcel(int? id)
        {
            var gv = new GridView();

            var list = (from mt in db.Applied.AsEnumerable()
                       join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                       join b in db.Batches on mt.BatchId equals b.BatchId
                       join c in db.Courses on mt.CourseId equals c.CourseId
                       join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                       join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                       where (mt.BatchId == id && mt.AdmissionStatus == true)
                       select new 
                       {
                           CandidateName = ap.FullName,
                           CDC_No = ap.CdcNo,
                           DateOfBirth = ap.DateOfBirth.ToString(),
                           DateOfJoining = b.StartDate.ToString(),
                           INDoS = ap.InDosNo,
                           PassportNo = ap.PassportNo
                           
                       }).ToList();
            //var Batches = C.ToList().Select(x => new BatchDropdown { BatchId = x.BatchId, BatchCode = Convert.ToDateTime(x.Name).ToString("dd-MM-yyyy") });
            var list2 = list.Select(x => new AdmissionConfirmDGExcelFormat { CandidateName= x.CandidateName, CDC_No = x.CDC_No, DateOfBirth = Convert.ToDateTime(x.DateOfBirth).ToString("dd-MM-yyyy"), DateOfJoining = Convert.ToDateTime(x.DateOfJoining).ToString("dd-MM-yyyy"),INDoS = x.INDoS,PassportNo = x.PassportNo });
            gv.DataSource = list2.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ConfirmAdmissions.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Index");
        }
        public ActionResult GetListConfirmedStudents(int? BatchId)
        {
            if (BatchId != null)
            {
                var list = from mt in db.Applied.AsEnumerable()
                           join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on mt.BatchId equals b.BatchId
                           join c in db.Courses on mt.CourseId equals c.CourseId
                           join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                           where (mt.BatchId == BatchId && mt.AdmissionStatus == true)
                           select new AdmissionConfirmListVM
                           {
                               ApplicationId = ap.ApplicationId,
                               ApplicationCode = ap.ApplicationCode,
                               Course = c.CourseName,
                               Batch = Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy"),
                               Cell = ap.CellNo,
                               Email = ap.Email,
                               Name = ap.FullName,
                               FeePaid = sfd.FeePaid.ToString()
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

        public ActionResult ConformStudentListPdf(int? id)
        {
            if (id != null)
            {
                var list = from mt in db.Applied.AsEnumerable()
                           join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on mt.BatchId equals b.BatchId
                           join c in db.Courses on mt.CourseId equals c.CourseId
                           join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                           where (mt.BatchId == id && mt.AdmissionStatus == true)
                           select new AdmissionConfirmListVM
                           {
                               ApplicationId = ap.ApplicationId,
                               ApplicationCode = ap.ApplicationCode,
                               Course = c.CourseName,
                               Batch = Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy"),
                               Cell = ap.CellNo,
                               Email = ap.Email,
                               Name = ap.FullName,
                               FeePaid = sfd.FeePaid.ToString()
                           };
                return new PdfActionResult(list);

            }
            return RedirectToAction("ConfirmAdmissions");
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

            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            var count = db.InterviewMasters.Count() + 1;
            AdmissionInterviewScheduleVM im = new AdmissionInterviewScheduleVM
            {

                InterviewCode = count.ToString().PadLeft(4, '0'),
                _AdmissionInterviewScheduleVM= new List<AdmissionInterviewScheduleVM>()

        };
            
            var Batches = new List<BatchDropdown>();

            ViewBag.Batches = new SelectList(Batches, "BatchId", "BatchCode");
            return View(im);
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
                                   InterviewCode = i.InterviewCode,
                                   BatchId = i.BatchId,
                                   CourseId = i.CourseId,
                                   Batch = b.StartDate.ToString(),
                                   Course = c.CourseName,
                                   InterviewDate = i.InterviewDate,
                                   InterviewTime = i.InterviewTime,
                                   Venue = i.Venue

                               };

            return PartialView("_InterviewScheduleList", schedulelist.ToList());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InterviewSchedule(AdmissionInterviewScheduleVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.InterviewMasterId == 0)
                {
                    string filepathname = string.Empty;
                    if (Request.Files.Count > 0)
                    {
                        var root = "/Uploads/InterviewFile/";
                      
                        var files = Request.Files[0];
                        var ext = Path.GetExtension(files.FileName);
                        var fileName = obj.InterviewCode + ext;
                        var path = Server.MapPath(root + fileName);
                        files.SaveAs(path);
                        filepathname = root + fileName;

                    }

                    InterviewMaster i = new InterviewMaster
                    {
                        InterviewCode = obj.InterviewCode,
                        BatchId = obj.BatchId,
                        CourseId = obj.CourseId,
                        InterviewDate = Convert.ToDateTime(obj.InterviewDates),
                        InterviewTime = obj.InterviewTime,
                        Venue = obj.Venue,
                        FilePath = filepathname
                    };
                    db.InterviewMasters.Add(i);
                    db.SaveChanges();
                }
                else
                {
                    string filepathname = string.Empty;
                    if (Request.Files.Count > 0)
                    {
                        var root = "/Uploads/InterviewFile/";
                       
                        var files = Request.Files[0];
                        var ext = Path.GetExtension(files.FileName);
                        var fileName = obj.InterviewCode + ext;
                        var path = Server.MapPath(root + fileName);
                        files.SaveAs(path);
                        filepathname = root + fileName;

                    }
                    InterviewMaster i = new InterviewMaster
                    {
                        InterviewMasterId = obj.InterviewMasterId,
                        InterviewCode = obj.InterviewCode,
                        BatchId = obj.BatchId,
                        CourseId = obj.CourseId,
                        InterviewDate = Convert.ToDateTime(obj.InterviewDates),
                        InterviewTime = obj.InterviewTime,
                        Venue = obj.Venue,
                        FilePath = filepathname
                    };
                    db.Entry(i).State = EntityState.Modified;
                    db.SaveChanges();
                }
               
               
            }
            return RedirectToAction("InterviewSchedule");
        }

        public ActionResult InterviewScheduleEdit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterviewMaster obj = db.InterviewMasters.Find(id);
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
                Venue = obj.Venue,
                _AdmissionInterviewScheduleVM= new List<AdmissionInterviewScheduleVM>()
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
            ViewBag.Courses = new SelectList(a.ToList(), "CourseId", "CourseName");
            return View("InterviewSchedule", vm);
        }
      
      
      



        #endregion

        #region Medical Schedule

        public ActionResult MedicalSchedule()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Courses = new SelectList(a.ToList(), "CourseId", "CourseName");

            //ViewBag.Courses = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
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
                                   MedicalMasterId = i.MedicalMasterId,
                                   MedicalCode = i.MedicalCode,
                                   Batch = b.StartDate.ToString(),
                                   Course = c.CourseName,
                                   CourseId = i.CourseId,
                                   MedicalDate = i.MedicalDate,
                                   MedicalFees = i.MedicalFees


                               };

            return PartialView("_MedicalScheduleList", schedulelist.ToList());
        }

        public ActionResult MedicalScheduleCreate()
        {
            
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");

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
                MedicalMasterId = obj.MedicalMasterId,
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

        

        public async Task<bool> SendHalltcketEmail(int applicationid,int cetmasterid,string HallticketFile)
        {
            var ap = db.Applications.Where(x => x.ApplicationId == applicationid).FirstOrDefault();
            var cet=db.CetMasters.Where(x => x.CetMasterId == cetmasterid).FirstOrDefault();
            var File1path = HallticketFile==null?null:HttpContext.Server.MapPath(HallticketFile);
            var File2path = cet.FilePath==null?null:HttpContext.Server.MapPath(cet.FilePath);
            EmailModel em = new EmailModel
            {
                From = ConfigurationManager.AppSettings["admsmail"],
                FromPass = ConfigurationManager.AppSettings["admsps"],
                To = ap.Email,
                Subject = "Hallticket",
                Body = "Dear "+ ap.FullName + " student Hall Ticket For Pre Sea Entrance Exam",
                File1= File1path,
                File2= File2path
            };

            var res = await MessageService.sendAttachmentEmail(em);
            return res;
        }

        #region UniqueIdentifier
        public JsonResult IsCetScheduleExists(string CourseId,string BatchId)
        {
            int courseid= CourseId == null ? 0 : Convert.ToInt32(CourseId);
            int batchid = BatchId == null ? 0 : Convert.ToInt32(BatchId);
            var data = db.CetMasters.Where(x => (x.CourseId == courseid) && (x.BatchId == batchid)).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  

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
            var s = from ap in db.Applications
                           join apd in db.Applied on ap.ApplicationId equals apd.ApplicationId
                           into aps from apd in aps.DefaultIfEmpty()
                           where (ap.BatchId == BatchId && ap.Scrutinee != true)
                           select new { ApplicationId = ap.ApplicationId, Name = ap.FullName, check = (apd==null)? true:false };

            var Students = s.ToList().Where(x => x.check == true);
            return Json(Students, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ScrutineeSearch(int? ApplicationId)
        {
            var list = from ap in db.Applications
                       where (ap.ApplicationId == ApplicationId)
                       select new FeesApplicantList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           
                           Name = ap.FullName,
                           
                           Email = ap.Email,
                           Cell = ap.CellNo,
                           Flag = ap.Scrutinee
                       };

            List<FeesApplicantList> obj = new List<FeesApplicantList>();
            obj.Add(list.FirstOrDefault());
            return PartialView("ScrutineeList", obj);
        }
        public async Task<ActionResult> ScrutineeAction(int? id, string flag1 = null)
        {

            if (id == null || flag1 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application ap = await db.Applications.FindAsync(id);

            if (ap == null)
            {
                return HttpNotFound();
            }
            if (flag1 == "R")
                ap.Scrutinee = false;
            if (flag1 == "A")
                ap.Scrutinee = true;

            db.SaveChanges();
            //if(ap.IsPackage==false)
            return RedirectToAction("Scrutinee");
            //else
            //return RedirectToAction("PackageScrutinee");
        }

        #endregion

        #region PackageScrutinee
        public ActionResult PackageScrutinee()
        {
            ViewBag.Packages = new SelectList(db.packages.ToList(), "PackageId", "PackageName");
            return View();
        }
        public ActionResult FillStudentsForPackageScrutinee(int PackageId)
        {
            var s = from ap in db.Applications
                    join apd in db.Applied on ap.ApplicationId equals apd.ApplicationId
                    into aps
                    from apd in aps.DefaultIfEmpty()
                    where (ap.PackageId == PackageId && ap.Scrutinee != true)
                    select new { ApplicationId = ap.ApplicationId, Name = ap.FullName, check = (apd == null) ? true : false };

            var Students = s.ToList().Where(x => x.check == true);
            return Json(Students, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CompanyReservation
        public ActionResult CompanyReservation()
        {
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");

            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            //var obj = new List<ApplicationNonCetVM>();
            return View();
        }

        public ActionResult DownloadExcelForCompanyReserv(int id)
        {
            var gv = new GridView();
            var b = db.Batches.Find(id);
            var seats = b.TotalSeats;
            var cc = db.CourseCategories.Find(b.CategoryId);
            //if (cc.CetRequired == true)
            //{ //CET course
            //    List<CompReservNonCetVM> list = new List<CompReservNonCetVM>();
            //}
            //else
            //{//Non-Cet Course
            //    List<CompReservNonCetVM> list = new List<CompReservNonCetVM>();
            //}
            List<CompReservNonCetVM> list = new List<CompReservNonCetVM>();
            for (int i = 0; i < 1; i++)
            {
                CompReservNonCetVM ob = new CompReservNonCetVM
                {
                    BatchId = b.BatchId,
                    CategoryId = b.CategoryId,
                    FirstName = "",
                    MiddleName="",
                    LastName="",
                    FullName="",
                    Email="",
                    CellNo = "",
                    DateOfBirth = "YYYY-MM-DD",
                    PlaceOfBirth = "",
                    Citizenship = "",
                    Gender = "",
                    PreferredMeal="",
                    CdcNo="",
                    PassportNo="",
                    InDosNo="",
                    GradeOfCompetencyNo="",
                    CertOfCompetencyNo="",
                    ShippingCompany="",
                    //CourseAttendedInTSR=false,
                    PermenentAddress="",
                    PermenentCity="",
                    PermenentState="",
                    PermenentPin="",
                    PermenentContactNo=""

                };
                list.Add(ob);
            }

            gv.DataSource = list.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=CompanyReservation.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("Index");
        }

        [HttpPost]
        public JsonResult UploadExcel(HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {

                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Uploads/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Sheet1";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<CompReservNonCetVM>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            //User TU = new User();
                            //TU.Name = a.Name;
                            //TU.Address = a.Address;
                            //TU.ContactNo = a.ContactNo;
                            //db.Users.Add(TU);

                            //db.SaveChanges();
                            if (a.BatchId == 0 || a.BatchId == null)
                            {

                            }
                            else
                            {
                                var b = db.Batches.Find(a.BatchId);
                                var c = db.Courses.Find(b.CourseId);
                                var n = db.Applications.Count(x => x.BatchId == a.BatchId);
                                n = n + 1;
                                Application ap = new Application();
                                ap.ApplicationCode = c.CourseCode.ToString() + b.BatchCode.ToString() + n.ToString().PadLeft(4, '0');
                                ap.FirstName = a.FirstName;
                                ap.MiddleName = a.MiddleName;
                                ap.LastName = a.LastName;
                                ap.FullName = a.FullName;
                                ap.Email = a.Email;
                                ap.CellNo = a.CellNo;
                                ap.DateOfBirth = Convert.ToDateTime(a.DateOfBirth);
                                ap.PlaceOfBirth = a.PlaceOfBirth;
                                ap.Citizenship = a.Citizenship;
                                ap.Gender = a.Gender;
                                ap.PreferredMeal = a.PreferredMeal;
                                ap.CdcNo = a.CdcNo;
                                ap.PassportNo = a.PassportNo;
                                ap.InDosNo = a.InDosNo;
                                ap.GradeOfCompetencyNo = a.GradeOfCompetencyNo;
                                ap.CertOfCompetencyNo = a.CertOfCompetencyNo;
                                ap.ShippingCompany = a.ShippingCompany;
                                //ap.CourseAttendedInTSR = a.CourseAttendedInTSR;
                                ap.PermenentAddress = a.PermenentAddress;
                                ap.PermenentCity = a.PermenentCity;
                                ap.PermenentContactNo = a.PermenentContactNo;
                                ap.PermenentPin = a.PermenentPin;
                                ap.PermenentState = a.PermenentState;

                                ap.BatchId = a.BatchId;
                                ap.CategoryId = a.CategoryId;
                                
                                ap.CourseId = b.CourseId;

                                db.Applications.Add(ap);
                                b.BookedSeats = b.BookedSeats + 1;
                                db.SaveChanges();

                                //Applied
                                Applied nca = new Applied
                                {
                                    AdmissionStatus = true,
                                    ApplicationId = ap.ApplicationId,
                                    BatchId = b.BatchId,
                                    CategoryId =(int) b.CategoryId,
                                    CourseId = (int)b.CourseId
                                };
                                db.Applied.Add(nca);

                                var cf = db.CourseFees.FirstOrDefault(x => x.CourseId == b.CourseId);
                                decimal tax;
                                if (cf.GstPercentage > 0)
                                    tax = (((decimal)cf.ActualFee / 100) * (decimal)cf.GstPercentage);
                                else
                                    tax = 0;
                                var totalFee = (decimal)cf.ActualFee + tax;
                                //feeReceipt
                                //FeeReceipt fr = new FeeReceipt
                                //{
                                //    Amount = Convert.ToDecimal(totalFee),
                                //    ApplicationId = ap.ApplicationId,
                                //    PaymentMode = "Cash",
                                //    PrintStatus = false,
                                //    FeesType = "CourseFee"
                                //};
                                //db.FeeReceipts.Add(fr);
                                //student fee details
                                StudentFeeDetail sfd = new StudentFeeDetail
                                {
                                    ApplicationId = Convert.ToInt32(ap.ApplicationId),
                                    TotalFee = totalFee,
                                    FeePaid = 0,
                                    FeeBal =  Convert.ToDecimal(totalFee),
                                    BatchId = b.BatchId
                                };
                                db.StudentFeeDetails.Add(sfd);
                                db.SaveChanges();

                                
                                EmailModel em = new EmailModel
                                {
                                    From = ConfigurationManager.AppSettings["admsmail"],
                                    FromPass = ConfigurationManager.AppSettings["admsps"],
                                    To = ap.Email,
                                    Subject = "Course Registration with TSR",
                                    Body = "Dear " + ap.FirstName + " " + ap.LastName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your seat has been confirmed for " + c.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman"
                                };

                                var res =  MessageService.sendEmail(em);

                                MessageService ms = new MessageService();
                                string msg = "Dear " + ap.FirstName + " " + ap.LastName + ", with the reference to your Enrolment ID " + ap.ApplicationCode + " This is to confirm that your seat has been confirmed for " + c.CourseName + " starting BATCH on " + Convert.ToDateTime(b.StartDate).ToString("dd-MM-yyyy") + "  Thanking you T.S.Rahaman";
                                string mobileno = ap.CellNo;
                                 ms.SendSmsAsync(msg, mobileno);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                   
                     RedirectToAction("CompanyReservation");
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
    }
    //return Json("", JsonRequestBehavior.AllowGet);
}
        #endregion

        #region DiscontinueStudent
        public ActionResult DiscontinueStudent()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.Packages = new SelectList(db.packages.ToList(), "PackageId", "PackageName");
            return View();
        }
        public ActionResult FillStudentsForDiscontinue(int BatchId)
        {
            var s = from ap in db.Applications
                    join apd in db.Applied on ap.ApplicationId equals apd.ApplicationId
                    into aps
                    from apd in aps.DefaultIfEmpty()
                    where (ap.BatchId == BatchId && ap.Scrutinee != true)
                    select new { ApplicationId = ap.ApplicationId, Name = ap.FullName, check = (apd == null) ? true : false };

            var Students = s.ToList().Where(x => x.check == true);
            return Json(Students, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DiscontinueSearch(int? BatchId)
        {
            var list = from apl in db.Applied
                       join ap in db.Applications on apl.ApplicationId equals ap.ApplicationId

                       where (apl.BatchId == BatchId && apl.AdmissionStatus == true)

                       select new FeesApplicantList
                       {
                           ApplicationCode = ap.ApplicationCode,
                           ApplicationId = ap.ApplicationId,
                           BatchId = apl.BatchId,
                           Name = ap.FullName,

                           Email = ap.Email,
                           Cell = ap.CellNo,
                           Flag = ap.Scrutinee
                       };

            //List<FeesApplicantList> obj = new List<FeesApplicantList>();
            //obj.Add(list.FirstOrDefault());
            return PartialView("DiscontinueList", list);
        }

        public async Task<ActionResult> DiscontinueAction(int? id, int? bid = null)
        {

            if (id == null || bid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Application ap = await db.Applications.FindAsync(id);
            Applied apl = await db.Applied.FirstOrDefaultAsync(x => x.ApplicationId == id && x.BatchId == bid);
            Batch b = await db.Batches.FindAsync(bid);
            if (apl == null)
            {
                return HttpNotFound();
            }

            apl.AdmissionStatus = false;
            b.BookedSeats = b.BookedSeats - 1;

            db.SaveChanges();
            //if(ap.IsPackage==false)
            return RedirectToAction("DiscontinueStudent");
            //else
            //return RedirectToAction("PackageScrutinee");
        }
        #endregion

    }
}

