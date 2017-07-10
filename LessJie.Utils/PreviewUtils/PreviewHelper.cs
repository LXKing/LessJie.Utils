using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LessJie.PreviewUtils
{
    public class PreviewHelper
    {

        public class PreviewResult
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        #region 预览Excel
        /// <summary>
        /// 预览Excel
        /// </summary>
        public static PreviewResult PreviewExcelByres(string physicalPath, string serverPath)
        {
            PreviewResult res = new PreviewResult();
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

        #region 预览Word
        /// <summary>
        /// 预览Word
        /// </summary>
        public static PreviewResult PreviewWordByres(string physicalPath, string serverPath, System.Text.Encoding encoding)
        {
            PreviewResult res = new PreviewResult();
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
    }
}
