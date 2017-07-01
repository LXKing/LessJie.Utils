using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LessJie.AppUtils
{
    /// <summary>
    /// 网站路径操作类
    /// </summary>
    public static class APPHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public enum SortType
        {
            /// <summary>
            /// 
            /// </summary>
            Photo = 1,
            /// <summary>
            /// 
            /// </summary>
            Article = 5,
            /// <summary>
            /// 
            /// </summary>
            Diary = 7,
            /// <summary>
            /// 
            /// </summary>
            Pic = 2,
            /// <summary>
            /// 
            /// </summary>
            Music = 6,
            /// <summary>
            /// 
            /// </summary>
            AddressList = 4,
            /// <summary>
            /// 
            /// </summary>
            Favorite = 3,
        }
        #region 根据给出的相对地址获取网站绝对地址
        /// <summary>
        /// 根据给出的相对地址获取网站绝对地址
        /// </summary>
        /// <param name="localPath">相对地址</param>
        /// <returns>绝对地址</returns>
        public static string GetWebPath(string localPath)
        {
            string path = HttpContext.Current.Request.ApplicationPath;
            string thisPath;
            string thisLocalPath;
            //如果不是根目录就加上"/" 根目录自己会加"/"
            if (path != "/")
            {
                thisPath = path + "/";
            }
            else
            {
                thisPath = path;
            }
            if (localPath.StartsWith("~/"))
            {
                thisLocalPath = localPath.Substring(2);
            }
            else
            {
                return localPath;
            }
            return thisPath + thisLocalPath;
        }

        #endregion

        #region 获取网站绝对地址
        /// <summary>
        ///  获取网站绝对地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebPath()
        {
            string path = System.Web.HttpContext.Current.Request.ApplicationPath;
            string thisPath;
            //如果不是根目录就加上"/" 根目录自己会加"/"
            if (path != "/")
            {
                thisPath = path + "/";
            }
            else
            {
                thisPath = path;
            }
            return thisPath;
        }
        #endregion

        #region 根据相对路径或绝对路径获取绝对路径
        /// <summary>
        /// 根据相对路径或绝对路径获取绝对路径
        /// </summary>
        /// <param name="localPath">相对路径或绝对路径</param>
        /// <returns>绝对路径</returns>
        public static string GetFilePath(string localPath)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(localPath, @"([A-Za-z]):\\([\S]*)"))
            {
                return localPath;
            }
            else
            {
                return System.Web.HttpContext.Current.Server.MapPath(localPath);
            }
        }
        #endregion

        #region 获取文件相对路径映射的物理路径
        /// <summary>
        /// 获取文件相对路径映射的物理路径
        /// </summary>
        /// <param name="virtualPath">文件的相对路径</param>        
        public static string GetPath(string virtualPath)
        {

            return HttpContext.Current.Server.MapPath(virtualPath);

        }
        #endregion

        #region 获取当前应用程序域
        /// <summary>
        /// 获取当前应用程序域
        /// </summary>
        public static AppDomain CurrentAppDomain
        {
            get
            {
                return Thread.GetDomain();
            }
        }
        #endregion

        #region 获取指定调用层级的方法名
        /// <summary>
        /// 获取指定调用层级的方法名
        /// </summary>
        /// <param name="level">调用的层数</param>        
        public static string GetMethodName(int level)
        {
            //创建一个堆栈跟踪
            StackTrace trace = new StackTrace();

            //获取指定调用层级的方法名
            return trace.GetFrame(level).GetMethod().Name;
        }
        #endregion

        #region 获取换行字符
        /// <summary>
        /// 获取换行字符
        /// </summary>
        public static string NewLine
        {
            get
            {
                return Environment.NewLine;
            }
        }
        #endregion
    }
}
