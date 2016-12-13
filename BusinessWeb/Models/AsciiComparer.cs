using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessWeb.Models
{
    /// <summary>
    /// 基于ASCII码排序规则的String比较器
    /// </summary>
    public class AsciiComparer : System.Collections.Generic.IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a == b)
                return 0;
            else if (string.IsNullOrEmpty(a))
                return -1;
            else if (string.IsNullOrEmpty(b))
                return 1;
            if (a.Length <= b.Length)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] < b[i])
                        return -1;
                    else if (a[i] > b[i])
                        return 1;
                    else
                        continue;
                }
                return a.Length == b.Length ? 0 : -1;
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                {
                    if (a[i] < b[i])
                        return -1;
                    else if (a[i] > b[i])
                        return 1;
                    else
                        continue;
                }
                return 1;
            }
        }
    }
}