using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace QuestionnaireSystem
{
    public class DrawingVerification : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var ms = this.DrawTransform();
            context.Response.ContentType = "image/png";
            context.Response.Clear();
            context.Response.BinaryWrite(ms.GetBuffer()); //透過GetBuffer把Stream裡面的位元組都取出
        }

        private Random random = new Random((int)DateTime.Now.Ticks);

        private MemoryStream DrawTransform()
        {
            int pPixel = 1;
            Color pColor = Color.FromArgb(random.Next(0, 100), random.Next(0, 100), random.Next(0, 100)); //字

            Bitmap bmp = new Bitmap(85, 35);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.FromArgb(random.Next(150, 256), random.Next(150, 256), random.Next(150, 256))); // 背景

            Color line = Color.Red;
            Pen pen1 = new Pen(line, pPixel);
            Point[] points =
            {
                new Point(random.Next(35),random.Next(85)),
                new Point(random.Next(10,35),random.Next(10,85)),
                new Point(random.Next(20,35),random.Next(20,85)),
                new Point(random.Next(30,35),random.Next(30,85)),
                new Point(random.Next(35),random.Next(40,85)),
                new Point(random.Next(35),random.Next(60,85)),
                new Point(random.Next(35),random.Next(70,85)),
                new Point(random.Next(35),random.Next(80,85)),
            };
            float tension = 100f;
            graphics.DrawCurve(pen1, points, tension);

            var outputText = RandomCode(5);
            HttpContext.Current.Session["Verify"] = outputText;

            string fontFamilyName = "Consolas"; // 字體
            Font font = new Font(new FontFamily(fontFamilyName), 20);
            graphics.DrawString(
                outputText,
                font,
                new SolidBrush(pColor),
                0, 0
            );

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string RandomCode(int inputNumber)
        {
            string StrChar = "2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";

            string[] rArray = StrChar.Split(',');
            string rNumber = "";
            int avoidSame = -1;
            Random random = new Random();
            for (int i = 1; i < inputNumber + 1; i++)
            {
                if (avoidSame != -1)
                {
                    random = new Random(i * avoidSame * unchecked((int)DateTime.Now.Ticks));
                }
                int num = random.Next(31);
                if (avoidSame != -1 && avoidSame == num)
                {
                    return RandomCode(inputNumber);
                }
                avoidSame = num;
                rNumber += rArray[num];
            }
            return rNumber;
        }

    }
}