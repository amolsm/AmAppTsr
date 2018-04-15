using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsr.Core.Entities;
using Tsr.Core.Models;

namespace Tsr.Infra
{
    public class Valid
    {
        public static int IsApplicationRepeat(ValidApplication obj)
        {
            int? app = 0;
            AppContext db = new AppContext();
            if (obj.PackageId == 0)
            {
                try
                {
                    app = db.Applications.FirstOrDefault(x => x.BatchId == obj.BatchId &&
                            x.CourseId == obj.CourseId && x.Email == obj.Email &&
                            x.CellNo == obj.CellNo && x.DateOfBirth == obj.DateOfBirth).ApplicationId;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                try
                {
                    app = db.Applications.FirstOrDefault(x => x.PackageId == obj.PackageId
                            && x.Email == obj.Email && x.CellNo == obj.CellNo && 
                            x.DateOfBirth == obj.DateOfBirth).ApplicationId;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            if (app != null)
                return (int)app;
            else
                return 0;
        }

        public static string CreateRecieptNo(string loc, string mode, string fy)
        {
            AppContext db = new AppContext();
            string rno = string.Empty;
            if (loc == "Nhava")
                rno = "Nh";
            else
                rno = "Wr";
            if (mode == "Online")
                rno = rno + "/Onl";
            else if(mode == "Cash")
                rno = rno + "/Csh";
            else if (mode == "Card")
                rno = rno + "/Crd";
            else if (mode == "DD")
                rno = rno + "/Ddn";
            else if (mode == "Cheque")
                rno = rno + "/Chq";

            var d = DateTime.Now.Month >= 4 ? DateTime.Now.Year  : DateTime.Now.Year-1;
           // var a = db.FeeReceipts.Where(x => x.PaymentMode == mode && x.Location == loc && x.Fy == fy).Count();
            var ds = DateTime.Now.Month >= 4 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            int z = Convert.ToInt32(ds.ToString().Substring(2));

            rno = rno + "/" + d.ToString() + "-" + z.ToString();

            int cnt = 0;
            try
            {
                cnt = db.FeeReceipts.Where(x => x.PaymentMode == mode && x.Location == loc && x.Fy == fy).Count();
            }
            catch (Exception)
            {
                cnt = 0;
            }
            cnt++;
            rno = rno + "/" + cnt.ToString().PadLeft(4, '0');
            return rno;
        }
    }
}
