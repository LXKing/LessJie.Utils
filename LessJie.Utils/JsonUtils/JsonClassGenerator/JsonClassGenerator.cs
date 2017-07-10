// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using LessJie.JsonUtils.JsonClassGenerator.CodeWriters;

namespace LessJie.JsonUtils.JsonClassGenerator
{
    public class JsonClassGenerator : IJsonClassGeneratorConfig
    {
        private bool _ExplicitDeserialization = false;
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
        public VisibilityTypeEnum InternalVisibility { get; set; }
        /// <summary>
        /// 使用显式的反序列化（过时）
        /// </summary>
        public bool ExplicitDeserialization
        {
            get { return _ExplicitDeserialization; }
            set { _ExplicitDeserialization = false; }
        }
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
        public IJsonClassGeneratorCodeWriter CodeWriter { get; set; }
        /// <summary>
        /// 输出流
        /// </summary>
        public TextWriter OutputStream { get; set; }
        //private TextWriter OutputStream = null;
        /// <summary>
        /// 总是使用空值
        /// </summary>
        public bool AlwaysUseNullableValues { get; set; }
        /// <summary>
        /// 生成的文档和数据的例子
        /// </summary>
        public bool ExamplesInDocumentation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));
        /// <summary>
        /// 
        /// </summary>
        private bool used = false;
        /// <summary>
        /// 
        /// </summary>
        public bool UseNamespaces { get { return Namespace != null; } }
        /// <summary>
        /// 
        /// </summary>
        public void GenerateClasses()
        {
            if (CodeWriter == null) CodeWriter = new CSharpCodeWriter();
            if (ExplicitDeserialization && !(CodeWriter is CSharpCodeWriter)) throw new ArgumentException("明确反序列化是过时的，是由C #提供商只支持。");

            if (used) throw new InvalidOperationException("这种情况jsonclassgenerator已经被使用。请创建一个新实例。");
            used = true;


            var writeToDisk = TargetFolder != null;
            if (writeToDisk && !Directory.Exists(TargetFolder)) Directory.CreateDirectory(TargetFolder);


            JObject[] examples;
            var example = Example.StartsWith("HTTP/") ? Example.Substring(Example.IndexOf("\r\n\r\n")) : Example;
            using (var sr = new StringReader(example))
            using (var reader = new JsonTextReader(sr))
            {
                var json = JToken.ReadFrom(reader);
                if (json is JArray)
                {
                    examples = ((JArray)json).Cast<JObject>().ToArray();
                }
                else if (json is JObject)
                {
                    examples = new[] { (JObject)json };
                }
                else
                {
                    throw new Exception("示例JSON必须是一个JSON数组或JSON对象。");
                }
            }


            Types = new List<JsonType>();
            Names.Add(MainClass);
            var rootType = new JsonType(this, examples[0]);
            rootType.IsRoot = true;
            rootType.AssignName(MainClass);
            GenerateClass(examples, rootType);

            if (writeToDisk)
            {

                var parentFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (writeToDisk && !NoHelperClass && ExplicitDeserialization) System.IO.File.WriteAllBytes(Path.Combine(TargetFolder, "JsonClassHelper.cs"), Properties.Resources.JsonClassHelper);
                if (SingleFile)
                {
                    WriteClassesToFile(Path.Combine(TargetFolder, MainClass + CodeWriter.FileExtension), Types);
                }
                else
                {

                    foreach (var type in Types)
                    {
                        var folder = TargetFolder;
                        if (!UseNestedClasses && !type.IsRoot && SecondaryNamespace != null)
                        {
                            var s = SecondaryNamespace;
                            if (s.StartsWith(Namespace + ".")) s = s.Substring(Namespace.Length + 1);
                            folder = Path.Combine(folder, s);
                            Directory.CreateDirectory(folder);
                        }
                        WriteClassesToFile(Path.Combine(folder, (UseNestedClasses && !type.IsRoot ? MainClass + "." : string.Empty) + type.AssignedName + CodeWriter.FileExtension), new[] { type });
                    }
                }
            }
            else if (OutputStream != null)
            {
                WriteClassesToFile(OutputStream, Types);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="types"></param>
        private void WriteClassesToFile(string path, IEnumerable<JsonType> types)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                WriteClassesToFile(sw, types);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="types"></param>
        private void WriteClassesToFile(TextWriter sw, IEnumerable<JsonType> types)
        {
            var inNamespace = false;
            var rootNamespace = false;

            CodeWriter.WriteFileStart(this, sw);
            foreach (var type in types)
            {
                if (UseNamespaces && inNamespace && rootNamespace != type.IsRoot && SecondaryNamespace != null) { CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace); inNamespace = false; }
                if (UseNamespaces && !inNamespace) { CodeWriter.WriteNamespaceStart(this, sw, type.IsRoot); inNamespace = true; rootNamespace = type.IsRoot; }
                CodeWriter.WriteClass(this, sw, type);
            }
            if (UseNamespaces && inNamespace) CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace);
            CodeWriter.WriteFileEnd(this, sw);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examples"></param>
        /// <param name="type"></param>
        private void GenerateClass(JObject[] examples, JsonType type)
        {
            var jsonFields = new Dictionary<string, JsonType>();
            var fieldExamples = new Dictionary<string, IList<object>>();

            var first = true;

            foreach (var obj in examples)
            {
                foreach (var prop in obj.Properties())
                {
                    JsonType fieldType;
                    var currentType = new JsonType(this, prop.Value);
                    var propName = prop.Name;
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {

                        var commonType = fieldType.GetCommonType(currentType);

                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        var commonType = currentType;
                        if (first) commonType = commonType.MaybeMakeNullable(this);
                        else commonType = commonType.GetCommonType(JsonType.GetNull(this));
                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }
                    var fe = fieldExamples[propName];
                    var val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        var v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                            fe.Add(v);
                    }
                }
                first = false;
            }

            if (UseNestedClasses)
            {
                foreach (var field in jsonFields)
                {
                    Names.Add(field.Key.ToLower());
                }
            }

            foreach (var field in jsonFields)
            {
                var fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                subexamples.Add((JObject)value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key));
                    GenerateClass(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                foreach (var item in (JArray)value)
                                {
                                    if (!(item is JObject)) throw new NotSupportedException("尚未支持非对象数组。");
                                    subexamples.Add((JObject)item);
                                }

                            }
                            else if (value.Type == JTokenType.Object)
                            {
                                foreach (var item in (JObject)value)
                                {
                                    if (!(item.Value is JObject)) throw new NotSupportedException("尚未支持非对象数组。");

                                    subexamples.Add((JObject)item.Value);
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignName(CreateUniqueClassNameFromPlural(field.Key));
                    GenerateClass(subexamples.ToArray(), field.Value.InternalType);
                }
            }

            type.Fields = jsonFields.Select(x => new FieldInfo(this, x.Key, x.Value, UsePascalCase, fieldExamples[x.Key])).ToArray();

            Types.Add(type);

        }
        /// <summary>
        /// 
        /// </summary>
        public IList<JsonType> Types { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        private HashSet<string> Names = new HashSet<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CreateUniqueClassName(string name)
        {
            name = ToTitleCase(name);

            var finalName = name;
            var i = 2;
            while (Names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = name + i.ToString();
                i++;
            }

            Names.Add(finalName);
            return finalName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plural"></param>
        /// <returns></returns>
        private string CreateUniqueClassNameFromPlural(string plural)
        {
            plural = ToTitleCase(plural);
            return CreateUniqueClassName(pluralizationService.Singularize(plural));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string ToTitleCase(string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        public bool HasSecondaryClasses
        {
            get { return Types.Count > 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly string[] FileHeader = new[] { 
            "Generated by LXKing JSON Class Generator",
            "https://github.com/LXKing",
            "849237567@qq.com"
        };
    }
}
