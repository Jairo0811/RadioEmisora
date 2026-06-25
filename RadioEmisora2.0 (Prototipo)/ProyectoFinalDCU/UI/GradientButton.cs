using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProyectoFinalDCU.UI
{
    public class GradientButton : Button
    {
        public Color Color1 { get; set; } = Color.FromArgb(95, 45, 230);
        public Color Color2 { get; set; } = Color.FromArgb(140, 70, 255);

        public int BorderRadius { get; set; } = 18;

        private bool hover = false;

        public GradientButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 11, FontStyle.Bold);
            Cursor = Cursors.Hand;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            MouseEnter += (s, e) =>
            {
                hover = true;
                Invalidate();
            };

            MouseLeave += (s, e) =>
            {
                hover = false;
                Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = ClientRectangle;

            using (GraphicsPath path = Rounded(rect, BorderRadius))
            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    rect,
                    hover ? Lighten(Color1, 25) : Color1,
                    hover ? Lighten(Color2, 25) : Color2,
                    LinearGradientMode.Horizontal))
            {
                e.Graphics.FillPath(brush, path);

                TextRenderer.DrawText(
                    e.Graphics,
                    Text,
                    Font,
                    rect,
                    ForeColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            }
        }

        GraphicsPath Rounded(Rectangle r, int radius)
        {
            GraphicsPath gp = new GraphicsPath();

            int d = radius * 2;

            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);

            gp.CloseFigure();

            return gp;
        }

        Color Lighten(Color c, int amount)
        {
            return Color.FromArgb(
                c.A,
                System.Math.Min(255, c.R + amount),
                System.Math.Min(255, c.G + amount),
                System.Math.Min(255, c.B + amount));
        }
    }
}