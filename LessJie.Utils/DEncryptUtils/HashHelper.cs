using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LessJie.DEncryptUtils
{
    /// <summary>
    /// 散列 
    /// </summary>
   public class HashHelper
    {
        /// <summary>
        /// 根据string散列10000
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHashID(string key)
        {
            if ( string.IsNullOrWhiteSpace( key ) ) {
                return "default";
            }
            key = key.ToLower();

            int hash;
            int i;
            for (hash = 0, i = 0; i < key.Length; ++i)
            {
                hash += key[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);

            return (Math.Abs(hash) % 10000).ToString();
        }
    }
}
