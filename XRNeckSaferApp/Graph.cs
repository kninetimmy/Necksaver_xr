using System;
using System.Drawing;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class Graph : Form
    {
        private MainForm mf;

        private int x1, y1, x2, y2, step;
        public int hmd, rot;
        Bitmap bm1, bm2;
        Graphics gr;
        float grx, gry;
        float borderL, borderT;
        Pen redPen;
        Pen greenPen;
        Pen black2Pen;
        Pen bluePen;
        Pen blue2Pen;
        Pen blackPen;
        Pen dashedPen;
        Font myFont;
        Brush myBrush;

        public Graph(MainForm f)
        {
            mf = f;
            StartPosition = FormStartPosition.Manual;
            Top = mf.Top;
            Left = mf.Right - 10;

            redPen = new Pen(System.Drawing.Color.Red, 2);
            greenPen = new Pen(System.Drawing.Color.Green, 2);
            bluePen = new Pen(System.Drawing.Color.Blue, 2);
            blue2Pen = new Pen(System.Drawing.Color.CadetBlue, 2);
            blackPen = new Pen(System.Drawing.Color.Black, 1);
            dashedPen = new Pen(System.Drawing.Color.Black, 1);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            myFont = new System.Drawing.Font("Arial", 11);
            myBrush = new SolidBrush(System.Drawing.Color.Black);
            borderL = 30;
            borderT = 2;

            InitializeComponent();
            black2Pen = new Pen(System.Drawing.Color.Black, 2);
            DrawBitmap1();
            DrawBitmap2();
            OKbutton.Location = new Point(Size.Width - 67, Size.Height - 66);
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Graph_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bm2, 0, 10);
            e.Graphics.DrawImage(bm1, 0, ClientSize.Height / 2);

            if (hmd > 0 && rot < 0) rot = 180;
            if (hmd < 0 && rot > 0) rot = 180;
            if (hmd < 0) hmd = -hmd;
            if (rot < 0) rot = -rot;

            if (hmd > 0)
            {
                //                e.Graphics.DrawString(rot.ToString(), myFont, myBrush, 0, 0);
                //                e.Graphics.DrawString(hmd.ToString(), myFont, myBrush, 0, 15);
                Point P1 = Scale(hmd, 0);
                Point P2 = Scale(hmd, rot);
                Point P3 = Scale(hmd, 180);
                e.Graphics.DrawLine(black2Pen, P1.X + borderL, ClientSize.Height / 2 + P1.Y + borderT, P2.X + borderL, ClientSize.Height / 2 + P2.Y + borderT);
                e.Graphics.DrawLine(black2Pen, P1.X + borderL, P1.Y + 10 + borderT, P3.X + borderL, P3.Y + 10 + borderT);
                P1 = Scale(0, rot);
                e.Graphics.DrawLine(black2Pen, P1.X + borderL, ClientSize.Height / 2 + P1.Y + borderT, P2.X + borderL, ClientSize.Height / 2 + P2.Y + borderT);

            }
        }

        void ActLine()
        {
            Line2(redPen, 0, 2);
        }
        void DeLine()
        {
            Line2(greenPen, 1, 2);
        }

        void LRLine()
        {
            Line(bluePen, 0, 3);
        }
        void FwdLine()
        {
            Line(blue2Pen, 0, 4);
        }

        void Line2(Pen p, int x, int l)
        {
            x2 = mf.conf.AutoSteps[0][x];
            y2 = 0;
            DLine(p, 0, 0, x2, y2);
            for (int i = 1; i < mf.conf.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = mf.conf.AutoSteps[i - 1][l];
                DLine(p, x2, (int)(y2 * 2), x1, (int)(y1 * 2));
                y1 = y2;
                x2 = mf.conf.AutoSteps[i][x];
                DLine(p, x1, y1 * 2, x2, y2 * 2);
            }
            x1 = x2;
            y1 = y2;
            y2 = mf.conf.AutoSteps[mf.conf.AutoSteps.Count - 1][l];
            DLine(p, x2, y2 * 2, x1, y1 * 2);
            y1 = y2;
            x2 = 180;
            DLine(p, x1, y1 * 2, x2, y2 * 2);
        }

        void Line(Pen p, int x, int l)
        {
            x2 = mf.conf.AutoSteps[0][x];
            y2 = 0;
            DLine(p, 0, 0, x2, y2);
            for (int i = 1; i < mf.conf.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = mf.conf.AutoSteps[i - 1][l];
                DLine(p, x2, y2 * 3, x1, y1 * 3);
                y1 = y2;
                x2 = mf.conf.AutoSteps[i][x];
                DLine(p, x1, y1 * 3, x2, y2 * 3);
            }
            x1 = x2;
            y1 = y2;
            y2 = mf.conf.AutoSteps[mf.conf.AutoSteps.Count - 1][l];
            DLine(p, x2, y2 * 3, x1, y1 * 3);
            y1 = y2;
            x2 = 180;
            DLine(p, x1, y1 * 3, x2, y2 * 3);
        }

        void RedLine()
        {

            x2 = mf.conf.AutoSteps[0][0];
            y2 = x2;
            step = mf.conf.AutoSteps[0][2];
            DLine(redPen, 0, 0, x2, y2);
            for (int i = 1; i < mf.conf.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = x2 + step;
                DLine(redPen, x2, y2, x1, y1);
                y1 = y2;
                x2 = mf.conf.AutoSteps[i][0];
                y2 = x2 + step;
                DLine(redPen, x1, y1, x2, y2);

                step = mf.conf.AutoSteps[i][2];
            }
            x1 = x2;
            y1 = y2;
            y2 = x2 + step;
            DLine(redPen, x2, y2, x1, y1);
            y1 = y2;
            x2 = 180;
            y2 = 180 + step;
            DLine(redPen, x1, y1, x2, y2);
        }
        void GreenLine()
        {

            x2 = mf.conf.AutoSteps[0][1];
            y2 = x2;
            step = mf.conf.AutoSteps[0][2];
            for (int i = 1; i < mf.conf.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = x2 + step;
                DLine(greenPen, x2, y2, x1, y1);
                x1 = x2;
                y1 = y2;
                x2 = mf.conf.AutoSteps[i][1];
                y2 = x2 + step;
                DLine(greenPen, x1, y1, x2, y2);

                step = mf.conf.AutoSteps[i][2];
            }
            x1 = x2;
            y1 = y2;
            y2 = x2 + step;
            DLine(greenPen, x2, y2, x1, y1);
            x1 = x2;
            y1 = y2;
            x2 = 180;
            y2 = 180 + step;
            DLine(greenPen, x1, y1, x2, y2);
        }

        Point Scale(int x, int y)
        {

            float sx = grx / 180F;
            float sy = gry / 180F;

            return new Point((int)(x * sx), (int)((180 - y) * sy));

        }

        private void DString(String s, Font f, Brush b, int x, int y)
        {
            DString(s, f, b, x, y, false);
        }
        private void DString(String s, Font f, Brush b, int x, int y, bool right)
        {
            Point P1 = Scale(x, y);
            if (right)
                gr.DrawString(s, f, b, P1.X - 40 + borderL, P1.Y - f.SizeInPoints + borderT - 4);
            else
                gr.DrawString(s, f, b, P1.X + borderL, P1.Y - f.SizeInPoints + borderT - 4);
        }

        private void DLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            Point P1 = Scale(x1, y1);
            Point P2 = Scale(x2, y2);

            gr.DrawLine(pen, P1.X + borderL, P1.Y + borderT, P2.X + borderL, P2.Y + borderT);
        }


        void DrawBitmap1()
        {
            grx = ClientSize.Width - 40;
            gry = ClientSize.Height / 2 - 30;
            bm1 = new Bitmap(ClientSize.Width, ClientSize.Height / 2);
            gr = Graphics.FromImage(bm1);

            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            DLine(blackPen, 0, 0, 0, 180);
            DLine(blackPen, 0, 0, 180, 0);
            DLine(blackPen, 180, 0, 180, 180);
            DLine(blackPen, 0, 180, 180, 180);
            for (int i = 30; i < 180; i += 30)
            {
                DLine(dashedPen, i, 0, i, 180);
                DString(i.ToString() + "°", myFont, myBrush, i, 0);
                DLine(dashedPen, 0, i, 180, i);
                DString(i.ToString() + "°", myFont, myBrush, 1, i);
            }
            gr.DrawString("Physical HMD yaw angle", myFont, myBrush, grx / 2 - 70, gry + 8);

            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            GreenLine();
            RedLine();
            gr.RotateTransform(270);
            gr.DrawString("Visual yaw angle", myFont, myBrush, -gry / 2 - 60, 7);

        }
        void DrawBitmap2()
        {

            grx = ClientSize.Width - 40;
            gry = ClientSize.Height / 2 - 30;

            bm2 = new Bitmap(ClientSize.Width, ClientSize.Height / 2);
            gr = Graphics.FromImage(bm2);

            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            DLine(blackPen, 0, 0, 0, 180);
            DLine(blackPen, 0, 0, 180, 0);
            DLine(blackPen, 180, 0, 180, 180);
            DLine(blackPen, 0, 180, 180, 180);
            for (int i = 30; i < 180; i += 30)
            {
                DLine(dashedPen, i, 0, i, 180);
                DString(i.ToString() + "°", myFont, myBrush, i, 0);
                DLine(dashedPen, 0, i, 180, i);
                DString((i / 2).ToString() + "°", myFont, myBrush, 1, i);
                DString((i / 3).ToString() + "cm", myFont, myBrush, 179, i, true);
            }
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            ActLine();
            DeLine();
            LRLine();
            FwdLine();

            Rectangle L1 = new Rectangle(65, 10, 125, 48);
            gr.FillRectangle(new SolidBrush(System.Drawing.Color.White), L1);
            gr.DrawRectangle(blackPen, L1);
            gr.DrawString("Rotational offset:", myFont, new SolidBrush(System.Drawing.Color.Black), L1.X + 1, L1.Y + 2);
            gr.DrawString(" Activation (act)", myFont, new SolidBrush(System.Drawing.Color.Red), L1.X + 1, L1.Y + 17);
            gr.DrawString(" Deactivation (de)", myFont, new SolidBrush(System.Drawing.Color.Green), L1.X + 1, L1.Y + 32);
            Rectangle L2 = new Rectangle(ClientSize.Width - 192, 10, 137, 48);
            gr.FillRectangle(new SolidBrush(System.Drawing.Color.White), L2);
            gr.DrawRectangle(blackPen, L2);
            gr.DrawString("Translational offset:", myFont, new SolidBrush(System.Drawing.Color.Black), L2.X + 1, L2.Y + 2);
            gr.DrawString(" Left/Right (L/R)", myFont, new SolidBrush(System.Drawing.Color.Blue), L2.X + 1, L2.Y + 17);
            gr.DrawString(" Forward (Fwd)", myFont, new SolidBrush(System.Drawing.Color.CadetBlue), L2.X + 1, L2.Y + 32);
            gr.RotateTransform(270);
            gr.DrawString("Autorot values", myFont, myBrush, -gry / 2 - 60, 7);

        }

        private void Graph_SizeChanged(object sender, EventArgs e)
        {
            Graph_ValuesChanged();
            OKbutton.Location = new Point(Size.Width - 67, Size.Height - 66);
        }
        public void Graph_ValuesChanged()
        {
            DrawBitmap1();
            DrawBitmap2();
            Invalidate();
        }
    }
}
