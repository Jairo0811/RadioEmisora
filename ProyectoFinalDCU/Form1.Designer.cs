namespace ProyectoFinalDCU
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.EscucharEmisora = new System.Windows.Forms.Button();
            this.ListadoDeEmisoras = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(3, 61);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(785, 143);
            this.axWindowsMediaPlayer1.TabIndex = 0;
            // 
            // EscucharEmisora
            // 
            this.EscucharEmisora.BackColor = System.Drawing.Color.Lime;
            this.EscucharEmisora.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EscucharEmisora.ForeColor = System.Drawing.Color.DarkBlue;
            this.EscucharEmisora.Location = new System.Drawing.Point(3, 210);
            this.EscucharEmisora.Name = "EscucharEmisora";
            this.EscucharEmisora.Size = new System.Drawing.Size(785, 73);
            this.EscucharEmisora.TabIndex = 5;
            this.EscucharEmisora.Text = "Escuchar Emisora";
            this.EscucharEmisora.UseVisualStyleBackColor = false;
            this.EscucharEmisora.Click += new System.EventHandler(this.ListadoDeEmisoras_Click);
            // 
            // ListadoDeEmisoras
            // 
            this.ListadoDeEmisoras.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListadoDeEmisoras.BackColor = System.Drawing.Color.DarkRed;
            this.ListadoDeEmisoras.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListadoDeEmisoras.FormattingEnabled = true;
            this.ListadoDeEmisoras.ItemHeight = 27;
            this.ListadoDeEmisoras.Items.AddRange(new object[] {
            "Radio Popular 950 AM",
            "Pimera 88.1 FM",
            "Fuego 90.1 FM",
            "Radio Cima 100.5 FM",
            "La Z 101.3 FM",
            "Escandalo 102.5 FM",
            "Mortal 104.9 FM",
            "La Bacana 105.9 FM",
            "Disco 106.1 FM"});
            this.ListadoDeEmisoras.Location = new System.Drawing.Point(3, 289);
            this.ListadoDeEmisoras.Name = "ListadoDeEmisoras";
            this.ListadoDeEmisoras.Size = new System.Drawing.Size(793, 139);
            this.ListadoDeEmisoras.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ListadoDeEmisoras);
            this.Controls.Add(this.EscucharEmisora);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "Form1";
            this.Text = "DOMINICAN RADIO MATIAS AM/FM";
            this.Resize += new System.EventHandler(this.Form1_Rezise);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.Button EscucharEmisora;
        private System.Windows.Forms.ListBox ListadoDeEmisoras;
    }
}

