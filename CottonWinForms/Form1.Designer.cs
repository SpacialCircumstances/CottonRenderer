namespace CottonWinForms
{
    partial class Form
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.target = new System.Windows.Forms.PictureBox();
            this.renderButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.target)).BeginInit();
            this.SuspendLayout();
            // 
            // target
            // 
            this.target.Location = new System.Drawing.Point(0, 0);
            this.target.Name = "target";
            this.target.Size = new System.Drawing.Size(800, 600);
            this.target.TabIndex = 0;
            this.target.TabStop = false;
            // 
            // renderButton
            // 
            this.renderButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderButton.Location = new System.Drawing.Point(0, 597);
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(800, 61);
            this.renderButton.TabIndex = 1;
            this.renderButton.Text = "Render!";
            this.renderButton.UseVisualStyleBackColor = true;
            this.renderButton.Click += new System.EventHandler(this.RenderButton_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 657);
            this.Controls.Add(this.renderButton);
            this.Controls.Add(this.target);
            this.Name = "Form";
            this.Text = "Cotton";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.target)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox target;
        private System.Windows.Forms.Button renderButton;
    }
}

