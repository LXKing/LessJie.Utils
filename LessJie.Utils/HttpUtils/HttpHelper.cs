using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.UI;
using System.Web;
using LessJie.ValidateUtils;

namespace LessJie.HttpUtils
{
    /// <summary>    
    /// �ϴ����ݲ���    
    /// </summary>    
    public class UploadEventArgs : EventArgs
    {
        int bytesSent;
        int totalBytes;
        /// <summary>    
        /// �ѷ��͵��ֽ���    
        /// </summary>    
        public int BytesSent
        {
            get { return bytesSent; }
            set { bytesSent = value; }
        }
        /// <summary>    
        /// ���ֽ���    
        /// </summary>    
        public int TotalBytes
        {
            get { return totalBytes; }
            set { totalBytes = value; }
        }
    }
    /// <summary>    
    /// �������ݲ���    
    /// </summary>    
    public class DownloadEventArgs : EventArgs
    {
        int bytesReceived;
        int totalBytes;
        byte[] receivedData;
        /// <summary>    
        /// �ѽ��յ��ֽ���    
        /// </summary>    
        public int BytesReceived
        {
            get { return bytesReceived; }
            set { bytesReceived = value; }
        }
        /// <summary>    
        /// ���ֽ���    
        /// </summary>    
        public int TotalBytes
        {
            get { return totalBytes; }
            set { totalBytes = value; }
        }
        /// <summary>    
        /// ��ǰ���������յ�����    
        /// </summary>    
        public byte[] ReceivedData
        {
            get { return receivedData; }
            set { receivedData = value; }
        }
    }

    public class HttpHelper
    {
        #region Ԥ���巽����
        //Ĭ�ϵı���
        private Encoding encoding = Encoding.Default;
        //Post���ݱ���
        private Encoding postencoding = Encoding.Default;
        //HttpWebRequest����������������
        private HttpWebRequest request = null;
        //��ȡӰ���������ݶ���
        private HttpWebResponse response = null;
        //���ñ��صĳ���ip�Ͷ˿�
        private IPEndPoint _IPEndPoint = null;
        string respHtml = "";
        WebProxy proxy;
        static CookieContainer cc;
        WebHeaderCollection requestHeaders;
        WebHeaderCollection responseHeaders;
        int bufferSize = 15240;
        public event EventHandler<UploadEventArgs> UploadProgressChanged;
        public event EventHandler<DownloadEventArgs> DownloadProgressChanged;
        #endregion

