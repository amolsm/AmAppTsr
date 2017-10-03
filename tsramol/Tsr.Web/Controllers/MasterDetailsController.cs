using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;
using Tsr.Web.Common;

namespace Tsr.Web.Controllers
{
    public class MasterDetailsController : Controller
    {
        AppContext db = new AppContext();

        public ActionResult Index()
        {
            return View();
        }

        #region Common
        public ActionResult FillCourse(int CategoryId)
        {
            var Courses = db.Courses.Where(c => c.CategoryId == CategoryId && c.IsActive == true);
            return Json(Courses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCourseForCourseFee(int CategoryId)
        {
            //var Courses = db.Courses.Where(c => c.CategoryId == CategoryId && c.IsActive == true);
            var Courses = from c in db.Courses
                    join cf in db.CourseFees on c.CourseId equals cf.CourseId
                    into ccf
                    from cf in ccf.DefaultIfEmpty()
                    where (cf == null && c.CategoryId == CategoryId && c.IsActive == true)
                    select new { c.CourseId, c.CourseName};
            return Json(Courses, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Organisation
        public async Task<ActionResult> OrganisationList()
        {
            return View(await db.Organisations.ToListAsync());
        }

        public ActionResult OrganisationCreate()
        {
            return PartialView("OrganisationCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationCreate(OrganisationMaster org)
        {
            if (ModelState.IsValid)
            {
                db.Organisations.Add(org);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("OrganisationCreate", org);
        }

        public async Task<ActionResult> OrganisationEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganisationMaster fp = await db.Organisations.FindAsync(id);
            if (fp == null)
            {
                return HttpNotFound();
            }
            return PartialView("OrganisationEdit", fp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganisationEdit(OrganisationMaster obj)
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
            return PartialView("OrganisationEdit", obj);
        }

        #endregion

        #region FeesPattern
        public async Task<ActionResult> FeesPatternList()
        {
            return View(await db.FeesPatterns.ToListAsync());
        }

        public ActionResult FeesPatternCreate()
        {
            return PartialView("FeesPatternCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FeesPatternCreate(FeesPattern obj)
        {
            if (ModelState.IsValid)
            {
                db.FeesPatterns.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("FeesPatternCreate", obj);
        }

        public async Task<ActionResult> FeesPatternEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeesPattern fp = await db.FeesPatterns.FindAsync(id);
            if (fp == null)
            {
                return HttpNotFound();
            }
            return PartialView("FeesPatternEdit", fp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FeesPatternEdit(FeesPattern obj)
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
            return PartialView("FeesPatternEdit", obj);
        }

        public async Task<ActionResult> FeesPatternDeactive( int? id, string flag = null)
        {
            
            if (id == null)
            {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeesPattern fp = await db.FeesPatterns.FindAsync(id);

            if (fp == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                fp.IsActive = false;
            if (flag == "A")
                fp.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("FeesPatternList");
        }

        #endregion

        #region Employee
        public async Task<ActionResult> EmployeeList()
        {
            
            var vm = from e in db.Employees
                     join d in db.Designations on e.DesignationId equals d.DesignationId
                     select new MasterEmployeeListVM { Emp = e, Desig = d };

            return View(await vm.ToListAsync());
        }

        public ActionResult EmployeeCreate()
        {
            ViewBag.Designations = new SelectList(db.Designations.ToList(),"DesignationId", "DesignationName");
            ViewBag.Organisations = new SelectList(db.Organisations.ToList(), "OrganisationId", "Name");
            return PartialView("EmployeeCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeCreate(Employee obj)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.Designations = new SelectList(db.Designations.ToList(), "DesignationId", "DesignationName");
            ViewBag.Organisations = new SelectList(db.Organisations.ToList(), "OrganisationId", "Name");
            return PartialView("EmployeeCreate", obj);
        }

        public async Task<ActionResult> EmployeeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee em = await db.Employees.FindAsync(id);
            if (em == null)
            {
                return HttpNotFound();
            }
            ViewBag.Designations = new SelectList(db.Designations.ToList(), "DesignationId", "DesignationName");
            ViewBag.Organisations = new SelectList(db.Organisations.ToList(), "OrganisationId", "Name");
            return PartialView("EmployeeEdit", em);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeEdit(Employee obj)
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
            return PartialView("EmployeeEdit", obj);
        }

        public async Task<ActionResult> EmployeeDeactive(int? id, string flag = null)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee fp = await db.Employees.FindAsync(id);

            if (fp == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                fp.IsActive = false;
            if (flag == "A")
                fp.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("EmployeeList");
        }

        public async Task<ActionResult> EmployeeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee em = await db.Employees.FindAsync(id);
            if (em == null)
            {
                return HttpNotFound();
            }
            ViewBag.Designations = new SelectList(db.Designations.ToList(), "DesignationId", "DesignationName");
            ViewBag.Organisations = new SelectList(db.Organisations.ToList(), "OrganisationId", "Name");
            return PartialView("EmployeeDetails", em);
        }

        #endregion

        #region Department
        public async Task<ActionResult> DepartmentList()
        {
            var obj = from d in db.Departments
                      join e in db.Employees
                      on d.DepartmentHead equals e.EmployeeId
                      select new MasterDetpartmentListVM
                      {
                          DepartmentId = d.DepartmentId,
                          DepartmentName = d.Name,
                          Address = d.Address,
                          Phone = d.Phone,
                          Extension = d.PhoneExtension,
                          Cellphone = d.DepHeadCell,
                          DepartmentHead = e.FirstName + " " + e.LastName,
                          IsActive = d.IsActive
                      };
            return View(await obj.ToListAsync());
        }

        public ActionResult DepartmentCreate()
        {
            var emp = db.Employees
                           .Where(x => x.IsActive == true)
                           .ToList()
                           .Select(x => new { EmployeeId = x.EmployeeId, EmployeeName = string.Format("{0} {1}", x.FirstName, x.LastName) });

            ViewBag.Employees = new SelectList(emp, "EmployeeId", "EmployeeName");
            return PartialView("DepartmentCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DepartmentCreate(Department obj)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("DepartmentCreate", obj);
        }

        public async Task<ActionResult> DepartmentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department obj = await db.Departments.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            var emp = db.Employees
                           .Where(x => x.IsActive == true)
                           .ToList()
                           .Select(x => new { EmployeeId = x.EmployeeId, EmployeeName = string.Format("{0} {1}", x.FirstName, x.LastName) });
            ViewBag.Employees = new SelectList(emp, "EmployeeId", "EmployeeName");
            return PartialView("DepartmentEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DepartmentEdit(Department obj)
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
            return PartialView("DepartmentEdit", obj);
        }

        public async Task<ActionResult> DepartmentDeactive(int? id, string flag = null)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department obj = await db.Departments.FindAsync(id);

            if (obj == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                obj.IsActive = false;
            if (flag == "A")
                obj.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }


        #endregion

        #region Designation
        public async Task<ActionResult> DesignationList()
        {
            return View(await db.Designations.ToListAsync());
        }

        public ActionResult DesignationCreate()
        {
            return PartialView("DesignationCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesignationCreate(Designation obj)
        {
            obj.CreatedBy = 1;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedBy = 1;
            obj.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Designations.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("DesignationCreate", obj);
        }

        public async Task<ActionResult> DesignationEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation obj = await db.Designations.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            
            return PartialView("DesignationEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesignationEdit(Designation obj)
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
            return PartialView("DesignationEdit", obj);
        }

        public async Task<ActionResult> DesignationDeactive(int? id, string flag = null)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation obj = await db.Designations.FindAsync(id);

            if (obj == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                obj.IsActive = false;
            if (flag == "A")
                obj.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("DesignationList");
        }

        #endregion

        #region Document
        public async Task<ActionResult> DocumentList()
        {
                        return View(await db.Documents.ToListAsync());
        }

        public ActionResult DocumentCreate()
        {
            return PartialView("DocumentCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DocumentCreate(Document obj)
        {
            if (ModelState.IsValid)
            {
                db.Documents.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("DocumentCreate", obj);
        }

        public async Task<ActionResult> DocumentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document obj = await db.Documents.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("DocumentEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DocumentEdit(Document obj)
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
            return PartialView("DocumentEdit", obj);
        }

        public async Task<ActionResult> DocumentDeactive(int? id, string flag = null)
        {

            if (id == null || flag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document obj = await db.Documents.FindAsync(id);

            if (obj == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                obj.IsActive = false;
            if (flag == "A")
                obj.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("DocumentList");
        }


        #endregion

        #region Package
        public async Task<ActionResult> PackageList()
        {
            var p = await db.packages.ToListAsync();
            //var list = new List<MasterPackageListVM>();

            //foreach (var item in p)
            //{
            //    db.PackageCourses.Where(x=>x.PackageId == item.PackageId).s
            //    list.Add( new MasterPackageListVM {
            //        IsActive = (bool)item.IsActive,
            //        PackageId = item.PackageId,
            //        PackageName = item.PackageName,
            //        Courses = db.PackageCourses.
            //    });
            //}
            return View(p);
        }

        public ActionResult PackageCreate()
        {
            return PartialView("PackageCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PackageCreate(MasterPackageCreateVM vm)
        {
            if (ModelState.IsValid)
            {
                Package obj = new Package {
                    PackageName = vm.PackageName,
                    Description = vm.Description,
                    IsActive = vm.IsActive,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now
                    
                };
                db.packages.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("PackageCreate", vm);
        }

        public async Task<ActionResult> PackageEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var obj = await db.packages.FindAsync(id);

            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("PackageEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PackageEdit(Package obj)
        {
            if (ModelState.IsValid)
            {
                obj.ModifiedBy = 1;
                obj.ModifiedDate = DateTime.Now;

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
            return PartialView("PackageEdit", obj);
        }

        public ActionResult PackageCourses(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var obj = (from pc in db.PackageCourses
                       join c in db.Courses on pc.CourseId equals c.CourseId
                       where (pc.PackageId == id)
                       select pc.CourseId).ToArray();

            var crs = from c1 in db.Courses
                      join cc1 in db.CourseCategories on c1.CategoryId equals cc1.CourseCategoryId
                      where (cc1.CetRequired == false)
                      select new { c1.CourseId, c1.CourseName};

            ViewBag.Courses = new MultiSelectList(crs, "CourseId", "CourseName", obj);
            ViewBag.PackageId = id;

            return PartialView("PackageCourses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PackageCourses(IEnumerable<int> CourseId, string id)
        {
            int PackageId = Convert.ToInt32(id);

            db.PackageCourses.RemoveRange(db.PackageCourses.Where(x => x.PackageId == PackageId));
            await db.SaveChangesAsync();

            foreach (var item in CourseId)
            {
                var obj = new PackageCourse { PackageId = PackageId, CourseId = item };
                db.PackageCourses.Add(obj);
                await db.SaveChangesAsync();
            }
            //return PartialView("CourseDocuments");
            return Json(new { success = true });
        }
        #endregion

        #region Course
        public async Task<ActionResult> CourseList()
        {
            var obj = from c in db.Courses
                      join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                      //join e in db.Employees on c.Coordinator equals e.EmployeeId
                      select new MasterCourseListVM
                      {
                          CourseId = c.CourseId,
                          CourseCode = c.CourseCode,
                          CourseName = c.CourseName,
                          //ApplicationFee = c.ApplicationFee,
                          CategoryName = cc.CategoryName,
                          //Coordinator = e.FirstName + " " + e.LastName,
                          Duration = c.Duration + " " + c.Unit,
                          //MinBalance = c.MinBalance,
                          TotalSeat = c.TotalSeats.ToString() ,
                          IsActive = c.IsActive                         
                      };
            return View(await obj.ToListAsync());
        }

        public ActionResult CourseCreate()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.Unit = DropdownData.CourseUnits();

            var obj = new MasterCourseCreateVM { CourseCode = db.Courses.Count().ToString().PadLeft(3, '0') };
            
            return PartialView("CourseCreate",obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseCreate(MasterCourseCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                //try
                //{
                    Course c = new Course
                    {
                        CategoryId = obj.CategoryId,
                        CourseCode = obj.CourseCode,
                        CourseName = obj.CourseName,
                        Duration = obj.Duration,
                        IsActive = obj.IsActive,
                        MaxAge = obj.MaxAge,
                        MinAge = obj.MinAge,
                        TotalSeats = obj.TotalSeats,
                        Unit = obj.Unit,
                        ShortName = obj.ShortName
                    };

                    c.CreatedBy = 1;
                    c.CreatedDate = DateTime.Now;
                    c.ModifiedBy = 1;
                    c.ModifiedDate = DateTime.Now;
                    db.Courses.Add(c);

                    await db.SaveChangesAsync();
                    return Json(new { success = true });
               // }
                //catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                //{
                //    var error = ex.EntityValidationErrors.First().ValidationErrors.First();
                //    this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                //    ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
                //    ViewBag.Unit = new SelectList(new List<SelectListItem>
                //        {
                //            new SelectListItem { Value = "Days", Text= "Days"},
                //            new SelectListItem {Value = "Weeks", Text="Weeks" },
                //            new SelectListItem {Value = "Months", Text="Months" },
                //            new SelectListItem {Value = "Year" , Text = "Years"}
                //        }, "Value", "Text");

                //    return PartialView("CourseCreate", obj);
                //}
            }

            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.Unit = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Days", Text= "Days"},
                new SelectListItem {Value = "Weeks", Text="Weeks" },
                new SelectListItem {Value = "Months", Text="Months" },
                new SelectListItem {Value = "Year" , Text = "Years"}
            }, "Value", "Text");

            return PartialView("CourseCreate", obj);
        }

        public async Task<ActionResult> CourseEdit(int? id)
        {
            Course obj = await db.Courses.FindAsync(id);
            var vm = new MasterCourseCreateVM {
                CourseId = obj.CourseId,
                ShortName = obj.ShortName,
                CategoryId = obj.CategoryId,
                
                CourseCode = obj.CourseCode,
                CourseName = obj.CourseName,
                
                Duration = obj.Duration,
                
                IsActive = obj.IsActive,
                MaxAge = obj.MaxAge,
                MinAge = obj.MinAge,
                
                TotalSeats = obj.TotalSeats,
                Unit = obj.Unit,
                CreatedBy = obj.CreatedBy,
                CreatedDate = obj.CreatedDate                
            };

            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.Units = DropdownData.CourseUnits();

            return PartialView("CourseEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseEdit(MasterCourseCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                Course c = new Course {
                    CourseId = obj.CourseId,
                    ShortName = obj.ShortName,
                    CategoryId = obj.CategoryId,
                    //Convention = obj.Convention,
                    //Coordinator = obj.Coordinator,
                    CourseCode = obj.CourseCode,
                    CourseName = obj.CourseName,
                    //Designation = obj.Designation,
                    Duration = obj.Duration,
                    //FeesPatternId = obj.FeesPatternId,
                    IsActive = obj.IsActive,
                    MaxAge = obj.MaxAge,
                    MinAge = obj.MinAge,
                   // MinBalance = obj.MinBalance,
                    TotalSeats = obj.TotalSeats,
                    Unit = obj.Unit
                };

                c.ModifiedBy = 1;
                c.ModifiedDate = DateTime.Now;

                db.Entry(c).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success=true });
            }
            return PartialView("CourseEdit", obj);
        }

        public ActionResult CourseDocuments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var obj = (from cd in db.CourseDocuments
                       join d in db.Documents on cd.DocumentId equals d.DocumentsListId
                       where (cd.CourseId == id)
                       select cd.DocumentId).ToArray();
            
            var docs = db.Documents.Where(x => x.IsActive == true).ToList();
            ViewBag.Documents = new MultiSelectList(docs, "DocumentsListId", "DocumentName", obj);

            return PartialView("CourseDocuments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseDocuments(IEnumerable<int> DocumentsListId, string id)
        {
            int CourseId = Convert.ToInt32(id);
            db.CourseDocuments.RemoveRange(db.CourseDocuments.Where(x => x.CourseId == CourseId));
            await db.SaveChangesAsync();
            
            foreach (var item in DocumentsListId)
            {
                var obj = new CourseDocument { CourseId = CourseId, DocumentId = item};
                db.CourseDocuments.Add(obj);
                await db.SaveChangesAsync();
            }
            //return PartialView("CourseDocuments");
            return Json(new { success = true });
        }

        public async Task<ActionResult> CourseDeactive(int? id, string flag = null)
        {

            if (id == null || flag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course obj = await db.Courses.FindAsync(id);

            if (obj == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                obj.IsActive = false;
            if (flag == "A")
                obj.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("CourseList");
        }
        #endregion

        #region CourseFees
        public async Task<ActionResult> CourseFeeList()
        {
            var obj = from c in db.Courses
                      join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                      join cf in db.CourseFees on c.CourseId equals cf.CourseId
                      where c.IsActive == true
                      select new CourseFeeListVM { CategoryId = cc.CourseCategoryId,
                        CategoryName = cc.CategoryName,
                        CourseFee = cf.ActualFee,
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        PackageFee = cf.PackageFee,
                        CourseFeeId = cf.CourseFeeId
                        };

            return View(await obj.ToListAsync());
        }

        public ActionResult CourseFeeCreate()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.FeesPattern = new SelectList(db.FeesPatterns.Where(x => x.IsActive == true).ToList(),"FeesPatternId","Name");

            return PartialView("CourseFeeCreate");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseFeeCreate(MastersCourseFeeCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                CourseFee cf = new CourseFee
                {
                    CategoryId = obj.CategoryId,
                    ActualFee = obj.ActualFee,
                    ApplicationFee = obj.ApplicationFee,
                    CourseId = obj.CourseId,
                    FeesPatternId = obj.FeesPatternId,
                    GstPercentage = obj.GstPercentage,
                    MinBalance = obj.MinBalance,
                    PackageFee = obj.PackageFee
                };

                cf.CreatedBy = 1;
                cf.CreatedDate = DateTime.Now;
                cf.ModifiedBy = 1;
                cf.ModifiedDate = DateTime.Now;
                db.CourseFees.Add(cf);

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.FeesPattern = new SelectList(db.FeesPatterns.Where(x => x.IsActive == true).ToList(), "FeesPatternId", "Name");

            return PartialView("CourseFeeCreate", obj);
        }

        public async Task<ActionResult> CourseFeeEdit(int? id)
        {
            CourseFee obj = await db.CourseFees.FindAsync(id);
            var vm = new MastersCourseFeeCreateVM
            {
                CourseId = obj.CourseId,
                CategoryId = obj.CategoryId,
                ActualFee = obj.ActualFee,
                ApplicationFee = obj.ApplicationFee,
                CourseFeesId = obj.CourseFeeId,
                CreatedBy = obj.CreatedBy,
                CreatedDate = obj.CreatedDate,
                FeesPatternId = obj.FeesPatternId,
                GstPercentage = obj.GstPercentage,
                MinBalance = obj.MinBalance,
                PackageFee = obj.PackageFee                
            };

            ViewBag.Categories = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.FeesPattern = new SelectList(db.FeesPatterns.Where(x => x.IsActive == true).ToList(), "FeesPatternId", "Name");
            ViewBag.Course = new SelectList(db.Courses.Where(x => x.IsActive == true).ToList(), "CourseId", "CourseName");
            return PartialView("CourseFeeEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseFeeEdit(MastersCourseFeeCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                CourseFee cf = new CourseFee
                {
                    CourseFeeId = (Int32) obj.CourseFeesId,
                    CategoryId = obj.CategoryId,
                    ActualFee = obj.ActualFee,
                    ApplicationFee = obj.ApplicationFee,
                    CourseId = obj.CourseId,
                    FeesPatternId = obj.FeesPatternId,
                    GstPercentage = obj.GstPercentage,
                    MinBalance = obj.MinBalance,
                    PackageFee = obj.PackageFee,
                    CreatedBy = obj.CreatedBy,
                    CreatedDate = obj.CreatedDate
                };

                
                cf.ModifiedBy = 1;
                cf.ModifiedDate = DateTime.Now;

                db.Entry(cf).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("CourseFeeEdit", obj);
        }

        #endregion

        #region Batches
        public ActionResult BatchList()
        {
            
            ViewBag.Category = new SelectList(db.CourseCategories.ToList(), "CourseCategoryId", "CategoryName");
            var obj = new List<BatchListVM>();
            return View( obj);
        }
        public ActionResult BatchListGet(int? CourseId)
        {
            var obj = from c in db.Courses
                      join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                      join b in db.Batches on c.CourseId equals b.CourseId
                      where c.IsActive == true && c.CourseId == CourseId
                      select new BatchListVM
                      {
                          CourseId = c.CourseId,
                          CourseName = c.CourseName,
                          BatchId = b.BatchId,
                          BatchCode = b.BatchCode,
                          StartDate = b.StartDate,
                          EndDate = b.EndDate,
                          CategoryName = cc.CategoryName,
                          IsActive = b.IsActive,
                          ReserveSeats = b.ReserveSeats,
                          OnlineBookingStatus = b.OnlineBookingStatus,
                      };
                      //}).AsEnumerable().Select(x=> new BatchListVM
                      //      {
                      //          BatchCode = x.BatchCode, BatchId = x.BatchId, CategoryName=x.CategoryName,
                      //          CourseId = x.CourseId, CourseName = x.CourseName, EndDate = x.EndDate,
                      //          IsActive = x.IsActive, OnlineBookingStatus = x.OnlineBookingStatus,
                      //          ReserveSeats = x.ReserveSeats, StartDate = x.StartDate
                      //      });

            return PartialView("BatchListGet", obj.ToList());
        }
        public ActionResult BatchCreate()
        {
            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            //ViewBag.Coordinator = new SelectList(db.Employees.Where(x=>x.IsActive == true).Select(em) ,"EmployeeId","");
            ViewBag.Coordinator = db.Employees.Where(x => x.IsActive == true)
                .Select(p => new SelectListItem
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EmployeeId.ToString()
                });

            return PartialView("BatchCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchCreate(BatchCreateVM obj)
        {
            if (ModelState.IsValid)
            {
                var c = db.Courses.FirstOrDefault(x=>x.CourseId == obj.CourseId);
                var totSeat = c.TotalSeats;
                Batch b = new Batch
                {
                    BatchCode = obj.BatchCode,
                    CategoryId = obj.CategoryId,
                    CoordinatorId = obj.CoordinatorId,
                    CourseId = obj.CourseId,
                    EndDate = obj.EndDate,
                    IsActive = obj.IsActive,
                    OnlineBookingStatus = obj.OnlineBookingStatus,
                    ReserveSeats = obj.ReserveSeats,
                    StartDate = obj.StartDate,
                    TotalSeats = totSeat,
                    BookedSeats = 0
                };

                b.CreatedBy = 1;
                b.CreatedDate = DateTime.Now;
                b.ModifiedBy = 1;
                b.ModifiedDate = DateTime.Now;
                db.Batches.Add(b);

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            ViewBag.FeesPattern = new SelectList(db.FeesPatterns.Where(x => x.IsActive == true).ToList(), "FeesPatternId", "Name");
            ViewBag.Coordinator = db.Employees.Where(x => x.IsActive == true)
                .Select(p => new SelectListItem
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EmployeeId.ToString()
                });

            return PartialView("BatchCreate", obj);
        }

        public async Task<ActionResult> BatchEdit(int? id)
        {
            Batch obj = await db.Batches.FindAsync(id);
            var vm = new BatchEditVM
            {
                CourseId = obj.CourseId,
                CategoryId = obj.CategoryId,
                BatchCode = obj.BatchCode,
                BatchId = obj.BatchId,
                CoordinatorId = obj.CoordinatorId,
                EndDate = obj.EndDate,
                OnlineBookingStatus = obj.OnlineBookingStatus,
                ReserveSeats = obj.ReserveSeats,
                StartDate = obj.StartDate,
                IsActive = obj.IsActive,
                CreatedBy = obj.CreatedBy,
                CreatedDate = obj.CreatedDate,
                CategoryName = db.CourseCategories.Find(obj.CategoryId).CategoryName,
                CourseName = db.Courses.Find(obj.CourseId).CourseName

            };

            ViewBag.Categories = new SelectList(db.CourseCategories.Where(x => x.IsActive == true).ToList(), "CourseCategoryId", "CategoryName");
            
            ViewBag.Coordinator = db.Employees.Where(x => x.IsActive == true)
                .Select(p => new SelectListItem
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EmployeeId.ToString()
                });
            return PartialView("BatchEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchEdit(BatchEditVM obj)
        {
            if (ModelState.IsValid)
            {
                Batch b = await db.Batches.FindAsync(obj.BatchId);
                b.BatchCode = obj.BatchCode;
                b.CoordinatorId = obj.CoordinatorId;
                b.StartDate = obj.StartDate;
                b.EndDate = obj.EndDate;
                b.IsActive = obj.IsActive;
                b.OnlineBookingStatus = obj.OnlineBookingStatus;
                b.ModifiedBy = 1;
                b.ModifiedDate = DateTime.Now;

                db.Entry(b).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("BatchEdit", obj);
        }
        #endregion

        #region CourseCategory
        public async Task<ActionResult> CourseCategoryList()
        {
            return View(await db.CourseCategories.ToListAsync());
        }

        public ActionResult CourseCategoryCreate()
        {
            return PartialView("CourseCategoryCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseCategoryCreate(CourseCategory obj)
        {
            obj.CreatedBy = 1;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedBy = 1;
            obj.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.CourseCategories.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("CourseCategoryCreate", obj);
        }

        public async Task<ActionResult> CourseCategoryEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategory obj = await db.CourseCategories.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("CourseCategoryEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CourseCategoryEdit( CourseCategory obj)
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
            return PartialView("CourseCategoryEdit", obj);
        }

        public async Task<ActionResult> CourseCategoryDeactive(int? id, string flag = null)
        {

            if (id == null || flag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategory obj = await db.CourseCategories.FindAsync(id);

            if (obj == null)
            {
                return HttpNotFound();
            }
            if (flag == "D")
                obj.IsActive = false;
            if (flag == "A")
                obj.IsActive = true;

            db.SaveChanges();
            return RedirectToAction("CourseCategoryList");
        }


        #endregion

        #region Medical
        public async Task<ActionResult> MedicalList()
        {
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "BatchCode");
            return View(await db.MedicalMasters.ToListAsync());
        }

        public ActionResult MedicalCreate()
        {
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "BatchCode");
            return PartialView("MedicalCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MedicalCreate(MedicalMaster obj)
        {
            if (ModelState.IsValid)
            {
                db.MedicalMasters.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("MedicalCreate", obj);
        }

        public async Task<ActionResult> MedicalEdit(int? id)
        {
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "BatchCode");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalMaster obj = await db.MedicalMasters.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("MedicalEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MedicalEdit(MedicalMaster obj)
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
            return PartialView("MedicalEdit", obj);
        }



        #endregion

        #region InterviewMaster
        public async Task<ActionResult> InterviewList()
        {           

            return View(await db.InterviewMasters.ToListAsync());
        }

        public ActionResult InterviewCreate()
        {
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            var count = db.InterviewMasters.Count() + 1;
            InterviewMaster im = new InterviewMaster
            {
                InterviewCode = count.ToString().PadLeft(4, '0')
            };
            return PartialView("InterviewCreate",im);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewCreate(InterviewMaster obj)
        {
            if (ModelState.IsValid)
            {
                db.InterviewMasters.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("InterviewCreate", obj);
        }

        public async Task<ActionResult> InterviewEdit(int? id)
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
            ViewBag.Batches = new SelectList(db.Batches.ToList(), "BatchId", "StartDate");
            var a = from c in db.Courses
                    join cc in db.CourseCategories on c.CategoryId equals cc.CourseCategoryId
                    where (c.IsActive == true && cc.CetRequired == true)
                    select new { c.CourseId, c.CourseName };
            ViewBag.Course = new SelectList(a.ToList(), "CourseId", "CourseName");
            return PartialView("InterviewEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewEdit(InterviewMaster obj)
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
            return PartialView("InterviewEdit", obj);
        }



        #endregion

        #region CertificateFormate
        public async Task<ActionResult> CertificateFormatList()
        {
           
            return View(await db.CertificateFormats.ToListAsync());
        }

        public ActionResult CertificateFormatCreate()
        {
           
            return PartialView("CertificateFormatCreate");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CertificateFormatCreate(CertificateFormat obj)
        {
            if (ModelState.IsValid)
            {
                db.CertificateFormats.Add(obj);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("CertificateFormatCreate", obj);
        }

        public async Task<ActionResult> CertificateFormatEdit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertificateFormat obj = await db.CertificateFormats.FindAsync(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return PartialView("CertificateFormatEdit", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CertificateFormatEdit(CertificateFormat obj)
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
            return PartialView("CertificateFormatEdit", obj);
        }



        #endregion
    }
}