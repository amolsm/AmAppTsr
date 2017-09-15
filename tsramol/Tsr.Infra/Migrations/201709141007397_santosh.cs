namespace Tsr.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class santosh : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationPackageDetails",
                c => new
                    {
                        ApplicationPackageDetailId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        PackageId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        BatchId = c.Int(nullable: false),
                        ConfirmStatus = c.Boolean(),
                    })
                .PrimaryKey(t => t.ApplicationPackageDetailId);
            
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        ApplicationId = c.Int(nullable: false, identity: true),
                        ApplicationCode = c.String(),
                        CategoryId = c.Int(),
                        CourseId = c.Int(),
                        BatchId = c.Int(),
                        IsPackage = c.Boolean(),
                        PackageId = c.Int(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        MotherName = c.String(),
                        DateOfBirth = c.DateTime(),
                        PlaceOfBirth = c.String(),
                        Gender = c.String(),
                        Citizenship = c.String(),
                        Caste = c.String(),
                        Religion = c.String(),
                        Height = c.Single(),
                        Weight = c.Single(),
                        Email = c.String(),
                        CellNo = c.String(),
                        FatherOccupation = c.String(),
                        AnnualIncome = c.String(),
                        IdentificationMark = c.String(),
                        PassportNo = c.String(),
                        ShirtSize = c.String(),
                        PantSize = c.String(),
                        ShoeSize = c.String(),
                        PreferredMeal = c.String(),
                        PermenentAddress = c.String(),
                        PermenentCity = c.String(),
                        PermenentState = c.String(),
                        PermenentPin = c.String(),
                        PermenentContactNo = c.String(),
                        FatherEmail = c.String(),
                        PresentAddress = c.String(),
                        PresentCity = c.String(),
                        PresentState = c.String(),
                        PresentPin = c.String(),
                        PresentContactNo = c.String(),
                        GuardianName = c.String(),
                        GuardianRelation = c.String(),
                        GuardianAddress = c.String(),
                        GuardianCity = c.String(),
                        GuardianState = c.String(),
                        GuardianPin = c.String(),
                        GuardianContact = c.String(),
                        GuardianEmail = c.String(),
                        SchoolName = c.String(),
                        SchoolBoard = c.String(),
                        SchoolAddress = c.String(),
                        SchoolCity = c.String(),
                        SchoolState = c.String(),
                        SchoolPin = c.String(),
                        SchoolPassingYear = c.String(),
                        SchoolPercentage = c.String(),
                        SchoolMath = c.Single(),
                        SchoolScience = c.Single(),
                        SchoolEnglish = c.Single(),
                        InterSchoolName = c.String(),
                        InterBoard = c.String(),
                        InterAddress = c.String(),
                        InterCity = c.String(),
                        InterState = c.String(),
                        InterPin = c.String(),
                        InterPassingYear = c.String(),
                        InterPercentage = c.String(),
                        InterMath = c.Single(),
                        InterPhysics = c.Single(),
                        InterChemistry = c.Single(),
                        InterRollNo = c.String(),
                        GradCollegeName = c.String(),
                        GradUniversity = c.String(),
                        GradAddress = c.String(),
                        GradCity = c.String(),
                        GradState = c.String(),
                        GradPin = c.String(),
                        GradPassingYear = c.String(),
                        GradPercentage = c.String(),
                        GradSubjects = c.String(),
                        GradPassAttempt = c.Boolean(),
                        CertOfCompetencyNo = c.String(),
                        CdcNo = c.String(),
                        InDosNo = c.String(),
                        GradeOfCompetencyNo = c.String(),
                        CategoryOfCandidate = c.String(),
                        ShippingCompany = c.String(),
                        RankOfCandidate = c.String(),
                        CourseAttendedInTSR = c.Boolean(),
                        FPFF_AFF_1995 = c.String(),
                    })
                .PrimaryKey(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Applieds",
                c => new
                    {
                        AppliedId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        BatchId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        AdmissionStatus = c.Boolean(),
                    })
                .PrimaryKey(t => t.AppliedId);
            
            CreateTable(
                "dbo.Batches",
                c => new
                    {
                        BatchId = c.Int(nullable: false, identity: true),
                        BatchCode = c.String(),
                        CategoryId = c.Int(),
                        CourseId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CoordinatorId = c.Int(),
                        ReserveSeats = c.Int(),
                        BookedSeats = c.Int(),
                        TotalSeats = c.Int(),
                        IsActive = c.Boolean(),
                        OnlineBookingStatus = c.Boolean(),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.BatchId);
            
            CreateTable(
                "dbo.CertificateDesigns",
                c => new
                    {
                        CertificateDesignId = c.Int(nullable: false, identity: true),
                        LineOfCertificate = c.String(),
                        CourseId = c.Int(nullable: false),
                        CourseName = c.String(),
                        Paragraph1 = c.String(),
                        Paragraph2 = c.String(),
                        Paragraph3 = c.String(),
                        Topic4 = c.String(),
                        PrincipalId = c.Int(nullable: false),
                        CertificateFormatId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CertificateDesignId);
            
            CreateTable(
                "dbo.CertificateFormats",
                c => new
                    {
                        CertificateFormatId = c.Int(nullable: false, identity: true),
                        FormatName = c.String(),
                        ActionName = c.String(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.CertificateFormatId);
            
            CreateTable(
                "dbo.CetInterviews",
                c => new
                    {
                        CetInterviewId = c.Int(nullable: false, identity: true),
                        InterviewMasterId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        BatchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CetInterviewId);
            
            CreateTable(
                "dbo.CetMarks",
                c => new
                    {
                        CetMarkId = c.Int(nullable: false, identity: true),
                        CetMasterId = c.Int(nullable: false),
                        BatchId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        Marks1 = c.Single(nullable: false),
                        Marks2 = c.Single(nullable: false),
                        Marks3 = c.Single(nullable: false),
                        Marks4 = c.Single(nullable: false),
                        Total = c.Single(nullable: false),
                        SelectStatus = c.Boolean(),
                    })
                .PrimaryKey(t => t.CetMarkId);
            
            CreateTable(
                "dbo.CetMasters",
                c => new
                    {
                        CetMasterId = c.Int(nullable: false, identity: true),
                        CetCode = c.String(),
                        CourseId = c.Int(),
                        BatchId = c.Int(),
                        StartDate = c.DateTime(),
                        CetDate = c.DateTime(),
                        CetTime = c.Time(precision: 7),
                        Venue = c.String(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.CetMasterId);
            
            CreateTable(
                "dbo.CetMedicals",
                c => new
                    {
                        CetMedicalId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        BatchId = c.Int(nullable: false),
                        MedicalMasterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CetMedicalId);
            
            CreateTable(
                "dbo.CourseCategory",
                c => new
                    {
                        CourseCategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 50),
                        HostelCompulsion = c.Boolean(),
                        IsActive = c.Boolean(),
                        CetRequired = c.Boolean(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CourseCategoryId);
            
            CreateTable(
                "dbo.CourseDocuments",
                c => new
                    {
                        CourseDocumentId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseDocumentId);
            
            CreateTable(
                "dbo.CourseFees",
                c => new
                    {
                        CourseFeeId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(),
                        CourseId = c.Int(),
                        FeesPatternId = c.Int(),
                        ActualFee = c.Decimal(storeType: "money"),
                        PackageFee = c.Decimal(storeType: "money"),
                        ApplicationFee = c.Decimal(storeType: "money"),
                        MinBalance = c.Decimal(precision: 18, scale: 2),
                        GstPercentage = c.Double(),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CourseFeeId);
            
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseCode = c.String(maxLength: 50),
                        CourseName = c.String(nullable: false, maxLength: 150),
                        CategoryId = c.Int(),
                        Duration = c.Int(),
                        Unit = c.String(maxLength: 10),
                        TotalSeats = c.Int(),
                        MinAge = c.Double(),
                        MaxAge = c.Double(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CourseId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 50),
                        PhoneExtension = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 50),
                        DepartmentHead = c.Int(nullable: false),
                        DepHeadCell = c.String(maxLength: 50),
                        Type = c.String(maxLength: 50),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Designation",
                c => new
                    {
                        DesignationId = c.Int(nullable: false, identity: true),
                        DesignationName = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.DesignationId);
            
            CreateTable(
                "dbo.Document",
                c => new
                    {
                        DocumentsListId = c.Int(nullable: false, identity: true),
                        DocumentName = c.String(nullable: false, maxLength: 50),
                        DocumentType = c.String(maxLength: 50),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.DocumentsListId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(),
                        FirstName = c.String(nullable: false),
                        MiddleName = c.String(),
                        LastName = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DesignationId = c.Int(nullable: false),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.FeeReceipts",
                c => new
                    {
                        FeeReceiptId = c.Int(nullable: false, identity: true),
                        FeeReceiptNo = c.String(),
                        ApplicationId = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        PaymentMode = c.String(),
                        FeesType = c.String(),
                        ReceiptDate = c.DateTime(),
                        PrintStatus = c.Boolean(),
                        ChequeNo = c.String(),
                        DDNo = c.String(),
                    })
                .PrimaryKey(t => t.FeeReceiptId);
            
            CreateTable(
                "dbo.FeesPattern",
                c => new
                    {
                        FeesPatternId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.FeesPatternId);
            
            CreateTable(
                "dbo.InterviewMasters",
                c => new
                    {
                        InterviewMasterId = c.Int(nullable: false, identity: true),
                        InterviewCode = c.String(),
                        BatchId = c.Int(nullable: false),
                        InterviewDate = c.DateTime(),
                        InterviewTime = c.Time(precision: 7),
                        Venue = c.String(),
                    })
                .PrimaryKey(t => t.InterviewMasterId);
            
            CreateTable(
                "dbo.MedicalMasters",
                c => new
                    {
                        MedicalMasterId = c.Int(nullable: false, identity: true),
                        MedicalCode = c.String(),
                        BatchId = c.Int(nullable: false),
                        MedicalDate = c.DateTime(),
                        MedicalFees = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.MedicalMasterId);
            
            CreateTable(
                "dbo.OnlinePaymentInfoes",
                c => new
                    {
                        OnlinePaymentInfoId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(),
                        BatchId = c.Int(),
                        CourseId = c.Int(),
                        CategoryId = c.Int(),
                        PackageId = c.Int(),
                        IsPackage = c.Boolean(),
                        mihpayid = c.String(),
                        mode = c.String(),
                        status = c.String(),
                        key = c.String(),
                        txnid = c.String(),
                        amount = c.String(),
                        Productinfo = c.String(),
                        Hash = c.String(),
                        bank_ref_num = c.String(),
                        PaymentDate = c.DateTime(),
                        udf1 = c.String(),
                        udf2 = c.String(),
                        udf3 = c.String(),
                        udf4 = c.String(),
                        udf5 = c.String(),
                    })
                .PrimaryKey(t => t.OnlinePaymentInfoId);
            
            CreateTable(
                "dbo.OrganisationMaster",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Address = c.String(maxLength: 250),
                        City = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        Website = c.String(maxLength: 50),
                        TagLine = c.String(maxLength: 50),
                        RegisteredNo = c.String(maxLength: 50),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.OrganisationId);
            
            CreateTable(
                "dbo.PackageCourses",
                c => new
                    {
                        PackageCourseId = c.Int(nullable: false, identity: true),
                        PackageId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PackageCourseId);
            
            CreateTable(
                "dbo.Package",
                c => new
                    {
                        PackageId = c.Int(nullable: false, identity: true),
                        PackageName = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 250),
                        IsActive = c.Boolean(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PackageId);
            
            CreateTable(
                "dbo.Principals",
                c => new
                    {
                        PrincipalId = c.Int(nullable: false, identity: true),
                        PricipalName = c.String(),
                        SignatureImgUrl = c.String(),
                    })
                .PrimaryKey(t => t.PrincipalId);
            
            CreateTable(
                "dbo.StudentFeeDetails",
                c => new
                    {
                        StudentFeeDetailId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(),
                        PackageId = c.Int(),
                        BatchId = c.Int(),
                        TotalFee = c.Decimal(precision: 18, scale: 2),
                        FeePaid = c.Decimal(precision: 18, scale: 2),
                        FeeBal = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.StudentFeeDetailId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StudentFeeDetails");
            DropTable("dbo.Principals");
            DropTable("dbo.Package");
            DropTable("dbo.PackageCourses");
            DropTable("dbo.OrganisationMaster");
            DropTable("dbo.OnlinePaymentInfoes");
            DropTable("dbo.MedicalMasters");
            DropTable("dbo.InterviewMasters");
            DropTable("dbo.FeesPattern");
            DropTable("dbo.FeeReceipts");
            DropTable("dbo.Employees");
            DropTable("dbo.Document");
            DropTable("dbo.Designation");
            DropTable("dbo.Departments");
            DropTable("dbo.Course");
            DropTable("dbo.CourseFees");
            DropTable("dbo.CourseDocuments");
            DropTable("dbo.CourseCategory");
            DropTable("dbo.CetMedicals");
            DropTable("dbo.CetMasters");
            DropTable("dbo.CetMarks");
            DropTable("dbo.CetInterviews");
            DropTable("dbo.CertificateFormats");
            DropTable("dbo.CertificateDesigns");
            DropTable("dbo.Batches");
            DropTable("dbo.Applieds");
            DropTable("dbo.Applications");
            DropTable("dbo.ApplicationPackageDetails");
        }
    }
}