        static HttpHelper()
        {

        }
        /// <summary>    
        /// ����WebClient��ʵ��    
        /// </summary>    
        public HttpHelper()
        {
            requestHeaders = new WebHeaderCollection();
            responseHeaders = new WebHeaderCollection();
        }
        /// <summary>    
        /// ���÷��ͺͽ��յ����ݻ����С    
        /// </summary>    
        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }
        /// <summary>    
        /// ��ȡ��Ӧͷ����    
        /// </summary>    
        public WebHeaderCollection ResponseHeaders
        {
            get { return responseHeaders; }
        }
        /// <summary>    
        /// ��ȡ����ͷ����    
        /// </summary>    
        public WebHeaderCollection RequestHeaders
        {
            get { return requestHeaders; }
        }
        /// <summary>    
        /// ��ȡ�����ô���    
        /// </summary>    
        public WebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }
        /// <summary>    
        /// ��ȡ��������������Ӧ���ı����뷽ʽ    
        /// </summary>    
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }
        /// <summary>    
        /// ��ȡ��������Ӧ��html����    
        /// </summary>    
        public string RespHtml
        {
            get { return respHtml; }
            set { respHtml = value; }
        }
        /// <summary>    
        /// ��ȡ�����������������Cookie����    
        /// </summary>    
        public CookieContainer CookieContainer
        {
            get { return cc; }
            set { cc = value; }
        }
        /// <summary>    
        ///  ��ȡ��ҳԴ����    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <returns></returns>    
        public string GetHtml(string url)
        {
            HttpWebRequest request = CreateRequest(url, "GET");
            respHtml = encoding.GetString(GetData(request));
            return respHtml;
        }
        /// <summary>    
        /// �����ļ�    
        /// </summary>    
        /// <param name="url">�ļ�URL��ַ</param>    
        /// <param name="filename">�ļ���������·��</param>    
        public void DownloadFile(string url, string filename)
        {
            FileStream fs = null;
            try
            {
                HttpWebRequest request = CreateRequest(url, "GET");
                byte[] data = GetData(request);
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                fs.Write(data, 0, data.Length);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// ��ָ��URL��������    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <returns></returns>    
        public byte[] GetData(string url)
        {
            HttpWebRequest request = CreateRequest(url, "GET");
            return GetData(request);
        }
        /// <summary>    
        /// ��ָ��URL�����ı�����    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <param name="postData">urlencode������ı�����</param>    
        /// <returns></returns>    
        public string Post(string url, string postData)
        {
            byte[] data = encoding.GetBytes(postData);
            return Post(url, data);
        }
        /// <summary>    
        /// ��ָ��URL�����ֽ�����    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <param name="postData">���͵��ֽ�����</param>    
        /// <returns></returns>    
        public string Post(string url, byte[] postData)
        {
            HttpWebRequest request = CreateRequest(url, "POST");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.KeepAlive = true;
            PostData(request, postData);
            respHtml = encoding.GetString(GetData(request));
            return respHtml;
        }
        /// <summary>    
        /// ��ָ����ַ����mulitpart���������    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <param name="mulitpartForm">mulitpart form data</param>    
        /// <returns></returns>    
        public string Post(string url, MultipartForm mulitpartForm)
        {
            HttpWebRequest request = CreateRequest(url, "POST");
            request.ContentType = mulitpartForm.ContentType;
            request.ContentLength = mulitpartForm.FormData.Length;
            request.KeepAlive = true;
            PostData(request, mulitpartForm.FormData);
            respHtml = encoding.GetString(GetData(request));
            return respHtml;
        }

        /// <summary>    
        /// ��ȡ���󷵻ص�����    
        /// </summary>    
        /// <param name="request">�������</param>    
        /// <returns></returns>    
        private byte[] GetData(HttpWebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            responseHeaders = response.Headers;
            //SaveCookiesToDisk();

            DownloadEventArgs args = new DownloadEventArgs();
            if (responseHeaders[HttpResponseHeader.ContentLength] != null)
                args.TotalBytes = Convert.ToInt32(responseHeaders[HttpResponseHeader.ContentLength]);

            MemoryStream ms = new MemoryStream();
            int count = 0;
            byte[] buf = new byte[bufferSize];
            while ((count = stream.Read(buf, 0, buf.Length)) > 0)
            {
                ms.Write(buf, 0, count);
                if (this.DownloadProgressChanged != null)
                {
                    args.BytesReceived += count;
                    args.ReceivedData = new byte[count];
                    Array.Copy(buf, args.ReceivedData, count);
                    this.DownloadProgressChanged(this, args);
                }
            }
            stream.Close();
            //��ѹ    
            if (ResponseHeaders[HttpResponseHeader.ContentEncoding] != null)
            {
                MemoryStream msTemp = new MemoryStream();
                count = 0;
                buf = new byte[100];
                switch (ResponseHeaders[HttpResponseHeader.ContentEncoding].ToLower())
                {
                    case "gzip":
                        GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
                        while ((count = gzip.Read(buf, 0, buf.Length)) > 0)
                        {
                            msTemp.Write(buf, 0, count);
                        }
                        return msTemp.ToArray();
                    case "deflate":
                        DeflateStream deflate = new DeflateStream(ms, CompressionMode.Decompress);
                        while ((count = deflate.Read(buf, 0, buf.Length)) > 0)
                        {
                            msTemp.Write(buf, 0, count);
                        }
                        return msTemp.ToArray();
                    default:
                        break;
                }
            }
            return ms.ToArray();
        }
        /// <summary>    
        /// ������������    
        /// </summary>    
        /// <param name="request">�������</param>    
        /// <param name="postData">�����͵��ֽ�����</param>    
        private void PostData(HttpWebRequest request, byte[] postData)
        {
            int offset = 0;
            int sendBufferSize = bufferSize;
            int remainBytes = 0;
            Stream stream = request.GetRequestStream();
            UploadEventArgs args = new UploadEventArgs();
            args.TotalBytes = postData.Length;
            while ((remainBytes = postData.Length - offset) > 0)
            {
                if (sendBufferSize > remainBytes) sendBufferSize = remainBytes;
                stream.Write(postData, offset, sendBufferSize);
                offset += sendBufferSize;
                if (this.UploadProgressChanged != null)
                {
                    args.BytesSent = offset;
                    this.UploadProgressChanged(this, args);
                }
            }
            stream.Close();
        }
        /// <summary>    
        /// ����HTTP����    
        /// </summary>    
        /// <param name="url">URL��ַ</param>    
        /// <returns></returns>    
        private HttpWebRequest CreateRequest(string url, string method)
        {
            Uri uri = new Uri(url);

            if (uri.Scheme == "https")
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);

            // Set a default policy level for the "http:" and "https" schemes.    
            HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            HttpWebRequest.DefaultCachePolicy = policy;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = false;
            request.AllowWriteStreamBuffering = false;
            request.Method = method;
            if (proxy != null)
                request.Proxy = proxy;
            request.CookieContainer = cc;
            foreach (string key in requestHeaders.Keys)
            {
                request.Headers.Add(key, requestHeaders[key]);
            }
            requestHeaders.Clear();
            return request;
        }

        #region Public

        /// <summary>
        /// �����ഫ������ݣ��õ���Ӧҳ������
        /// </summary>
        /// <param name="item">���������</param>
        /// <returns>����HttpResult����</returns>
        public HttpResult GetHtml(HttpItem item)
        {
            //���ز���
            HttpResult result = new HttpResult();
            try
            {
                //׼������
                SetRequest(item);
            }
            catch (Exception ex)
            {
                //���ò���ʱ����
                return new HttpResult() { Cookie = string.Empty, Header = null, Html = ex.Message, StatusDescription = "���ò���ʱ����" + ex.Message };
            }
            try
            {
                //��������
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    GetData(item, result);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (response = (HttpWebResponse)ex.Response)
                    {
                        GetData(item, result);
                    }
                }
                else
                {
                    result.Html = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
            }
            if (item.IsToLower) result.Html = result.Html.ToLower();
            return result;
        }
        #endregion

        #region GetData

        /// <summary>
        /// ��ȡ���ݵĲ������ķ���
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void GetData(HttpItem item, HttpResult result)
        {
            if (response == null)
            {
                return;
            }
            #region base
            //��ȡStatusCode
            result.StatusCode = response.StatusCode;
            //��ȡStatusDescription
            result.StatusDescription = response.StatusDescription;
            //��ȡHeaders
            result.Header = response.Headers;
            //��ȡ�����ʵ�URl
            result.ResponseUri = response.ResponseUri.ToString();
            //��ȡCookieCollection
            if (response.Cookies != null) result.CookieCollection = response.Cookies;
            //��ȡset-cookie
            if (response.Headers["set-cookie"] != null) result.Cookie = response.Headers["set-cookie"];
            #endregion

            #region byte
            //������ҳByte
            byte[] ResponseByte = GetByte();
            #endregion

            #region Html
            if (ResponseByte != null && ResponseByte.Length > 0)
            {
                //���ñ���
                SetEncoding(item, result, ResponseByte);
                //�õ����ص�HTML
                result.Html = encoding.GetString(ResponseByte);
            }
            else
            {
                //û�з����κ�Html����
                result.Html = string.Empty;
            }
            #endregion
        }
        /// <summary>
        /// ���ñ���
        /// </summary>
        /// <param name="item">HttpItem</param>
        /// <param name="result">HttpResult</param>
        /// <param name="ResponseByte">byte[]</param>
        private void SetEncoding(HttpItem item, HttpResult result, byte[] ResponseByte)
        {
            //�Ƿ񷵻�Byte��������
            if (item.ResultType == ResultType.Byte) result.ResultByte = ResponseByte;
            //�����￪ʼ����Ҫ���ӱ�����
            if (encoding == null)
            {
                Match meta = Regex.Match(Encoding.Default.GetString(ResponseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = string.Empty;
                if (meta != null && meta.Groups.Count > 0)
                {
                    c = meta.Groups[1].Value.ToLower().Trim();
                }
                if (c.Length > 2)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch
                    {
                        if (string.IsNullOrEmpty(response.CharacterSet))
                        {
                            encoding = Encoding.UTF8;
                        }
                        else
                        {
                            encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(response.CharacterSet))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);
                    }
                }
            }
        }
        /// <summary>
        /// ��ȡ��ҳByte
        /// </summary>
        /// <returns></returns>
        private byte[] GetByte()
        {
            byte[] ResponseByte = null;
            using (MemoryStream _stream = new MemoryStream())
            {
                //GZIIP����
                if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    //��ʼ��ȡ�������ñ��뷽ʽ
                    new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 10240);
                }
                else
                {
                    //��ʼ��ȡ�������ñ��뷽ʽ
                    response.GetResponseStream().CopyTo(_stream, 10240);
                }
                //��ȡByte
                ResponseByte = _stream.ToArray();
            }
            return ResponseByte;
        }


        #endregion

        #region SetRequest

        /// <summary>
        /// Ϊ����׼������
        /// </summary>
        ///<param name="item">�����б�</param>
        private void SetRequest(HttpItem item)
        {

            // ��֤֤��
            SetCer(item);
            if (item.IPEndPoint != null)
            {
                _IPEndPoint = item.IPEndPoint;
                //���ñ��صĳ���ip�Ͷ˿�
                request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(BindIPEndPointCallback);
            }
            //����Header����
            if (item.Header != null && item.Header.Count > 0) foreach (string key in item.Header.AllKeys)
                {
                    request.Headers.Add(key, item.Header[key]);
                }
            // ���ô���
            SetProxy(item);
            if (item.ProtocolVersion != null) request.ProtocolVersion = item.ProtocolVersion;
            request.ServicePoint.Expect100Continue = item.Expect100Continue;
            //����ʽGet����Post
            request.Method = item.Method;
            request.Timeout = item.Timeout;
            request.KeepAlive = item.KeepAlive;
            request.ReadWriteTimeout = item.ReadWriteTimeout;
            if (!string.IsNullOrWhiteSpace(item.Host))
            {
                request.Host = item.Host;
            }
            if (item.IfModifiedSince != null) request.IfModifiedSince = System.Convert.ToDateTime(item.IfModifiedSince);
            //Accept
            request.Accept = item.Accept;
            //ContentType��������
            request.ContentType = item.ContentType;
            //UserAgent�ͻ��˵ķ������ͣ�����������汾�Ͳ���ϵͳ��Ϣ
            request.UserAgent = item.UserAgent;
            // ����
            encoding = item.Encoding;
            //���ð�ȫƾ֤
            request.Credentials = item.ICredentials;
            //����Cookie
            SetCookie(item);
            //��Դ��ַ
            request.Referer = item.Referer;
            //�Ƿ�ִ����ת����
            request.AllowAutoRedirect = item.Allowautoredirect;
            if (item.MaximumAutomaticRedirections > 0)
            {
                request.MaximumAutomaticRedirections = item.MaximumAutomaticRedirections;
            }
            //����Post����
            SetPostData(item);
            //�����������
            if (item.Connectionlimit > 0) request.ServicePoint.ConnectionLimit = item.Connectionlimit;
        }
        /// <summary>
        /// ����֤��
        /// </summary>
        /// <param name="item"></param>
        private void SetCer(HttpItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.CerPath))
            {
                //��һ��һ��Ҫд�ڴ������ӵ�ǰ�档ʹ�ûص��ķ�������֤����֤��
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //��ʼ�����񣬲����������URL��ַ
                request = (HttpWebRequest)WebRequest.Create(item.URL);
                SetCerList(item);
                //��֤����ӵ�������
                request.ClientCertificates.Add(new X509Certificate(item.CerPath));
            }
            else
            {
                //��ʼ�����񣬲����������URL��ַ
                request = (HttpWebRequest)WebRequest.Create(item.URL);
                SetCerList(item);
            }
        }
        /// <summary>
        /// ���ö��֤��
        /// </summary>
        /// <param name="item"></param>
        private void SetCerList(HttpItem item)
        {
            if (item.ClentCertificates != null && item.ClentCertificates.Count > 0)
            {
                foreach (X509Certificate c in item.ClentCertificates)
                {
                    request.ClientCertificates.Add(c);
                }
            }
        }
        /// <summary>
        /// ����Cookie
        /// </summary>
        /// <param name="item">Http����</param>
        private void SetCookie(HttpItem item)
        {
            if (!string.IsNullOrEmpty(item.Cookie)) request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
            //����CookieCollection
            if (item.ResultCookieType == ResultCookieType.CookieCollection)
            {
                request.CookieContainer = new CookieContainer();
                if (item.CookieCollection != null && item.CookieCollection.Count > 0)
                    request.CookieContainer.Add(item.CookieCollection);
            }
        }
        /// <summary>
        /// ����Post����
        /// </summary>
        /// <param name="item">Http����</param>
        private void SetPostData(HttpItem item)
        {
            //��֤�ڵõ����ʱ�Ƿ��д�������
            if (!request.Method.Trim().ToLower().Contains("get"))
            {
                if (item.PostEncoding != null)
                {
                    postencoding = item.PostEncoding;
                }
                byte[] buffer = null;
                //д��Byte����
                if (item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
                {
                    //��֤�ڵõ����ʱ�Ƿ��д�������
                    buffer = item.PostdataByte;
                }//д���ļ�
                else if (item.PostDataType == PostDataType.FilePath && !string.IsNullOrWhiteSpace(item.Postdata))
                {
                    StreamReader r = new StreamReader(item.Postdata, postencoding);
                    buffer = postencoding.GetBytes(r.ReadToEnd());
                    r.Close();
                } //д���ַ���
                else if (!string.IsNullOrWhiteSpace(item.Postdata))
                {
                    buffer = postencoding.GetBytes(item.Postdata);
                }
                if (buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
                else
                {
                    request.ContentLength = 0;
                }
            }
        }
        /// <summary>
        /// ���ô���
        /// </summary>
        /// <param name="item">��������</param>
        private void SetProxy(HttpItem item)
        {
            bool isIeProxy = false;
            if (!string.IsNullOrWhiteSpace(item.ProxyIp))
            {
                isIeProxy = item.ProxyIp.ToLower().Contains("ieproxy");
            }
            if (!string.IsNullOrWhiteSpace(item.ProxyIp) && !isIeProxy)
            {
                //���ô��������
                if (item.ProxyIp.Contains(":"))
                {
                    string[] plist = item.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), System.Convert.ToInt32(plist[1].Trim()));
                    //��������
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    //����ǰ�������
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(item.ProxyIp, false);
                    //��������
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    //����ǰ�������
                    request.Proxy = myProxy;
                }
            }
            else if (isIeProxy)
            {
                //����ΪIE����
            }
            else
            {
                request.Proxy = item.WebProxy;
            }
        }


        #endregion

        #region private main
        /// <summary>
        /// �ص���֤֤������
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="certificate">֤��</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }

        /// <summary>
        /// ͨ������������ԣ������ڷ������ӵ�ʱ��󶨿ͻ��˷���������ʹ�õ�IP��ַ�� 
        /// </summary>
        /// <param name="servicePoint"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        private IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            return _IPEndPoint;//�˿ں�
        }
        #endregion

        #region ҳ�洦��������������

        /// <summary>
        /// �õ���ǰҳ����ʵ��
        /// </summary>
        /// <returns></returns>
        public static Page GetCurrentPage()
        {
            return (Page)HttpContext.Current.Handler;
        }

        /// <summary>
        /// ��System.Web.HttpRequest��Url�л�ȡ�����õ�ҳ������
        /// </summary>
        /// <returns>ҳ������</returns>
        public static string GetPageName()
        {
            int start = 0;
            int end = 0;
            string Url = HttpContext.Current.Request.RawUrl;
            start = Url.LastIndexOf("/") + 1;
            end = Url.IndexOf("?");
            if (end <= 0)
            {
                return Url.Substring(start, Url.Length - start);
            }
            else
            {
                return Url.Substring(start, end - start);
            }
        }

        /// <summary>
        /// ��ȡQueryStringֵ
        /// </summary>
        /// <param name="queryStringName">QueryString����</param>
        /// <returns>QueryStringֵ</returns>
        public static string GetQueryString(string queryStringName)
        {
            if ((HttpContext.Current.Request.QueryString[queryStringName] != null) &&
                (HttpContext.Current.Request.QueryString[queryStringName] != "undefined"))
            {
                return HttpContext.Current.Request.QueryString[queryStringName].Trim();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// ҳ����ת
        /// </summary>
        /// <param name="url">URL��ַ</param>
        public void Redirect(string url)
        {
            Page page = GetCurrentPage();
            page.Response.Redirect(url);
        }

        /// <summary>
        /// ��ȡ��ǰ����ҳ������ڸ�Ŀ¼�Ĳ㼶
        /// </summary>
        /// <returns></returns>
        public static string GetRelativeLevel()
        {
            string ApplicationPath = HttpContext.Current.Request.ApplicationPath;
            if (ApplicationPath.Trim() == "/")
            {
                ApplicationPath = "";
            }

            int i = ApplicationPath == "" ? 1 : 2;
            return "";//Nandasoft.Helper.NDHelperString.Repeat("../", Nandasoft.Helper.NDHelperString.RepeatTime(HttpContext.Current.Request.Path, "/") - i);
        }

        /// <summary>
        /// дjavascript�ű�
        /// </summary>
        /// <param name="script">�ű�����</param>
        private static void WriteScript(string script)
        {
            Page page = GetCurrentPage();

            // NDGridViewScriptFirst(page.Form.Controls, page);

            //ScriptManager.RegisterStartupScript(page, page.GetType(), System.Guid.NewGuid().ToString(), script, true);

        }

        //private void NDGridViewScriptFirst(ControlCollection ctls, Page page)
        //{

        //    foreach (Control ctl in ctls)
        //    {
        //        if (ctl is NDGridView)
        //        {
        //            NDGridView ndgv = (NDGridView)ctl;
        //            ScriptManager.RegisterStartupScript(page, page.GetType(), ndgv.ClientScriptKey, ndgv.ClientScriptName, true);
        //        }
        //        else
        //        {
        //            NDGridViewScriptFirst(ctl.Controls, page);
        //        }
        //    }
        //}

        /// <summary>
        /// ���ؿͻ���������汾
        /// �����IE���ͣ����ذ汾����
        /// �������IE���ͣ�����-1
        /// </summary>
        /// <returns>һλ���ְ汾��</returns>
        public static int GetClientBrowserVersion()
        {
            string USER_AGENT = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];

            if (USER_AGENT.IndexOf("MSIE") < 0) return -1;

            string version = USER_AGENT.Substring(USER_AGENT.IndexOf("MSIE") + 5, 1);
            if (!DataValidate.IsInt(version)) return -1;

            return Convert.ToInt32(version);
        }

        #endregion
    }





    /// <summary>    
    /// ���ļ����ı����ݽ���Multipart��ʽ�ı���    
    /// </summary>    
    public class MultipartForm
    {
        private Encoding encoding;
        private MemoryStream ms;
        private string boundary;
        private byte[] formData;
        /// <summary>    
        /// ��ȡ�������ֽ�����    
        /// </summary>    
        public byte[] FormData
        {
            get
            {
                if (formData == null)
                {
                    byte[] buffer = encoding.GetBytes("--" + this.boundary + "--\r\n");
                    ms.Write(buffer, 0, buffer.Length);
                    formData = ms.ToArray();
                }
                return formData;
            }
        }
        /// <summary>    
        /// ��ȡ�˱������ݵ�����    
        /// </summary>    
        public string ContentType
        {
            get { return string.Format("multipart/form-data; boundary={0}", this.boundary); }
        }
        /// <summary>    
        /// ��ȡ�����ö��ַ������õı�������    
        /// </summary>    
        public Encoding StringEncoding
        {
            set { encoding = value; }
            get { return encoding; }
        }
        /// <summary>    
        /// ʵ����    
        /// </summary>    
        public MultipartForm()
        {
            boundary = string.Format("--{0}--", Guid.NewGuid());
            ms = new MemoryStream();
            encoding = Encoding.Default;
        }
        /// <summary>    
        /// ���һ���ļ�    
        /// </summary>    
        /// <param name="name">�ļ�������</param>    
        /// <param name="filename">�ļ�������·��</param>    
        public void AddFlie(string name, string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("������Ӳ����ڵ��ļ���", filename);
            FileStream fs = null;
            byte[] fileData = { };
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                this.AddFlie(name, Path.GetFileName(filename), fileData, fileData.Length);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// ���һ���ļ�    
        /// </summary>    
        /// <param name="name">�ļ�������</param>    
        /// <param name="filename">�ļ���</param>    
        /// <param name="fileData">�ļ�����������</param>    
        /// <param name="dataLength">���������ݴ�С</param>    
        public void AddFlie(string name, string filename, byte[] fileData, int dataLength)
        {
            if (dataLength <= 0 || dataLength > fileData.Length)
            {
                dataLength = fileData.Length;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("--{0}\r\n", this.boundary);
            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n", name, filename);
            sb.AppendFormat("Content-Type: {0}\r\n", this.GetContentType(filename));
            sb.Append("\r\n");
            byte[] buf = encoding.GetBytes(sb.ToString());
            ms.Write(buf, 0, buf.Length);
            ms.Write(fileData, 0, dataLength);
            byte[] crlf = encoding.GetBytes("\r\n");
            ms.Write(crlf, 0, crlf.Length);
        }
        /// <summary>    
        /// ����ַ���    
        /// </summary>    
        /// <param name="name">�ı�������</param>    
        /// <param name="value">�ı�ֵ</param>    
        public void AddString(string name, string value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("--{0}\r\n", this.boundary);
            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n", name);
            sb.Append("\r\n");
            sb.AppendFormat("{0}\r\n", value);
            byte[] buf = encoding.GetBytes(sb.ToString());
            ms.Write(buf, 0, buf.Length);
        }
        /// <summary>    
        /// ��ע����ȡ�ļ�����    
        /// </summary>    
        /// <param name="filename">������չ�����ļ���</param>    
        /// <returns>�磺application/stream</returns>    
        private string GetContentType(string filename)
        {
            Microsoft.Win32.RegistryKey fileExtKey = null; ;
            string contentType = "application/stream";
            try
            {
                fileExtKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename));
                contentType = fileExtKey.GetValue("Content Type", contentType).ToString();
            }
            finally
            {
                if (fileExtKey != null) fileExtKey.Close();
            }
            return contentType;
        }
    }

    #region public calss
    /// <summary>
    /// Http����ο���
    /// </summary>
    public class HttpItem
    {
        /// <summary>
        /// ����URL������д
        /// </summary>
        public string URL { get; set; }
        string _Method = "GET";
        /// <summary>
        /// ����ʽĬ��ΪGET��ʽ,��ΪPOST��ʽʱ��������Postdata��ֵ
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 100000;
        /// <summary>
        /// Ĭ������ʱʱ��
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 30000;
        /// <summary>
        /// Ĭ��д��Post���ݳ�ʱ��
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        /// <summary>
        /// ����Host�ı�ͷ��Ϣ
        /// </summary>
        public string Host { get; set; }
        Boolean _KeepAlive = true;
        /// <summary>
        ///  ��ȡ������һ��ֵ����ֵָʾ�Ƿ��� Internet ��Դ�����־�������Ĭ��Ϊtrue��
        /// </summary>
        public Boolean KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        string _Accept = "text/html, application/xhtml+xml, */*";
        /// <summary>
        /// �����ͷֵ Ĭ��Ϊtext/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "text/html";
        /// <summary>
        /// ���󷵻�����Ĭ�� text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        /// <summary>
        /// �ͻ��˷�����ϢĬ��Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        /// <summary>
        /// �������ݱ���Ĭ��ΪNUll,�����Զ�ʶ��,һ��Ϊutf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get; set; }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post����������
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        /// <summary>
        /// Post����ʱҪ���͵��ַ���Post����
        /// </summary>
        public string Postdata { get; set; }
        /// <summary>
        /// Post����ʱҪ���͵�Byte���͵�Post����
        /// </summary>
        public byte[] PostdataByte { get; set; }
        /// <summary>
        /// Cookie���󼯺�
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        /// <summary>
        /// ����ʱ��Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// ��Դ��ַ���ϴη��ʵ�ַ
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// ֤�����·��
        /// </summary>
        public string CerPath { get; set; }
        /// <summary>
        /// ���ô�����󣬲���ʹ��IEĬ�����þ�����ΪNull�����Ҳ�Ҫ����ProxyIp
        /// </summary>
        public WebProxy WebProxy { get; set; }
        private Boolean isToLower = false;
        /// <summary>
        /// �Ƿ�����Ϊȫ��Сд��Ĭ��Ϊ��ת��
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        private Boolean allowautoredirect = false;
        /// <summary>
        /// ֧����תҳ�棬��ѯ���������ת���ҳ�棬Ĭ���ǲ���ת
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// ���������
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        /// <summary>
        /// ����Proxy �������û���
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// ���� ����������
        /// </summary>
        public string ProxyPwd { get; set; }
        /// <summary>
        /// ���� ����IP,���Ҫʹ��IE���������Ϊieproxy
        /// </summary>
        public string ProxyIp { get; set; }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// ���÷�������String��Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// header����
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        /// <summary>
        //     ��ȡ��������������� HTTP �汾�����ؽ��:��������� HTTP �汾��Ĭ��Ϊ System.Net.HttpVersion.Version11��
        /// </summary>
        public Version ProtocolVersion { get; set; }
        private Boolean _expect100continue = false;
        /// <summary>
        ///  ��ȡ������һ�� System.Boolean ֵ����ֵȷ���Ƿ�ʹ�� 100-Continue ��Ϊ����� POST ������Ҫ 100-Continue ��Ӧ����Ϊ true������Ϊ false��Ĭ��ֵΪ true��
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }
        /// <summary>
        /// ����509֤�鼯��
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }
        /// <summary>
        /// ���û��ȡPost��������,Ĭ�ϵ�ΪDefault����
        /// </summary>
        public Encoding PostEncoding { get; set; }
        private ResultCookieType _ResultCookieType = ResultCookieType.String;
        /// <summary>
        /// Cookie��������,Ĭ�ϵ���ֻ�����ַ�������
        /// </summary>
        public ResultCookieType ResultCookieType
        {
            get { return _ResultCookieType; }
            set { _ResultCookieType = value; }
        }
        private ICredentials _ICredentials = CredentialCache.DefaultCredentials;
        /// <summary>
        /// ��ȡ����������������֤��Ϣ��
        /// </summary>
        public ICredentials ICredentials
        {
            get { return _ICredentials; }
            set { _ICredentials = value; }
        }
        /// <summary>
        /// �������󽫸�����ض���������Ŀ
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }
        private DateTime? _IfModifiedSince = null;
        /// <summary>
        /// ��ȡ������IfModifiedSince��Ĭ��Ϊ��ǰ���ں�ʱ��
        /// </summary>
        public DateTime? IfModifiedSince
        {
            get { return _IfModifiedSince; }
            set { _IfModifiedSince = value; }
        }
        #region ip-port
        private IPEndPoint _IPEndPoint = null;
        /// <summary>
        /// ���ñ��صĳ���ip�Ͷ˿�
        /// </summary>]
        /// <example>
        ///item.IPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"),80);
        /// </example>
        public IPEndPoint IPEndPoint
        {
            get { return _IPEndPoint; }
            set { _IPEndPoint = value; }
        }
        #endregion
    }
    /// <summary>
    /// Http���ز�����
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Http���󷵻ص�Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// Cookie���󼯺�
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        private string _html = string.Empty;
        /// <summary>
        /// ���ص�String�������� ֻ��ResultType.Stringʱ�ŷ������ݣ��������Ϊ��
        /// </summary>
        public string Html
        {
            get { return _html; }
            set { _html = value; }
        }
        /// <summary>
        /// ���ص�Byte���� ֻ��ResultType.Byteʱ�ŷ������ݣ��������Ϊ��
        /// </summary>
        public byte[] ResultByte { get; set; }
        /// <summary>
        /// header����
        /// </summary>
        public WebHeaderCollection Header { get; set; }
        /// <summary>
        /// ����״̬˵��
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// ����״̬��,Ĭ��ΪOK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// �����ʵ�URl
        /// </summary>
        public string ResponseUri { get; set; }
        /// <summary>
        /// ��ȡ�ض����URl
        /// </summary>
        public string RedirectUrl
        {
            get
            {
                try
                {
                    if (Header != null && Header.Count > 0)
                    {
                        if (Header.AllKeys.Any(k => k.ToLower().Contains("location")))
                        {
                            string baseurl = Header["location"].ToString().Trim();
                            string locationurl = baseurl.ToLower();
                            if (!string.IsNullOrWhiteSpace(locationurl))
                            {
                                bool b = locationurl.StartsWith("http://") || locationurl.StartsWith("https://");
                                if (!b)
                                {
                                    baseurl = new Uri(new Uri(ResponseUri), baseurl).AbsoluteUri;
                                }
                            }
                            return baseurl;
                        }
                    }
                }
                catch { }
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// ��ʾֻ�����ַ��� ֻ��Html������
        /// </summary>
        String,
        /// <summary>
        /// ��ʾ�����ַ������ֽ��� ResultByte��Html�������ݷ���
        /// </summary>
        Byte
    }
    /// <summary>
    /// Post�����ݸ�ʽĬ��Ϊstring
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// �ַ������ͣ���ʱ����Encoding�ɲ�����
        /// </summary>
        String,
        /// <summary>
        /// Byte���ͣ���Ҫ����PostdataByte������ֵ����Encoding������Ϊ��
        /// </summary>
        Byte,
        /// <summary>
        /// ���ļ���Postdata��������Ϊ�ļ��ľ���·������������Encoding��ֵ
        /// </summary>
        FilePath
    }
    /// <summary>
    /// Cookie��������
    /// </summary>
    public enum ResultCookieType
    {
        /// <summary>
        /// ֻ�����ַ������͵�Cookie
        /// </summary>
        String,
        /// <summary>
        /// CookieCollection��ʽ��Cookie����ͬʱҲ����String���͵�cookie
        /// </summary>
        CookieCollection
    }
    #endregion
}