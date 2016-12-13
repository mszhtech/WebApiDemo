using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ApiWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }

        protected void Application_Error(object sender, EventArgs e)
        {

            Exception ex = System.Web.HttpContext.Current.Server.GetLastError().GetBaseException();
            string message = ex.ToString() + "\r\n";
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                message += System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            }
        }
    }
}
