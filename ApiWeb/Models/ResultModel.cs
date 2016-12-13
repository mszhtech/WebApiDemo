using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiWeb.Models
{
    /// <summary>
    /// 统一接口Json返回结果格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultModel<T> where T : class
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int ReturnCode { get; set; }

        /// <summary>
        /// 附加消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public T Result { get; set; }
    }
}