using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;
using LessJie.ConfigUtils;
using LessJie.DEncryptUtils;

namespace LessJie.StaticPageUtils
{
    /// <summary>
    /// 缓存文件服务提供程序 缓存文件与文件系统的相关操作的服务提供程序定义
    /// </summary>
    public class CachedFileProvider {
        /// <summary>
        /// 获取一个值指示当前页面的缓存路径
        /// </summary>
        public string CachedPagePath { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public CachedFileProvider() {
            var cachedPagePath = ConfigHelper.As_GetValue( "cachedPagePath" );
            if ( string.IsNullOrWhiteSpace( cachedPagePath ) )
                throw new ConfigurationErrorsException("缓存文件路径未正确配置。");

            this.CachedPagePath = cachedPagePath;
        }

        /// <summary>
        /// 通过HTTP请求中携带的路径信息创建缓存文件名称
        /// </summary>
        /// <param name="request">HTTP请求对象</param>
        /// <returns></returns>
        public string CreateCachedFileName( HttpRequest request ) {
            var fileName = request.Path.Replace( '/', '_' ).ToLower();
            fileName += ".html";

            return fileName;
        }

        /// <summary>
        /// 通过缓存文件名称创建缓存文件的物理存储目录的绝对路径
        /// </summary>
        /// <param name="fileName">缓存文件名称</param>
        /// <returns></returns>
        public string CreateCachedFolderPath( string fileName ) {
            var folderName = HashHelper.GetHashID( fileName );
            var folderPath = string.Concat( this.CachedPagePath, "\\", folderName, "\\" );

            return folderPath;
        }

        /// <summary>
        /// 移除指定的一个或多个缓存文件
        /// </summary>
        /// <param name="fileNames">缓存文件名称</param>
        public void RemoveCachedFile( params string[] fileNames ) {
            if ( fileNames == null || fileNames.Length == 0 )
                return;

            foreach ( var fileName in fileNames ) {
                var cachedFolderPath = this.CreateCachedFolderPath( fileName );
                var cachedFileFullName = cachedFolderPath + fileName;

                // 将文件删除操作提交到线程池中，由于文件可能存在正在使用或者其他不可预测问题
                // 所以我在这里设置了“重试”的机制，每次重试理论上（间隔时间取决于CPU时间片分配）都会间隔1秒时间
                ThreadPool.QueueUserWorkItem( state => {
                    var fileFullName = state.ToString();
                    var retryCount = 0;

                    while ( File.Exists( fileFullName ) && retryCount < 5 ) { // 重试次数为 5
                        try {
                            File.Delete( fileFullName );
                        } catch ( Exception ) {
                            Thread.Sleep( 1000 );
                            retryCount++;
                        }
                    }
                }, cachedFileFullName );
            }
        }
    }
}