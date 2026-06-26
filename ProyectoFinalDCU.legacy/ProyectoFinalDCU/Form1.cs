using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProyectoFinalDCU
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, string> emisoras = new Dictionary<string, string>
        {
            { "Radio Popular 950 AM", "https://stream-175.surfernetwork.com/v5y1wkvoilivv" },
            { "Primera FM 88.1 FM", "https://n0f.radiojar.com/5705c71sn2zuv" },
            { "Fuego 90.1 FM", "https://radiordomi.com/8390/stream" },
            { "Radio Cima 100.5 FM", "https://sonicpanel.streaming10.net:8146/stream" },
            { "Z 101.3 FM", "https://streaming.z101digital.com/z101" },
            { "Escándalo 102.5 FM", "https://stream-284.surfernetwork.com/iwqlyzpyry1uv" },
            { "Mortal 104.9 FM", "https://n07.radiojar.com/x84tcy2wm2zuv" },
            { "Independencia 93.3 FM", "https://n06.radiojar.com/nc893hafc8zuv" },
            { "Disco 106.1 FM", "https://radiordomi.com/8118/stream" },
            { "Alofoke 99.3 FM", "https://radiordomi.com:8566/stream" },
            { "La Mega 97.9 FM", "https://liveaudio.lamusica.com/NY_WSKQ_icy" }
        };

        public Form1()
        {
            InitializeComponent();
            CargarEmisoras();
        }

        private void CargarEmisoras()
        {
            ListadoDeEmisoras.Items.Clear();

            foreach (string nombre in emisoras.Keys)
            {
                ListadoDeEmisoras.Items.Add(nombre);
            }

            if (ListadoDeEmisoras.Items.Count > 0)
                ListadoDeEmisoras.SelectedIndex = 0;
        }

        private void ListadoDeEmisoras_Click(object sender, EventArgs e)
        {
            ReproducirEmisoraSeleccionada();
        }

        private void ListadoDeEmisoras_DoubleClick(object sender, EventArgs e)
        {
            ReproducirEmisoraSeleccionada();
        }

        private void ReproducirEmisoraSeleccionada()
        {
            if (ListadoDeEmisoras.SelectedItem == null)
            {
                MessageBox.Show(
                    "Selecciona una emisora.",
                    "RadioEmisora RD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string nombreEmisora = ListadoDeEmisoras.SelectedItem.ToString();

            if (!emisoras.ContainsKey(nombreEmisora))
            {
                MessageBox.Show(
                    "No se encontró el enlace de esta emisora.",
                    "RadioEmisora RD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            try
            {
                axWindowsMediaPlayer1.URL = emisoras[nombreEmisora];
                axWindowsMediaPlayer1.Ctlcontrols.play();

                Text = "Reproduciendo: " + nombreEmisora;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo reproducir la emisora.\n\nDetalle: " + ex.Message,
                    "Error de reproducción",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Form1_Rezise(object sender, EventArgs e)
        {
            int margen = 10;
            int espacio = 12;

            int altoBoton = 41;
            int altoLista = 160;

            ListadoDeEmisoras.Left = margen;
            ListadoDeEmisoras.Width = ClientSize.Width - (margen * 2);
            ListadoDeEmisoras.Height = altoLista;
            ListadoDeEmisoras.Top = ClientSize.Height - margen - altoBoton - espacio - altoLista;

            EscucharEmisora.Left = margen;
            EscucharEmisora.Width = ClientSize.Width - (margen * 2);
            EscucharEmisora.Height = altoBoton;
            EscucharEmisora.Top = ListadoDeEmisoras.Bottom + espacio;

            axWindowsMediaPlayer1.Left = margen;
            axWindowsMediaPlayer1.Top = 35;
            axWindowsMediaPlayer1.Width = ClientSize.Width - (margen * 2);
            axWindowsMediaPlayer1.Height = ListadoDeEmisoras.Top - axWindowsMediaPlayer1.Top - espacio;
        }
    }
}