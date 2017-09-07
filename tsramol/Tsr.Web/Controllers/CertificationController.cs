using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Models;
using Tsr.Infra;

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
        public ActionResult CertificateDesignList()
        {
            //var obj = from cd in db.CertificateDesigns
            //          join c in db.Courses on cd.CourseId equals c.CourseId
            //          select new CertificateDesignList
            //          {
            //              CD_Id = cd.CD_Id,
            //              CourseName = c.CourseName,
            //              TopicBeforeCourseTitle = cd.TopicBeforeCourseTitle,
            //              CourseTitle = cd.CourseTitle,
            //              Topic1 = cd.Topic1,
            //              Topic2 = cd.Topic2,
            //              Topic3 = cd.Topic3,
            //              Topic4 = cd.Topic4,
            //              CourseIncharge = cd.CourseIncharge,
            //              CreatedDate = cd.CreatedDate

            //          };

            return View();
        }

        public ActionResult DesignCreate()
        {
            ViewBag.Course = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            //ViewBag.Coordinator = new SelectList(db.Employees.Where(x=>x.IsActive == true).Select(em) ,"EmployeeId","");
            ViewBag.Coordinator = db.Employees.Where(x => x.IsActive == true)
                .Select(p => new SelectListItem
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EmployeeId.ToString()
                });

            return PartialView("DesignCreate");
        }
        #endregion
    }
}