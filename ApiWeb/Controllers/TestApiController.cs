using ApiWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace ApiWeb.Controllers
{
    /// <summary>
    /// 接口提供方-都提供了根据年龄获取对应学生信息的接口
    /// </summary>
    public class TestApiController : BaseController
    {
        /// <summary>
        /// 初始化测试数据
        /// </summary>
        private List<stu> stulist = new List<stu>() { 
                new stu() { Name = "joy", Age = 19 }, 
                new stu() { Name = "steve", Age = 29 }, 
                new stu() { Name = "james", Age = 39 }, 
                new stu() { Name = "rio", Age = 49 }, 
                new stu() { Name = "tom", Age = 79 }, 
                new stu() { Name = "jerry", Age = 20 },
                new stu() { Name = "jim", Age = 21 },
                new stu() { Name = "jack", Age = 22 },
                new stu() { Name = "jeef", Age = 23 },
                new stu() { Name = "thomas", Age = 24 },
                new stu() { Name = "martin", Age = 20 },
                new stu() { Name = "bill", Age = 25 }
            };

        /// <summary>
        /// 接口对外公开
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("NoSecure")]
        public HttpResponseMessage NoSecure(int age)
        {
            var result = new ResultModel<object>()
            {
                ReturnCode = 0,
                Message = string.Empty,
                Result = string.Empty
            };

            var dataResult = stulist.Where(T => T.Age == age).ToList();
            result.Result = dataResult;

            return GetHttpResponseMessage(result);
        }

        /// <summary>
        /// 接口加密
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SecureBySign")]
        public HttpResponseMessage SecureBySign([FromUri]int age, long _timestamp, string appKey, string _sign)
        {
            var result = new ResultModel<object>()
            {
                ReturnCode = 0,
                Message = string.Empty,
                Result = string.Empty
            };

            #region 校验签名是否合法
            var param = new SortedDictionary<string, string>(new AsciiComparer());
            param.Add("age", age.ToString());
            param.Add("appKey", appKey);
            param.Add("_timestamp", _timestamp.ToString());

            string currentSign = SignHelper.GetSign(param, appKey);

            if (_sign != currentSign)
            {
                result.ReturnCode = -2;
                result.Message = "签名不合法";
                return GetHttpResponseMessage(result);
            }
            #endregion

            var dataResult = stulist.Where(T => T.Age == age).ToList();
            result.Result = dataResult;

            return GetHttpResponseMessage(result);
        }

        /// <summary>
        /// 接口加密并根据时间戳判断有效性
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SecureBySign/Expired")]
        public HttpResponseMessage SecureBySign_Expired([FromUri]int age, long _timestamp, string appKey, string _sign)
        {
            var result = new ResultModel<object>()
            {
                ReturnCode = 0,
                Message = string.Empty,
                Result = string.Empty
            };

            #region 判断请求是否过期---假设过期时间是20秒
            DateTime requestTime = GetDateTimeByTicks(_timestamp);
            
            if (requestTime.AddSeconds(20) < DateTime.Now)
            {
                result.ReturnCode = -1;
                result.Message = "接口过期";
                return GetHttpResponseMessage(result);
            }
            #endregion

            #region 校验签名是否合法
            var param = new SortedDictionary<string, string>(new AsciiComparer());
            param.Add("age", age.ToString());
            param.Add("appKey", appKey);
            param.Add("_timestamp", _timestamp.ToString());

            string currentSign = SignHelper.GetSign(param, appKey);

            if (_sign != currentSign)
            {
                result.ReturnCode = -2;
                result.Message = "签名不合法";
                return GetHttpResponseMessage(result);
            }
            #endregion

            var dataResult = stulist.Where(T => T.Age == age).ToList();
            result.Result = dataResult;

            return GetHttpResponseMessage(result);
        }

        /// <summary>
        /// 接口加密并根据时间戳判断有效性而且带着私有key校验
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SecureBySign/Expired/KeySecret")]
        public HttpResponseMessage SecureBySign_Expired_KeySecret([FromUri]int age, long _timestamp, string appKey, string _sign)
        {
            //key集合，这里随便弄两个测试数据
            //如果调用方比较多，需要审核授权，根据一定的规则生成key把这些数据存放在数据库中，如果功能扩展开来，可以针对不同的调用方做不同的功能权限管理
            //在调用接口时动态从库里取，每个调用方在调用时带上他的key，调用方一般把自己的key放到网站配置中
            Dictionary<string, string> keySecretDic = new Dictionary<string, string>();
            keySecretDic.Add("key_zhangsan", "D9U7YY5D7FF2748AED89E90HJ88881E6");//张三的key,
            keySecretDic.Add("key_lisi", "I9O6ZZ3D7FF2748AED89E90ZB7732M9");//李四的key

            var result = new ResultModel<object>()
            {
                ReturnCode = 0,
                Message = string.Empty,
                Result = string.Empty
            };

            #region 判断请求是否过期---假设过期时间是20秒
            DateTime requestTime = GetDateTimeByTicks(_timestamp);

            if (requestTime.AddSeconds(20) < DateTime.Now)
            {
                result.ReturnCode = -1;
                result.Message = "接口过期";
                return GetHttpResponseMessage(result);
            }
            #endregion

            #region 根据appkey获取key值
            string secret = keySecretDic.Where(T => T.Key == appKey).FirstOrDefault().Value;
            #endregion

            #region 校验签名是否合法
            var param = new SortedDictionary<string, string>(new AsciiComparer());
            param.Add("age", age.ToString());
            param.Add("appKey", appKey);

            param.Add("appSecret", secret);//把secret加入进行加密

            param.Add("_timestamp", _timestamp.ToString());

            string currentSign = SignHelper.GetSign(param, appKey);

            if (_sign != currentSign)
            {
                result.ReturnCode = -2;
                result.Message = "签名不合法";
                return GetHttpResponseMessage(result);
            }
            #endregion

            var dataResult = stulist.Where(T => T.Age == age).ToList();
            result.Result = dataResult;

            return GetHttpResponseMessage(result);
        }


        #region 辅助方法

        /// <summary>
        /// 时间戳转换为时间
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private DateTime GetDateTimeByTicks(long ticks)
        {
            return new DateTime(ticks);
        }

        #endregion
    }


    /// <summary>
    /// 测试数据类
    /// </summary>
    class stu
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
