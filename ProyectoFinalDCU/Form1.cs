using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProyectoFinalDCU
{
    public partial class Form1 : Form
    {
        private readonly List<Emisora> emisoras = new List<Emisora>
{
    new Emisora("Mortal 104.9 FM", "Urbano - Hip Hop", ""),
    new Emisora("Cima 100.5 FM", "Pop - Baladas", ""),
    new Emisora("Fuego 90.1 FM", "Urbano - Reggaetón", ""),
    new Emisora("Escándalo 102.5 FM", "Variada", ""),
    new Emisora("Disco 106.1 FM", "Pop - Disco", ""),
    new Emisora("Radio Disney 97.3 FM", "Pop - Juvenil", ""),
    new Emisora("Radio Popular 950 AM", "Noticias - Informativa", ""),
    new Emisora("Z 101.3 FM", "Noticias - Opinión", ""),
    new Emisora("Primera 88.1 FM", "Variada", ""),
    new Emisora("Alofoke 99.3 FM", "Urbano - Entretenimiento", ""),
    new Emisora("Independencia 93.3 FM", "Variada", ""),
    new Emisora("La Mega 97.9 FM", "Urbano - Tropical", ""),

    // Prueba técnica para validar reproducción
    new Emisora("Radio de Prueba", "Prueba", "https://stream.live.vc.bbcmedia.co.uk/bbc_world_service")
};
        private List<Emisora> emisorasFiltradas;

        public Form1()
        {
            InitializeComponent();

            txtBuscar.Text = "Buscar emisora...";
            txtBuscar.ForeColor = Color.FromArgb(150, 160, 185);

            emisorasFiltradas = new List<Emisora>(emisoras);
            CargarEmisoras(emisorasFiltradas);

            lblEstado.Text = "EN ESPERA";
            lblEmisoraActual.Text = "Selecciona FM";
            lblCategoria.Text = "Radio dominicana";
            lblAhoraSuena.Text = "🎵 Ahora suena: ninguna emisora";
        }

        private void CargarEmisoras(List<Emisora> lista)
        {
            ListadoDeEmisoras.Items.Clear();

            foreach (Emisora emisora in lista)
                ListadoDeEmisoras.Items.Add(emisora.Nombre);

            ListadoDeEmisoras.SelectedIndex = -1;
        }

        private void EscucharEmisora_Click(object sender, EventArgs e)
        {
            if (ListadoDeEmisoras.SelectedIndex < 0)
            {
                MessageBox.Show("Selecciona una emisora.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Emisora emisoraSeleccionada = emisorasFiltradas[ListadoDeEmisoras.SelectedIndex];

            if (string.IsNullOrWhiteSpace(emisoraSeleccionada.Url))
            {
                MessageBox.Show(
                    "Esta emisora todavía no tiene un stream directo configurado.",
                    "Stream no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                lblEstado.Text = "STREAM NO DISPONIBLE";
                lblEstado.ForeColor = Color.FromArgb(220, 180, 60);
                return;
            }

            try
            {
                lblEstado.Text = "CONECTANDO";
                lblEstado.ForeColor = Color.FromArgb(45, 180, 255);

                axWindowsMediaPlayer1.URL = emisoraSeleccionada.Url;
                axWindowsMediaPlayer1.Ctlcontrols.play();

                lblEstado.Text = "REPRODUCIENDO";
                lblEstado.ForeColor = Color.FromArgb(40, 220, 120);

                lblEmisoraActual.Text = emisoraSeleccionada.Nombre;
                lblCategoria.Text = emisoraSeleccionada.Categoria;
                lblAhoraSuena.Text = "🎵 Ahora suena: " + emisoraSeleccionada.Nombre;
            }
            catch (Exception ex)
            {
                lblEstado.Text = "ERROR";
                lblEstado.ForeColor = Color.FromArgb(220, 60, 75);

                MessageBox.Show(
                    "No se pudo reproducir la emisora.\n\nDetalle: " + ex.Message,
                    "Error de reproducción",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();

            lblEstado.Text = "DETENIDO";
            lblEstado.ForeColor = Color.FromArgb(220, 60, 75);
            lblAhoraSuena.Text = "🎵 Ahora suena: ninguna emisora";
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "Buscar emisora...";
            txtBuscar.ForeColor = Color.FromArgb(150, 160, 185);

            emisorasFiltradas = new List<Emisora>(emisoras);
            CargarEmisoras(emisorasFiltradas);

            lblEstado.Text = "LISTA ACTUALIZADA";
            lblEstado.ForeColor = Color.FromArgb(45, 180, 255);
        }

        private void btnFavoritos_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Función de favoritos pendiente para la versión 2.1.",
                "Favoritos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnAcercaDe_Click(object sender, EventArgs e)
        {
            FrmAcercaDe acercaDe = new FrmAcercaDe();
            acercaDe.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ListadoDeEmisoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListadoDeEmisoras.SelectedIndex >= 0)
            {
                Emisora emisoraSeleccionada = emisorasFiltradas[ListadoDeEmisoras.SelectedIndex];

                lblEstado.Text = "SELECCIONADA";
                lblEstado.ForeColor = Color.FromArgb(45, 180, 255);

                lblEmisoraActual.Text = emisoraSeleccionada.Nombre;
                lblCategoria.Text = emisoraSeleccionada.Categoria;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscar.Text == "Buscar emisora...")
                return;

            string texto = txtBuscar.Text.ToLower();

            emisorasFiltradas = emisoras
                .Where(x => x.Nombre.ToLower().Contains(texto) || x.Categoria.ToLower().Contains(texto))
                .ToList();

            CargarEmisoras(emisorasFiltradas);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // El diseño usa paneles fijos por ahora.
        }
    }

    public class Emisora
    {
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Url { get; set; }

        public Emisora(string nombre, string categoria, string url)
        {
            Nombre = nombre;
            Categoria = categoria;
            Url = url;
        }
    }
}