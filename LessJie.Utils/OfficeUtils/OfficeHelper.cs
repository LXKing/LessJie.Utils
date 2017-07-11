
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LessJie.OfficeUtils
{
    public class OfficeHelper
    {

        public class OfficeResult
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        #region Excel转成Html
        /// <summary>
        /// Excel转成Html
        /// </summary>
        /// <param name="physicalPath">文件物理路径 绝对路径</param>
        /// <param name="serverPath">文件服务器路径 相对路径</param>
        /// <returns></returns>
        public static OfficeResult ExcelToHtml(string physicalPath, string serverPath)
        {
            OfficeResult res = new OfficeResult();
            try
            {
                Microsoft.Office.Interop.Excel.Application application = null;
                Microsoft.Office.Interop.Excel.Workbook workbook = null;
                application = new Microsoft.Office.Interop.Excel.Application();
                object missing = Type.Missing;
                object trueObject = true;
                application.Visible = false;
                application.DisplayAlerts = false;
                workbook = application.Workbooks.Open(physicalPath, missing, trueObject, missing, missing, missing,
                  missing, missing, missing, missing, missing, missing, missing, missing, missing);
                //Save Excel to Html
                object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
                string htmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
                string outputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
                workbook.SaveAs(outputFile, format, missing, missing, missing,
                         missing, XlSaveAsAccessMode.xlNoChange, missing,
                         missing, missing, missing, missing);

                //            workbook.SaveAs(outputFile, format, missing, missing, missing,
                //missing, missing, missing, missing, missing, missing, Encoding.UTF8, missing, missing, missing, missing);
                workbook.Close();
                application.Quit();
                //return Path.GetDirectoryName(System.Web.HttpContext.Current.Server.UrlDecode(url)) + "\\" + htmlName;
                string previewFile = Path.GetDirectoryName(serverPath) + "\\" + htmlName;
                res.code = 1;
                res.message = previewFile;
                return res;
            }
            catch (Exception ex)
            {
                res.code = 0;
                res.message = ex.Message;
                return res;
            }
        }
        #endregion

        #region Word转成Html
        /// <summary>
        /// Word转成Html
        /// </summary>
        /// <param name="physicalPath">文件物理路径 绝对路径</param>
        /// <param name="serverPath">文件服务器路径 相对路径</param>
        /// <param name="encoding">文件编码格式</param>
        /// <returns></returns>
        public static OfficeResult WordToHtml(string physicalPath, string serverPath, System.Text.Encoding encoding)
        {
            OfficeResult res = new OfficeResult();
            try
            {
                ////转换格式
                //var fileType = PreviewDocument.EncodingType.GetType(physicalPath);
                //if (fileType != Encoding.UTF8)
                //{
                //    PreviewDocument.EncodingType.ChangeEncoding(physicalPath, fileType);
                //}
                Microsoft.Office.Interop.Word._Application application = null;
                Microsoft.Office.Interop.Word._Document doc = null;
                application = new Microsoft.Office.Interop.Word.Application();
                object missing = Type.Missing;
                object trueObject = true;
                application.Visible = false;
                application.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                doc = application.Documents.Open(physicalPath, missing, trueObject, missing, missing, missing,
                  missing, missing, missing, missing, encoding, missing, missing, missing, missing, missing);
                //Save Excel to Html
                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;
                string htmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
                string outputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
                //doc.SaveAs(outputFile, format, missing, missing, missing,
                //         missing, XlSaveAsAccessMode.xlNoChange, missing,
                //         missing, missing, missing, missing);
                doc.SaveAs(outputFile, format, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing, Encoding.UTF8, missing, missing, missing, missing);
                doc.Close();
                application.Quit();
                //return Path.GetDirectoryName(System.Web.HttpContext.Current.Server.UrlDecode(url)) + "//" + htmlName;
                //
                string previewFile = Path.GetDirectoryName(serverPath) + "\\" + htmlName;
                res.code = 1;
                res.message = previewFile;
                return res;

            }
            catch (Exception ex)
            {
                res.code = 0;
                res.message = ex.Message;
                return res;
            }
        }
        #endregion

        #region PPT转成Html
        /// <summary>
        /// PPT转成Html
        /// </summary>
        /// <param name="physicalPath">文件物理路径 绝对路径</param>
        /// <param name="serverPath">文件服务器路径 相对路径</param>
        /// <returns></returns>
        public static OfficeResult PPTToHtml(string physicalPath, string serverPath)
        {
            OfficeResult res = new OfficeResult();
            try
            {
                //-------------------------------------------------
                Microsoft.Office.Interop.PowerPoint.Application ppApp = new Microsoft.Office.Interop.PowerPoint.Application();
                Microsoft.Office.Interop.PowerPoint.Presentation prsPres = ppApp.Presentations.Open(physicalPath, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);
                string htmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
                string outputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
                prsPres.SaveAs(outputFile, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsHTML, MsoTriState.msoTrue);
                prsPres.Close();
                ppApp.Quit();
                //---------------------------------------------------------------
                //return Path.GetDirectoryName(System.Web.HttpContext.Current.Server.UrlDecode(url)) + "\\" + htmlName;
                string previewFile = Path.GetDirectoryName(serverPath) + "\\" + htmlName;
                res.code = 1;
                res.message = previewFile;
                return res;
            }
            catch (Exception ex)
            {
                res.code = 0;
                res.message = ex.Message;
                return res;
            }
        }
        #endregion

        /// <summary>
        /// 防止乱码方法
        /// </summary>
        /// <param name="strFilePath"></param>
        private void TransHTMLEncoding(string strFilePath)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(strFilePath, Encoding.GetEncoding(0));
                string html = sr.ReadToEnd();
                sr.Close();
                html = System.Text.RegularExpressions.Regex.Replace(html, @"<meta[^>]*>", "<meta http-equiv=Content-Type content='text/html; charset=gb2312'>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(strFilePath, false, Encoding.Default);

                sw.Write(html);
                sw.Close();
            }
            catch (Exception ex)
            {
                //Page.RegisterStartupScript("alt", "<script>alert('" + ex.Message + "')</script>");
            }
        }
    }
}
