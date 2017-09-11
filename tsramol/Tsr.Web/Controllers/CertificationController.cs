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

namespace Tsr.Web.Controllers
{
    public class CertificationController : Controller
    {
        AppContext db = new AppContext();
        #region CheckList
        public ActionResult CheckList()
        {
            return View();
        }
        #endregion

        #region DesignCertificate
        public ActionResult DesignCertificate()
        {
            ViewBag.Course = new SelectList(db.Courses.ToList(), "CourseId", "CourseName");
            var obj = new List<CertificateDesignCertificateVM>();

            return View(obj);

        }
        public async Task<ActionResult> CertificateDesignList()
        {
            var obj = from cd in db.CertificateDesigns
                      join c in db.Courses on cd.CourseId equals c.CourseId
                      join p in db.Principals on cd.PrincipalId equals p.PrincipalId into tmpMapp
                      from mappings in tmpMapp.DefaultIfEmpty()
                      select new CertificateDesignList
                      {
                          CertificateDesignId = cd.CertificateDesignId,
                          CourseName = c.CourseName,
                          CourseNameTitle = cd.CourseName,
                          LineOfCertificate=cd.LineOfCertificate,
                          Paragraph1 = cd.Paragraph1,
                          Paragraph2 = cd.Paragraph2,
                          Paragraph3 = cd.Paragraph3,
                          Topic4 = cd.Topic4,
                          PrincipalName = mappings.PricipalName,
                          CreatedDate = cd.CreatedDate

                      };

            return View(await obj.ToListAsync());
        }

        public ActionResult DesignCreate()
        {
            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");

            ViewBag.Principal = db.Principals.ToList()
                .Select(p => new SelectListItem
                {
                    Text = p.PricipalName,
                    Value = p.PrincipalId.ToString()
                });

            return PartialView("DesignCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesignCreate(CertificateDesignCertificateVM obj)
        {
            if (ModelState.IsValid)
            {
              
                CertificateDesign b = new CertificateDesign
                {
                   
                    CourseId = obj.CourseId,
                    PrincipalId=obj.PrincipalId,
                    LineOfCertificate=obj.LineOfCertificate,
                    CourseName=obj.CourseName,
                    Paragraph1=obj.Paragraph1,
                    Paragraph2=obj.Paragraph2,
                    Paragraph3=obj.Paragraph3,
                    Topic4=obj.Topic4

                
                };

                db.CertificateDesigns.Add(b);

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");

            ViewBag.Principal = db.Principals.ToList()
                .Select(p => new SelectListItem
                {
                    Text = p.PricipalName,
                    Value = p.PrincipalId.ToString()
                });
            return PartialView("DesignCreate", obj);
        }

        public async Task<ActionResult> DesignEdit(int? id)
        {
            CertificateDesign obj = await db.CertificateDesigns.FindAsync(id);
            var vm = new CertificateDesignCertificateVM
            {
                CertificateDesignId=obj.CertificateDesignId,
                LineOfCertificate=obj.LineOfCertificate,
                CourseId = obj.CourseId,
                PrincipalId = obj.PrincipalId,
                CourseName =obj.CourseName,
                Paragraph1=obj.Paragraph1,
                Paragraph2=obj.Paragraph2,
                Paragraph3=obj.Paragraph3,
                Topic4=obj.Topic4
                
            };

            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");
            ViewBag.Principal=new SelectList(db.Principals.ToList(),"PrincipalId","PricipalName");

            return PartialView("DesignEdit", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesignEdit(CertificateDesignCertificateVM obj)
        {
            if (ModelState.IsValid)
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
                    CreatedDate= DateTime.UtcNow
                };
                
                db.Entry(cf).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("DesignEdit", obj);
        }
        #endregion

        #region PrincipalSignature
        public ActionResult PrincipalSignImage()
        {
            CertificatePrincipalVM c = new CertificatePrincipalVM();
            c._principalList  = db.Principals.ToList();
            return View(c);
        }

      


        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult PrincipalSignImage(CertificatePrincipalVM obj, HttpPostedFileBase file)
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
                            PrincipalId=obj.PrincipalId,
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
                PrincipalId=p.PrincipalId,
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

            var CertifcateList = from a in db.Applications
                                 join b in db.Applied on a.ApplicationId equals b.ApplicationId
                                 join c in db.CertificateDesigns on a.CourseId equals c.CourseId
                                 join p in db.Principals on c.PrincipalId equals p.PrincipalId
                                 where b.AdmissionStatus == true && a.CategoryId == 2 && a.CourseId ==1 && a.BatchId == 1
                                 select new CertificationCertificateVM.Certificate
                                 {
                                     CertificateNo = "0330590012017",
                                     ApplicantName = a.FirstName + "" + a.MiddleName == null ? "" : a.MiddleName + "" + a.LastName == null ? "" : a.LastName,
                                     CDCNo = a.CdcNo,
                                     PassportNo = a.PassportNo
                                    

                                 };

            CertificationCertificateVM ccvm = new CertificationCertificateVM();
            ccvm._CertificateList = CertifcateList.ToList();
            return View(ccvm);
        }

        [HttpPost]
        public ActionResult Certificate(CertificationCertificateVM obj)
        {
            if (ModelState.IsValid)
            {


                var CertifcateList = from a in db.Applications
                                     join b in db.Applied on a.ApplicationId equals b.ApplicationId
                                     join c in db.CertificateDesigns on a.CourseId equals c.CourseId
                                     join p in db.Principals on c.PrincipalId equals p.PrincipalId
                                     where b.AdmissionStatus == true && a.CategoryId == obj.CategoryId && a.CourseId == obj.CourseId && a.BatchId == obj.BatchId
                                     select new CertificationCertificateVM.Certificate
                                     {
                                         CertificateNo= "0330590012017",
                                         ApplicantName =a.FirstName+""+a.MiddleName==null?"":a.MiddleName+""+a.LastName==null?"":a.LastName,
                                         CDCNo=a.CdcNo,
                                         PassportNo=a.PassportNo


                                     };

                CertificationCertificateVM ccvm = new CertificationCertificateVM();
                ccvm._CertificateList = CertifcateList.ToList();
                ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");

                return View(ccvm);
            }
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");

            return View(obj);
        }
        #endregion

    }
}