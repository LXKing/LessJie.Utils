using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using Utils.ConfigUtils;
using Utils.HttpUtils;

namespace Utils.StaticPageUtils
{
    /// <summary>
    /// 启用缓存的Asp.net页 拥有一系列缓存方案的基础页的类定义，需要应用缓存技术的Asp.net页面可以继
    //               承该类以达到构建缓存的目的
    /// </summary>
    public class CacheEnabledPage : Page {
        // 启用缓存策略的标识
        private const string _FLAG_ENABLE = "true";
        // 捕获有效时间内容
        private readonly Regex _expiryRegex = new Regex( "<!-- expiry\\[([\\s\\S]+?)\\] -->", RegexOptions.Compiled);

        private CachedFileProvider _cachedFileProvider;

        /// <summary>
        /// 是否启用缓存缓存策略
        /// </summary>
        protected string CacheEnabled { get; private set; }
        /// <summary>
        /// 页面缓存时间（单位：分钟，默认值：1440）
        /// </summary>
        protected int CachedExpiryMinutes { get; set; }
        /// <summary>
        /// 获取一个值指示当前页面的缓存文件名称
        /// </summary>
        protected string CachedPageFileName { get; private set; }
        /// <summary>
        /// 获取一个值指示当前页面的缓存文件的物理存储文件的绝对路径
        /// </summary>
        protected string CachedPageFileFullName { get; private set; }
        /// <summary>
        /// 获取一个值指示当前页面的缓存文件的物理存储目录的绝对路径
        /// </summary>
        protected string CachedPageFileFolderPath { get; private set; }
        /// <summary>
        /// 获取一个值指示当前页是否可以被缓存
        /// </summary>
        protected bool CanCache { get; private set; }
        /// <summary>
        /// 当请求参数中的参数名称包含在参数过滤列表中时，不使用缓存
        /// </summary>
        protected List<string> ParamsFilter { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public CacheEnabledPage() {
            var cacheEnabled = ConfigHelper.As_GetValue( "cacheEnabled" );
            if ( string.IsNullOrWhiteSpace( cacheEnabled ) )
                throw new ConfigurationErrorsException( "是否启用缓存策略未能正确配置。" );

            this._cachedFileProvider = new CachedFileProvider();

            // 初始化设置
            this.CacheEnabled = cacheEnabled;
            this.CachedExpiryMinutes = 1440;
            this.CanCache = false;
            this.ParamsFilter = new List<string>();
        }

        /// <summary>
        /// 清除页面缓存
        /// </summary>
        /// <param name="cachedFileNames">需要清除的文件名称</param>
        public static void FlushCache( params string[] cachedFileNames ) {
            var cacheManageUrl = ConfigHelper.As_GetValue( "cacheManageUrl" );
            if ( string.IsNullOrWhiteSpace( cacheManageUrl ) )
                throw new ConfigurationErrorsException( "缓存管理地址配置未能正确读取。" );

            if ( cachedFileNames == null || cachedFileNames.Length == 0 )
                return;

            var postData = string.Concat( "action=flushcache&page=", string.Join( "||", cachedFileNames ) );
            ThreadPool.QueueUserWorkItem( state => {
                var httpHelper = new HttpHelper();
                var httpItem = new HttpItem {
                    URL = cacheManageUrl,
                    Method = "POST",
                    ContentType = "application/x-www-form-urlencoded",
                    PostDataType = PostDataType.String,
                    Postdata = postData,
                    ResultType = ResultType.String
                };

                httpHelper.GetHtml( httpItem );
            } );
        }

        // 当页面初始化时执行
        protected override void OnPreInit( EventArgs e ) {
            if ( this.CheckCacheEnable() ) {
                this.CachedPageFileName = this.BuildCachedPageFileName();
                this.CachedPageFileFolderPath = this._cachedFileProvider.CreateCachedFolderPath( this.CachedPageFileName );
                this.CachedPageFileFullName = this.CachedPageFileFolderPath + this.CachedPageFileName;

                if ( File.Exists( this.CachedPageFileFullName ) ) {
                    // 读取缓存文件中的第一行数据，第一行数据包含页面有效期数据
                    using ( var fs = File.OpenRead( this.CachedPageFileFullName ) ) {
                        var streamReader = new StreamReader( fs );
                        var expiryText = streamReader.ReadLine();
                        streamReader.Close();
                        
                        // 检查缓存文件是否过期，如果没有过期则直接向客户端发送缓存页面内容
                        if ( !string.IsNullOrWhiteSpace( expiryText ) && this._expiryRegex.IsMatch( expiryText ) ) {
                            var expiryMatch = this._expiryRegex.Match( expiryText );
                            var expiryDateTime = System.Convert.ToDateTime( expiryMatch.Groups[ 1 ].Value );
                            if ( expiryDateTime >= DateTime.Now ) {
                                this.Response.Clear();
                                this.Response.ContentType = "text/html";
                                this.Response.WriteFile( this.CachedPageFileFullName );
                                this.Response.End();
                            } else {
                                this.CanCache = true;
                            }
                        } else {
                            this.CanCache = true;
                        }
                    }
                } else {
                    this.CanCache = true;
                }
            }

            // 未启用缓存策略或者满足缓存策略条件的请求
            base.OnPreInit( e );
        }

        protected override void Render( HtmlTextWriter writer ) {
            if ( this.CanCache ) {
                // 将底层构建的内容都拦截到自定义的流中
                var stringWriter = new StringWriter();
                var htmlTextWriter = new HtmlTextWriter( stringWriter );
                // 这句话很重要，页面所有的元素要呈现为HTML内容都要经过它本身的Render()函数
                // 而这个 htmlTextWriter 就贯穿于流程始末，拦截到所有元素呈现的内容
                // 否则页面呈现的内容一个字符都获取不到
                base.Render( htmlTextWriter );
                htmlTextWriter.Flush();
                htmlTextWriter.Close();
                // 这句话比上面那句 base.Render( htmlTextWriter ); 更重要。
                // 原因就是，如果你想让页面呈现内容一字不差的发送到客户端就是这句话在起作用
                // 这句话以及上面那句虽然很简单，但是如果你忽略了它们中的任意一句，它们会让你知道什么叫噩梦~！
                writer.Write( stringWriter );

                // 以下代码将由底层构建的用于呈现到客户端的内容保存到物理文件中，很简单
                var htmlText = stringWriter.ToString();
                stringWriter.Close();
                stringWriter.Dispose();

                // 保存页面最终呈现的内容到文件，并附加缓存有效期
                // 好吧，这里使用文件的共享方式来控制对文件的并发写入控制
                // 所以这里必须处理好异常，虽然不能写入但是至少我能做到让上面的代码呈现的页面内容顺利的响应到客户端
                // 葛小亮 2015-12-22
                try {
                    if ( !Directory.Exists( this.CachedPageFileFolderPath ) )
                        Directory.CreateDirectory( this.CachedPageFileFolderPath );

                    using ( var fs = File.Open( this.CachedPageFileFullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read ) ) {
                        fs.SetLength( 0 ); // 清空原有文件的内容
                        var streamWriter = new StreamWriter( fs, Encoding.UTF8 );
                        streamWriter.WriteLine( "<!-- expiry[{0}] -->",
                                                DateTime.Now.AddMinutes( this.CachedExpiryMinutes )
                                                        .ToString( "yyyy-MM-dd HH:mm:ss" ) );
                        streamWriter.Write( htmlText );
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                } catch ( IOException ) {
                    // 隐藏掉由于IO操作导致的异常，例如：并发写文件因为不共享写入导致的错误。
                }
            } else {
                base.Render( writer );
            }
        }

        /// <summary>
        /// 构建页面缓存文件的名称
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildCachedPageFileName() {
            return this._cachedFileProvider.CreateCachedFileName( this.Request );
        }

        /// <summary>
        /// 是否是游客身份
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsGuest() {
            return true;
        }

        /// <summary>
        /// 检查请求参数中是否与参数过滤列表有交集
        /// </summary>
        /// <returns></returns>
        private bool CheckFilter() {
            return this.Request.Params.AllKeys.Length > 0
                   && this.ParamsFilter.Count > 0
                   && this.Request.Params.AllKeys.Intersect( this.ParamsFilter ).Any();
        }

        /// <summary>
        /// 验证是否应用缓存策略
        /// </summary>
        /// <returns></returns>
        private bool CheckCacheEnable() {
            var enabled = this.CacheEnabled == _FLAG_ENABLE;
            var isGuest = this.IsGuest();
            var filter = this.CheckFilter();

            return enabled && isGuest && !this.IsPostBack && !filter;
        }
    }
}