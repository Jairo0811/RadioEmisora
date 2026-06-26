namespace ProyectoFinalDCU
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.ListadoDeEmisoras = new System.Windows.Forms.ListBox();
            this.EscucharEmisora = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();

            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(10, 35);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(780, 180);
            this.axWindowsMediaPlayer1.TabIndex = 0;

            // 
            // ListadoDeEmisoras
            // 
            this.ListadoDeEmisoras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListadoDeEmisoras.BackColor = System.Drawing.Color.Maroon;
            this.ListadoDeEmisoras.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListadoDeEmisoras.ForeColor = System.Drawing.Color.White;
            this.ListadoDeEmisoras.FormattingEnabled = true;
            this.ListadoDeEmisoras.ItemHeight = 27;
            this.ListadoDeEmisoras.Location = new System.Drawing.Point(10, 227);
            this.ListadoDeEmisoras.Name = "ListadoDeEmisoras";
            this.ListadoDeEmisoras.Size = new System.Drawing.Size(780, 160);
            this.ListadoDeEmisoras.TabIndex = 3;
            this.ListadoDeEmisoras.DoubleClick += new System.EventHandler(this.ListadoDeEmisoras_DoubleClick);

            // 
            // EscucharEmisora
            // 
            this.EscucharEmisora.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EscucharEmisora.BackColor = System.Drawing.Color.Lime;
            this.EscucharEmisora.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EscucharEmisora.ForeColor = System.Drawing.Color.DarkBlue;
            this.EscucharEmisora.Location = new System.Drawing.Point(10, 399);
            this.EscucharEmisora.Name = "EscucharEmisora";
            this.EscucharEmisora.Size = new System.Drawing.Size(780, 41);
            this.EscucharEmisora.TabIndex = 5;
            this.EscucharEmisora.Text = "Escuchar Emisora";
            this.EscucharEmisora.UseVisualStyleBackColor = false;
            this.EscucharEmisora.Click += new System.EventHandler(this.ListadoDeEmisoras_Click);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Controls.Add(this.ListadoDeEmisoras);
            this.Controls.Add(this.EscucharEmisora);
            this.Name = "Form1";
            this.Text = "DOMINICAN RADIO MATIAS AM/FM";
            this.Resize += new System.EventHandler(this.Form1_Rezise);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);
        }



        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.ListBox ListadoDeEmisoras;
        private System.Windows.Forms.Button EscucharEmisora;
    }
}