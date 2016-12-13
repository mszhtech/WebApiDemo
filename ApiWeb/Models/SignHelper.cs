using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ApiWeb.Models
{
    public class SignHelper
    {
        public static string GetSign(SortedDictionary<string, string> paramList, string appKey = "")
        {
            paramList.Remove("_sign");
            StringBuilder sb = new StringBuilder(appKey);
            foreach (var p in paramList)
                sb.Append(p.Key).Append(p.Value);
            sb.Append(appKey);
            return GetMD5(sb.ToString());
        }
        public static string GetMD5(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            var sb = new StringBuilder(32);
            var md5 = System.Security.Cryptography.MD5.Create();
            var output = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            for (int i = 0; i < output.Length; i++)
                sb.Append(output[i].ToString("X").PadLeft(2, '0'));
            return sb.ToString();
        }
    }
}