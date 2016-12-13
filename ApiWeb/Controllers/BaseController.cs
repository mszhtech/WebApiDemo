using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ApiWeb.Controllers
{
    public class BaseController : ApiController
    {
        public HttpResponseMessage GetHttpResponseMessage(object result)
        {
            return new HttpResponseMessage()
            {
                Content =
                   new StringContent(JsonConvert.SerializeObject(result), System.Text.Encoding.UTF8,
                       "application/json")
            };
        }
	}
}