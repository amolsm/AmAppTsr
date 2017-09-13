using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsr.Core.Entities;

namespace Tsr.Infra
{
    public class AppContext : DbContext
    {
        public AppContext() : base("DefaultConnection") { }

        public DbSet<OrganisationMaster> Organisations { get; set; }
        public DbSet<FeesPattern> FeesPatterns { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseDocument> CourseDocuments { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseFee> CourseFees { get; set; }
        public DbSet<Package> packages { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<CetMaster> CetMasters { get; set; }
        public DbSet<OnlinePaymentInfo> OnlinePaymentInfos { get; set; }
        public DbSet<Applied> Applied { get; set; }
        public DbSet<FeeReceipt> FeeReceipts { get; set; }
        public DbSet<StudentFeeDetail> StudentFeeDetails { get; set; }
        public DbSet<CetMark> CetMarks { get; set; }
        public DbSet<CetInterview> CetInterviews { get; set; }
        public DbSet<InterviewMaster> InterviewMasters { get; set; }
        public DbSet<MedicalMaster> MedicalMasters { get; set; }
        public DbSet<CetMedical> CetMedicals { get; set; }

        public DbSet<CertificateDesign> CertificateDesigns { get; set; }

        public DbSet<Principal> Principals { get; set; }
        public DbSet<PackageCourse> PackageCourses { get; set; }
        public DbSet<ApplicationPackageDetail> ApplicationPackageDetails { get; set; }
    }
}
