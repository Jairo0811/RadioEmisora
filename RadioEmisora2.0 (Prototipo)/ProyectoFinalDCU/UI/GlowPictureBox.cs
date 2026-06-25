using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProyectoFinalDCU.UI
{
    public class GlowPictureBox : PictureBox
    {
        public Color GlowColor { get; set; } =
            Color.FromArgb(150, 95, 45, 230);

        public int GlowSize { get; set; } = 20;

        public GlowPictureBox()
        {
            DoubleBuffered = true;
            SizeMode = PictureBoxSizeMode.Zoom;
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle glow =
                new Rectangle(
                    GlowSize / 2,
                    GlowSize / 2,
                    Width - GlowSize,
                    Height - GlowSize);

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(glow);

                using (PathGradientBrush brush =
                    new PathGradientBrush(gp))
                {
                    brush.CenterColor = GlowColor;
                    brush.SurroundColors =
                        new Color[]
                        {
                            Color.Transparent
                        };

                    g.FillEllipse(
                        brush,
                        glow.X - 10,
                        glow.Y - 10,
                        glow.Width + 20,
                        glow.Height + 20);
                }
            }

            base.OnPaint(pe);
        }
    }
}