using System;
using System.Drawing;
using System.Windows.Forms;
using ProyectoFinalDCU.UI;

namespace ProyectoFinalDCU.Controls
{
    public class DashboardPanel : RoundedPanel
    {
        private Label lblTitulo;
        private Label lblResumen;
        private Label lblHistorial;
        private GradientButton btnEscuchar;

        public event EventHandler EscucharClick;

        public DashboardPanel()
        {
            CrearUI();
        }

        private void CrearUI()
        {
            BackColor = Color.FromArgb(10, 16, 35);
            BorderRadius = 30;
            BorderSize = 1;
            BorderColor = Color.FromArgb(95, 45, 230);
            UseGradient = true;
            GradientColor1 = Color.FromArgb(10, 16, 35);
            GradientColor2 = Color.FromArgb(32, 20, 70);

            lblTitulo = new Label();
            lblTitulo.Text = "📻 RadioEmisora RD";
            lblTitulo.Font = new Font("Segoe UI", 34, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(55, 50);
            lblTitulo.Size = new Size(680, 80);

            lblResumen = new Label();
            lblResumen.Font = new Font("Segoe UI", 16);
            lblResumen.ForeColor = Color.FromArgb(225, 230, 250);
            lblResumen.Location = new Point(60, 150);
            lblResumen.Size = new Size(650, 210);

            lblHistorial = new Label();
            lblHistorial.Font = new Font("Segoe UI", 14);
            lblHistorial.ForeColor = Color.FromArgb(190, 200, 230);
            lblHistorial.Location = new Point(60, 350);
            lblHistorial.Size = new Size(650, 120);

            btnEscuchar = new GradientButton();
            btnEscuchar.Text = "▶ Escuchar ahora";
            btnEscuchar.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            btnEscuchar.Size = new Size(260, 65);
            btnEscuchar.Location = new Point(60, 505);
            btnEscuchar.Color1 = Color.FromArgb(95, 45, 230);
            btnEscuchar.Color2 = Color.FromArgb(170, 80, 255);
            btnEscuchar.BorderRadius = 22;
            btnEscuchar.Click += (s, e) => EscucharClick?.Invoke(this, EventArgs.Empty);

            Controls.Add(lblTitulo);
            Controls.Add(lblResumen);
            Controls.Add(lblHistorial);
            Controls.Add(btnEscuchar);
        }

        public void ActualizarDatos(int total, int fm, int am, int online, int favoritas, string ultima, string historial)
        {
            lblResumen.Text =
                "Bienvenido 👋\n\n" +
                "📻 Emisoras registradas: " + total + "\n" +
                "📡 FM: " + fm + "     AM: " + am + "     Online: " + online + "\n" +
                "⭐ Favoritas: " + favoritas + "\n\n" +
                "🎵 Última emisora:\n   " + ultima;

            lblHistorial.Text =
                "🕒 Historial reciente:\n" +
                historial;
        }
    }
}