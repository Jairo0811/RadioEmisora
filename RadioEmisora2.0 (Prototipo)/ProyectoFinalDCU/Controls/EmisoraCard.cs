using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ProyectoFinalDCU.Models;

namespace ProyectoFinalDCU.Controls
{
    public class EmisoraCard : Panel
    {
        private PictureBox picLogo;
        private Label lblEstado;
        private Label lblNombre;
        private Label lblCategoria;
        private Label lblFrecuencia;
        private Label lblFavorita;

        private bool hover = false;
        private bool seleccionada = false;

        public Emisora Emisora { get; private set; }

        public event EventHandler CardClick;

        public EmisoraCard(Emisora emisora)
        {
            Emisora = emisora;

            Width = 270;
            Height = 118;
            Margin = new Padding(0, 0, 0, 14);
            Cursor = Cursors.Hand;
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            CrearControles();
            CargarLogo();

            Click += OnCardClick;
            MouseEnter += (s, e) => { hover = true; Invalidate(); };
            MouseLeave += (s, e) => { hover = false; Invalidate(); };

            foreach (Control control in Controls)
            {
                control.Click += OnCardClick;
                control.MouseEnter += (s, e) => { hover = true; Invalidate(); };
                control.MouseLeave += (s, e) => { hover = false; Invalidate(); };
            }
        }

        private void CrearControles()
        {
            lblEstado = new Label();
            lblEstado.Text = string.IsNullOrWhiteSpace(Emisora.StreamUrl) ? "● OFFLINE" : "● EN VIVO";
            lblEstado.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblEstado.ForeColor = string.IsNullOrWhiteSpace(Emisora.StreamUrl)
                ? Color.FromArgb(255, 95, 95)
                : Color.FromArgb(40, 220, 120);
            lblEstado.Location = new Point(16, 10);
            lblEstado.Size = new Size(120, 18);
            lblEstado.BackColor = Color.Transparent;

            lblFavorita = new Label();
            lblFavorita.Text = Emisora.Favorita ? "★" : "☆";
            lblFavorita.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lblFavorita.ForeColor = Emisora.Favorita ? Color.Gold : Color.FromArgb(120, 130, 160);
            lblFavorita.Location = new Point(235, 6);
            lblFavorita.Size = new Size(28, 28);
            lblFavorita.BackColor = Color.Transparent;
            lblFavorita.TextAlign = ContentAlignment.MiddleCenter;

            picLogo = new PictureBox();
            picLogo.Size = new Size(58, 58);
            picLogo.Location = new Point(14, 42);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.BackColor = Color.Transparent;

            lblNombre = new Label();
            lblNombre.Text = Emisora.Nombre;
            lblNombre.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblNombre.ForeColor = Color.White;
            lblNombre.Location = new Point(85, 36);
            lblNombre.Size = new Size(150, 24);
            lblNombre.BackColor = Color.Transparent;

            lblCategoria = new Label();
            lblCategoria.Text = "🎵 " + Emisora.Categoria;
            lblCategoria.Font = new Font("Segoe UI", 8);
            lblCategoria.ForeColor = Color.FromArgb(190, 200, 225);
            lblCategoria.Location = new Point(85, 63);
            lblCategoria.Size = new Size(170, 20);
            lblCategoria.BackColor = Color.Transparent;

            lblFrecuencia = new Label();
            lblFrecuencia.Text = "📡 " + Emisora.Frecuencia;
            lblFrecuencia.Font = new Font("Segoe UI", 8);
            lblFrecuencia.ForeColor = Color.FromArgb(150, 160, 185);
            lblFrecuencia.Location = new Point(85, 84);
            lblFrecuencia.Size = new Size(170, 20);
            lblFrecuencia.BackColor = Color.Transparent;

            Controls.Add(lblEstado);
            Controls.Add(lblFavorita);
            Controls.Add(picLogo);
            Controls.Add(lblNombre);
            Controls.Add(lblCategoria);
            Controls.Add(lblFrecuencia);
        }

        public void RefrescarFavorita()
        {
            lblFavorita.Text = Emisora.Favorita ? "★" : "☆";
            lblFavorita.ForeColor = Emisora.Favorita ? Color.Gold : Color.FromArgb(120, 130, 160);
            Invalidate();
        }

        private void CargarLogo()
        {
            string rutaLogo = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..",
                "..",
                "Assets",
                "Logos",
                Emisora.Logo
            );

            if (File.Exists(rutaLogo))
            {
                using (Bitmap temp = new Bitmap(rutaLogo))
                {
                    picLogo.Image = new Bitmap(temp);
                }
            }
        }

        private void OnCardClick(object sender, EventArgs e)
        {
            CardClick?.Invoke(this, EventArgs.Empty);
        }

        public void Seleccionar()
        {
            seleccionada = true;
            Invalidate();
        }

        public void Deseleccionar()
        {
            seleccionada = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int radius = 18;

            Color fondo = Color.FromArgb(18, 26, 48);
            Color borde = Color.FromArgb(35, 45, 85);

            if (hover)
            {
                fondo = Color.FromArgb(24, 34, 62);
                borde = Emisora.ColorTema;
            }

            if (seleccionada)
            {
                fondo = Color.FromArgb(18, 26, 48);
                borde = Emisora.ColorTema;
            }

            using (GraphicsPath path = CrearRectanguloRedondeado(rect, radius))
            using (SolidBrush brush = new SolidBrush(fondo))
            using (Pen pen = new Pen(borde, seleccionada ? 3 : 1))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            if (seleccionada)
            {
                using (SolidBrush glow = new SolidBrush(Color.FromArgb(35, Emisora.ColorTema)))
                using (GraphicsPath path = CrearRectanguloRedondeado(rect, radius))
                {
                    e.Graphics.FillPath(glow, path);
                }
            }
        }

        private GraphicsPath CrearRectanguloRedondeado(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
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