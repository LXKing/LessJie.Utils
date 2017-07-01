using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LessJie.JsonUtils.JsonClassGenerator;
using LessJie.JsonUtils.JsonClassGenerator.CodeWriters;

namespace LessJie.JsonUtils
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




        /// <summary>
        /// Json生成Class
        /// </summary>
        /// <param name="jsonClassGenerator">JsonClassGenerator对象</param>
        public static void ClassGenerate(JsonClassGeneratorParams jsonClassGeneratorParams, CodeWriterTypeEnum codeWriterTypeEnum)
        {
            try
            {
                var jsonClassGenerator = new JsonClassGenerator.JsonClassGenerator();
                //if (jsonClassGenerator == null) return;
                switch (codeWriterTypeEnum)
                {
                    case CodeWriterTypeEnum.CSharpCodeWriter:
                        jsonClassGenerator.CodeWriter = new CSharpCodeWriter();
                        break;
                    case CodeWriterTypeEnum.VisualBasicCodeWriter:
                        jsonClassGenerator.CodeWriter = new VisualBasicCodeWriter();
                        break;
                    case CodeWriterTypeEnum.TypeScriptCodeWriter:
                        jsonClassGenerator.CodeWriter = new TypeScriptCodeWriter();
                        break;
                    default:
                        return;
                }

                if (jsonClassGenerator.SecondaryNamespace.Contains("JsonTypes") || jsonClassGenerator.SecondaryNamespace == string.Empty)
                {
                    jsonClassGenerator.SecondaryNamespace = jsonClassGenerator.Namespace == string.Empty ? string.Empty : jsonClassGenerator.Namespace + "." + jsonClassGenerator.MainClass + "JsonTypes";
                }

                var writer = jsonClassGenerator.CodeWriter;

                jsonClassGenerator.UsePascalCase = !(writer is TypeScriptCodeWriter);
                jsonClassGenerator.ExplicitDeserialization = writer is CSharpCodeWriter;

                jsonClassGenerator.GenerateClasses();
            }
            catch (Exception ex)
            {
                throw new Exception("无法生成代码： " + ex.Message);
            }
        }




        public class JsonClassGeneratorParams
        {
            /// <summary>
            /// Json数据
            /// </summary>
            public string Example { get; set; }
            /// <summary>
            /// 输出文件夹
            /// </summary>
            public string TargetFolder { get; set; }
            /// <summary>
            /// 命名空间
            /// </summary>
            public string Namespace { get; set; }
            /// <summary>
            /// 第二命名空间
            /// </summary>
            public string SecondaryNamespace { get; set; }
            /// <summary>
            /// 使用性能 true/领域 false
            /// </summary>
            public bool UseProperties { get; set; }
            /// <summary>
            /// 类的访问级别
            /// </summary>
            public VisibilityTypeEnum VisibilityType { get; set; }
            /// <summary>
            /// 不生成辅助类
            /// </summary>
            public bool NoHelperClass { get; set; }
            /// <summary>
            /// 主类名
            /// </summary>
            public string MainClass { get; set; }
            /// <summary>
            /// 使用帕斯卡案例
            /// </summary>
            public bool UsePascalCase { get; set; }
            /// <summary>
            /// 使用嵌套类
            /// </summary>
            public bool UseNestedClasses { get; set; }
            /// <summary>
            /// 应用混淆排除属性
            /// </summary>
            public bool ApplyObfuscationAttributes { get; set; }
            /// <summary>
            /// 生成单文件
            /// </summary>
            public bool SingleFile { get; set; }
            /// <summary>
            /// 生成代码类型
            /// </summary>
            public CodeWriterTypeEnum CodeWriterType { get; set; }
            /// <summary>
            /// 输出流
            /// </summary>
            public TextWriter OutputStream { get; set; }
            /// <summary>
            /// 总是使用空值
            /// </summary>
            public bool AlwaysUseNullableValues { get; set; }
            /// <summary>
            /// 生成的文档和数据的例子
            /// </summary>
            public bool ExamplesInDocumentation { get; set; }
        }

    }
}
