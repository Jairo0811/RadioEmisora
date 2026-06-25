using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ProyectoFinalDCU.Controls;
using ProyectoFinalDCU.Models;
using ProyectoFinalDCU.Services;
using ProyectoFinalDCU.UI;

namespace ProyectoFinalDCU
{
    public partial class Form1 : Form
    {
        private readonly List<Emisora> emisoras;
        private List<Emisora> emisorasFiltradas;
        private Emisora emisoraSeleccionada;
        private EmisoraCard tarjetaSeleccionada;
        private EqualizerControl equalizer;
        private AppConfig config;
        private DashboardPanel dashboard;

        public Form1()
        {
            InitializeComponent();

            emisoras = RadioService.ObtenerEmisoras();
            config = ConfigService.CargarConfig();

            AplicarFavoritosGuardados();

            emisorasFiltradas = new List<Emisora>(emisoras);

            ConfigurarInicio();
            CrearEcualizador();
            CargarTarjetas(emisorasFiltradas);
            RestaurarUltimaEmisora();
            CrearDashboard();
            MostrarDashboard();
        }

        private void AplicarFavoritosGuardados()
        {
            List<string> favoritos = FavoriteService.CargarFavoritos();

            foreach (Emisora emisora in emisoras)
                emisora.Favorita = favoritos.Contains(emisora.Nombre);
        }

        private void GuardarFavoritos()
        {
            List<string> favoritos = emisoras
                .Where(x => x.Favorita)
                .Select(x => x.Nombre)
                .ToList();

            FavoriteService.GuardarFavoritos(favoritos);
        }

        private void ConfigurarInicio()
        {
            txtBuscar.Text = "Buscar emisora...";
            txtBuscar.ForeColor = Color.FromArgb(150, 160, 185);

            lblEstado.Text = "EN ESPERA";
            lblEstado.ForeColor = Color.FromArgb(45, 180, 255);

            lblEmisoraActual.Text = "Selecciona FM";
            lblCategoria.Text = "Radio dominicana";
            lblAhoraSuena.Text = "🎵 Ahora suena: ninguna emisora";

            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.Visible = false;

            panelHero.UseGradient = true;
            panelHero.GradientColor1 = Color.FromArgb(10, 16, 35);
            panelHero.GradientColor2 = Color.FromArgb(18, 24, 48);

            CargarLogo(null);
        }

        private void CrearEcualizador()
        {
            equalizer = new EqualizerControl();
            equalizer.Size = new Size(170, 45);
            equalizer.Location = new Point(560, 22);
            equalizer.BarColor = Color.FromArgb(95, 45, 230);
            equalizer.Stop();

            panelNowPlaying.Controls.Add(equalizer);
            equalizer.BringToFront();
        }

        private void CrearDashboard()
        {
            dashboard = new DashboardPanel();
            dashboard.Location = new Point(25, 25);
            dashboard.Size = new Size(810, 635);
            dashboard.EscucharClick += Dashboard_EscucharClick;

            panelContent.Controls.Add(dashboard);
            dashboard.BringToFront();
        }

        private void MostrarDashboard()
        {
            int total = emisoras.Count;
            int fm = emisoras.Count(x => x.Frecuencia.Contains("FM"));
            int am = emisoras.Count(x => x.Frecuencia.Contains("AM"));
            int online = emisoras.Count(x => x.Frecuencia.Contains("Online"));
            int favoritas = emisoras.Count(x => x.Favorita);

            string ultima = string.IsNullOrWhiteSpace(config.UltimaEmisora)
                ? "Ninguna todavía"
                : config.UltimaEmisora;

            string historialTexto = "   Sin historial";

            if (config.Historial.Count > 0)
                historialTexto = string.Join("\n", config.Historial.Take(5).Select(x => "   • " + x));

            dashboard.ActualizarDatos(total, fm, am, online, favoritas, ultima, historialTexto);
            dashboard.Visible = true;
            dashboard.BringToFront();
        }

        private void OcultarDashboard()
        {
            if (dashboard != null)
                dashboard.Visible = false;
        }

        private void Dashboard_EscucharClick(object sender, EventArgs e)
        {
            OcultarDashboard();

            if (!string.IsNullOrWhiteSpace(config.UltimaEmisora))
            {
                Emisora ultima = emisoras.FirstOrDefault(x => x.Nombre == config.UltimaEmisora);

                if (ultima != null)
                {
                    emisoraSeleccionada = ultima;
                    SeleccionarTarjetaVisual(ultima);

                    lblEstado.Text = string.IsNullOrWhiteSpace(ultima.StreamUrl) ? "● OFFLINE" : "● EN VIVO";
                    lblEmisoraActual.Text = ultima.Nombre;
                    lblCategoria.Text = ultima.Categoria + " • " + ultima.Frecuencia;
                    lblAhoraSuena.Text = "🎵 Última emisora: " + ultima.Nombre;

                    CargarLogo(ultima);
                    AplicarTemaEmisora(ultima);
                }
            }
        }

