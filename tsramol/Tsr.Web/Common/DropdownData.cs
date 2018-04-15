using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tsr.Web.Common
{
    public class DropdownData
    {
        public static SelectList CourseUnits()
        {
            return
                new SelectList(new List<SelectListItem>
            {
               new SelectListItem { Value = "Days", Text= "Days"},
                new SelectListItem {Value = "Weeks", Text="Weeks" },
                new SelectListItem {Value = "Months", Text="Months" },
                new SelectListItem {Value = "Year" , Text = "Years"}

            }, "Value", "Text");
        }
        public static SelectList CourseType()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "False", Text= "Single Course"},
                new SelectListItem {Value = "True", Text="Package Courses" }

            }, "Value", "Text");
        }
        public static SelectList YesNo()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "True", Text= "Yes"},
                new SelectListItem {Value = "False", Text="No" }

            }, "Value", "Text");
        }
        public static SelectList Gender()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Male", Text= "Male"},
                new SelectListItem {Value = "Female", Text="Female" },
                new SelectListItem {Value = "Transgender", Text="Transgender" }

            }, "Value", "Text");
        }
        public static SelectList ShirtSize()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text= "M"},
                new SelectListItem {Value = "L", Text="L" },
                new SelectListItem {Value = "XL", Text="XL" },
                new SelectListItem {Value = "XXL", Text="XXL" }

            }, "Value", "Text");
        }
        public static SelectList PantSize()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "27", Text= "27"},
                new SelectListItem {Value = "28", Text="28" },
                new SelectListItem {Value = "29", Text="29" },
                new SelectListItem {Value = "30", Text="30" },
                new SelectListItem { Value = "31", Text= "31"},
                new SelectListItem {Value = "32", Text="32" },
                new SelectListItem {Value = "33", Text="33" },
                new SelectListItem {Value = "34", Text="34" },
                new SelectListItem { Value = "35", Text= "35"},
                new SelectListItem {Value = "36", Text="36" },
                new SelectListItem {Value = "37", Text="37" },
                new SelectListItem {Value = "38", Text="38" },
                new SelectListItem {Value = "39", Text="39" },
                new SelectListItem {Value = "40", Text="40" }

            }, "Value", "Text");
        }
        public static SelectList ShoeSize()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "5", Text= "5"},
                new SelectListItem {Value = "6", Text="6" },
                new SelectListItem {Value = "7", Text="7" },
                new SelectListItem {Value = "8", Text="8" },
                new SelectListItem {Value = "9", Text="9" },
                new SelectListItem {Value = "10", Text="10" },
                new SelectListItem {Value = "11", Text="11" }

            }, "Value", "Text");
        }
        public static SelectList Meals()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Veg", Text= "Veg"},
                new SelectListItem {Value = "Non-Veg", Text="Non-Veg" }

            }, "Value", "Text");
        }

        public static SelectList PaymentMode()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Online", Text= "Online"},
                new SelectListItem {Value = "Cash", Text="Cash" },
                new SelectListItem {Value = "Card", Text="Card" },
                new SelectListItem {Value = "DD", Text="DD" },
                new SelectListItem {Value = "Cheque", Text="Cheque" }
                
            }, "Value", "Text");
        }

        public static SelectList PaymentLocation()
        {
            return
                new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Nhava", Text= "Nhava"},
                new SelectListItem {Value = "Worli", Text="Worli" },
               
            }, "Value", "Text");
        }
        public class BatchDropdownlist
        {
            public int BatchId { get; set; }
            public string BatchCode { get; set; }
        }

        public static SelectList PaymentModeList()
        {
            return
                new SelectList(new List<SelectListItem>
            {   new SelectListItem { Value = "All", Text= "All"},
                new SelectListItem { Value = "Online", Text= "Online"},
                new SelectListItem {Value = "Cash", Text="Cash" },
                new SelectListItem {Value = "Card", Text="Card" },
                new SelectListItem {Value = "DD", Text="DD" },
                new SelectListItem {Value = "Cheque", Text="Cheque" }
            }, "Value", "Text");
        }

    }
    public class BatchDropdownlist
    {
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
    }

  

}