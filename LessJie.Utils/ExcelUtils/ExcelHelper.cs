using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Text;

namespace LessJie.ExcelUtils
{
    /// <summary>
    /// DataTable数据导出类型
    /// </summary>
    public enum ExcelType
    {
        /// <summary>
        /// 返回可下载的文件流
        /// </summary>
        FileStream,
        /// <summary>
        /// 可读模式
        /// </summary>
        ReadMode,
    }
    /// <summary>
    /// Excel操作类
    /// </summary>
    /// Microsoft Excel 11.0 Object Library
    public class ExcelHelper
    {
        /// <summary>
        /// Excel启动之前时间
        /// </summary>
        private static DateTime beforeTime;
        /// <summary>
        /// Excel启动之后时间
        /// </summary>
        private static DateTime afterTime;

        #region  清理过时的Excel文件
        /// <summary>
        /// 清理过时的Excel文件
        /// </summary>
        /// <param name="FilePath"></param>
        private static void ClearExcelFile(string FilePath)
        {
            String[] Files = System.IO.Directory.GetFiles(FilePath);
            if (Files.Length > 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        System.IO.File.Delete(Files[i]);
                    }
                    catch
                    {
                    }
                }
            }
        }
        #endregion

        #region Kill Excel进程
        /// <summary>
        /// 结束Excel进程
        /// </summary>
        public static void KillExcelProcess()
        {
            Process[] myProcesses;
            DateTime startTime;
            myProcesses = Process.GetProcessesByName("Excel");

            //得不到Excel进程ID，暂时只能判断进程启动时间
            foreach (Process myProcess in myProcesses)
            {
                startTime = myProcess.StartTime;
                if (startTime > beforeTime && startTime < afterTime)
                {
                    myProcess.Kill();
                }
            }
        }
        #endregion

        #region 数据导出至Excel文件
        /// <summary>
        /// DataTable数据导出至Excel文件
        /// </summary>
        /// <param name="dtData">DataTable数据</param>
        /// <param name="et"> 1.FileStream 导出Excel文件，自动返回可下载的文件流  2.ReadMode 导出Excel文件，转换为可读模式
        /// <param name="customFileName"></param>
        public static void DataTableToExcel(System.Data.DataTable dtData, ExcelType et)
        {
            HttpContext curContext = HttpContext.Current;
            StringWriter strWriter = null;
            HtmlTextWriter htmlWriter = null;

            if (dtData != null)
            {
                switch (et)
                {
                    case ExcelType.FileStream:
                        GridView gvExport = null;
                        curContext.Response.ContentType = "application/vnd.ms-excel";
                        curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                        curContext.Response.Charset = "utf-8";
                        strWriter = new StringWriter();
                        htmlWriter = new HtmlTextWriter(strWriter);
                        gvExport = new GridView();
                        gvExport.DataSource = dtData.DefaultView;
                        gvExport.AllowPaging = false;
                        gvExport.DataBind();
                        gvExport.RenderControl(htmlWriter);
                        curContext.Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=gb2312\"/>" + strWriter.ToString());
                        curContext.Response.End();
                        break;
                    case ExcelType.ReadMode:
                        DataGrid dgExport = null;
                        curContext.Response.ContentType = "application/vnd.ms-excel";
                        curContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
                        curContext.Response.Charset = "";
                        strWriter = new StringWriter();
                        htmlWriter = new HtmlTextWriter(strWriter);
                        dgExport = new DataGrid();
                        dgExport.DataSource = dtData.DefaultView;
                        dgExport.AllowPaging = false;
                        dgExport.DataBind();
                        dgExport.RenderControl(htmlWriter);
                        curContext.Response.Write(strWriter.ToString());
                        curContext.Response.End();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// DataTable数据导出至Excel文件，并自定义文件名
        /// </summary>
        public static void DataTableToExcel(System.Data.DataTable dtData, String FileName)
        {
            GridView dgExport = null;
            HttpContext curContext = HttpContext.Current;
            StringWriter strWriter = null;
            HtmlTextWriter htmlWriter = null;

            if (dtData != null)
            {
                HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8);
                curContext.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8) + ".xls");
                curContext.Response.ContentType = "application nd.ms-excel";
                curContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
                curContext.Response.Charset = "GB2312";
                strWriter = new StringWriter();
                htmlWriter = new HtmlTextWriter(strWriter);
                dgExport = new GridView();
                dgExport.DataSource = dtData.DefaultView;
                dgExport.AllowPaging = false;
                dgExport.DataBind();
                dgExport.RenderControl(htmlWriter);
                curContext.Response.Write(strWriter.ToString());
                curContext.Response.End();
            }
        }

        /// <summary>
        /// GridView数据导出至Excel文件
        /// </summary>
        /// <param name="obj"></param>
        public static void GridViewToExcel(GridView obj)
        {
            try
            {
                string style = "";
                if (obj.Rows.Count > 0)
                {
                    style = @"<style> .text { mso-number-format:\@; } </script> ";
                }
                else
                {
                    style = "no data.";
                }

                HttpContext.Current.Response.ClearContent();
                DateTime dt = DateTime.Now;
                string filename = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=ExportData" + filename + ".xls");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                obj.RenderControl(htw);
                HttpContext.Current.Response.Write(style);
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
            }
            catch
            {

            }
        }

        /// <summary>
        /// GridView数据导出至Excel文件，并自定义文件名
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="fileName"></param>
        public static void GridViewToExcel(GridView gv, string fileName)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.Charset = "utf-8";


            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //  Create a form to contain the grid
                    Table table = new Table();
                    table.GridLines = GridLines.Both;  //单元格之间添加实线

                    //  add the header row to the table
                    if (gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //  add each of the data rows to the table
                    foreach (GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //  add the footer row to the table
                    if (gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);
                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }

        /// <summary>
        /// 用文字替换任何包含的控件
        /// </summary>
        /// <param name="control"></param>
        private static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }

        /// <summary>
        /// 将整个网页导出来Excel
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="FileName"></param>
        protected void HtmlToExcel(string strContent, string FileName)
        {
            FileName = FileName + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Charset = "gb2312";
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //this.Page.EnableViewState = false; 
            // 添加头信息，为"文件下载/另存为"对话框指定默认文件名 
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");
            // 把文件流发送到客户端 
            HttpContext.Current.Response.Write("<html><head><meta http-equiv=Content-Type content=\"text/html; charset=utf-8\">");
            HttpContext.Current.Response.Write(strContent);

            HttpContext.Current.Response.Write("</body></html>");
            // 停止页面的执行 
            //Response.End();
        }

        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="Table">DataTable对象</param>
        /// <param name="ExcelFilePath">Excel文件路径</param>
        public static bool OutputToExcel(DataTable Table, string ExcelFilePath)
        {
            if (File.Exists(ExcelFilePath))
            {
                throw new Exception("该文件已经存在！");
            }

            if ((Table.TableName.Trim().Length == 0) || (Table.TableName.ToLower() == "table"))
            {
                Table.TableName = "Sheet1";
            }

            //数据表的列数
            int ColCount = Table.Columns.Count;

            //用于记数，实例化参数时的序号
            int i = 0;

            //创建参数
            OleDbParameter[] para = new OleDbParameter[ColCount];

            //创建表结构的SQL语句
            string TableStructStr = @"Create Table " + Table.TableName + "(";

            //连接字符串
            string connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelFilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection objConn = new OleDbConnection(connString);

            //创建表结构
            OleDbCommand objCmd = new OleDbCommand();

            //数据类型集合
            ArrayList DataTypeList = new ArrayList();
            DataTypeList.Add("System.Decimal");
            DataTypeList.Add("System.Double");
            DataTypeList.Add("System.Int16");
            DataTypeList.Add("System.Int32");
            DataTypeList.Add("System.Int64");
            DataTypeList.Add("System.Single");

            //遍历数据表的所有列，用于创建表结构
            foreach (DataColumn col in Table.Columns)
            {
                //如果列属于数字列，则设置该列的数据类型为double
                if (DataTypeList.IndexOf(col.DataType.ToString()) >= 0)
                {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.Double);
                    objCmd.Parameters.Add(para[i]);

                    //如果是最后一列
                    if (i + 1 == ColCount)
                    {
                        TableStructStr += col.ColumnName + " double)";
                    }
                    else
                    {
                        TableStructStr += col.ColumnName + " double,";
                    }
                }
                else
                {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.VarChar);
                    objCmd.Parameters.Add(para[i]);

                    //如果是最后一列
                    if (i + 1 == ColCount)
                    {
                        TableStructStr += col.ColumnName + " varchar)";
                    }
                    else
                    {
                        TableStructStr += col.ColumnName + " varchar,";
                    }
                }
                i++;
            }

            //创建Excel文件及文件结构
            try
            {
                objCmd.Connection = objConn;
                objCmd.CommandText = TableStructStr;

                if (objConn.State == ConnectionState.Closed)
                {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            //插入记录的SQL语句
            string InsertSql_1 = "Insert into " + Table.TableName + " (";
            string InsertSql_2 = " Values (";
            string InsertSql = "";

            //遍历所有列，用于插入记录，在此创建插入记录的SQL语句
            for (int colID = 0; colID < ColCount; colID++)
            {
                if (colID + 1 == ColCount)  //最后一列
                {
                    InsertSql_1 += Table.Columns[colID].ColumnName + ")";
                    InsertSql_2 += "@" + Table.Columns[colID].ColumnName + ")";
                }
                else
                {
                    InsertSql_1 += Table.Columns[colID].ColumnName + ",";
                    InsertSql_2 += "@" + Table.Columns[colID].ColumnName + ",";
                }
            }

            InsertSql = InsertSql_1 + InsertSql_2;

            //遍历数据表的所有数据行
            for (int rowID = 0; rowID < Table.Rows.Count; rowID++)
            {
                for (int colID = 0; colID < ColCount; colID++)
                {
                    if (para[colID].DbType == DbType.Double && Table.Rows[rowID][colID].ToString().Trim() == "")
                    {
                        para[colID].Value = 0;
                    }
                    else
                    {
                        para[colID].Value = Table.Rows[rowID][colID].ToString().Trim();
                    }
                }
                try
                {
                    objCmd.CommandText = InsertSql;
                    objCmd.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    string str = exp.Message;
                }
            }
            try
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return true;
        }

        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="Table">DataTable对象</param>
        /// <param name="Columns">要导出的数据列集合</param>
        /// <param name="ExcelFilePath">Excel文件路径</param>
        public static bool OutputToExcel(DataTable Table, ArrayList Columns, string ExcelFilePath)
        {
            if (File.Exists(ExcelFilePath))
            {
                throw new Exception("该文件已经存在！");
            }

            //如果数据列数大于表的列数，取数据表的所有列
            if (Columns.Count > Table.Columns.Count)
            {
                for (int s = Table.Columns.Count + 1; s <= Columns.Count; s++)
                {
                    Columns.RemoveAt(s);   //移除数据表列数后的所有列
                }
            }

            //遍历所有的数据列，如果有数据列的数据类型不是 DataColumn，则将它移除
            DataColumn column = new DataColumn();
            for (int j = 0; j < Columns.Count; j++)
            {
                try
                {
                    column = (DataColumn)Columns[j];
                }
                catch (Exception)
                {
                    Columns.RemoveAt(j);
                }
            }
            if ((Table.TableName.Trim().Length == 0) || (Table.TableName.ToLower() == "table"))
            {
                Table.TableName = "Sheet1";
            }

            //数据表的列数
            int ColCount = Columns.Count;

            //创建参数
            OleDbParameter[] para = new OleDbParameter[ColCount];

            //创建表结构的SQL语句
            string TableStructStr = @"Create Table " + Table.TableName + "(";

            //连接字符串
            string connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelFilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection objConn = new OleDbConnection(connString);

            //创建表结构
            OleDbCommand objCmd = new OleDbCommand();

            //数据类型集合
            ArrayList DataTypeList = new ArrayList();
            DataTypeList.Add("System.Decimal");
            DataTypeList.Add("System.Double");
            DataTypeList.Add("System.Int16");
            DataTypeList.Add("System.Int32");
            DataTypeList.Add("System.Int64");
            DataTypeList.Add("System.Single");

            DataColumn col = new DataColumn();

            //遍历数据表的所有列，用于创建表结构
            for (int k = 0; k < ColCount; k++)
            {
                col = (DataColumn)Columns[k];

                //列的数据类型是数字型
                if (DataTypeList.IndexOf(col.DataType.ToString().Trim()) >= 0)
                {
                    para[k] = new OleDbParameter("@" + col.Caption.Trim(), OleDbType.Double);
                    objCmd.Parameters.Add(para[k]);

                    //如果是最后一列
                    if (k + 1 == ColCount)
                    {
                        TableStructStr += col.Caption.Trim() + " Double)";
                    }
                    else
                    {
                        TableStructStr += col.Caption.Trim() + " Double,";
                    }
                }
                else
                {
                    para[k] = new OleDbParameter("@" + col.Caption.Trim(), OleDbType.VarChar);
                    objCmd.Parameters.Add(para[k]);

                    //如果是最后一列
                    if (k + 1 == ColCount)
                    {
                        TableStructStr += col.Caption.Trim() + " VarChar)";
                    }
                    else
                    {
                        TableStructStr += col.Caption.Trim() + " VarChar,";
                    }
                }
            }

            //创建Excel文件及文件结构
            try
            {
                objCmd.Connection = objConn;
                objCmd.CommandText = TableStructStr;

                if (objConn.State == ConnectionState.Closed)
                {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            //插入记录的SQL语句
            string InsertSql_1 = "Insert into " + Table.TableName + " (";
            string InsertSql_2 = " Values (";
            string InsertSql = "";

            //遍历所有列，用于插入记录，在此创建插入记录的SQL语句
            for (int colID = 0; colID < ColCount; colID++)
            {
                if (colID + 1 == ColCount)  //最后一列
                {
                    InsertSql_1 += Columns[colID].ToString().Trim() + ")";
                    InsertSql_2 += "@" + Columns[colID].ToString().Trim() + ")";
                }
                else
                {
                    InsertSql_1 += Columns[colID].ToString().Trim() + ",";
                    InsertSql_2 += "@" + Columns[colID].ToString().Trim() + ",";
                }
            }

            InsertSql = InsertSql_1 + InsertSql_2;

            //遍历数据表的所有数据行
            DataColumn DataCol = new DataColumn();
            for (int rowID = 0; rowID < Table.Rows.Count; rowID++)
            {
                for (int colID = 0; colID < ColCount; colID++)
                {
                    //因为列不连续，所以在取得单元格时不能用行列编号，列需得用列的名称
                    DataCol = (DataColumn)Columns[colID];
                    if (para[colID].DbType == DbType.Double && Table.Rows[rowID][DataCol.Caption].ToString().Trim() == "")
                    {
                        para[colID].Value = 0;
                    }
                    else
                    {
                        para[colID].Value = Table.Rows[rowID][DataCol.Caption].ToString().Trim();
                    }
                }
                try
                {
                    objCmd.CommandText = InsertSql;
                    objCmd.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    string str = exp.Message;
                }
            }
            try
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return true;
        }

        /// <summary>
        /// 导出Grid的数据(全部)到Excel
        /// 字段全部为BoundField类型时可用
        /// 要是字段为TemplateField模板型时就取不到数据
        /// </summary>
        /// <param name="grid">grid的ID</param>
        /// <param name="dt">数据源</param>
        /// <param name="excelFileName">要导出Excel的文件名</param>
        public static void OutputExcel(GridView grid, DataTable dt, string excelFileName)
        {
            Page page = (Page)HttpContext.Current.Handler;
            page.Response.Clear();
            string fileName = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(excelFileName));
            page.Response.AddHeader("Content-Disposition", "attachment:filename=" + fileName + ".xls");
            page.Response.ContentType = "application/vnd.ms-excel";
            page.Response.Charset = "utf-8";

            StringBuilder s = new StringBuilder();
            s.Append("<HTML><HEAD><TITLE>" + fileName + "</TITLE><META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>");

            int count = grid.Columns.Count;

            s.Append("<table border=1>");
            s.AppendLine("<tr>");
            for (int i = 0; i < count; i++)
            {

                if (grid.Columns[i].GetType() == typeof(BoundField))
                    s.Append("<td>" + grid.Columns[i].HeaderText + "</td>");

                //s.Append("<td>" + grid.Columns[i].HeaderText + "</td>");

            }
            s.Append("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                s.AppendLine("<tr>");
                for (int n = 0; n < count; n++)
                {
                    if (grid.Columns[n].Visible && grid.Columns[n].GetType() == typeof(BoundField))
                        s.Append("<td>" + dr[((BoundField)grid.Columns[n]).DataField].ToString() + "</td>");

                }
                s.AppendLine("</tr>");
            }

            s.Append("</table>");
            s.Append("</body></html>");

            page.Response.BinaryWrite(System.Text.Encoding.GetEncoding("utf-8").GetBytes(s.ToString()));
            page.Response.End();
        }

        #endregion

        /// <summary>
        /// 获取Excel文件数据表列表
        /// </summary>
        public static ArrayList GetExcelTables(string ExcelFileName)
        {
            DataTable dt = new DataTable();
            ArrayList TablesList = new ArrayList();
            if (File.Exists(ExcelFileName))
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + ExcelFileName))
                {
                    try
                    {
                        conn.Open();
                        dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }

                    //获取数据表个数
                    int tablecount = dt.Rows.Count;
                    for (int i = 0; i < tablecount; i++)
                    {
                        string tablename = dt.Rows[i][2].ToString().Trim().TrimEnd('$');
                        if (TablesList.IndexOf(tablename) < 0)
                        {
                            TablesList.Add(tablename);
                        }
                    }
                }
            }
            return TablesList;
        }

        /// <summary>
        /// 将Excel文件导出至DataTable(第一行作为表头)
        /// </summary>
        /// <param name="ExcelFilePath">Excel文件路径</param>
        /// <param name="TableName">数据表名，如果数据表名错误，默认为第一个数据表名</param>
        public static DataTable InputFromExcel(string ExcelFilePath, string TableName)
        {
            if (!File.Exists(ExcelFilePath))
            {
                throw new Exception("Excel文件不存在！");
            }

            //如果数据表名不存在，则数据表名为Excel文件的第一个数据表
            ArrayList TableList = new ArrayList();
            TableList = GetExcelTables(ExcelFilePath);

            if (TableList.IndexOf(TableName) < 0)
            {
                TableName = TableList[0].ToString().Trim();
            }

            DataTable table = new DataTable();
            OleDbConnection dbcon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ExcelFilePath + ";Extended Properties=Excel 8.0");
            OleDbCommand cmd = new OleDbCommand("select * from [" + TableName + "$]", dbcon);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

            try
            {
                if (dbcon.State == ConnectionState.Closed)
                {
                    dbcon.Open();
                }
                adapter.Fill(table);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (dbcon.State == ConnectionState.Open)
                {
                    dbcon.Close();
                }
            }
            return table;
        }

        /// <summary>
        /// 获取Excel文件指定数据表的数据列表
        /// </summary>
        /// <param name="ExcelFileName">Excel文件名</param>
        /// <param name="TableName">数据表名</param>
        public static ArrayList GetExcelTableColumns(string ExcelFileName, string TableName)
        {
            DataTable dt = new DataTable();
            ArrayList ColsList = new ArrayList();
            if (File.Exists(ExcelFileName))
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + ExcelFileName))
                {
                    conn.Open();
                    dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, TableName, null });

                    //获取列个数
                    int colcount = dt.Rows.Count;
                    for (int i = 0; i < colcount; i++)
                    {
                        string colname = dt.Rows[i]["Column_Name"].ToString().Trim();
                        ColsList.Add(colname);
                    }
                }
            }
            return ColsList;
        }
    }

    #region 操作EXCEL导出数据报表的类
    //public class DataToExcel
    //{
    //    public DataToExcel()
    //    {
    //    }

    //    #region 操作EXCEL的一个类(需要Excel.dll支持)

    //    private int titleColorindex = 15;
    //    / <summary>
    //    / 标题背景色
    //    / </summary>
    //    public int TitleColorIndex
    //    {
    //        set { titleColorindex = value; }
    //        get { return titleColorindex; }
    //    }

    //    private DateTime beforeTime;			//Excel启动之前时间
    //    private DateTime afterTime;				//Excel启动之后时间

    //    #region 创建一个Excel示例
    //    / <summary>
    //    / 创建一个Excel示例
    //    / </summary>
    //    public void CreateExcel()
    //    {
    //        Excel.Application excel = new Excel.Application();
    //        excel.Application.Workbooks.Add(true);
    //        excel.Cells[1, 1] = "第1行第1列";
    //        excel.Cells[1, 2] = "第1行第2列";
    //        excel.Cells[2, 1] = "第2行第1列";
    //        excel.Cells[2, 2] = "第2行第2列";
    //        excel.Cells[3, 1] = "第3行第1列";
    //        excel.Cells[3, 2] = "第3行第2列";

    //        //保存
    //        excel.ActiveWorkbook.SaveAs("./tt.xls", XlFileFormat.xlExcel9795, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
    //        //打开显示
    //        excel.Visible = true;
    //        //			excel.Quit();
    //        //			excel=null;            
    //        //			GC.Collect();//垃圾回收
    //    }
    //    #endregion

    //    #region 将DataTable的数据导出显示为报表
    //    / <summary>
    //    / 将DataTable的数据导出显示为报表
    //    / </summary>
    //    / <param name = "dt" > 要导出的数据 </ param >
    //    / < param name="strTitle">导出报表的标题</param>
    //    / <param name = "FilePath" > 保存文件的路径 </ param >
    //    / < returns ></ returns >
    //    public string OutputExcel(System.Data.DataTable dt, string strTitle, string FilePath)
    //    {
    //        beforeTime = DateTime.Now;

    //        Excel.Application excel;
    //        Excel._Workbook xBk;
    //        Excel._Worksheet xSt;

    //        int rowIndex = 4;
    //        int colIndex = 1;

    //        excel = new Excel.ApplicationClass();
    //        xBk = excel.Workbooks.Add(true);
    //        xSt = (Excel._Worksheet)xBk.ActiveSheet;

    //        //取得列标题			
    //        foreach (DataColumn col in dt.Columns)
    //        {
    //            colIndex++;
    //            excel.Cells[4, colIndex] = col.ColumnName;

    //            //设置标题格式为居中对齐
    //            xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).Font.Bold = true;
    //            xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;
    //            xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).Select();
    //            xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).Interior.ColorIndex = titleColorindex;//19;//设置为浅黄色，共计有56种
    //        }


    //        //取得表格中的数据			
    //        foreach (DataRow row in dt.Rows)
    //        {
    //            rowIndex++;
    //            colIndex = 1;
    //            foreach (DataColumn col in dt.Columns)
    //            {
    //                colIndex++;
    //                if (col.DataType == System.Type.GetType("System.DateTime"))
    //                {
    //                    excel.Cells[rowIndex, colIndex] = (Convert.ToDateTime(row[col.ColumnName].ToString())).ToString("yyyy-MM-dd");
    //                    xSt.get_Range(excel.Cells[rowIndex, colIndex], excel.Cells[rowIndex, colIndex]).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//设置日期型的字段格式为居中对齐
    //                }
    //                else
    //                    if (col.DataType == System.Type.GetType("System.String"))
    //                {
    //                    excel.Cells[rowIndex, colIndex] = "'" + row[col.ColumnName].ToString();
    //                    xSt.get_Range(excel.Cells[rowIndex, colIndex], excel.Cells[rowIndex, colIndex]).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//设置字符型的字段格式为居中对齐
    //                }
    //                else
    //                {
    //                    excel.Cells[rowIndex, colIndex] = row[col.ColumnName].ToString();
    //                }
    //            }
    //        }

    //        //加载一个合计行			
    //        int rowSum = rowIndex + 1;
    //        int colSum = 2;
    //        excel.Cells[rowSum, 2] = "合计";
    //        xSt.get_Range(excel.Cells[rowSum, 2], excel.Cells[rowSum, 2]).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
    //        //设置选中的部分的颜色			
    //        xSt.get_Range(excel.Cells[rowSum, colSum], excel.Cells[rowSum, colIndex]).Select();
    //        //xSt.get_Range(excel.Cells[rowSum,colSum],excel.Cells[rowSum,colIndex]).Interior.ColorIndex =Assistant.GetConfigInt("ColorIndex");// 1;//设置为浅黄色，共计有56种

    //        //取得整个报表的标题			
    //        excel.Cells[2, 2] = strTitle;

    //        //设置整个报表的标题格式			
    //        xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, 2]).Font.Bold = true;
    //        xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, 2]).Font.Size = 22;

    //        //设置报表表格为最适应宽度			
    //        xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, colIndex]).Select();
    //        xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, colIndex]).Columns.AutoFit();

    //        //设置整个报表的标题为跨列居中			
    //        xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, colIndex]).Select();
    //        xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, colIndex]).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenterAcrossSelection;

    //        //绘制边框			
    //        xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, colIndex]).Borders.LineStyle = 1;
    //        xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, 2]).Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThick;//设置左边线加粗
    //        xSt.get_Range(excel.Cells[4, 2], excel.Cells[4, colIndex]).Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThick;//设置上边线加粗
    //        xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[rowSum, colIndex]).Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThick;//设置右边线加粗
    //        xSt.get_Range(excel.Cells[rowSum, 2], excel.Cells[rowSum, colIndex]).Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThick;//设置下边线加粗



    //        afterTime = DateTime.Now;

    //        //显示效果			
    //        //excel.Visible=true;			
    //        //excel.Sheets[0] = "sss";

    //        ClearFile(FilePath);
    //        string filename = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".xls";
    //        excel.ActiveWorkbook.SaveAs(FilePath + filename, Excel.XlFileFormat.xlExcel9795, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);

    //        //wkbNew.SaveAs strBookName;
    //        //excel.Save(strExcelFileName);

    //        #region  结束Excel进程

    //        //需要对Excel的DCOM对象进行配置:dcomcnfg


    //        //excel.Quit();
    //        //excel=null;            

    //        xBk.Close(null, null, null);
    //        excel.Workbooks.Close();
    //        excel.Quit();


    //        //注意：这里用到的所有Excel对象都要执行这个操作，否则结束不了Excel进程
    //        //			if(rng != null)
    //        //			{
    //        //				System.Runtime.InteropServices.Marshal.ReleaseComObject(rng);
    //        //				rng = null;
    //        //			}
    //        //			if(tb != null)
    //        //			{
    //        //				System.Runtime.InteropServices.Marshal.ReleaseComObject(tb);
    //        //				tb = null;
    //        //			}
    //        if (xSt != null)
    //        {
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(xSt);
    //            xSt = null;
    //        }
    //        if (xBk != null)
    //        {
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(xBk);
    //            xBk = null;
    //        }
    //        if (excel != null)
    //        {
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
    //            excel = null;
    //        }
    //        GC.Collect();//垃圾回收
    //        #endregion

    //        return filename;

    //    }
    //    #endregion

    //    #region Kill Excel进程

    //    / <summary>
    //    / 结束Excel进程
    //    / </summary>
    //    public void KillExcelProcess()
    //    {
    //        Process[] myProcesses;
    //        DateTime startTime;
    //        myProcesses = Process.GetProcessesByName("Excel");

    //        得不到Excel进程ID，暂时只能判断进程启动时间
    //        foreach (Process myProcess in myProcesses)
    //        {
    //            startTime = myProcess.StartTime;
    //            if (startTime > beforeTime && startTime < afterTime)
    //            {
    //                myProcess.Kill();
    //            }
    //        }
    //    }
    //    #endregion

    //    #endregion

    //    #region 将DataTable的数据导出显示为报表(不使用Excel对象，使用COM.Excel)

    //    #region 使用示例
    //    /*使用示例：
    //     * DataSet ds=(DataSet)Session["AdBrowseHitDayList"];
    //        string ExcelFolder=Assistant.GetConfigString("ExcelFolder");
    //        string FilePath=Server.MapPath(".")+"\\"+ExcelFolder+"\\";

    //        生成列的中文对应表
    //        Hashtable nameList = new Hashtable();
    //        nameList.Add("ADID", "广告编码");
    //        nameList.Add("ADName", "广告名称");
    //        nameList.Add("year", "年");
    //        nameList.Add("month", "月");
    //        nameList.Add("browsum", "显示数");
    //        nameList.Add("hitsum", "点击数");
    //        nameList.Add("BrowsinglIP", "独立IP显示");
    //        nameList.Add("HitsinglIP", "独立IP点击");
    //        利用excel对象
    //        DataToExcel dte=new DataToExcel();
    //        string filename="";
    //        try
    //        {			
    //            if(ds.Tables[0].Rows.Count>0)
    //            {
    //                filename=dte.DataExcel(ds.Tables[0],"标题",FilePath,nameList);
    //            }
    //        }
    //        catch
    //        {
    //            dte.KillExcelProcess();
    //        }

    //        if(filename!="")
    //        {
    //            Response.Redirect(ExcelFolder+"\\"+filename,true);
    //        }
    //     * 
    //     * */

    //    #endregion

    //    / <summary>
    //    / 将DataTable的数据导出显示为报表(不使用Excel对象)
    //    / </summary>
    //    / <param name = "dt" > 数据DataTable </ param >
    //    / < param name="strTitle">标题</param>
    //    / <param name = "FilePath" > 生成文件的路径 </ param >
    //    / < param name="nameList"></param>
    //    / <returns></returns>
    //    public string DataExcel(System.Data.DataTable dt, string strTitle, string FilePath, Hashtable nameList)
    //    {
    //        COM.Excel.cExcelFile excel = new COM.Excel.cExcelFile();
    //        ClearFile(FilePath);
    //        string filename = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".xls";
    //        excel.CreateFile(FilePath + filename);
    //        excel.PrintGridLines = false;

    //        COM.Excel.cExcelFile.MarginTypes mt1 = COM.Excel.cExcelFile.MarginTypes.xlsTopMargin;
    //        COM.Excel.cExcelFile.MarginTypes mt2 = COM.Excel.cExcelFile.MarginTypes.xlsLeftMargin;
    //        COM.Excel.cExcelFile.MarginTypes mt3 = COM.Excel.cExcelFile.MarginTypes.xlsRightMargin;
    //        COM.Excel.cExcelFile.MarginTypes mt4 = COM.Excel.cExcelFile.MarginTypes.xlsBottomMargin;

    //        double height = 1.5;
    //        excel.SetMargin(ref mt1, ref height);
    //        excel.SetMargin(ref mt2, ref height);
    //        excel.SetMargin(ref mt3, ref height);
    //        excel.SetMargin(ref mt4, ref height);

    //        COM.Excel.cExcelFile.FontFormatting ff = COM.Excel.cExcelFile.FontFormatting.xlsNoFormat;
    //        string font = "宋体";
    //        short fontsize = 9;
    //        excel.SetFont(ref font, ref fontsize, ref ff);

    //        byte b1 = 1,
    //            b2 = 12;
    //        short s3 = 12;
    //        excel.SetColumnWidth(ref b1, ref b2, ref s3);

    //        string header = "页眉";
    //        string footer = "页脚";
    //        excel.SetHeader(ref header);
    //        excel.SetFooter(ref footer);


    //        COM.Excel.cExcelFile.ValueTypes vt = COM.Excel.cExcelFile.ValueTypes.xlsText;
    //        COM.Excel.cExcelFile.CellFont cf = COM.Excel.cExcelFile.CellFont.xlsFont0;
    //        COM.Excel.cExcelFile.CellAlignment ca = COM.Excel.cExcelFile.CellAlignment.xlsCentreAlign;
    //        COM.Excel.cExcelFile.CellHiddenLocked chl = COM.Excel.cExcelFile.CellHiddenLocked.xlsNormal;

    //        // 报表标题
    //        int cellformat = 1;
    //        //			int rowindex = 1,colindex = 3;					
    //        //			object title = (object)strTitle;
    //        //			excel.WriteValue(ref vt, ref cf, ref ca, ref chl,ref rowindex,ref colindex,ref title,ref cellformat);

    //        int rowIndex = 1;//起始行
    //        int colIndex = 0;



    //        //取得列标题				
    //        foreach (DataColumn colhead in dt.Columns)
    //        {
    //            colIndex++;
    //            string name = colhead.ColumnName.Trim();
    //            object namestr = (object)name;
    //            IDictionaryEnumerator Enum = nameList.GetEnumerator();
    //            while (Enum.MoveNext())
    //            {
    //                if (Enum.Key.ToString().Trim() == name)
    //                {
    //                    namestr = Enum.Value;
    //                }
    //            }
    //            excel.WriteValue(ref vt, ref cf, ref ca, ref chl, ref rowIndex, ref colIndex, ref namestr, ref cellformat);
    //        }

    //        //取得表格中的数据			
    //        foreach (DataRow row in dt.Rows)
    //        {
    //            rowIndex++;
    //            colIndex = 0;
    //            foreach (DataColumn col in dt.Columns)
    //            {
    //                colIndex++;
    //                if (col.DataType == System.Type.GetType("System.DateTime"))
    //                {
    //                    object str = (object)(Convert.ToDateTime(row[col.ColumnName].ToString())).ToString("yyyy-MM-dd"); ;
    //                    excel.WriteValue(ref vt, ref cf, ref ca, ref chl, ref rowIndex, ref colIndex, ref str, ref cellformat);
    //                }
    //                else
    //                {
    //                    object str = (object)row[col.ColumnName].ToString();
    //                    excel.WriteValue(ref vt, ref cf, ref ca, ref chl, ref rowIndex, ref colIndex, ref str, ref cellformat);
    //                }
    //            }
    //        }
    //        int ret = excel.CloseFile();

    //        //			if(ret!=0)
    //        //			{
    //        //				//MessageBox.Show(this,"Error!");
    //        //			}
    //        //			else
    //        //			{
    //        //				//MessageBox.Show(this,"请打开文件c:\\test.xls!");
    //        //			}
    //        return filename;

    //    }

    //    #endregion

    //    #region  清理过时的Excel文件

    //    private void ClearFile(string FilePath)
    //    {
    //        String[] Files = System.IO.Directory.GetFiles(FilePath);
    //        if (Files.Length > 10)
    //        {
    //            for (int i = 0; i < 10; i++)
    //            {
    //                try
    //                {
    //                    System.IO.File.Delete(Files[i]);
    //                }
    //                catch
    //                {
    //                }

    //            }
    //        }
    //    }
    //    #endregion

    //}
    #endregion
}