        private void AgregarAlHistorial(Emisora emisora)
        {
            if (emisora == null)
                return;

            config.Historial.Remove(emisora.Nombre);
            config.Historial.Insert(0, emisora.Nombre);

            if (config.Historial.Count > 10)
                config.Historial = config.Historial.Take(10).ToList();

            ConfigService.GuardarConfig(config);
        }

        private void CargarTarjetas(List<Emisora> lista)
        {
            panelTarjetasEmisoras.Controls.Clear();

            foreach (Emisora emisora in lista)
            {
                EmisoraCard tarjeta = new EmisoraCard(emisora);
                tarjeta.CardClick += Tarjeta_CardClick;
                panelTarjetasEmisoras.Controls.Add(tarjeta);
            }
        }

        private void RestaurarUltimaEmisora()
        {
            if (string.IsNullOrWhiteSpace(config.UltimaEmisora))
                return;

            Emisora ultima = emisoras.FirstOrDefault(x => x.Nombre == config.UltimaEmisora);

            if (ultima == null)
                return;

            emisoraSeleccionada = ultima;

            lblEstado.Text = string.IsNullOrWhiteSpace(ultima.StreamUrl) ? "● OFFLINE" : "● EN VIVO";
            lblEmisoraActual.Text = ultima.Nombre;
            lblCategoria.Text = ultima.Categoria + " • " + ultima.Frecuencia;
            lblAhoraSuena.Text = "🎵 Última emisora: " + ultima.Nombre;

            CargarLogo(ultima);
            AplicarTemaEmisora(ultima);
            SeleccionarTarjetaVisual(ultima);
        }

        private void SeleccionarTarjetaVisual(Emisora emisora)
        {
            foreach (Control control in panelTarjetasEmisoras.Controls)
            {
                EmisoraCard tarjeta = control as EmisoraCard;

                if (tarjeta == null)
                    continue;

                if (tarjeta.Emisora.Nombre == emisora.Nombre)
                {
                    tarjetaSeleccionada = tarjeta;
                    tarjeta.Seleccionar();
                }
                else
                {
                    tarjeta.Deseleccionar();
                }
            }
        }

        private void Tarjeta_CardClick(object sender, EventArgs e)
        {
            OcultarDashboard();

            EmisoraCard tarjeta = sender as EmisoraCard;

            if (tarjeta == null)
                return;

            if (tarjetaSeleccionada != null)
                tarjetaSeleccionada.Deseleccionar();

            tarjetaSeleccionada = tarjeta;
            tarjetaSeleccionada.Seleccionar();

            emisoraSeleccionada = tarjeta.Emisora;

            config.UltimaEmisora = emisoraSeleccionada.Nombre;
            ConfigService.GuardarConfig(config);

            lblEstado.Text = string.IsNullOrWhiteSpace(emisoraSeleccionada.StreamUrl)
                ? "● OFFLINE"
                : "● EN VIVO";

            lblEmisoraActual.Text = emisoraSeleccionada.Nombre;
            lblCategoria.Text = emisoraSeleccionada.Categoria + " • " + emisoraSeleccionada.Frecuencia;
            lblAhoraSuena.Text = "🎵 Ahora suena: ninguna emisora";

            CargarLogo(emisoraSeleccionada);
            AplicarTemaEmisora(emisoraSeleccionada);
        }

        private void CargarLogo(Emisora emisora)
        {
            if (picLogoEmisora.Image != null)
            {
                picLogoEmisora.Image.Dispose();
                picLogoEmisora.Image = null;
            }

            if (emisora == null || string.IsNullOrWhiteSpace(emisora.Logo))
                return;

            string rutaLogo = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..",
                "..",
                "Assets",
                "Logos",
                emisora.Logo
            );

            if (File.Exists(rutaLogo))
            {
                using (Bitmap temp = new Bitmap(rutaLogo))
                    picLogoEmisora.Image = new Bitmap(temp);
            }
        }

        private void AplicarTemaEmisora(Emisora emisora)
        {
            if (emisora == null)
                return;

            Color color = emisora.ColorTema;

            lblEstado.ForeColor = color;
            lblCategoria.ForeColor = color;

            picLogoEmisora.GlowColor = Color.FromArgb(180, color.R, color.G, color.B);
            picLogoEmisora.Invalidate();

            EscucharEmisora.Color1 = OscurecerColor(color, 25);
            EscucharEmisora.Color2 = AclararColor(color, 55);
            EscucharEmisora.Invalidate();

            panelHero.BorderColor = color;
            panelHero.GradientColor1 = Color.FromArgb(10, 16, 35);
            panelHero.GradientColor2 = Color.FromArgb(
                80,
                Math.Max(0, color.R / 2),
                Math.Max(0, color.G / 2),
                Math.Max(0, color.B / 2)
            );
            panelHero.Invalidate();

            equalizer.BarColor = color;
            equalizer.Invalidate();
        }

