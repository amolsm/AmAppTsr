using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsr.Web.Common
{
    public class RemotePost
    {
        private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        public string Url = "https://test.payu.in/_payment";
        public string Method = "post";
        public string FormName = "form1";
        public void Add(string name, string value)
        {
            Inputs.Add(name, value);
        }
        public void Post()
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Write("");
            System.Web.HttpContext.Current.Response.Write(string.Format("", FormName));
            System.Web.HttpContext.Current.Response.Write(string.Format("", FormName, Method, Url));
        for (int i = 0; i < Inputs.Keys.Count; i++)
            {
                System.Web.HttpContext.Current.Response.Write(string.Format("", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
            }
            System.Web.HttpContext.Current.Response.Write("");
            System.Web.HttpContext.Current.Response.Write("");
            System.Web.HttpContext.Current.Response.End();
        }
    }
}