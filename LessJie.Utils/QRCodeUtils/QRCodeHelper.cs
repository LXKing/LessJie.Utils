
using System.Drawing;
using System.Drawing.Drawing2D;
using ThoughtWorks.QRCode.Codec;

namespace LessJie.QRCodeUtils
{
    /// <summary>
    /// 二维码帮助类
    /// </summary>
    public class QRCodeHelper
    {
        #region QRCodeByZXing
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="msg">二维码信息</param>
        /// <returns>图片</returns>
        //public static Bitmap QRCodeByZXing(string msg)
        //{

        //}
        #endregion

        //#region QRCodeByLXing
        
        //public static Image QRCodeByLXing(string content, QRCodeEncoder.ENCODE_MODE qrCodeEncodeMode, QRCodeEncoder.ERROR_CORRECTION qrCodeErrorCorrect, int qrCodeScale, int qrCodeVersion, Color qrCodeForegroundColor, Color qrCodeBackgroundColor)
        //{
        //    QRCodeEncoder qrc = new QRCodeEncoder();
        //    //设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC
        //    //qrc.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
        //    qrc.QRCodeEncodeMode = qrCodeEncodeMode;
        //    //设置编码错误纠正 容错级别大小：L M Q H
        //    //qrc.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
        //    qrc.QRCodeErrorCorrect = qrCodeErrorCorrect;
        //    //设置编码测量度(比例) 最大40
        //    //qrc.QRCodeScale = 4;
        //    qrc.QRCodeScale = qrCodeScale;
        //    //设置编码版本
        //    //qrc.QRCodeVersion = 8;
        //    qrc.QRCodeVersion = qrCodeVersion;
        //    //设置前景色
        //    //qrc.QRCodeForegroundColor = Color.FromArgb(51, 255, 0);
        //    qrc.QRCodeForegroundColor = Color.FromArgb(51, 255, 0);
        //    //设置背景色
        //    //qrc.QRCodeBackgroundColor = Color.Red;
        //    qrc.QRCodeBackgroundColor = Color.Red;
        //    return qrc.Encode(content);
        //}
        //#endregion



        #region 默认二维码(渐变)
        /// <summary>
        /// 默认二维码(渐变)
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="Gradient">是否生成渐变二维码</param>
        /// <returns></returns>
        public static Image QRCodeByLXing(string content, bool Gradient)
        {
            QRCodeEncoder qrc = new QRCodeEncoder();
            //设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC
            qrc.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //设置编码测量度(比例) 最大40
            qrc.QRCodeScale = 4;
            //设置编码版本
            qrc.QRCodeVersion = 8;
            //设置编码错误纠正 容错级别大小：L M Q H
            qrc.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            //设置前景色
            qrc.QRCodeForegroundColor = Color.FromArgb(51, 255, 0);
            //设置背景色
            qrc.QRCodeBackgroundColor = Color.Red;

            if (Gradient)
            {
                //渐变二维码
                return qrc.Encode(content, Color.Red, Color.Blue, LinearGradientMode.Vertical);
            }
            else
            {
                return qrc.Encode(content);
            }
        }
        #endregion


