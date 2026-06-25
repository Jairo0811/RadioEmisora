using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProyectoFinalDCU.UI
{
    public class RoundedPanel : Panel
    {
        public int BorderRadius { get; set; } = 22;
        public Color BorderColor { get; set; } = Color.FromArgb(40, 50, 90);
        public int BorderSize { get; set; } = 1;

        public bool UseGradient { get; set; } = false;
        public Color GradientColor1 { get; set; } = Color.FromArgb(12, 18, 35);
        public Color GradientColor2 { get; set; } = Color.FromArgb(20, 28, 55);

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(12, 18, 35);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 0 || Height <= 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = ClientRectangle;
            rect.Width--;
            rect.Height--;

            if (BorderRadius <= 0)
            {
                Region = new Region(rect);

                Brush brush;

                if (UseGradient)
                    brush = new LinearGradientBrush(rect, GradientColor1, GradientColor2, LinearGradientMode.ForwardDiagonal);
                else
                    brush = new SolidBrush(BackColor);

                using (brush)
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                if (BorderSize > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderSize))
                    {
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                }

                return;
            }

            using (GraphicsPath path = GetRoundPath(rect, BorderRadius))
            {
                Region = new Region(path);

                Brush brush;

                if (UseGradient)
                    brush = new LinearGradientBrush(rect, GradientColor1, GradientColor2, LinearGradientMode.ForwardDiagonal);
                else
                    brush = new SolidBrush(BackColor);

                using (brush)
                {
                    e.Graphics.FillPath(brush, path);
                }

                if (BorderSize > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderSize))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        private GraphicsPath GetRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int maxRadius = System.Math.Min(rect.Width, rect.Height) / 2;
            radius = System.Math.Min(radius, maxRadius);

            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}