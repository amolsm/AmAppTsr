﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsr.Infra;
using Tsr.Core.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Tsr.ToPdf;
using Tsr.Web.Common;
using Aspose.Email.Clients.Exchange.WebService;
using Aspose.Email;
using Aspose.Email.Clients.Exchange;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Tsr.Web.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        AppContext db = new AppContext();

        #region trainedReport
        public ActionResult TrainedCandidates()
        {
            var obj = new List<ReportDatewiseTrainedVM>();
            return View(obj);
        }
        public ActionResult GetListTrainedCandidates(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate != null && EndDate != null)
            {
                int pcnt = 0, rcnt = 0;
                var list = (from mt in db.Applied.AsEnumerable()
                           join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on mt.BatchId equals b.BatchId
                           join c in db.Courses on mt.CourseId equals c.CourseId
                           //join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                           join cf in db.CourseFees on c.CourseId equals cf.CourseId
                           where (b.StartDate >= StartDate && b.StartDate <= EndDate && mt.AdmissionStatus == true)
                           orderby (c.CategoryId)
                           select new
                           {
                               CourseId = c.CourseId,
                               //CatetoryId = c.CategoryId,
                               CourseName = c.CourseName,
                               IsPackage = ap.IsPackage,
                               //PackageAmt = sfd.FeePaid
                               //PackageAmt = (Convert.ToBoolean(ap.IsPackage)) ? cf.PackageFee : 0,
                               //RegularAmt = (Convert.ToBoolean(!ap.IsPackage)) ? cf.ActualFee : 0,
                               PackageAmt = cf.PackageFee + ((decimal)cf.PackageFee / 100 * (decimal)cf.GstPercentage),
                               RegularAmt = cf.ActualFee + ((decimal)cf.ActualFee / 100 * (decimal)cf.GstPercentage),
                               tax = cf.GstPercentage,
                              pcnt = (Convert.ToBoolean(ap.IsPackage)) ? ++pcnt : pcnt,
                               rcnt = (Convert.ToBoolean(!ap.IsPackage)) ? ++rcnt : rcnt
                           }).ToList();


                var list2 = list.GroupBy(l => l.CourseId)
                            .Select(x => new ReportDatewiseTrainedVM
                            {
                                CourseName = x.First().CourseName,
                                IsPackage = x.First().IsPackage,
                                //PackageAmt = Convert.ToDecimal(x.Sum(p => p.PackageAmt)),
                                // RegularAmt = Convert.ToDecimal(x.Sum(p=>p.RegularAmt)),
                                PackageAmt = Convert.ToDecimal(x.First().PackageAmt), 
                                RegularAmt = Convert.ToDecimal(x.First().RegularAmt),
                                //tax = Convert.ToDecimal( x.First().tax),
                                pcnt = x.Count(p=>p.IsPackage == true),
                                rcnt = x.Count(p => p.IsPackage != true)
                            }
                            ).ToList();

                return PartialView("TrainedCandidatesList", list2.ToList());
            }
           
            var obj = new List<ReportDatewiseTrainedVM>();
            return PartialView("TrainedCandidatesList",obj);
        }
        public ActionResult ExportToExcelTrainedCandidates(DateTime? StartDate, DateTime? EndDate)
        {
            var gv = new GridView();

            int pcnt = 0, rcnt = 0;
            var list = from mt in db.Applied.AsEnumerable()
                       join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                       join b in db.Batches on mt.BatchId equals b.BatchId
                       join c in db.Courses on mt.CourseId equals c.CourseId
                       //join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                       join cf in db.CourseFees on c.CourseId equals cf.CourseId
                       where (b.StartDate >= StartDate && b.StartDate <= EndDate && mt.AdmissionStatus == true)
                       orderby (c.CategoryId)
                       select new
                       {
                           CourseId = c.CourseId,
                           //CatetoryId = c.CategoryId,
                           CourseName = c.CourseName,
                           IsPackage = ap.IsPackage,
                           
                           PackageAmt = cf.PackageFee + ((decimal)cf.PackageFee / 100 * (decimal)cf.GstPercentage),
                           RegularAmt = cf.ActualFee + ((decimal)cf.ActualFee / 100 * (decimal)cf.GstPercentage),
                           tax = cf.GstPercentage,
                           pcnt = (ap.IsPackage.ToString()=="true") ? ++pcnt : pcnt,
                           rcnt = (ap.IsPackage.ToString()!="true") ? ++rcnt : rcnt
                       };

            int cnt = 1;
            var list2 = list.GroupBy(l => l.CourseId)
                        .Select(x => new 
                        {
                            SrNo = cnt++,
                            CourseName = x.First().CourseName,
                           
                                PackageAmount = x.First().PackageAmt,
                            PackageCandidates = x.Count(p => p.IsPackage == true),
                            RegularAmount = x.First().RegularAmt,
                            RegularCandidates = x.Count(p => p.IsPackage != true),
                            TotalCandidates = x.Count(p => p.IsPackage == true) + x.Count(p => p.IsPackage != true),
                            TotalAmount = (x.First().PackageAmt * x.Count(p => p.IsPackage == true)) + (x.First().RegularAmt * x.Count(p => p.IsPackage != true))
                        }
                        ).ToList();

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
        public ActionResult GetListTrainedCandidates2(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate != null && EndDate != null)
            {
                int pcnt = 0, rcnt = 0;
                var list = from mt in db.Applied.AsEnumerable()
                           join ap in db.Applications on mt.ApplicationId equals ap.ApplicationId
                           join b in db.Batches on mt.BatchId equals b.BatchId
                           join c in db.Courses on mt.CourseId equals c.CourseId
                           join sfd in db.StudentFeeDetails on ap.ApplicationId equals sfd.ApplicationId
                           join cf in db.CourseFees on c.CourseId equals cf.CourseId
                           where (b.StartDate >= StartDate && b.EndDate <= EndDate && mt.AdmissionStatus == true)
                           orderby (c.CategoryId)
                           select new
                           {
                               CourseId = c.CourseId,
                               //CatetoryId = c.CategoryId,
                               CourseName = c.CourseName,
                               IsPackage = ap.IsPackage,
                               //PackageAmt = sfd.FeePaid
                               PackageAmt = (Convert.ToBoolean(ap.IsPackage)) ? cf.PackageFee : 0,
                               RegularAmt = (Convert.ToBoolean(!ap.IsPackage)) ? sfd.FeePaid : 0,
                               pcnt = (Convert.ToBoolean(ap.IsPackage)) ? ++pcnt : pcnt,
                               rcnt = (Convert.ToBoolean(!ap.IsPackage)) ? ++rcnt : rcnt
                           };


                var list2 = list.GroupBy(l => l.CourseId)
                            .Select(x => new ReportDatewiseTrainedVM
                            {
                                CourseName = x.First().CourseName,
                                IsPackage = x.First().IsPackage,
                                PackageAmt = Convert.ToDecimal(x.Sum(p => p.PackageAmt)),
                                RegularAmt = Convert.ToDecimal(x.Sum(p => p.RegularAmt)),
                                pcnt = x.Count(p => p.IsPackage == true),
                                rcnt = x.Count(p => p.IsPackage == false)
                            }
                            ).ToList();

                return PartialView("TrainedCandidatesList", list2.ToList());
            }

            var obj = new List<ReportDatewiseTrainedVM>();
            return PartialView("TrainedCandidatesList", obj);
        }
        #endregion
        public ActionResult test()
        {
            EmailModel em = new EmailModel
            {
                //From = ConfigurationManager.AppSettings["admsmail"],
                //FromPass = ConfigurationManager.AppSettings["admsps"],
                To = "amolmurkute@gmail.com",
                Subject = "Course Registration with TSR",
                Body = "test"
            };

            var res = MessageService.sendEmail2(em);
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ApplicationForm()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationForm(ReportApplicationVM vm)
        {

            if (ModelState.IsValid)
            {
                
                DateTime ? revdate= Convert.ToDateTime("2017-05-15");
                var applied = db.Applied.ToList();
                var batches = db.Batches.ToList();
                var courses = db.Courses.ToList();
                var list = from apl in applied.Where(x=>x.CategoryId == vm.CategoryId && x.CourseId == vm.CourseId && x.BatchId == vm.BatchId).Select(x=>x.ApplicationId).Distinct()
                           join ap in db.Applications on apl equals ap.ApplicationId
                           select new ReportApplicationVM

                            {
                              
                                
                               revno = "00",
                               revdate = revdate/*DateTime.Now*/,
                               ApplicationNo = "01"/*ap.ApplicationId.ToString()*/,
                               ApplicationId=ap.ApplicationId,
                               NameOfApplicant =ap.FullName,
                               Nationality=ap.Citizenship,
                               DateOfBirth=ap.DateOfBirth,
                               CDCNo=ap.CdcNo,
                               PassportNo=ap.PassportNo,
                               INDOSNo=ap.InDosNo,
                               EnrollNo=ap.ApplicationCode,
                               Address=ap.PermenentAddress,
                               EmailId=ap.Email,
                               CertificateofCompetency=ap.GradeOfCompetencyNo,
                               COCNo=ap.CertOfCompetencyNo,
                               ShippingCompany=ap.ShippingCompany,
                               CellNo=ap.CellNo
                               
                              



                               
                           };
                try
                {
                    using (MemoryStream stream = new System.IO.MemoryStream())
                    {

                        Document pdfDoc = new Document(PageSize.A4, 36, 72, 108, 180);
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        pdfDoc.AddTitle("Application-Registration Form");
                        PdfContentByte cb = writer.DirectContent;


                        int count = 0;
                        foreach (var app in list)
                        {

                            count++;


                            cb.RoundRectangle(20f, 180f, 550f, 650f, 10f);
                            cb.Stroke();
                            iTextSharp.text.Font f = FontFactory.GetFont("Arial", 50, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                            BaseFont bf_arial = f.GetCalculatedBaseFont(false);
                            iTextSharp.text.Font f1 = FontFactory.GetFont("Courier New", 50, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                            BaseFont bf_arial1 = f1.GetCalculatedBaseFont(false);
                            iTextSharp.text.Font f2 = FontFactory.GetFont("Calibri (Body)", 50, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            BaseFont bf_arial2 = f2.GetCalculatedBaseFont(false);
                            iTextSharp.text.Font f3 = FontFactory.GetFont("Arial", 50, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            BaseFont bf_arial3 = f3.GetCalculatedBaseFont(false);
                            iTextSharp.text.Font f4 = FontFactory.GetFont("Courier New", 50, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK);
                            BaseFont bf_arial4 = f4.GetCalculatedBaseFont(false);
                            iTextSharp.text.Font f5 = FontFactory.GetFont("Calibri (Body)", 50, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                            BaseFont bf_arial5 = f5.GetCalculatedBaseFont(false);
                            iTextSharp.text.Font f6 = FontFactory.GetFont("Courier New", 50, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                            BaseFont bf_arial6 = f6.GetCalculatedBaseFont(false);
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial, 10);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "REV. NO.", 35f, 800f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial, 10);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.revno, 100f, 800f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial1, 12.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "TRAINING SHIP RAHAMAN", 220f, 800f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial, 10);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "REV DATE", 35f, 785f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial, 10);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, Convert.ToDateTime(app.revdate).ToString("dd-MM-yyyy"), 100f, 785f, 0);
                            cb.EndText();

                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial1, 11);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "APPLICATION-REGISTRATION FORM", 200f, 785f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial, 9.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "TSR:ALL:APLC:" + app.ApplicationNo, 450f, 800f, 0);
                            cb.EndText();

                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial2, 11);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "SrNo", 110f, 760f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial2, 11);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Course Name", 150f, 760f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial2, 11);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Course Date", 380f, 760f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial2, 11);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Batch No:", 460f, 760f, 0);
                            cb.EndText();
                         
                            
                                var courselist = (from a in applied
                                                  join bs in batches on a.BatchId equals bs.BatchId
                                                  join cs in courses on a.CourseId equals cs.CourseId
                                                  where (a.ApplicationId == app.ApplicationId)
                                                  select new { cs.ShortName, bs.StartDate, bs.BatchCode }).ToList();
                            var y = 750f;
                            int i = 0;
                            foreach (var cdata in courselist)
                            {
                                i++;
                                y = y - 10;

                                cb.BeginText();
                                cb.SetFontAndSize(bf_arial3, 7.5f);
                                cb.ShowTextAligned(Element.ALIGN_LEFT, i.ToString(), 110f, y, 0);
                                cb.EndText();
                              
                                cb.BeginText();
                                cb.SetFontAndSize(bf_arial3, 7.5f);
                                cb.ShowTextAligned(Element.ALIGN_LEFT, cdata.ShortName == null ? "" : cdata.ShortName, 115f, y, 0);
                                cb.EndText();
                                cb.BeginText();
                                cb.SetFontAndSize(bf_arial3, 7.5f);
                                cb.ShowTextAligned(Element.ALIGN_LEFT, cdata.StartDate==null?"": Convert.ToDateTime(cdata.StartDate).ToString("dd-MM-yyyy"), 380f, y, 0);
                                cb.EndText();
                                cb.BeginText();
                                cb.SetFontAndSize(bf_arial3, 7.5f);
                                cb.ShowTextAligned(Element.ALIGN_LEFT, cdata.BatchCode == null ? "" : cdata.BatchCode, 460f, y, 0);
                                cb.EndText();
                            }
                           
                           
                          
                            string imageURL = Server.MapPath("~/Img/avatar.png");
                            iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
                            png.ScaleToFit(90f, 90f);
                            png.SpacingBefore = 1f;
                            png.SpacingAfter = 1f;
                            png.Alignment = Element.ALIGN_LEFT;
                            png.SetAbsolutePosition(35f, 650f);
                            pdfDoc.Add(png);
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(new Chunk("Personal Details", FontFactory.GetFont(FontFactory.COURIER_BOLD, 11.5f, iTextSharp.text.Font.UNDERLINE))), 230f, 640f, 0);

                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "NAME OF APPLICANT", 35f, 610f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.NameOfApplicant == null ? "" : app.NameOfApplicant, 170f, 610f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(170f, 605f);
                            cb.LineTo(550f, 605f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "NATIONALITY:", 35f, 585f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.Nationality == null ? "" : app.Nationality, 170f, 585f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(170f, 580f);
                            cb.LineTo(265f, 580f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "DATE OF BIRTH:", 275f, 585f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.DateOfBirth == null ? "" : Convert.ToDateTime(app.DateOfBirth).ToString("dd-MM-yyyy"), 370f, 585f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(370f, 580f);
                            cb.LineTo(460f, 580f);
                            cb.Stroke();
                            cb.Rectangle(470f, 500f, 95f, 95f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "C.D.C. NO.:", 35f, 560f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.CDCNo == null ? "" : app.CDCNo, 170f, 560f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(170f, 555f);
                            cb.LineTo(265f, 555f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "PASSPORT NO.:", 275f, 560f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.PassportNo == null ? "" : app.PassportNo, 370f, 560f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(370f, 555f);
                            cb.LineTo(460f, 555f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "INDoS No:", 35f, 535f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.INDOSNo == null ? "" : app.INDOSNo, 170f, 535f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(170f, 530f);
                            cb.LineTo(265f, 530f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "ENROLLMENT NO:", 275f, 535f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.EnrollNo == null ? "" : app.EnrollNo, 370f, 535f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(370f, 530f);
                            cb.LineTo(460f, 530f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "CERT. OF COMPETENCY:", 35f, 505f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.CertificateofCompetency == null ? "" : app.CertificateofCompetency, 170f, 505f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(170f, 500f);
                            cb.LineTo(265f, 500f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "C.O.C NO.:", 275f, 505f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.COCNo == null ? "" : app.COCNo, 370f, 505f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(370f, 500f);
                            cb.LineTo(460f, 500f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "SHIPPING COMPANY:", 35f, 480f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial5, 11.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.ShippingCompany == null ? "" : app.ShippingCompany, 170f, 480f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(170f, 475f);
                            cb.LineTo(550f, 475f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial1, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "DECLARATION :", 35f, 460f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "I HEREBY DECLARE THAT THE INFORMATION GIVEN ABOVE IS TRUE TO THE BEST OF MY KNOWLEDGE AND BELIEF AND NOTHING HAS BEEN", 35f, 445f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "CONCEALED. ANY VALUABLES BROUGHT TO THE TSR IS AT MY OWN RISK. THE TSR AND OR THE MANAGEMENT IS NOT LIABLE AND SHALL", 35f, 435f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "NOT BE RESPONSIBLE FOR ANY LOSS OR DAMAGE AND I AM SOLELY RESPONSIBLE FOR THE SAFEKEEPING OF ANY SUCH ITEMS.", 35f, 425f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial1, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "UNDERTAKING :", 35f, 410f, 0);
                            cb.EndText();

                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "  I ALSO AGREE THAT I AM PERSONALLY LIABLE FOR ALL COST AND CHARGES INCURRED IN THE ANY EVENT OF DAMAGING TSR PROPERTY. I", 35f, 395f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "HEREBY INDEMNIFY THE SMYSW FOUNDATION, ITS PRINCIPAL AND ITS OFFICERS FROM ANY CLAIM WHATSOEVER ARISING FROM PERSONAL", 35f, 385f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "INJURY, DEATH, SICKNESS OR ANY OTHER INJURY SUFFERED BY ME AS A RESULT OF MY UNDERGOING THE SAID COURSE. I CONSENT TO", 35f, 375f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "ANY EMERGENCY MEDICAL TREATMENT THAT MIGHT BE NECESSARY, AND TO PAY ALL CHARGES CONNECTED THEREWITH TO THE", 35f, 365f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "FOUNDATION AND IN THE EVENT OF MY SUSTAINING ANY INJURY OR ILLNESS DURING THE PERIOD OF MY TRAINING AT ‘T.S. RAHAMAN", 35f, 355f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "THAT MAY WARRANT HOSPITALISATION, I OR MY NEXT OF KIN / RELATIVE WHOSE NAME AND ADDRESS IS MENTIONED BELOW SHALL BEAR", 35f, 345f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "IN FULL, ALL THE EXPENSES INCURRED FOR MY HOSPITALISATION BEFORE MY DISCHARGE FROM THE HOSPITAL, AND ABSOLVE SMYSW ", 35f, 335f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "FOUNDATION FROM ANY FINANCIAL OR OTHER RESPONSIBILITY, WHATSOEVER FOR THE SAID TREATMENT.", 35f, 325f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial1, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "NAME, ADDRESS & TELEPHONE NO. NEXT OF KIN OR RELATIVE:", 35f, 310f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "NAME", 35f, 280f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.NameOfApplicant == null ? "" : app.NameOfApplicant, 100f, 280f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(100f, 275f);
                            cb.LineTo(300f, 275f);
                            cb.Stroke();
                            //
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "TEL.No.", 350f, 280f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.CellNo == null ? "" : app.CellNo, 400f, 280f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(400f, 275f);
                            cb.LineTo(550f, 275f);
                            cb.Stroke();
                            //
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "ADDRESS", 35f, 260f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.Address == null ? "" : app.Address, 100f, 260f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(100f, 255f);
                            cb.LineTo(550f, 255f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "E-MAIL ID:", 35f, 240f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, app.EmailId == null ? "" : app.EmailId, 100f, 240f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(100f, 235f);
                            cb.LineTo(550f, 235f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial1, 7.5f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "* I HAVE READ TERMS & CONDITIONS /CANCELATION POLICY AND COURSE REQUIREMENTS ON T.S.RAHAMAN WEBSITE", 35f, 225f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "SIGNATURE OF CANDIDATE:", 35f, 190f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 175f, 190f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(175f, 185f);
                            cb.LineTo(300f, 185f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "DATE:", 320f, 190f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 350f, 190f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(350f, 185f);
                            cb.LineTo(450f, 185f);
                            cb.Stroke();
                            cb.RoundRectangle(20f, 25f, 550f, 150f, 10f);
                            cb.Stroke();
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(new Chunk("FOR OFFICE USE ONLY", FontFactory.GetFont(FontFactory.COURIER_BOLD, 10f, iTextSharp.text.Font.UNDERLINE))), 230f, 160f, 0);
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "FEES RECEIVED : YES/NO RECEIPT NO:", 35f, 125f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 245f, 125f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(245f, 120f);
                            cb.LineTo(350f, 120f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "DATE:", 360f, 125f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 390f, 125f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(390f, 120f);
                            cb.LineTo(450f, 120f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "PAID AT:", 455f, 125f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 495f, 125f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(495f, 120f);
                            cb.LineTo(550f, 120f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "RESIDENTIAL / NON RESIDENTIAL :", 35f, 100f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 240f, 100f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(240f, 95f);
                            cb.LineTo(340f, 95f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "CERTIFICATE NO.:", 350f, 100f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 450f, 100f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(450f, 95f);
                            cb.LineTo(550f, 95f);
                            cb.Stroke();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "SIGNATURE OF BOOKING CLERK:", 35f, 75f, 0);
                            cb.EndText();
                            cb.BeginText();
                            cb.SetFontAndSize(bf_arial3, 10f);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 240f, 75f, 0);
                            cb.EndText();
                            cb.SetLineWidth(1.0f);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.MoveTo(240f, 70f);
                            cb.LineTo(350f, 70f);
                            cb.Stroke();


                            pdfDoc.NewPage();
                        }




                        pdfDoc.Close();
                        return File(stream.ToArray(), "application/pdf");
                    }
                }
                catch(Exception ex)
                {

                    string msg = ex.ToString();
                    ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
                }


            }
            return View(vm);
        }
        public ActionResult GetAdmissionApplicantList(int? id)
        {
            var list = from ap in db.Applied
                       join a in db.Applications on ap.ApplicationId equals a.ApplicationId
                       join b in db.Batches on ap.BatchId equals b.BatchId
                       //join opi in db.OnlinePaymentInfos on ap.ApplicationId equals opi.ApplicationId

                       where (b.BatchId == id)
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
            
            return new PdfActionResult(list);
        }

        public ActionResult GetApplicationApplicantList(int? id)
        {
           
                var list = from ap in db.Applications
                           join b in db.Batches on ap.BatchId equals b.BatchId
                           join opi in db.OnlinePaymentInfos on ap.ApplicationId equals opi.ApplicationId
                           into opis
                           from opi in opis.DefaultIfEmpty()
                               //join apl in db.Applied on ap.ApplicationId equals apl.ApplicationId
                           where (b.BatchId == id)
                           select new ApplicationApplicantsList
                           {
                               ApplicationCode = ap.ApplicationCode,
                               ApplicationId = ap.ApplicationId,
                               BatchName = b.BatchCode,
                               Name = ap.FullName,
                               PaymentStatus = (opi == null) ? "Pending" : "Success",
                               Email = ap.Email,
                               Cell = ap.CellNo
                           };
                return new PdfActionResult(list);
            }

        public async Task<ActionResult>  Test1()
        {
            MessageService ms = new MessageService();
            string msg = "Dear " + "Santosh" + ", with the reference to your Enrolment ID " + "TEST" + " you are selected for the Interview Date " + "  Thanking you T.S.Rahaman";
            string mobileno = "8369668508";
            await ms.SendSmsAsync(msg, mobileno);

           // string MailUser = "onlinebooking@tsrahaman.org";
           // string MailPass = "OB2017tsr";
           // string MailTo = "amolmurkute@gmail.com";

           //try
           // {
           //     IEWSClient client = EWSClient.GetEWSClient("https://outlook.office365.com/ews/exchange.asmx", "onlinebooking@tsrahaman.org", "OB2017tsr", "tsrahaman.org");

           //     //Microsoft.Exchange.WebServices.Data.ExchangeService service = new Microsoft.Exchange.WebServices.Data.ExchangeService(Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2010_SP1);
                //service.Credentials = new System.Net.NetworkCredential(MailUser, MailPass, "tsrahaman.org");
                //service.AutodiscoverUrl(MailUser, RedirectionUrlValidationCallback);
                //Microsoft.Exchange.WebServices.Data.EmailMessage emailMessage = new Microsoft.Exchange.WebServices.Data.EmailMessage(service);
                //emailMessage.Subject = "test";
                //emailMessage.Body = new Microsoft.Exchange.WebServices.Data.MessageBody("test");
                //emailMessage.ToRecipients.Add(MailTo);
                //emailMessage.Save();
                //emailMessage.SendAndSaveCopy();
                //Create instance of type MailMessage
               //MailMessage msg = new MailMessage();
               // msg.From = "onlinebooking@tsrahaman.org";
               // msg.To = "skygroup1402@gmail.com";
               // msg.Subject = "Sending message from exchange server";
               // msg.HtmlBody = "<h3>sending message from exchange server</h3>";

               // // Send the message
               // client.Send(msg);
                //IEWSClient client = EWSClient.GetEWSClient("https://outlook.office365.com/ews/exchange.asmx", "onlinebooking@tsrahaman.org", "OB2017tsr", "tsrahaman.org");

                // Call ListMessages method to list messages info from Inbox
                //ExchangeMessageInfoCollection msgCollection = client.ListMessages(client.MailboxInfo.InboxUri);

                //string result=string.Empty;
                //// Loop through the collection to display the basic information
                //foreach (ExchangeMessageInfo msgInfo in msgCollection)
                //{

                //    string ms = "Subject: " + msgInfo.Subject;
                //    ms+="From: " + msgInfo.From.ToString();
                //    ms += "To: " + msgInfo.To.ToString();
                //    ms += "Message ID: " + msgInfo.MessageId;
                //    ms += "Unique URI: " + msgInfo.UniqueUri;
                //    result+= ms;
                //}

                ViewData["Message"] = "Msg Send";
            //}
            //catch (Exception ex)
            //{
            //    ViewData["Message"] = ex.ToString();
            //}
           
            return View();
        }
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }


        public ActionResult PaymentDetails()
        {
            ReportPaymentVM pr = new ReportPaymentVM();
            ViewBag.PaymentMode = DropdownData.PaymentModeList();
            return View(pr);
        }

     

        [HttpPost]
        [MulitipleButton(Name = "action", Argument = "PayInfoReport")]
        public ActionResult PayInfoReport(DateTime? StartDate, DateTime? EndDate,string PaymentMode)
        {
            if(ModelState.IsValid)
            { 
           IQueryable<ReportPaymentVM> q = from fr in db.FeeReceipts
                       join ap in db.Applications on fr.ApplicationId equals ap.ApplicationId
                       join c in db.Courses on ap.CourseId equals c.CourseId into pi1
                       from c in pi1.DefaultIfEmpty()
                       join b in db.Batches on ap.BatchId equals b.BatchId into pi2
                       from b in pi2.DefaultIfEmpty()
                       join p in db.packages on ap.PackageId equals p.PackageId into pi3
                       from p in pi3.DefaultIfEmpty()
                       where (fr.ReceiptDate >= StartDate && fr.ReceiptDate <= EndDate)
                       select new ReportPaymentVM
                       {
                           ApplicantName = ap.FullName,
                           CourseOrPackageName = (c == null) ? p.PackageName : c.CourseName,
                           BatchCode = (b == null) ? String.Empty : b.BatchCode,
                           BatchStartDate = (b == null) ? null : b.StartDate,
                           BatchEndDate = (b == null) ? null : b.EndDate,
                           PaymentAmount = fr.Amount,
                           PaymentMode = fr.PaymentMode

                       };
                if (PaymentMode != "All")
                {
                    q = q.Where(p => p.PaymentMode == PaymentMode);
                }
                var data = q.ToList(); 
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            return new PdfActionResult(data);
           }
             else
            {
                ReportPaymentVM pr = new ReportPaymentVM();
                ViewBag.PaymentMode = DropdownData.PaymentModeList();
                return View("PaymentDetails",pr);
            }
        }

        [HttpPost]
        [MulitipleButton(Name = "action", Argument = "PrintReceipts")]
        public ActionResult PrintReceipts(DateTime? StartDate, DateTime? EndDate, string PaymentMode)
        {
            if (ModelState.IsValid)
            {
                var data = (from fr in db.FeeReceipts.AsEnumerable()
                                                          join ap in db.Applications on fr.ApplicationId equals ap.ApplicationId
                                                          join sd in db.StudentFeeDetails on ap.ApplicationId equals sd.ApplicationId into sdf
                                                          from sd in sdf.DefaultIfEmpty()
                                                          join c in db.Courses on ap.CourseId equals c.CourseId into pi1
                                                          from c in pi1.DefaultIfEmpty()
                                                          join b in db.Batches on ap.BatchId equals b.BatchId into pi2
                                                          from b in pi2.DefaultIfEmpty()
                                                          join p in db.packages on ap.PackageId equals p.PackageId into pi3
                                                          from p in pi3.DefaultIfEmpty()
                                                          join gst in db.CourseFees on ap.CourseId equals gst.CourseId into pi4
                                                          from gst in pi4.DefaultIfEmpty()
                                                          where (fr.ReceiptDate >= StartDate && fr.ReceiptDate <= EndDate)
                                                          select new FeesViewPaymentDetailsVM
                                                          {
                                                              //FeeReceiptNo = Common.FeeReceiptNumber.GetReceiptNo(fr.FeeReceiptId, fr.PaymentMode),//fr.FeeReceiptNo,
                                                              FeeReceiptNo = fr.FeeReceiptNo,
                                                              PaymentMode = fr.PaymentMode,
                                                              FeesType = fr.FeesType,
                                                              ReceiptDate = fr.ReceiptDate,
                                                              Name = ap.FullName,
                                                              Course = (c == null) ? p.PackageName + " (Package)" : c.ShortName,
                                                              Batch = (b == null) ? String.Empty : b.BatchCode,
                                                              StudentId = ap.ApplicationId,
                                                              ApplicationId = ap.ApplicationId,
                                                              ApplicationCode = ap.ApplicationCode,
                                                              FeePaid = fr.Amount,
                                                              FeeBal = (sd == null) ? 0 : sd.FeeBal,
                                                              AmountInRs = Common.AmountInWords.ConvertNumbertoWords(Convert.ToInt64(fr.Amount)),
                                                              BatchStartDate = (b == null) ? null : b.StartDate,
                                                              PaymentDate = fr.ReceiptDate,
                                                              FeeReceiptId = fr.FeeReceiptId,
                                                              GST = (gst == null) ? 0 : Convert.ToDecimal(gst.GstPercentage)


                                                          });
                if (PaymentMode != "All")
                {
                    data = data.Where(p => p.PaymentMode == PaymentMode);
                }
               
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
                return new PdfActionResult(data);
            }
            else
            {
                ReportPaymentVM pr = new ReportPaymentVM();
                ViewBag.PaymentMode = DropdownData.PaymentModeList();
                return View("PaymentDetails",pr);
            }
        }
        }

   

    

}
