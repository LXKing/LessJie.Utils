using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LessJie.ControlUtils
{
    /// <summary>
    /// 控件辅助类 
    /// 1.数据展示
    /// 2.绑定数据
    /// 3.控件状态设置
    /// </summary>
    public class ControlHelper
    {
        #region 绑定服务器数据控件 简单绑定DataList
        /// <summary>
        /// 简单绑定DataList
        /// </summary>
        /// <param name="ctrl">控件ID</param>
        /// <param name="mydv">数据视图</param>
        public static void BindDataList(Control ctrl, DataView mydv)
        {
            ((DataList)ctrl).DataSourceID = null;
            ((DataList)ctrl).DataSource = mydv;
            ((DataList)ctrl).DataBind();
        }
        #endregion

        #region 绑定服务器数据控件 SqlDataReader简单绑定DataList
        /// <summary>
        /// SqlDataReader简单绑定DataList
        /// </summary>
        /// <param name="ctrl">控件ID</param>
        /// <param name="mydv">数据视图</param>
        public static void BindDataReaderList(System.Web.UI.Control ctrl, SqlDataReader mydv)
        {
            ((DataList)ctrl).DataSourceID = null;
            ((DataList)ctrl).DataSource = mydv;
            ((DataList)ctrl).DataBind();
        }
        #endregion

        #region 绑定服务器数据控件 简单绑定GridView
        /// <summary>
        /// 简单绑定GridView
        /// </summary>
        /// <param name="ctrl">控件ID</param>
        /// <param name="mydv">数据视图</param>
        public static void BindGridView(System.Web.UI.Control ctrl, DataView mydv)
        {
            ((GridView)ctrl).DataSourceID = null;
            ((GridView)ctrl).DataSource = mydv;
            ((GridView)ctrl).DataBind();
        }
        #endregion

        #region 绑定服务器控件 简单绑定Repeater
        /// <summary>
        /// 绑定服务器控件 简单绑定Repeater
        /// </summary>
        /// <param name="ctrl">控件ID</param>
        /// <param name="mydv">数据视图</param>
        public static void BindRepeater(System.Web.UI.Control ctrl, DataView mydv)
        {
            ((Repeater)ctrl).DataSourceID = null;
            ((Repeater)ctrl).DataSource = mydv;
            ((Repeater)ctrl).DataBind();
        }
        #endregion


        #region 控件状态设置

        /// <summary>
        /// 锁定页面上的一些组件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="obj">不需锁定的控件</param>
        public static void LockPage(Page page, object[] obj)
        {
            Control htmlForm = null;
            foreach (Control ctl in page.Controls)
            {
                if (ctl is HtmlForm)
                {
                    htmlForm = ctl;
                    break;
                }
            }
            //foreach (Control ctl in page.Controls[1].Controls)
            foreach (Control ctl in htmlForm.Controls)
            {
                if (IsContains(obj, ctl) == false)
                {
                    //锁定
                    LockControl(page, ctl);
                }
                else
                {
                    //解除锁定
                    UnLockControl(page, ctl);
                }
            }
        }

        /// <summary>
        /// 解除锁定页面上的一些组件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="obj">继续保持锁定的控件</param>
        public static void UnLockPage(Page page, object[] obj)
        {
            Control htmlForm = null;
            foreach (Control ctl in page.Controls)
            {
                if (ctl is HtmlForm)
                {
                    htmlForm = ctl;
                    break;
                }
            }
            //foreach (Control ctl in page.Controls[1].Controls)
            foreach (Control ctl in htmlForm.Controls)
            {
                if (IsContains(obj, ctl) == false)
                {
                    //解除锁定
                    UnLockControl(page, ctl);
                }
                else
                {
                    //锁定
                    LockControl(page, ctl);
                }
            }
        }

        /// <summary>
        /// 禁用控件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="ctl"></param>
        private static void LockControl(Page page, Control ctl)
        {
            //WebControl
            if (ctl is Button || ctl is CheckBox || ctl is HyperLink || ctl is LinkButton
                || ctl is ListControl || ctl is TextBox)
            {
                ((WebControl)ctl).Enabled = false;

                #region 多行文本框不能禁用，应设为只读，不然滚动条不能使用

                if (ctl is TextBox)
                {
                    if (((TextBox)ctl).TextMode == TextBoxMode.MultiLine)
                    {
                        ((TextBox)ctl).Enabled = true;
                        ((TextBox)ctl).ReadOnly = true;
                    }
                }

                #endregion

                #region 时间控件禁用时不显示图片



                #endregion
            }

            //HtmlControl
            if (ctl is HtmlInputFile)
            {
                ((HtmlInputFile)ctl).Disabled = true;
            }
        }

        /// <summary>
        /// 开放控件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="ctl"></param>
        private static void UnLockControl(Page page, Control ctl)
        {
            //WebControl
            if (ctl is Button || ctl is CheckBox || ctl is HyperLink || ctl is LinkButton
                || ctl is ListControl || ctl is TextBox)
            {
                ((WebControl)ctl).Enabled = true;

                //文本框去掉只读属性
                if (ctl is TextBox)
                {
                    ((TextBox)ctl).ReadOnly = false;
                }

                ////时间输入文本框不禁用时显示按钮
                //if (ctl is WebDateTimeEdit)
                //{
                //    ((WebDateTimeEdit)ctl).SpinButtons.Display = ButtonDisplay.OnRight;
                //}

                ////时间选择文本框不禁用时显示按钮
                //if (ctl is WebDateChooser)
                //{
                //    page.ClientScript.RegisterStartupScript(typeof(string), "Display" + ctl.ClientID + "Image", "<script language=javascript>" +
                //        "document.getElementById('" + ctl.ClientID + "_img" + "').style.display='';</script>");
                //}
            }

            //HtmlControl
            if (ctl is HtmlInputFile)
            {
                ((HtmlInputFile)ctl).Disabled = false;
            }
        }

        /// <summary>
        /// 数组中是否包含当前控件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ctl"></param>
        /// <returns></returns>
        private static bool IsContains(object[] obj, Control ctl)
        {
            foreach (Control c in obj)
            {
                if (c.ID == ctl.ID)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