        private Color AclararColor(Color color, int cantidad)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, color.R + cantidad),
                Math.Min(255, color.G + cantidad),
                Math.Min(255, color.B + cantidad)
            );
        }

        private Color OscurecerColor(Color color, int cantidad)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - cantidad),
                Math.Max(0, color.G - cantidad),
                Math.Max(0, color.B - cantidad)
            );
        }

        private void EscucharEmisora_Click(object sender, EventArgs e)
        {
            OcultarDashboard();

            if (emisoraSeleccionada == null)
            {
                MessageBox.Show("Selecciona una emisora.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(emisoraSeleccionada.StreamUrl))
            {
                MessageBox.Show(
                    "Esta emisora todavía no tiene un stream directo configurado.",
                    "Stream no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                lblEstado.Text = "STREAM NO DISPONIBLE";
                lblEstado.ForeColor = Color.FromArgb(220, 180, 60);
                equalizer.Stop();
                return;
            }

            try
            {
                lblEstado.Text = "CONECTANDO";

                axWindowsMediaPlayer1.URL = emisoraSeleccionada.StreamUrl;
                axWindowsMediaPlayer1.Ctlcontrols.play();

                lblEstado.Text = "REPRODUCIENDO";
                lblEmisoraActual.Text = emisoraSeleccionada.Nombre;
                lblCategoria.Text = emisoraSeleccionada.Categoria + " • " + emisoraSeleccionada.Frecuencia;
                lblAhoraSuena.Text = "🎵 Ahora suena: " + emisoraSeleccionada.Nombre;

                config.UltimaEmisora = emisoraSeleccionada.Nombre;
                AgregarAlHistorial(emisoraSeleccionada);

                CargarLogo(emisoraSeleccionada);
                AplicarTemaEmisora(emisoraSeleccionada);

                equalizer.Start();
            }
            catch (Exception ex)
            {
                lblEstado.Text = "ERROR";
                lblEstado.ForeColor = Color.FromArgb(220, 60, 75);
                equalizer.Stop();

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

            equalizer.Stop();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "Buscar emisora...";
            txtBuscar.ForeColor = Color.FromArgb(150, 160, 185);

            emisorasFiltradas = new List<Emisora>(emisoras);
            emisoraSeleccionada = null;
            tarjetaSeleccionada = null;

            CargarTarjetas(emisorasFiltradas);

            lblEstado.Text = "LISTA ACTUALIZADA";
            lblEstado.ForeColor = Color.FromArgb(45, 180, 255);
            lblEmisoraActual.Text = "Selecciona FM";
            lblCategoria.Text = "Radio dominicana";
            lblAhoraSuena.Text = "🎵 Ahora suena: ninguna emisora";

            panelHero.BorderColor = Color.FromArgb(35, 45, 85);
            panelHero.GradientColor1 = Color.FromArgb(10, 16, 35);
            panelHero.GradientColor2 = Color.FromArgb(18, 24, 48);
            panelHero.Invalidate();

            equalizer.Stop();
            CargarLogo(null);
            MostrarDashboard();
        }

        private void btnFavoritos_Click(object sender, EventArgs e)
        {
            OcultarDashboard();

            if (emisoraSeleccionada == null)
            {
                MessageBox.Show(
                    "Selecciona una emisora para marcarla como favorita.",
                    "Favoritos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            emisoraSeleccionada.Favorita = !emisoraSeleccionada.Favorita;
            GuardarFavoritos();

            if (tarjetaSeleccionada != null)
                tarjetaSeleccionada.RefrescarFavorita();

            string mensaje = emisoraSeleccionada.Favorita
                ? "Emisora agregada a favoritos."
                : "Emisora eliminada de favoritos.";

            lblAhoraSuena.Text = "⭐ " + mensaje;
            MostrarDashboard();
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

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscar.Text == "Buscar emisora...")
                return;

            OcultarDashboard();

            string texto = txtBuscar.Text.ToLower();

            emisorasFiltradas = emisoras
                .Where(x =>
                    x.Nombre.ToLower().Contains(texto) ||
                    x.Categoria.ToLower().Contains(texto) ||
                    x.Frecuencia.ToLower().Contains(texto) ||
                    x.Provincia.ToLower().Contains(texto)
                )
                .ToList();

            emisoraSeleccionada = null;
            tarjetaSeleccionada = null;

            CargarTarjetas(emisorasFiltradas);
            CargarLogo(null);
            equalizer.Stop();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Diseño fijo por ahora.
        }
    }
}