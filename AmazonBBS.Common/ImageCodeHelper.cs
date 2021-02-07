using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.Common
{
    public class CodeHelper
    {
        private int Length { get; set; }
        public CodeHelper(int length)
        {
            Length = length;
        }
        public string GenerateCode()
        {
            List<string> codes = new List<string>();

            //数字
            for (int i = 0; i <= 9; i++)
            {
                codes.Add(i.ToString());
            }
            //大写字母
            for (int i = 65; i <= 90; i++)
            {
                byte[] btNumber = new byte[] { (byte)i };
                string code = Encoding.ASCII.GetString(btNumber);
                codes.Add(code);
            }

            //小写字母
            for (int i = 97; i <= 122; i++)
            {
                byte[] btNumber = new byte[] { (byte)i };
                string code = Encoding.ASCII.GetString(btNumber);
                codes.Add(code);
            }

            StringBuilder sb = new StringBuilder();
            Random rd = new Random();
            while (Length-- > 0)
            {
                int index = rd.Next(0, codes.Count);
                sb.Append(codes[index]);
            }
            return sb.ToString();
        }

        public MemoryStream GeneratePicture(string checkCode)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return null;

            Bitmap image = new Bitmap((int)Math.Ceiling((checkCode.Length * 25.0)), 40);//图片长度
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.FromArgb(218, 215, 233));

                //画图片的背景噪音线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                Font font = new Font("Arial", 18, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 15, 5);//显示坐标

                //画图片的前景噪音点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms;
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
