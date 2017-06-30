using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LessJie.Utils.Json
{
    /// <summary>
    /// Json序列化帮助类
    /// </summary>
    public class JsonHelper
    {
        #region Json序列化
        /// <summary>
        /// Json序列化 注：三种序列化后的数据格式可能有所不同，请自行选择
        /// eg：时间序列化后会有所不同 通常采用ObjectToJsonByJSS进行序列化
        /// </summary>
        public class JsonSerialize
        {
            /// <summary>
            /// Json序列化 BY：JavaScriptSerializer
            /// </summary>
            /// <param name="obj">要序列化的对象</param>
            /// <returns>JSON格式的数据</returns>
            public static string ObjectToJsonByJSS(object obj)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(obj);
            }

            /// <summary>
            /// Json序列化 BY：DataContractJsonSerializer
            /// </summary>
            /// <param name="obj">要序列化的对象</param>
            /// <returns>JSON格式的数据</returns>
            public static string ObjectToJsonByDCJS(object obj)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, obj);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            /// <summary>
            /// Json序列化 BY：JsonConvert
            /// </summary>
            /// <param name="obj">要序列化的对象</param>
            /// <returns>JSON格式的数据</returns>
            public static string ObjectToJsonByJC(object obj)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
        }

        #endregion


        #region Json反序列化
        /// <summary>
        /// Json反序列化
        /// </summary>
        public class JsonDeserialize
        {
            /// <summary>
            /// Json反序列化 BY：JavaScriptSerializer
            /// </summary>
            /// <typeparam name="T">反序列化后的对象类型</typeparam>
            /// <param name="input">JSON格式的数据</param>
            /// <returns>反序列化后的对象</returns>
            public static object JsonToObjectByJSS<T>(string input)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Deserialize<T>(input);
            }

            /// <summary>
            /// Json反序列化 BY：DataContractJsonSerializer
            /// </summary>
            /// <param name="jsonString">JSON格式的数据</param>
            /// <param name="obj">反序列化后的对象类型</param>
            /// <returns>反序列化后的对象</returns>
            public static object JsonToObjectByDCJS(string jsonString, object obj)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    return serializer.ReadObject(mStream);
                }

            }

            /// <summary>
            /// Json反序列化 BY：JsonConvert
            /// </summary>
            /// <typeparam name="T">反序列化后的对象类型</typeparam>
            /// <param name="str">JSON格式的数据</param>
            /// <returns>反序列化后的对象</returns>
            public static T JsonToObjectByJC<T>(string str)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            }
        }

        #endregion
    }
}
