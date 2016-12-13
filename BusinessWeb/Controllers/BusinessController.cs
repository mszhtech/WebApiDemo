using BusinessWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace BusinessWeb.Controllers
{
    /// <summary>
    /// 接口调用方
    /// </summary>
    public class BusinessController : Controller
    {
        private static string mykey = "key_zhangsan";
        private static string mysecret = System.Configuration.ConfigurationManager.AppSettings[mykey];
        private static string siteHost = System.Configuration.ConfigurationManager.AppSettings["siteHost"];

        /// <summary>
        /// 请求公开接口
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestNoSecure(int age)
        {
            string responseString = HttpGet(siteHost + "NoSecure", "age=" + age);

            return Content(responseString);
        }

        /// <summary>
        /// 请求加密接口
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestSecureBySign(int age)
        {
            long timeStamp = DateTime.Now.Ticks;

            var param = new SortedDictionary<string, string>(new AsciiComparer());
            param.Add("age", age.ToString());
            param.Add("appKey", mykey);
            param.Add("_timestamp", timeStamp.ToString());


            string _sign = SignHelper.GetSign(param, mykey);

            string responseString = HttpGet(siteHost + "SecureBySign", "age=" + age + "&_timestamp=" + timeStamp.ToString() + "&appKey=" + mykey + "&_sign=" + _sign);

            return Content(responseString);
        }

        /// <summary>
        /// 请求加密并根据时间戳判断有效性的接口
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestSecureBySign_Expired(int age)
        {
            long timeStamp = DateTime.Now.Ticks;

            var param = new SortedDictionary<string, string>(new AsciiComparer());
            param.Add("age", age.ToString());
            param.Add("appKey", mykey);
            param.Add("_timestamp", timeStamp.ToString());


            string _sign = SignHelper.GetSign(param, mykey);

            string responseString = HttpGet(siteHost + "SecureBySign/Expired", "age=" + age + "&_timestamp=" + timeStamp.ToString() + "&appKey=" + mykey + "&_sign=" + _sign);

            return Content(responseString);
        }

        /// <summary>
        /// 请求加密并根据时间戳判断有效性的接口
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestSecureBySign_Expired_KeySecret(int age)
        {
            long timeStamp = DateTime.Now.Ticks;

            var param = new SortedDictionary<string, string>(new AsciiComparer());
            param.Add("age", age.ToString());
            param.Add("appKey", mykey);
            param.Add("appSecret", mysecret);//把secret加入进行加密
            param.Add("_timestamp", timeStamp.ToString());

            string _sign = SignHelper.GetSign(param, mykey);

            string responseString = HttpGet(siteHost + "SecureBySign/Expired/KeySecret", "age=" + age + "&_timestamp=" + timeStamp.ToString() + "&appKey=" + mykey + "&_sign=" + _sign);

            return Content(responseString);
        }


        #region 辅助方法
        /// <summary>
        ///  创建返回实例
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        protected ActionResult JsonResult(object obj)
        {
            var formatter = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var content = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Formatting.Indented, formatter);
            return Content(content, "application/json", System.Text.UTF8Encoding.UTF8);
        }


        /// <summary>
        /// body是要传递的参数,格式"roleId=1&uid=2"
        /// post的cotentType填写:"application/x-www-form-urlencoded"
        /// soap填写:"text/xml; charset=utf-8"
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string body)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 20000;

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }


        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="Url">地址</param>
        /// <param name="postDataStr">参数：格式"roleId=1&uid=2"</param>
        /// <returns></returns>
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion
    }
}