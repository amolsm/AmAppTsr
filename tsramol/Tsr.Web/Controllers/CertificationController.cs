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
using Tsr.ToPdf;
using Tsr.Web.Common;

using System.Web.UI;

namespace Tsr.Web.Controllers
{
    public class CertificationController : Controller
    {
        AppContext db = new AppContext();
        #region CheckList
        public ActionResult CheckList()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            var obj = new List<CheckListVM>();
            return View(obj);
        }

        [HttpGet]
        public ActionResult GetCheckList(int? BatchId)
        {
            var list = from apd in db.Applied
                       join ap in db.Applications on apd.ApplicationId equals ap.ApplicationId
                       where (apd.BatchId == BatchId && apd.AdmissionStatus == true)
                        select new CheckListVM
                        {
                            //Course = cr.CourseName,
                           // Batchfrom = b.StartDate,
                            //Batchto = b.EndDate,
                            StudentId = ap.ApplicationCode,
                            Name = ap.FullName,
                            DOB = ap.DateOfBirth,
                            Cdcno = ap.CdcNo,
                            PassportNo = ap.PassportNo,
                            Rank = ap.RankOfCandidate,
                            Grade = ap.GradeOfCompetencyNo,
                            IndosNo = ap.InDosNo


                        };
            return PartialView("_ApplicantsList", list.ToList());
        }

