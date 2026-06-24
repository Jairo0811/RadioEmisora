namespace ProyectoFinalDCU
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelHero;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Panel panelNowPlaying;

        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblEmisoras;
        private System.Windows.Forms.ListBox ListadoDeEmisoras;

        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblEmisoraActual;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.Label lblAhoraSuena;

        private System.Windows.Forms.Button EscucharEmisora;
        private System.Windows.Forms.Button btnDetener;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnFavoritos;
        private System.Windows.Forms.Button btnAcercaDe;
        private System.Windows.Forms.Button btnSalir;

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            this.panelSidebar = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelHero = new System.Windows.Forms.Panel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.panelNowPlaying = new System.Windows.Forms.Panel();

            this.lblLogo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblEmisoras = new System.Windows.Forms.Label();
            this.ListadoDeEmisoras = new System.Windows.Forms.ListBox();

            this.lblEstado = new System.Windows.Forms.Label();
            this.lblEmisoraActual = new System.Windows.Forms.Label();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.lblAhoraSuena = new System.Windows.Forms.Label();

            this.EscucharEmisora = new System.Windows.Forms.Button();
            this.btnDetener = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnFavoritos = new System.Windows.Forms.Button();
            this.btnAcercaDe = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();

            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();

            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.panelSidebar.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelHero.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelNowPlaying.SuspendLayout();
            this.SuspendLayout();

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(8, 12, 25);
            this.ClientSize = new System.Drawing.Size(1180, 720);
            this.MinimumSize = new System.Drawing.Size(1050, 650);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RadioEmisora RD";
            this.Resize += new System.EventHandler(this.Form1_Resize);

            // panelSidebar
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(12, 18, 35);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Width = 340;
            this.panelSidebar.Controls.Add(this.lblLogo);
            this.panelSidebar.Controls.Add(this.lblTitulo);
            this.panelSidebar.Controls.Add(this.lblSubtitulo);
            this.panelSidebar.Controls.Add(this.txtBuscar);
            this.panelSidebar.Controls.Add(this.lblEmisoras);
            this.panelSidebar.Controls.Add(this.ListadoDeEmisoras);

            // lblLogo
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI Emoji", 44F);
            this.lblLogo.ForeColor = System.Drawing.Color.White;
            this.lblLogo.Location = new System.Drawing.Point(0, 25);
            this.lblLogo.Size = new System.Drawing.Size(340, 70);
            this.lblLogo.Text = "🎧";
            this.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblTitulo
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 23F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(20, 100);
            this.lblTitulo.Size = new System.Drawing.Size(300, 48);
            this.lblTitulo.Text = "RadioEmisora RD";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblSubtitulo
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(170, 180, 205);
            this.lblSubtitulo.Location = new System.Drawing.Point(20, 148);
            this.lblSubtitulo.Size = new System.Drawing.Size(300, 28);
            this.lblSubtitulo.Text = "Tu música, tu compañía";
            this.lblSubtitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // txtBuscar
            this.txtBuscar.BackColor = System.Drawing.Color.FromArgb(24, 34, 60);
            this.txtBuscar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtBuscar.ForeColor = System.Drawing.Color.White;
            this.txtBuscar.Location = new System.Drawing.Point(25, 205);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(290, 34);
            this.txtBuscar.TabIndex = 0;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);

            // lblEmisoras
            this.lblEmisoras.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblEmisoras.ForeColor = System.Drawing.Color.FromArgb(45, 180, 255);
            this.lblEmisoras.Location = new System.Drawing.Point(25, 265);
            this.lblEmisoras.Size = new System.Drawing.Size(290, 28);
            this.lblEmisoras.Text = "📻 EMISORAS DISPONIBLES";

            // ListadoDeEmisoras
            this.ListadoDeEmisoras.BackColor = System.Drawing.Color.FromArgb(18, 26, 48);
            this.ListadoDeEmisoras.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListadoDeEmisoras.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ListadoDeEmisoras.ForeColor = System.Drawing.Color.White;
            this.ListadoDeEmisoras.FormattingEnabled = true;
            this.ListadoDeEmisoras.ItemHeight = 28;
            this.ListadoDeEmisoras.Location = new System.Drawing.Point(25, 305);
            this.ListadoDeEmisoras.Name = "ListadoDeEmisoras";
            this.ListadoDeEmisoras.Size = new System.Drawing.Size(290, 336);
            this.ListadoDeEmisoras.TabIndex = 1;
            this.ListadoDeEmisoras.SelectedIndexChanged += new System.EventHandler(this.ListadoDeEmisoras_SelectedIndexChanged);

            // panelContent
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(8, 12, 25);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Controls.Add(this.panelHero);
            this.panelContent.Controls.Add(this.panelControls);
            this.panelContent.Controls.Add(this.panelNowPlaying);
            this.panelContent.Controls.Add(this.btnActualizar);
            this.panelContent.Controls.Add(this.btnFavoritos);
            this.panelContent.Controls.Add(this.btnAcercaDe);
            this.panelContent.Controls.Add(this.btnSalir);

            // panelHero
            this.panelHero.BackColor = System.Drawing.Color.FromArgb(12, 18, 35);
            this.panelHero.Location = new System.Drawing.Point(35, 35);
            this.panelHero.Size = new System.Drawing.Size(760, 235);
            this.panelHero.Controls.Add(this.lblEstado);
            this.panelHero.Controls.Add(this.lblEmisoraActual);
            this.panelHero.Controls.Add(this.lblCategoria);
            this.panelHero.Controls.Add(this.axWindowsMediaPlayer1);

            // lblEstado
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(45, 180, 255);
            this.lblEstado.Location = new System.Drawing.Point(35, 30);
            this.lblEstado.Size = new System.Drawing.Size(360, 30);
            this.lblEstado.Text = "EN ESPERA";

            // lblEmisoraActual
            this.lblEmisoraActual.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold);
            this.lblEmisoraActual.ForeColor = System.Drawing.Color.White;
            this.lblEmisoraActual.Location = new System.Drawing.Point(35, 70);
            this.lblEmisoraActual.Size = new System.Drawing.Size(420, 70);
            this.lblEmisoraActual.Text = "Selecciona FM";

            // lblCategoria
            this.lblCategoria.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.lblCategoria.ForeColor = System.Drawing.Color.FromArgb(170, 80, 255);
            this.lblCategoria.Location = new System.Drawing.Point(40, 145);
            this.lblCategoria.Size = new System.Drawing.Size(420, 35);
            this.lblCategoria.Text = "Radio dominicana";

            // axWindowsMediaPlayer1
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(480, 55);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(250, 125);
            this.axWindowsMediaPlayer1.TabIndex = 2;

            // panelControls
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(12, 18, 35);
            this.panelControls.Location = new System.Drawing.Point(35, 295);
            this.panelControls.Size = new System.Drawing.Size(760, 145);
            this.panelControls.Controls.Add(this.EscucharEmisora);
            this.panelControls.Controls.Add(this.btnDetener);

            // EscucharEmisora
            this.EscucharEmisora.BackColor = System.Drawing.Color.FromArgb(95, 45, 230);
            this.EscucharEmisora.FlatAppearance.BorderSize = 0;
            this.EscucharEmisora.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EscucharEmisora.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.EscucharEmisora.ForeColor = System.Drawing.Color.White;
            this.EscucharEmisora.Location = new System.Drawing.Point(160, 42);
            this.EscucharEmisora.Name = "EscucharEmisora";
            this.EscucharEmisora.Size = new System.Drawing.Size(210, 65);
            this.EscucharEmisora.Text = "▶ Reproducir";
            this.EscucharEmisora.UseVisualStyleBackColor = false;
            this.EscucharEmisora.Click += new System.EventHandler(this.EscucharEmisora_Click);

            // btnDetener
            this.btnDetener.BackColor = System.Drawing.Color.FromArgb(210, 55, 75);
            this.btnDetener.FlatAppearance.BorderSize = 0;
            this.btnDetener.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetener.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnDetener.ForeColor = System.Drawing.Color.White;
            this.btnDetener.Location = new System.Drawing.Point(400, 42);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(210, 65);
            this.btnDetener.Text = "⏹ Detener";
            this.btnDetener.UseVisualStyleBackColor = false;
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);

            // panelNowPlaying
            this.panelNowPlaying.BackColor = System.Drawing.Color.FromArgb(12, 18, 35);
            this.panelNowPlaying.Location = new System.Drawing.Point(35, 465);
            this.panelNowPlaying.Size = new System.Drawing.Size(760, 90);
            this.panelNowPlaying.Controls.Add(this.lblAhoraSuena);

            // lblAhoraSuena
            this.lblAhoraSuena.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblAhoraSuena.ForeColor = System.Drawing.Color.White;
            this.lblAhoraSuena.Location = new System.Drawing.Point(30, 25);
            this.lblAhoraSuena.Size = new System.Drawing.Size(700, 40);
            this.lblAhoraSuena.Text = "🎵 Ahora suena: ninguna emisora";

            // btnActualizar
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(0, 95, 200);
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(35, 585);
            this.btnActualizar.Size = new System.Drawing.Size(170, 55);
            this.btnActualizar.Text = "↻ Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);

            // btnFavoritos
            this.btnFavoritos.BackColor = System.Drawing.Color.FromArgb(85, 35, 150);
            this.btnFavoritos.FlatAppearance.BorderSize = 0;
            this.btnFavoritos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFavoritos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnFavoritos.ForeColor = System.Drawing.Color.White;
            this.btnFavoritos.Location = new System.Drawing.Point(225, 585);
            this.btnFavoritos.Size = new System.Drawing.Size(170, 55);
            this.btnFavoritos.Text = "☆ Favoritos";
            this.btnFavoritos.UseVisualStyleBackColor = false;
            this.btnFavoritos.Click += new System.EventHandler(this.btnFavoritos_Click);

            // btnAcercaDe
            this.btnAcercaDe.BackColor = System.Drawing.Color.FromArgb(0, 105, 120);
            this.btnAcercaDe.FlatAppearance.BorderSize = 0;
            this.btnAcercaDe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcercaDe.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnAcercaDe.ForeColor = System.Drawing.Color.White;
            this.btnAcercaDe.Location = new System.Drawing.Point(415, 585);
            this.btnAcercaDe.Size = new System.Drawing.Size(170, 55);
            this.btnAcercaDe.Text = "ⓘ Acerca de";
            this.btnAcercaDe.UseVisualStyleBackColor = false;
            this.btnAcercaDe.Click += new System.EventHandler(this.btnAcercaDe_Click);

            // btnSalir
            this.btnSalir.BackColor = System.Drawing.Color.FromArgb(170, 20, 40);
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSalir.ForeColor = System.Drawing.Color.White;
            this.btnSalir.Location = new System.Drawing.Point(605, 585);
            this.btnSalir.Size = new System.Drawing.Size(190, 55);
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);

            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelSidebar);

            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.panelContent.ResumeLayout(false);
            this.panelHero.ResumeLayout(false);
            this.panelControls.ResumeLayout(false);
            this.panelNowPlaying.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}