        #region 自定义渐变二维码(默认参数使用的是默认二维码的值)
        /// <summary>
        /// 自定义渐变二维码(默认参数使用的是默认二维码的值)
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="QRCodeForegroundColor">设置前景色eg:1. Color.FromArgb(51, 255, 0) 2. Color.Red</param>
        /// <param name="QRCodeBackgroundColor">设置背景色eg:1. Color.FromArgb(51, 255, 0) 2. Color.Red</param>
        /// <param name="GradientStartColor">设置渐变开始颜色(开始 结束表达不恰当 反正就是计算两个颜色的色差)eg:1. Color.FromArgb(51, 255, 0) 2. Color.Red</param>
        /// <param name="GradientEndColor">设置渐变结束颜色(开始 结束表达不恰当 反正就是计算两个颜色的色差)eg:1. Color.FromArgb(51, 255, 0) 2. Color.Red</param>
        /// <param name="brushType">指定线性渐变的方向 </param>
        /// <param name="QRCodeEncodeMode">设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC</param>
        /// <param name="QRCodeScale">设置编码测量度(比例) 最大40 对应int类型值为4</param>
        /// <param name="QRCodeVersion">设置编码版本 此值具体如何设置暂不清楚 请参照ThoughtWorks.QRCode开源项目</param>
        /// <param name="QRCodeErrorCorrect">设置编码错误纠正 容错级别大小：L M Q H</param>
        /// <returns>return Image</returns>
        public static Image QRCodeByLXing(string content, Color QRCodeForegroundColor, Color QRCodeBackgroundColor,
            Color GradientStartColor, Color GradientEndColor, LinearGradientMode brushType = LinearGradientMode.Vertical,
            QRCodeEncoder.ENCODE_MODE QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE, 
            int QRCodeScale = 4, int QRCodeVersion = 8, 
            QRCodeEncoder.ERROR_CORRECTION QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H)
        {
            QRCodeEncoder qrc = new QRCodeEncoder();
            //设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC
            qrc.QRCodeEncodeMode = QRCodeEncodeMode;
            //设置编码测量度(比例) 最大40
            qrc.QRCodeScale = QRCodeScale;
            //设置编码版本
            qrc.QRCodeVersion = QRCodeVersion;
            //设置编码错误纠正 容错级别大小：L M Q H
            qrc.QRCodeErrorCorrect = QRCodeErrorCorrect;

            //设置前景色
            qrc.QRCodeForegroundColor = QRCodeForegroundColor;
            //设置背景色
            qrc.QRCodeBackgroundColor = QRCodeBackgroundColor;
            //渐变二维码
            return qrc.Encode(content, GradientStartColor, GradientEndColor, brushType);
        }
        #endregion


        #region 自定义二维码(默认参数使用的是默认二维码的值)
        /// <summary>
        /// 自定义二维码(默认参数使用的是默认二维码的值)
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="QRCodeForegroundColor">设置前景色eg:1. Color.FromArgb(51, 255, 0) 2. Color.Red</param>
        /// <param name="QRCodeBackgroundColor">设置背景色eg:1. Color.FromArgb(51, 255, 0) 2. Color.Red</param>
        /// <param name="QRCodeEncodeMode">设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC</param>
        /// <param name="QRCodeScale">设置编码测量度(比例) 最大40 对应int类型值为4</param>
        /// <param name="QRCodeVersion">设置编码版本 此值具体如何设置暂不清楚 请参照ThoughtWorks.QRCode开源项目</param>
        /// <param name="QRCodeErrorCorrect">设置编码错误纠正 容错级别大小：L M Q H</param>
        /// <returns>return Image</returns>
        public static Image QRCodeByLXing(string content, Color QRCodeForegroundColor, Color QRCodeBackgroundColor,
            QRCodeEncoder.ENCODE_MODE QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
            int QRCodeScale = 4, int QRCodeVersion = 8,
            QRCodeEncoder.ERROR_CORRECTION QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H)
        {
            QRCodeEncoder qrc = new QRCodeEncoder();
            //设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC
            qrc.QRCodeEncodeMode = QRCodeEncodeMode;
            //设置编码测量度(比例) 最大40
            qrc.QRCodeScale = QRCodeScale;
            //设置编码版本
            qrc.QRCodeVersion = QRCodeVersion;
            //设置编码错误纠正 容错级别大小：L M Q H
            qrc.QRCodeErrorCorrect = QRCodeErrorCorrect;

            //设置前景色
            qrc.QRCodeForegroundColor = QRCodeForegroundColor;
            //设置背景色
            qrc.QRCodeBackgroundColor = QRCodeBackgroundColor;

            //二维码
            return qrc.Encode(content);
        }
        #endregion
    }
}