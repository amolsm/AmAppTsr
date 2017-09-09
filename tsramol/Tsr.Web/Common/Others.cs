using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsr.Web.Common
{
    public static class Others
    {
        public static string GetFullName(string FirstName,string MiddleName,string LastName)
        {
            return FirstName==null?"": FirstName + ""+ MiddleName == null ? "" : MiddleName + "" + LastName == null ? "" : LastName;
        }
    }
}