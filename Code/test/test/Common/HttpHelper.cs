using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace test.Common
{
    public class HttpHelper
    {
        private static readonly object LockObj = new object();
        private static string _baseAddress;
        private static string _authorization;
        private static readonly int _timeout = 30 * 60 * 1000; // 30分钟超时(毫秒)

        public static string BaseAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_baseAddress))
                {
                    _baseAddress = ConfigurationManager.AppSettings["ApiAddress"];
                }
                return _baseAddress;
            }
        }

        public HttpHelper()
        {
            InitializeAuthorization();
        }

        private void InitializeAuthorization()
        {
            if (!string.IsNullOrEmpty(GlobalParameter.Authorization) &&
                _authorization != GlobalParameter.Authorization)
            {
                _authorization = GlobalParameter.Authorization;
            }
        }

        public string GetQueryString(string url, Dictionary<string, string> dic)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic != null && dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    // 对参数值进行URL编码
                    builder.AppendFormat("{0}={1}", item.Key, Uri.EscapeDataString(item.Value));
                    i++;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 创建HttpWebRequest实例
        /// </summary>
        private HttpWebRequest CreateRequest(string url, string method)
        {
            InitializeAuthorization();

            // 处理基础地址
            Uri requestUri;
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                requestUri = new Uri(url);
            }
            else
            {
                requestUri = new Uri(new Uri(BaseAddress), url);
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = method;
            request.Timeout = _timeout;
            request.ContentType = "application/json";

            // 添加授权头
            if (!string.IsNullOrEmpty(_authorization))
            {
                request.Headers.Add("Authorization", _authorization);
            }

            return request;
        }

        /// <summary>
        /// 同步Post请求
        /// </summary>
        public string Post(string url, string strJson)
        {
            try
            {
                HttpWebRequest request = CreateRequest(url, "POST");

                // 写入请求数据
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(strJson);
                }

                // 获取响应
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                // 处理HTTP错误响应
                if (ex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                    {
                        XtraMessageBox.Show($"HTTP错误: {errorResponse.StatusCode}");
                    }
                }
                else
                {
                    XtraMessageBox.Show($"请求错误: {ex.Message}");
                }
                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"发生错误: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 泛型同步Post请求
        /// </summary>
        public TResult Post<TResult>(string url, object data)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(data);
                string result = Post(url, jsonData);

                if (string.IsNullOrEmpty(result))
                {
                    return default(TResult);
                }

                var dto = JsonConvert.DeserializeObject<ResultDto<TResult>>(result);
                return dto != null ? dto.Data : default(TResult);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Post请求异常: {ex.Message}");
                return default(TResult);
            }
        }

        /// <summary>
        /// 同步Get请求
        /// </summary>
        public string Get(string url)
        {
            try
            {
                HttpWebRequest request = CreateRequest(url, "GET");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                    {
                        XtraMessageBox.Show($"HTTP错误: {errorResponse.StatusCode}");
                    }
                }
                else
                {
                    XtraMessageBox.Show($"请求错误: {ex.Message}");
                }
                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"发生错误: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 泛型同步Get请求
        /// </summary>
        public TResult Get<TResult>(string url)
        {
            try
            {
                string result = Get(url);

                if (string.IsNullOrEmpty(result))
                {
                    return default(TResult);
                }

                var resultDto = JsonConvert.DeserializeObject<ResultDto<TResult>>(result);
                return resultDto != null ? resultDto.Data : default(TResult);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Get请求异常: {ex.Message}");
                return default(TResult);
            }
        }
    }

    public class ResultDto<TResult>
    {
        public string Msg { get; set; }
        public TResult Data { get; set; }
        public bool Success { get; set; }
    }
}
