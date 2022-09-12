using System;
using System.Drawing;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class Graph : Form
    {
        private int x1, y1, x2, y2, step;
        public int hmd, rot;
        Bitmap bm1, bm2;
        Graphics _graphics;
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

        public Graph(int mainFormTop, int mainFormRight)
        {
            StartPosition = FormStartPosition.Manual;
            Top = mainFormTop;
            Left = mainFormRight - 10;

            redPen = new Pen(Color.Red, 2);
            greenPen = new Pen(Color.Green, 2);
            bluePen = new Pen(Color.Blue, 2);
            blue2Pen = new Pen(Color.CadetBlue, 2);
            blackPen = new Pen(Color.Black, 1);
            dashedPen = new Pen(Color.Black, 1);
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            myFont = new Font("Arial", 11);
            myBrush = new SolidBrush(Color.Black);
            borderL = 30;
            borderT = 2;

            InitializeComponent();
            black2Pen = new Pen(Color.Black, 2);
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
            x2 = Config.Instance.AutoSteps[0][x];
            y2 = 0;
            DLine(p, 0, 0, x2, y2);
            for (int i = 1; i < Config.Instance.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = Config.Instance.AutoSteps[i - 1][l];
                DLine(p, x2, (int)(y2 * 2), x1, (int)(y1 * 2));
                y1 = y2;
                x2 = Config.Instance.AutoSteps[i][x];
                DLine(p, x1, y1 * 2, x2, y2 * 2);
            }
            x1 = x2;
            y1 = y2;
            y2 = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][l];
            DLine(p, x2, y2 * 2, x1, y1 * 2);
            y1 = y2;
            x2 = 180;
            DLine(p, x1, y1 * 2, x2, y2 * 2);
        }

        void Line(Pen p, int x, int l)
        {
            x2 = Config.Instance.AutoSteps[0][x];
            y2 = 0;
            DLine(p, 0, 0, x2, y2);
            for (int i = 1; i < Config.Instance.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = Config.Instance.AutoSteps[i - 1][l];
                DLine(p, x2, y2 * 3, x1, y1 * 3);
                y1 = y2;
                x2 = Config.Instance.AutoSteps[i][x];
                DLine(p, x1, y1 * 3, x2, y2 * 3);
            }
            x1 = x2;
            y1 = y2;
            y2 = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][l];
            DLine(p, x2, y2 * 3, x1, y1 * 3);
            y1 = y2;
            x2 = 180;
            DLine(p, x1, y1 * 3, x2, y2 * 3);
        }

        void RedLine()
        {

            x2 = Config.Instance.AutoSteps[0][0];
            y2 = x2;
            step = Config.Instance.AutoSteps[0][2];
            DLine(redPen, 0, 0, x2, y2);
            for (int i = 1; i < Config.Instance.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = x2 + step;
                DLine(redPen, x2, y2, x1, y1);
                y1 = y2;
                x2 = Config.Instance.AutoSteps[i][0];
                y2 = x2 + step;
                DLine(redPen, x1, y1, x2, y2);

                step = Config.Instance.AutoSteps[i][2];
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

            x2 = Config.Instance.AutoSteps[0][1];
            y2 = x2;
            step = Config.Instance.AutoSteps[0][2];
            for (int i = 1; i < Config.Instance.AutoSteps.Count; i++)
            {
                x1 = x2;
                y1 = y2;
                y2 = x2 + step;
                DLine(greenPen, x2, y2, x1, y1);
                x1 = x2;
                y1 = y2;
                x2 = Config.Instance.AutoSteps[i][1];
                y2 = x2 + step;
                DLine(greenPen, x1, y1, x2, y2);

                step = Config.Instance.AutoSteps[i][2];
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
                _graphics.DrawString(s, f, b, P1.X - 40 + borderL, P1.Y - f.SizeInPoints + borderT - 4);
            else
                _graphics.DrawString(s, f, b, P1.X + borderL, P1.Y - f.SizeInPoints + borderT - 4);
        }

        private void DLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            Point P1 = Scale(x1, y1);
            Point P2 = Scale(x2, y2);

            _graphics.DrawLine(pen, P1.X + borderL, P1.Y + borderT, P2.X + borderL, P2.Y + borderT);
        }


        void DrawBitmap1()
        {
            grx = ClientSize.Width - 40;
            gry = ClientSize.Height / 2 - 30;
            bm1 = new Bitmap(ClientSize.Width, ClientSize.Height / 2);
            _graphics = Graphics.FromImage(bm1);

            _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

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
            _graphics.DrawString("Physical HMD yaw angle", myFont, myBrush, grx / 2 - 70, gry + 8);

            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            GreenLine();
            RedLine();
            _graphics.RotateTransform(270);
            _graphics.DrawString("Visual yaw angle", myFont, myBrush, -gry / 2 - 60, 7);

        }
        void DrawBitmap2()
        {

            grx = ClientSize.Width - 40;
            gry = ClientSize.Height / 2 - 30;

            bm2 = new Bitmap(ClientSize.Width, ClientSize.Height / 2);
            _graphics = Graphics.FromImage(bm2);

            _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

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
            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            ActLine();
            DeLine();
            LRLine();
            FwdLine();

            Rectangle L1 = new Rectangle(65, 10, 125, 48);
            _graphics.FillRectangle(new SolidBrush(Color.White), L1);
            _graphics.DrawRectangle(blackPen, L1);
            _graphics.DrawString("Rotational offset:", myFont, new SolidBrush(Color.Black), L1.X + 1, L1.Y + 2);
            _graphics.DrawString(" Activation (act)", myFont, new SolidBrush(Color.Red), L1.X + 1, L1.Y + 17);
            _graphics.DrawString(" Deactivation (de)", myFont, new SolidBrush(Color.Green), L1.X + 1, L1.Y + 32);
            Rectangle L2 = new Rectangle(ClientSize.Width - 192, 10, 137, 48);
            _graphics.FillRectangle(new SolidBrush(Color.White), L2);
            _graphics.DrawRectangle(blackPen, L2);
            _graphics.DrawString("Translational offset:", myFont, new SolidBrush(Color.Black), L2.X + 1, L2.Y + 2);
            _graphics.DrawString(" Left/Right (L/R)", myFont, new SolidBrush(Color.Blue), L2.X + 1, L2.Y + 17);
            _graphics.DrawString(" Forward (Fwd)", myFont, new SolidBrush(Color.CadetBlue), L2.X + 1, L2.Y + 32);
            _graphics.RotateTransform(270);
            _graphics.DrawString("Autorot values", myFont, myBrush, -gry / 2 - 60, 7);

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
