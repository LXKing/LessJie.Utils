using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.Text;

namespace Utils.UrlUtils
{
    /// <summary>
    /// URL的操作类
    /// Encoding UTF8
    /// </summary>
    public class UrlOperate
    {
        static System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 对URL进行解码
        /// </summary>
        /// <param name="html">html</param>
        /// <returns></returns>
        public static string URLDecode(string html)
        {
            return HttpUtility.UrlDecode(html, Encoding.UTF8);
        }
        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns></returns>
        public static string UrlEncode(string URL)
        {
            return HttpUtility.UrlEncode(URL, Encoding.UTF8);
        }

        #region URL的64位编码
        public static string Base64Encrypt(string sourthUrl)
        {
            string eurl = HttpUtility.UrlEncode(sourthUrl);
            eurl = Convert.ToBase64String(encoding.GetBytes(eurl));
            return eurl;
        }
        #endregion

        #region URL的64位解码
        public static string Base64Decrypt(string eStr)
        {        
            if (!IsBase64(eStr))
            {
                return eStr;
            }
            byte[] buffer = Convert.FromBase64String(eStr);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }
        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)
        {
            if ((eStr.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 添加URL参数
        /// <summary>
        /// 添加URL参数
        /// </summary>
        public static string AddParam(string url, string paramName, string value)
        {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.Query))
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "?" + paramName + "=" + eval);
            }
            else
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "&" + paramName + "=" + eval);
            }
        }
        #endregion

        #region 更新URL参数
        /// <summary>
        /// 更新URL参数
        /// </summary>
        public static string UpdateParam(string url, string paramName, string value)
        {
            string keyWord = paramName + "=";
            int index = url.IndexOf(keyWord) + keyWord.Length;
            int index1 = url.IndexOf("&", index);
            if (index1 == -1)
            {
                url = url.Remove(index, url.Length - index);
                url = string.Concat(url, value);
                return url;
            }
            url = url.Remove(index, index1 - index);
            url = url.Insert(index, value);
            return url;
        }
        #endregion

        #region 分析URL所属的域
        public static void GetDomain(string fromUrl, out string domain, out string subDomain)
        {
            domain = "";
            subDomain = "";
            try
            {
                if (fromUrl.IndexOf("的名片") > -1)
                {
                    subDomain = fromUrl;
                    domain = "名片";
                    return;
                }

                UriBuilder builder = new UriBuilder(fromUrl);
                fromUrl = builder.ToString();

                Uri u = new Uri(fromUrl);

                if (u.IsWellFormedOriginalString())
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";

                    }
                    else
                    {
                        string Authority = u.Authority;
                        string[] ss = u.Authority.Split('.');
                        if (ss.Length == 2)
                        {
                            Authority = "www." + Authority;
                        }
                        int index = Authority.IndexOf('.', 0);
                        domain = Authority.Substring(index + 1, Authority.Length - index - 1).Replace("comhttp","com");
                        subDomain = Authority.Replace("comhttp", "com"); 
                        if (ss.Length < 2)
                        {
                            domain = "不明路径";
                            subDomain = "不明路径";
                        }
                    }
                }
                else
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";
                    }
                    else
                    {
                        subDomain = domain = "不明路径";
                    }
                }
            }
            catch
            {
                subDomain = domain = "不明路径";
            }
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nvc = new NameValueCollection();
            baseUrl = "";

            if (url == "")
                return;

            int questionMarkIndex = url.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);

            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }

        #endregion

        #region 类内部调用
        /// <summary>
        /// HttpContext Current
        /// </summary>
        public static HttpContext Current
        {
            get { return HttpContext.Current; }
        }
        /// <summary>
        /// HttpContext Current  HttpRequest Request   get { return Current.Request;
        /// </summary>
        public static HttpRequest Request
        {
            get { return Current.Request; }
        }
        /// <summary>
        ///  HttpContext Current  HttpRequest Request   get { return Current.Request; HttpResponse Response  return Current.Response;
        /// </summary>
        public static HttpResponse Response
        {
            get { return Current.Response; }
        }
        #endregion

        #region 等于Request.QueryString;如果为null 返回 空“” ，否则返回Request.QueryString[name]
        /// <summary>
        /// 等于Request.QueryString;如果为null 返回 空“” ，否则返回Request.QueryString[name]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Q(string name)
        {
            return Request.QueryString[name] == null ? "" : Request.QueryString[name];
        }
        #endregion

        /// <summary>
        /// 等于  Request.Form  如果为null 返回 空“” ，否则返回 Request.Form[name]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string F(string name)
        {
            return Request.Form[name] == null ? "" : Request.Form[name].ToString();
        }

        #region 获取url中的id
        /// <summary>
        /// 获取url中的id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int QId(string name)
        {
            return StrToId(Q(name));
        }
        #endregion

        #region 获取正确的Id，如果不是正整数，返回0
        /// <summary>
        /// 获取正确的Id，如果不是正整数，返回0
        /// </summary>
        /// <param name="_value"></param>
        /// <returns>返回正确的整数ID，失败返回0</returns>
        public static int StrToId(string _value)
        {
            if (IsNumberId(_value))
                return int.Parse(_value);
            else
                return 0;
        }
        #endregion

        #region 检查一个字符串是否是纯数字构成的，一般用于查询字符串参数的有效性验证。
        /// <summary>
        /// 检查一个字符串是否是纯数字构成的，一般用于查询字符串参数的有效性验证。
        /// </summary>
        /// <param name="_value">需验证的字符串。。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool IsNumberId(string _value)
        {
            return QuickValidate("^[1-9]*[0-9]*$", _value);
        }
        #endregion

        #region 快速验证一个字符串是否符合指定的正则表达式。
        /// <summary>
        /// 快速验证一个字符串是否符合指定的正则表达式。
        /// </summary>
        /// <param name="_express">正则表达式的内容。</param>
        /// <param name="_value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool QuickValidate(string _express, string _value)
        {
            if (_value == null) return false;
            Regex myRegex = new Regex(_express);
            if (_value.Length == 0)
            {
                return false;
            }
            return myRegex.IsMatch(_value);
        }
        #endregion


        public static string RequestQueryString()
        {
            string queryString = string.Empty;
            HttpRequest request = HttpContext.Current.Request;

            for (int i = 0; i < request.QueryString.Keys.Count; i++)
            {
                if (string.IsNullOrEmpty(queryString))
                    queryString += "?" + request.QueryString.Keys[i] + "=" + request.QueryString[i];
                else
                    queryString += "&" + request.QueryString.Keys[i] + "=" + request.QueryString[i];
            }
            return queryString;
        }

        public static string GetQueryString(string paramKey)
        {
            HttpRequest request = HttpContext.Current.Request;

            if (request.QueryString[paramKey] != null)
            {
                return request.QueryString[paramKey].ToString().Replace("'", "''");
            }
            else
            {
                return "";
            }
        }

        public static int GetIntQueryString(string paramKey)
        {
            HttpRequest request = HttpContext.Current.Request;
            int iOut = 0;
            if (request.QueryString[paramKey] != null)
            {
                string sOut = request[paramKey].ToString();
                if (!String.IsNullOrEmpty(sOut))
                    int.TryParse(sOut, out iOut);
            }
            return iOut;
        }

        public static short GetShortQueryString(string paramKey)
        {
            HttpRequest request = HttpContext.Current.Request;
            short iOut = 0;
            if (request.QueryString[paramKey] != null)
            {
                string sOut = request[paramKey].ToString();
                if (!String.IsNullOrEmpty(sOut))
                    short.TryParse(sOut, out iOut);
            }
            return iOut;
        }

        public static DateTime GetDateTimeQueryString(string paramKey)
        {
            HttpRequest request = HttpContext.Current.Request;
            DateTime iOut = DateTime.MinValue;
            if (request.QueryString[paramKey] != null)
            {
                string sOut = request[paramKey].ToString();
                if (!String.IsNullOrEmpty(sOut))
                    DateTime.TryParse(sOut, out iOut);
            }
            return iOut;
        }

        public static string GetForm(string paramKey)
        {
            HttpRequest request = HttpContext.Current.Request;
            if (request.Form[paramKey] != null)
            {
                return request.Form[paramKey].ToString().Replace("'", "''");
            }
            else
            {
                return "";
            }
        }

        public static string WebApplicationRootPath
        {
            get
            {
                string path = HttpContext.Current.Request.ApplicationPath;
                if (path.Length > 1) path = path + "/";
                return path;
            }
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="strObject">可转为string类型的变量</param>
        /// <param name="strLen">截取字符位数，程序自动判断全半角，全角占2位，半角占1位</param>
        /// <returns>截取后字符串</returns>
        public static string TrimLen(object strObject, int strLen)
        {
            if (strObject == null || strObject == System.DBNull.Value) return string.Empty;

            string str = strObject.ToString();
            if (string.IsNullOrEmpty(str)) return string.Empty;

            char[] arrChar = str.ToCharArray();
            string returnStr = str;
            int charCount = 0;

            for (int i = 0; i < arrChar.Length; i++)
            {
                if ((int)arrChar[i] > 255)
                    charCount += 2;
                else
                    charCount++;

                if (charCount >= strLen)
                {
                    returnStr = str.Substring(0, i + 1);
                    if (str.Length > returnStr.Length) returnStr += "...";
                    break;
                }
            }

            return returnStr;
        }

        // 控制时间格式
        public static string TrimDate(object dateObject, string dateType)
        {
            if (dateObject == null || dateObject == System.DBNull.Value) return string.Empty;

            try
            {
                Convert.ToDateTime(dateObject);
            }
            catch (Exception)
            {
                return dateObject.ToString();
            }

            return Convert.ToDateTime(dateObject).ToString(dateType);
        }

        // 将文本彩色显示
        public static string ShowColorText(object textObject, string color)
        {
            if (textObject == null || textObject == System.DBNull.Value) return string.Empty;

            return String.Format("<font color={0}>{1}</font>", color, textObject.ToString());
        }

        // 时间到期加亮显示
        public static string HighlightDate(object dateObject)
        {
            if (dateObject == null || dateObject == System.DBNull.Value) return string.Empty;

            DateTime srcDate;
            try
            {
                srcDate = Convert.ToDateTime(dateObject);
            }
            catch (Exception)
            {
                return dateObject.ToString();
            }

            if (srcDate < System.DateTime.Today)
                return ShowColorText(TrimDate(srcDate, "yyyy-MM-dd"), "red");
            else
                return TrimDate(srcDate, "yyyy-MM-dd");
        }

        public static string ShowImage(object boolObject)
        {
            if (boolObject == null || boolObject == System.DBNull.Value) return string.Empty;

            bool visible = false;

            try
            {
                visible = Convert.ToBoolean(boolObject);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return visible == false ? "" : "display:none";
        }

        public static string FormatQuoteString(object objStr)
        {
            if (objStr == null) return string.Empty;

            string str = objStr.ToString();
            str = str.Replace("'", "\\'");
            str = str.Replace("\"", "\\'");
            return str;
        }

        public static void NoCacheThisPage()
        {
            HttpContext.Current.Response.Expires = -1;
            HttpContext.Current.Response.ExpiresAbsolute = DateTime.Today.AddDays(-1);
            HttpContext.Current.Response.CacheControl = "no-cache";
        }
        public static string GetClientIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ip == null)
            {
                ip = System.Web.HttpContext.Current.Request["Remote_Addr"];
            }
            if (ip == null)
            {
                ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            return ip;
        }

        public static void RedirectError()
        {

            System.Web.HttpContext.Current.Response.Redirect("http://www.jjoobb.cn/Error.html");
        }

        /// <summary>
        /// 判断请求来源是否允许，允许则返回true
        /// </summary>
        public static bool IsAllowDomain
        {
            get
            {
                if (HttpContext.Current.Request.UrlReferrer == null) return false;
                string reqDomain = HttpContext.Current.Request.UrlReferrer.Host.ToLower();
                return isAllowUrl(reqDomain);
            }
        }
        /// <summary>
        /// 判断请求来源是否允许
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool isAllowUrl(string url)
        {
            string[] AllowDomains = new string[] {
                "sufeinet.com",
            };
            foreach (string str in AllowDomains)
            {
                if (url.EndsWith(str)) return true;
            }
            return false;
        }
        /// <summary>
        /// 判断请求来源是否在排除之列(总站)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool isDenyUrl(string url)
        {
            string[] denyArr = new string[] { "www.sufeinet", "tool.sufeinet" };
            foreach (string str in denyArr)
            {
                if (url.StartsWith(str)) return true;
            }
            return false;
        }

        public static bool isAllowOpenUrl(string url)
        {
            return true;
        }
        /// <summary>
        /// 判断请求来源是否允许，允许则返回true
        /// </summary>
        public static bool IsAllowOpenDomain
        {
            get
            {
                if (HttpContext.Current.Request.UrlReferrer == null) return false;
                string reqDomain = HttpContext.Current.Request.UrlReferrer.Host.ToLower();
                return isAllowUrl(reqDomain);
            }
        }
    }
}
