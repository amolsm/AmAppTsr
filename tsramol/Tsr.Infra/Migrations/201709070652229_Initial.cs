namespace Tsr.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
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
                "dbo.CetMedicals",
                c => new
                    {
                        CetMedicalId = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        BatchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CetMedicalId);
            
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
            
            DropTable("dbo.CertificateDesigns");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CertificateDesigns",
                c => new
                    {
                        CD_Id = c.Int(nullable: false, identity: true),
                        TopicBeforeCourseTitle = c.String(),
                        CourseId = c.Int(nullable: false),
                        CourseTitle = c.String(),
                        Topic1 = c.String(),
                        Topic2 = c.String(),
                        Topic3 = c.String(),
                        Topic4 = c.String(),
                        CourseIncharge = c.String(),
                        PricipalId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CD_Id);
            
            DropTable("dbo.MedicalMasters");
            DropTable("dbo.InterviewMasters");
            DropTable("dbo.CetMedicals");
            DropTable("dbo.CetInterviews");
        }
    }
}
