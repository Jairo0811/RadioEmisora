using System.Drawing;
using System.Windows.Forms;

namespace ProyectoFinalDCU
{
    public class FrmAcercaDe : Form
    {
        public FrmAcercaDe()
        {
            this.Text = "Acerca de";
            this.Size = new Size(420, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(12, 18, 35);

            PictureBox picITLA = new PictureBox();
            picITLA.Size = new Size(170, 105);
            picITLA.SizeMode = PictureBoxSizeMode.Zoom;
            picITLA.Location = new Point(125, 15);
            picITLA.Image = Image.FromFile(
                System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "..",
                    "Assets",
                    "itla.png"
                )
            );

            Label lblTitulo = new Label();
            lblTitulo.Text = "RadioEmisora RD";
            lblTitulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Location = new Point(0, 125);
            lblTitulo.Size = new Size(420, 40);

            Label lblVersion = new Label();
            lblVersion.Text = "Versión 2.0";
            lblVersion.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblVersion.ForeColor = Color.FromArgb(150, 160, 185);
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;
            lblVersion.Location = new Point(0, 165);
            lblVersion.Size = new Size(420, 25);

            Label lblInfo = new Label();
            lblInfo.Text =
                "Proyecto Académico Modernizado\n\n" +
                "Francis Jairo Matías Rosario\n" +
                "Matrícula: 2015-2984\n\n" +
              "Diseño Centrado en el Usuario\n" +
"(SOF-010)\n\n" +
                "Profesor: Juan Martínez López\n\n";
               
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.ForeColor = Color.FromArgb(210, 220, 240);
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInfo.Location = new Point(20, 200);
            lblInfo.Size = new Size(380, 195);

            Button btnAceptar = new Button();
            btnAceptar.Text = "Aceptar";
            btnAceptar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnAceptar.ForeColor = Color.White;
            btnAceptar.BackColor = Color.FromArgb(95, 45, 230);
            btnAceptar.FlatStyle = FlatStyle.Flat;
            btnAceptar.FlatAppearance.BorderSize = 0;
            btnAceptar.Size = new Size(170, 45);
            btnAceptar.Location = new Point(125, 420);
            btnAceptar.Click += (s, e) => this.Close();

            this.Controls.Add(btnAceptar);
            this.Controls.Add(lblInfo);
            this.Controls.Add(lblVersion);
            this.Controls.Add(lblTitulo);
            this.Controls.Add(picITLA);
        }
    }
}