        public ActionResult Export(int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                int BatchIds = 0;
                BatchIds = id;
                string coursename = string.Empty;
                string StartDate = string.Empty;
                string EndDate = string.Empty;
                string BatchCode = string.Empty;
                var batch = from b in db.Batches
                            join c in db.Courses on b.CourseId equals c.CourseId
                            where b.BatchId == id
                            select new { c.CourseName, b.StartDate, b.EndDate, b.BatchCode };
                var list = (from apd in db.Applied
                            join ap in db.Applications on apd.ApplicationId equals ap.ApplicationId
                            join cr in db.Courses on apd.CourseId equals cr.CourseId
                            join b in db.Batches on apd.BatchId equals b.BatchId
                            where (apd.BatchId == id && apd.AdmissionStatus == true)
                            select new CheckListVM
                            {
                                Course = cr.CourseName,
                                Batchfrom = b.StartDate,
                                Batchto = b.EndDate,
                                StudentId = ap.ApplicationCode,
                                Name = ap.FullName,
                                DOB = ap.DateOfBirth,
                                Cdcno = ap.CdcNo,
                                PassportNo = ap.PassportNo,
                                Rank = ap.RankOfCandidate,
                                Grade = ap.GradeOfCompetencyNo,
                                IndosNo = ap.InDosNo
                            });
                foreach (var s in batch)
                {
                    BatchCode = s.BatchCode;
                    coursename = s.CourseName;
                    StartDate = Convert.ToDateTime(s.StartDate).ToString("dd-MM-yyyy");
                    EndDate = Convert.ToDateTime(s.EndDate).ToString("dd-MM-yyyy");

                }
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.AddTitle("Check List");

                PdfPTable table = new PdfPTable(16);
                iTextSharp.text.Font f = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                FontFactory.Register("C:\\Windows\\Fonts\\OLDENGL.ttf", "OldEnglishTextMT");
                iTextSharp.text.Font O = FontFactory.GetFont("OldEnglishTextMT", 25, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                table.TotalWidth = 775f;
                table.LockedWidth = true;
                Paragraph comb = new Paragraph();
                comb.Add(new Chunk("SIR MOHAMMAD YUSUF SEAMEN WELFARE FOUNDATION\n Tel: 022-27212800/900   Fax: 02227212495\n\n ", f));
                comb.Add(new Chunk("Training Ship Rahaman\n", O));
                comb.Add(new Chunk("\n CHECK LIST for course :  " + (coursename == null ? "" : coursename) + "   Batch from  " + (StartDate == null ? "" : StartDate.ToString()) + "  to  " + (EndDate == null ? "" : EndDate.ToString()) + "  (" + BatchCode + ")", f));

                PdfPCell header = new PdfPCell(new Phrase(comb));
                header.Colspan = 16;
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                header.UseVariableBorders = true;
                header.BackgroundColor = BaseColor.WHITE;
                table.AddCell(header);
                Chunk main = new Chunk("Check List", f);
                PdfPCell mainheading = new PdfPCell(new Phrase(main));
                mainheading.BackgroundColor = BaseColor.WHITE;
                mainheading.HorizontalAlignment = Element.ALIGN_CENTER;
                mainheading.Colspan = 16;
                //table.AddCell(mainheading);

                Chunk cthead1 = new Chunk("SrNo", f);
                PdfPCell thead1 = new PdfPCell(new Phrase(cthead1));
                thead1.Border = Rectangle.LEFT_BORDER;
                thead1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead1);
                Chunk cthead2 = new Chunk("Student Id", f);
                PdfPCell thead2 = new PdfPCell(new Phrase(cthead2));
                thead2.Border = Rectangle.NO_BORDER;
                thead2.BackgroundColor = BaseColor.WHITE;
                thead2.Colspan = 2;
                table.AddCell(thead2);
                Chunk cthead3 = new Chunk("Name", f);
                PdfPCell thead3 = new PdfPCell(new Phrase(cthead3));
                thead3.Border = Rectangle.NO_BORDER;
                thead3.Colspan = 2;
                thead3.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead3);
                Chunk cthead4 = new Chunk("DOB", f);
                PdfPCell thead4 = new PdfPCell(new Phrase(cthead4));
                thead4.Border = Rectangle.NO_BORDER;
                thead4.BackgroundColor = BaseColor.WHITE;
                thead4.Colspan = 2;
                table.AddCell(thead4);
                Chunk cthead5 = new Chunk("Cdcno", f);
                PdfPCell thead5 = new PdfPCell(new Phrase(cthead5));
                thead5.Border = Rectangle.NO_BORDER;
                thead5.BackgroundColor = BaseColor.WHITE;
                thead5.Colspan = 2;
                table.AddCell(thead5);
                Chunk cthead6 = new Chunk("Passport No", f);
                PdfPCell thead6 = new PdfPCell(new Phrase(cthead6));
                thead6.Border = Rectangle.NO_BORDER;
                thead6.Colspan = 2;
                thead6.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead6);
                Chunk cthead7 = new Chunk("Rank", f);
                PdfPCell thead7 = new PdfPCell(new Phrase(cthead7));
                thead7.Border = Rectangle.NO_BORDER;
                thead7.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead7);
                Chunk cthead8 = new Chunk("Grade", f);
                PdfPCell thead8 = new PdfPCell(new Phrase(cthead8));
                thead8.Border = Rectangle.NO_BORDER;
                thead8.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead8);
                Chunk cthead9 = new Chunk("Indos No", f);
                PdfPCell thead9 = new PdfPCell(new Phrase(cthead9));
                thead9.Border = Rectangle.NO_BORDER;
                thead9.BackgroundColor = BaseColor.WHITE;
                thead9.Colspan = 2;
                table.AddCell(thead9);
                Chunk cthead10 = new Chunk("Sign", f);
                PdfPCell thead10 = new PdfPCell(new Phrase(cthead10));
                thead10.Border = Rectangle.RIGHT_BORDER;
                thead10.BackgroundColor = BaseColor.WHITE;
                table.AddCell(thead10);


                //actual width of table in points

                //fix the absolute width of the table
                table.LockedWidth = true;

                //relative col widths in proportions - 1/3 and 2/3
                float[] widths = new float[] { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f };
                table.SetWidths(widths);
                table.HorizontalAlignment = 0;


                //leave a gap before and after the table
                table.SpacingBefore = 20f;
                table.SpacingAfter = 20f;


                int i = 0;
                foreach (var s in list)
                {
                    i++;
                    var dob = Convert.ToDateTime(s.DOB).ToString("dd-MM-yyyy");

                    PdfPCell cell1 = new PdfPCell(new Phrase(i.ToString()));
                    cell1.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    table.AddCell(cell1);
                    PdfPCell cell2 = new PdfPCell(new Phrase(s.StudentId.ToString()));
                    cell2.Border = Rectangle.BOTTOM_BORDER;
                    cell2.Colspan = 2;
                    table.AddCell(cell2);
                    PdfPCell cell3 = new PdfPCell(new Phrase((s.Name==null)?"":s.Name.ToString()));
                    cell3.Border = Rectangle.BOTTOM_BORDER;
                    cell3.Colspan = 2;
                    table.AddCell(cell3);
                    PdfPCell cell4 = new PdfPCell(new Phrase((dob==null)?"":dob.ToString()));
                    cell4.Border = Rectangle.BOTTOM_BORDER;
                    cell4.Colspan = 2;
                    table.AddCell(cell4);
                    PdfPCell cell5 = new PdfPCell(new Phrase((s.Cdcno == null)?"":s.Cdcno.ToString()));
                    cell5.Border = Rectangle.BOTTOM_BORDER;
                    cell5.Colspan = 2;
                    table.AddCell(cell5);
                    PdfPCell cell6 = new PdfPCell(new Phrase(s.PassportNo == null ? "" : s.PassportNo.ToString()));
                    cell6.Border = Rectangle.BOTTOM_BORDER;
                    cell6.Colspan = 2;
                    table.AddCell(cell6);
                    PdfPCell cell7 = new PdfPCell(new Phrase(""));
                    cell7.Border = Rectangle.BOTTOM_BORDER;
                    table.AddCell(cell7);
                    PdfPCell cell8 = new PdfPCell(new Phrase(""));
                    cell8.Border = Rectangle.BOTTOM_BORDER;
                    table.AddCell(cell8);
                    PdfPCell cell9 = new PdfPCell(new Phrase(s.IndosNo == null ? "" : s.IndosNo.ToString()));
                    cell9.Border = Rectangle.BOTTOM_BORDER;
                    cell9.Colspan = 2;
                    table.AddCell(cell9);
                    PdfPCell cell10 = new PdfPCell(new Phrase(""));
                    cell10.Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    cell10.FixedHeight = 50f;
                    table.AddCell(cell10);

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
        public ActionResult CheckListEdit()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            var obj = new List<CheckListVM>();
            return View(obj);
        }
        [HttpGet]
        public ActionResult GetCheckListEdit(int? BatchId)
        {
            var list = (from apd in db.Applied.Where(x=>x.BatchId == BatchId && x.AdmissionStatus == true).Select(x => x.ApplicationId).Distinct()
                        join ap in db.Applications on apd equals ap.ApplicationId
                        
                        select new CheckListVM
                        {
                            ApplicationId = ap.ApplicationId,
                            StudentId = ap.ApplicationCode,
                            Name = ap.FullName,
                            DOB = ap.DateOfBirth,
                            Cdcno = ap.CdcNo,
                            PassportNo = ap.PassportNo,
                            Rank = ap.RankOfCandidate,
                            Grade = ap.GradeOfCompetencyNo,
                            IndosNo = ap.InDosNo


                        }).ToList();
            return PartialView("_ApplicantsListEdit", list.ToList());
        }
        //[HttpGet]
        //public async Task<ActionResult> CheckListEditModal(int? id)
        //{
        //    Application ap = await db.Applications.FindAsync(id);

        //    var obj = new CheckListVM
        //    {
        //        ApplicationId = ap.ApplicationId,
        //        Name = ap.FullName,
                
        //        DOB = ap.DateOfBirth,
        //        Cdcno = ap.CdcNo,
        //        PassportNo = ap.PassportNo,
        //        Rank = ap.RankOfCandidate,
        //        Grade = ap.GradeOfCompetencyNo,
        //        IndosNo = ap.InDosNo


        //    };

        //    return PartialView("CheckListEditModal", obj);
        //}


        [HttpPost]
        public JsonResult CheckListEdit(CheckListVM obj)
        {

           
                Application c = db.Applications.Find(obj.ApplicationId);
                c.FullName = obj.Name;
                c.DateOfBirth = obj.DOB;
                c.CdcNo = obj.Cdcno;
                c.PassportNo = obj.PassportNo;
                c.RankOfCandidate = obj.Rank;
                c.GradeOfCompetencyNo = obj.Grade;
                c.InDosNo = obj.IndosNo;

               
                db.SaveChanges();
                var list = db.Applications.Where(x => x.ApplicationId == obj.ApplicationId);
               
                return Json(list);




        }
        #endregion

        #region DesignCertificate
        public ActionResult DesignCertificate()
        {
            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");

            ViewBag.Principal = db.Principals.ToList()
                .Select(p => new SelectListItem
                {
                    Text = p.PricipalName,
                    Value = p.PrincipalId.ToString()
                });
            ViewBag.CertificateFormat = db.CertificateFormats.ToList()
              .Select(p => new SelectListItem
              {
                  Text = p.FormatName,
                  Value = p.CertificateFormatId.ToString()
              });
            CertificateDesignCertificateVM obj = new CertificateDesignCertificateVM();
            obj._certificatedesignlist = CertificateDesignList();

            return View(obj);

        }
        public List<CertificateDesignList> CertificateDesignList()
        {
            var obj = from cd in db.CertificateDesigns
                      join c in db.Courses on cd.CourseId equals c.CourseId
                      join f in db.CertificateFormats on cd.CertificateFormatId equals f.CertificateFormatId
                      into tmpMappc
                      from mappingsc in tmpMappc.DefaultIfEmpty()
                      join p in db.Principals on cd.PrincipalId equals p.PrincipalId into tmpMapp
                      from mappings in tmpMapp.DefaultIfEmpty()
                      select new CertificateDesignList
                      {
                          CertificateDesignId = cd.CertificateDesignId,
                          CourseName = c.CourseName,
                          CourseNameTitle = cd.CourseName,
                          LineOfCertificate = cd.LineOfCertificate,
                          Paragraph1 = cd.Paragraph1,
                          Paragraph2 = cd.Paragraph2,
                          Paragraph3 = cd.Paragraph3,
                          Topic4 = cd.Topic4,
                          Topic5=cd.Topic5,
                          PrincipalName = mappings.PricipalName,
                          CertificateFormat = mappingsc.FormatName,
                          CreatedDate = cd.CreatedDate

                      };

            return obj.ToList();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DesignCertificate(CertificateDesignCertificateVM obj)
        {
           
            if (ModelState.IsValid)
            {
                if (obj.CertificateDesignId == 0)
                {
                    CertificateDesign b = new CertificateDesign
                    {

                        CourseId = obj.CourseId,
                        PrincipalId = obj.PrincipalId,
                        LineOfCertificate = obj.LineOfCertificate,
                        CourseName = obj.CourseName,
                        Paragraph1 = obj.Paragraph1,
                        Paragraph2 = obj.Paragraph2,
                        Paragraph3 = obj.Paragraph3,
                        Topic4 = obj.Topic4,
                        Topic5=obj.Topic5,
                        CertificateFormatId = obj.CertificateFormatId


                    };

                    db.CertificateDesigns.Add(b);

                    db.SaveChanges();
                }
                else
                {
                    CertificateDesign cf = new CertificateDesign
                    {
                        CertificateDesignId = (Int32)obj.CertificateDesignId,
                        LineOfCertificate = obj.LineOfCertificate,
                        CourseId = obj.CourseId,
                        PrincipalId = obj.PrincipalId,
                        CourseName = obj.CourseName,
                        Paragraph1 = obj.Paragraph1,
                        Paragraph2 = obj.Paragraph2,
                        Paragraph3 = obj.Paragraph3,
                        Topic4 = obj.Topic4,
                        Topic5=obj.Topic5,
                        CreatedDate = DateTime.UtcNow,
                        CertificateFormatId = obj.CertificateFormatId
                    };

                    db.Entry(cf).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("DesignCertificate");
            }
            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");
            ViewBag.Principal = new SelectList(db.Principals.ToList(), "PrincipalId", "PricipalName");
            ViewBag.CertificateFormat = new SelectList(db.CertificateFormats.ToList(), "CertificateFormatId", "FormatName");
            obj._certificatedesignlist = CertificateDesignList();
            return View(obj);
        }

        public ActionResult DesignEdit(int? id)
        {
            CertificateDesign obj =  db.CertificateDesigns.Find(id);
            var vm = new CertificateDesignCertificateVM
            {
                CertificateDesignId = obj.CertificateDesignId,
                LineOfCertificate = obj.LineOfCertificate,
                CertificateFormatId=obj.CertificateFormatId,
                CourseId = obj.CourseId,
                PrincipalId = obj.PrincipalId,
                CourseName = obj.CourseName,
                Paragraph1 = obj.Paragraph1,
                Paragraph2 = obj.Paragraph2,
                Paragraph3 = obj.Paragraph3,
                Topic4 = obj.Topic4,
                Topic5=obj.Topic5

            };

            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");
            ViewBag.Principal = new SelectList(db.Principals.ToList(), "PrincipalId", "PricipalName");
            ViewBag.CertificateFormat = new SelectList(db.CertificateFormats.ToList(), "CertificateFormatId", "FormatName");
            vm._certificatedesignlist= CertificateDesignList();
            return View("DesignCertificate", vm);
        }

        #endregion

        #region PrincipalSignature
        public ActionResult PrincipalSignImage()
        {
            CertificatePrincipalVM c = new CertificatePrincipalVM();
            c._principalList = db.Principals.ToList();
            return View(c);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrincipalSignImage(CertificatePrincipalVM obj, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] {
                ".Jpg", ".png", ".jpg", "jpeg"
                  };
                string fileName = file.FileName.ToString();
                string imgPath = "/Uploads/Photo/" + fileName;
                file.SaveAs(Server.MapPath(imgPath));
              
                var ext = Path.GetExtension(file.FileName);
                if (allowedExtensions.Contains(ext)) //check what type of extension  
                {
                    if (obj.PrincipalId == 0)
                    {
                        Principal b = new Principal
                        {
                            PricipalName = obj.PricipalName,
                            SignatureImgUrl = imgPath
                        };

                        db.Principals.Add(b);

                        db.SaveChanges();
                    }
                    else
                    {
                        Principal b = new Principal
                        {
                            PrincipalId = obj.PrincipalId,
                            PricipalName = obj.PricipalName,
                            SignatureImgUrl = imgPath
                        };
                        db.Entry(b).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    obj._principalList = db.Principals.ToList();
                    return View(obj);
                }
                else
                {
                    obj._principalList = db.Principals.ToList();
                    return View(obj);
                }

            }
            obj._principalList = db.Principals.ToList();
            return View(obj);
        }

        public ActionResult PrincipalSignEdit(int id)
        {
            var p = db.Principals.Where(m => m.PrincipalId == id).FirstOrDefault();
            var obj = new CertificatePrincipalVM
            {
                PrincipalId = p.PrincipalId,
                PricipalName = p.PricipalName,
                SignatureImgUrl = p.SignatureImgUrl
            };
            obj._principalList = db.Principals.ToList();
            return View("PrincipalSignImage", obj);
        }



        #endregion
        #region Certificate
        [HttpGet]
        public ActionResult Certificate()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            CertificationCertificateVM ccvm = new CertificationCertificateVM();
            var obj = new List<CertificationCertificateVM.Certificate>();
            ccvm._CertificateList = obj.ToList();
            ccvm.PerformAction = null;
            return View(ccvm);
        }

        [HttpPost]
        public ActionResult Certificate(CertificationCertificateVM obj)
        {
            if (ModelState.IsValid)
            {

                 
                var CertifcateList = from appld in db.Applied
                                     join app in db.Applications on appld.ApplicationId equals app.ApplicationId
                                     join cd in db.CertificateDesigns on appld.CourseId equals cd.CourseId
                                     join c in db.Courses on appld.CourseId equals c.CourseId
                                     join pr in db.Principals on cd.PrincipalId equals pr.PrincipalId
                                     join b in db.Batches on appld.BatchId equals b.BatchId
                                     join e in db.Employees on b.CoordinatorId equals e.EmployeeId
                                     where appld.AdmissionStatus == true && appld.BatchId == obj.BatchId
                                     select new CertificationCertificateVM.Certificate
                                     { 
                                         
                                         CertificateNo ="",
                                         BatchCode=b.BatchCode,
                                         CourseCode=c.CourseCode,
                                         ApplicationID=app.ApplicationId,
                                         ApplicationCode=app.ApplicationCode,
                                         BatchId=b.BatchId,
                                         ApplicantName = app.FullName,
                                         CDCNo = app.CdcNo,
                                         DateofBirth = app.DateOfBirth,
                                         PassportNo = app.PassportNo,
                                         Grade = app.GradeOfCompetencyNo,
                                         Number = app.CertOfCompetencyNo,
                                         Indosno = app.InDosNo,
                                         LineOfCertificate = cd.LineOfCertificate,
                                         CourseName = cd.CourseName,
                                         StartDate = b.StartDate,
                                         EndDate = b.EndDate,
                                         Paragraph1 = cd.Paragraph1,
                                         Paragraph2 = cd.Paragraph2,
                                         Paragraph3 = cd.Paragraph3,
                                         CourseInCharge = e.FirstName  +" "+ e.MiddleName +" " + e.LastName,
                                         Topic4 = cd.Topic4 == null ? "" : cd.Topic4,
                                         Topic5=cd.Topic5==null?"":cd.Topic5,
                                         DateOfIssue = b.EndDate,
                                         PrincipalName = pr.PricipalName,
                                         PrincipalSign = pr.SignatureImgUrl,
                                         DateofExpiry=b.CourseExpiryDate


                                     };

                var certificateformatid = db.CertificateDesigns.Where(m => m.CourseId == obj.CourseId).Select(m => m.CertificateFormatId).FirstOrDefault();
                var performaction = db.CertificateFormats.Where(m => m.CertificateFormatId == certificateformatid).Select(m => m.ActionName).FirstOrDefault();
                CertificationCertificateVM ccvm = new CertificationCertificateVM();
                ccvm.PerformAction = performaction;
               
                ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
                int count = db.Certificates.Where(x=>x.BatchId==obj.BatchId).Count();

                foreach (var item in CertifcateList.ToList())
                {
                    var existingitem = db.Certificates.Where(c => (c.ApplicationId == item.ApplicationID && c.BatchId == item.BatchId)).Select(m => m.CertificateId).ToList();
                   
                    if (existingitem.Count == 0)
                    {   count++;
                        string year = DateTime.Now.Year.ToString();
                        Certificate c = new Certificate
                        {
                            CertificateCode = item.CourseCode + item.BatchCode + count.ToString().PadLeft(3, '0') + year,
                            ApplicationId = item.ApplicationID,
                            BatchId = item.BatchId,
                            CreateDate = DateTime.Now,
                            IsPrint=true
                        };
                        db.Certificates.Add(c);
                        db.SaveChanges();
                    }
                }
                
                var CertifcatesList = from appld in db.Applied
                                     join app in db.Applications on appld.ApplicationId equals app.ApplicationId
                                     join cd in db.CertificateDesigns on appld.CourseId equals cd.CourseId
                                     join c in db.Courses on appld.CourseId equals c.CourseId
                                     join pr in db.Principals on cd.PrincipalId equals pr.PrincipalId
                                     join b in db.Batches on appld.BatchId equals b.BatchId
                                     join e in db.Employees on b.CoordinatorId equals e.EmployeeId
                                     where appld.AdmissionStatus == true && appld.BatchId == obj.BatchId
                                     select new CertificationCertificateVM.Certificate
                                     {

                                         CertificateNo = db.Certificates.Where(x=>(x.ApplicationId==app.ApplicationId && x.BatchId==b.BatchId)).Select(x=>x.CertificateCode).FirstOrDefault(),
                                         BatchCode = b.BatchCode,
                                         CourseCode = c.CourseCode,
                                         ApplicationID = app.ApplicationId,
                                         ApplicationCode=app.ApplicationCode,
                                         BatchId = b.BatchId,
                                         ApplicantName = app.FullName,
                                         CDCNo = app.CdcNo,
                                         DateofBirth = app.DateOfBirth,
                                         PassportNo = app.PassportNo,
                                         Grade = app.GradeOfCompetencyNo,
                                         Number = app.CertOfCompetencyNo,
                                         Indosno = app.InDosNo,
                                         LineOfCertificate = cd.LineOfCertificate,
                                         CourseName = cd.CourseName,
                                         StartDate = b.StartDate,
                                         EndDate = b.EndDate,
                                         Paragraph1 = cd.Paragraph1,
                                         Paragraph2 = cd.Paragraph2,
                                         Paragraph3 = cd.Paragraph3,
                                         CourseInCharge = e.FirstName +" "+ e.MiddleName +" "+ e.LastName,
                                         Topic4 = cd.Topic4 == null ? "" : cd.Topic4,
                                         Topic5 = cd.Topic5 == null ? "" : cd.Topic5,
                                         DateOfIssue = b.EndDate,
                                         PrincipalName = pr.PricipalName,
                                         PrincipalSign = pr.SignatureImgUrl,
                                         DateofExpiry = b.CourseExpiryDate


                                     };
               
                ccvm._CertificateList = CertifcatesList.OrderBy(x=>x.CertificateNo).ToList();
                ViewBag.batchId = obj.BatchId;
                return View(ccvm);
            }
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");

            return View(obj);
        }
        #endregion
        #region UniqueIdentifier
        public JsonResult IsCourseFormatExists(string CourseId)
        {
            int courseid = 0;
            try { courseid = Convert.ToInt32(CourseId); }
            catch { courseid = 0; }
           
            var data = db.CertificateDesigns.Where(x => x.CourseId == courseid).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  

        }
        #endregion

        #region ResultFormat
        public ActionResult ViewResultFormat(int? id)
        {
            var studentlist = from appld in db.Certificates
                              join app in db.Applications on appld.ApplicationId equals app.ApplicationId
                              join b in db.Batches on appld.BatchId equals b.BatchId
                              join c in db.Courses on b.CourseId equals c.CourseId
                              where  appld.BatchId == id
                              select new CertificateApplicantList
                              {
                                  ApplicantId=app.ApplicationId,
                                  ApplicantName=app.FullName.ToUpper(),
                                  Rank=app.RankOfCandidate.ToUpper(),
                                  DateOfBirth=app.DateOfBirth,
                                  IndosNo=app.InDosNo.ToUpper(),
                                  PassportNo=app.PassportNo.ToUpper(),
                                  CdcNo=app.CdcNo.ToUpper(),
                                  CertificateNo=appld.CertificateCode
                                   
                              };
            var batchdetails = db.Batches.Where(x => x.BatchId == id).FirstOrDefault();
            var coursedetails = db.Courses.Where(x => x.CourseId == batchdetails.CourseId).FirstOrDefault();
            CertificateViewResultFormat cvrf = new CertificateViewResultFormat();
            cvrf.CourseName = coursedetails.CourseName;
            cvrf.BatchCode = batchdetails.BatchCode;
            cvrf.From = batchdetails.StartDate;
            cvrf.To = batchdetails.EndDate;
            cvrf.Date = batchdetails.StartDate;
            cvrf.CurrentDate = DateTime.Now;
            cvrf._CertificateApplicantList = studentlist.ToList();

                              
                             
           return new PdfActionResult(cvrf);
        }
        #endregion

        #region ResultSheet
        public ActionResult ResultSheet(int? id)
        {
            var studentlist = from appld in db.Applied
                              join app in db.Applications on appld.ApplicationId equals app.ApplicationId
                              join b in db.Batches on appld.BatchId equals b.BatchId
                              join c in db.Courses on appld.CourseId equals c.CourseId
                              where appld.AdmissionStatus == true && appld.BatchId == id
                              select new CertificateApplicantList
                              {
                                  ApplicantId = app.ApplicationId,
                                  ApplicantName = app.FullName.ToUpper(),
                                  Rank = app.RankOfCandidate.ToUpper(),
                                  DateOfBirth = app.DateOfBirth,
                                  IndosNo = app.InDosNo.ToUpper(),
                                  PassportNo = app.PassportNo.ToUpper(),
                                  CdcNo = app.CdcNo.ToUpper()
                                  
                              };
            var batchdetails = db.Batches.Where(x => x.BatchId == id).FirstOrDefault();
            var coursedetails = db.Courses.Where(x => x.CourseId == batchdetails.CourseId).FirstOrDefault();
            CertificateViewResultFormat cvrf = new CertificateViewResultFormat();
            cvrf.CourseName = coursedetails.CourseName;
            cvrf.BatchCode = batchdetails.BatchCode;
            cvrf.From = batchdetails.StartDate;
            cvrf.To = batchdetails.EndDate;
            cvrf.Date = batchdetails.StartDate;
            cvrf.CurrentDate = DateTime.Now;
            cvrf._CertificateApplicantList = studentlist.ToList();



            return new PdfActionResult(cvrf);
        }
        #endregion

        #region ViewResultSheet
        public ActionResult ViewResultSheet()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            CertificationCertificateVM ccvm = new CertificationCertificateVM();
            return View(ccvm);
        }

        #endregion

    }
}