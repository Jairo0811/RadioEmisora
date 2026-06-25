using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoFinalDCU.UI
{
    public class EqualizerControl : Control
    {
        private readonly Timer timer;
        private readonly Random random = new Random();
        private int[] heights;

        public Color BarColor { get; set; } = Color.FromArgb(95, 45, 230);
        public bool IsRunning { get; private set; }

        public EqualizerControl()
        {
            DoubleBuffered = true;
            heights = new int[14];

            for (int i = 0; i < heights.Length; i++)
                heights[i] = random.Next(8, 38);

            timer = new Timer();
            timer.Interval = 120;
            timer.Tick += (s, e) =>
            {
                for (int i = 0; i < heights.Length; i++)
                    heights[i] = random.Next(8, Math.Max(12, Height - 8));

                Invalidate();
            };
        }

        public void Start()
        {
            IsRunning = true;
            timer.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            timer.Stop();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int barWidth = 6;
            int gap = 5;
            int totalWidth = heights.Length * (barWidth + gap);
            int startX = (Width - totalWidth) / 2;

            for (int i = 0; i < heights.Length; i++)
            {
                int h = IsRunning ? heights[i] : 8;
                int x = startX + i * (barWidth + gap);
                int y = (Height - h) / 2;

                using (SolidBrush brush = new SolidBrush(BarColor))
                {
                    e.Graphics.FillRectangle(brush, x, y, barWidth, h);
                }
            }
        }
    }
}