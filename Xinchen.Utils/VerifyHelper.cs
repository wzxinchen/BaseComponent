using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Xinchen.Utils
{
    public class VerifyHelper
    {
        public static MemoryStream GetVerifyImage(string code, int width, int height, float fontSize)
        {
            Bitmap image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);
            Random random = new Random();
            for (int i = 0; i < 12; i++)
            {
                int num2 = random.Next(image.Width);
                int num3 = random.Next(image.Width);
                int num4 = random.Next(image.Height);
                int num5 = random.Next(image.Height);
                graphics.DrawLine(new Pen(Color.LightGray), num2, num4, num3, num5);
            }
            Font font = new Font("Arial", fontSize, FontStyle.Italic | FontStyle.Bold);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Gray, 1.2f, true);
            graphics.DrawString(code, font, brush, (float)0f, (float)0f);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Gif);
            image.Dispose();
            graphics.Dispose();
            return stream;
        }

        public static MemoryStream GetVerifyImage(string code)
        {
            return GetVerifyImage(code, 60, 0x16, 14f);
        }
    }
}
