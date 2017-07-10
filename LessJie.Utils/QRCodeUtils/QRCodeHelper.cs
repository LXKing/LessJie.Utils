
using System.Drawing;
using ThoughtWorks.QRCode.Codec;

namespace LessJie.QRCodeUtils
{
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

        #region QRCodeByLXing

        public static Image QRCodeByLXing(string content, QRCodeEncoder.ENCODE_MODE qrCodeEncodeMode, QRCodeEncoder.ERROR_CORRECTION qrCodeErrorCorrect, int qrCodeScale, int qrCodeVersion, Color qrCodeForegroundColor, Color qrCodeBackgroundColor)
        {
            QRCodeEncoder qrc = new QRCodeEncoder();
            //设置编码模式 三种模式：BYTE ，ALPHA_NUMERIC，NUMERIC
            //qrc.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrc.QRCodeEncodeMode = qrCodeEncodeMode;
            //设置编码错误纠正 容错级别大小：L M Q H
            //qrc.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            qrc.QRCodeErrorCorrect = qrCodeErrorCorrect;
            //设置编码测量度(比例) 最大40
            //qrc.QRCodeScale = 4;
            qrc.QRCodeScale = qrCodeScale;
            //设置编码版本
            //qrc.QRCodeVersion = 8;
            qrc.QRCodeVersion = qrCodeVersion;
            //设置前景色
            //qrc.QRCodeForegroundColor = Color.FromArgb(51, 255, 0);
            qrc.QRCodeForegroundColor = Color.FromArgb(51, 255, 0);
            //设置背景色
            //qrc.QRCodeBackgroundColor = Color.Red;
            qrc.QRCodeBackgroundColor = Color.Red;
            return qrc.Encode(content);
        }

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
                return qrc.Encode(content, Color.Red, Color.Blue, 4);
            }
            else
            {
                return qrc.Encode(content);
            }
        }
        #endregion
    }
}