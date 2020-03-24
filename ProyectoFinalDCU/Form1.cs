using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinalDCU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void ListadoDeEmisoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListadoDeEmisoras.Items.Add("Radio Popular 950 AM");
            ListadoDeEmisoras.Items.Add("Pimera 88.1 FM");
            ListadoDeEmisoras.Items.Add("Fuego 90.1 FM");
            ListadoDeEmisoras.Items.Add("Radio Cima 100.5 FM");
            ListadoDeEmisoras.Items.Add("Z 101.3 FM");
            ListadoDeEmisoras.Items.Add("Escandalo 102.5 FM");
            ListadoDeEmisoras.Items.Add("Mortal 104.9 FM");
            ListadoDeEmisoras.Items.Add("La Bacana 105.9 FM FM");
            ListadoDeEmisoras.Items.Add("Disco 106.1 FM");
        }

        private void ListadoDeEmisoras_Click(object sender, EventArgs e)
        {
            string URL = string.Empty;

            switch (ListadoDeEmisoras.SelectedIndex)
            {
                case 1:
                    URL = "http://www.radios.com.do/popular-santo-domingo/";
                    break;

                case 2:
                    URL = "http://www.radios.com.do/#primera-88-1-fm";
                    break;

                case 3:
                    URL = "http://www.radios.com.do/fuego-90-fm/";
                    break;

                case 4:
                    URL = "http://www.radios.com.do/cima/";
                    break;

                case 5:
                    URL = "http://www.radios.com.do/la-z/";
                    break;

                case 6:
                    URL = "http://www.radios.com.do/escandalo-fm-santo-domingo/";
                    break;

                case 7:
                    URL = "http://www.mortal1049.com/";
                    break;

                case 8:
                    URL = "http://www.radios.com.do/la-bakana-105-9/";
                    break;

                case 9:
                    URL = "http://www.radios.com.do/disco/";
                    break;

                default:
                    MessageBox.Show("Selecciona una emisora", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

            }
            if (!URL.Equals("")) axWindowsMediaPlayer1.URL = URL;
        }

        private void Form1_Rezise(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Width = this.Width - 30;
            ListadoDeEmisoras.Width = this.Width - 30;
            EscucharEmisora.Width = this.Width - 30;
        }
        
    }
}
