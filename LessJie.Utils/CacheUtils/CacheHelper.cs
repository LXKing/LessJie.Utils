using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace LessJie.CacheUtils
{
    /// <summary>
    /// Cache工具类
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey">键</param>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey">键</param>
        /// <param name="objObject">Cache值</param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject, TimeSpan Timeout)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, DateTime.MaxValue, Timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void RemoveAllCache(string CacheKey)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(CacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// 添加到缓存中
        /// </summary>
        /// <param name="key">用于引用对象的缓存键。</param>
        /// <param name="value">要插入到缓存中的对象</param>
        /// <param name="minutes">所插入对象将到期并被从缓存中移除的时间</param>
        public static void Insert(string key, object value, double minutes)
        {
            Insert(key, value, null, DateTime.Now.AddMinutes(minutes), TimeSpan.Zero);
        }
        /// <summary>
        /// 添加到缓存中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependencies"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        public static void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration);
        }
        /// <summary>
        /// 得到缓存中缓存的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static T GetCache<T>(string key)
        {
            object obj = HttpRuntime.Cache.Get(key);
            if (obj != null)
            {
                return (T)obj;
            }
            return default(T);
        }
    }
}
