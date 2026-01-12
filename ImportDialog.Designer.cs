namespace icsEditor
{
    partial class ImportDialog
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

        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnRemplacer = new System.Windows.Forms.Button();
            this.btnAjouter = new System.Windows.Forms.Button();
            this.btnAnnuler = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(20, 20);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(410, 60);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRemplacer
            //
            this.btnRemplacer.Location = new System.Drawing.Point(30, 100);
            this.btnRemplacer.Name = "btnRemplacer";
            this.btnRemplacer.Size = new System.Drawing.Size(110, 30);
            this.btnRemplacer.TabIndex = 1;
            this.btnRemplacer.Text = "Remplacer";
            this.btnRemplacer.UseVisualStyleBackColor = true;
            this.btnRemplacer.Click += new System.EventHandler(this.btnRemplacer_Click);
            //
            // btnAjouter
            //
            this.btnAjouter.Location = new System.Drawing.Point(160, 100);
            this.btnAjouter.Name = "btnAjouter";
            this.btnAjouter.Size = new System.Drawing.Size(110, 30);
            this.btnAjouter.TabIndex = 2;
            this.btnAjouter.Text = "Ajouter";
            this.btnAjouter.UseVisualStyleBackColor = true;
            this.btnAjouter.Click += new System.EventHandler(this.btnAjouter_Click);
            //
            // btnAnnuler
            //
            this.btnAnnuler.Location = new System.Drawing.Point(290, 100);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Size = new System.Drawing.Size(110, 30);
            this.btnAnnuler.TabIndex = 3;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.UseVisualStyleBackColor = true;
            this.btnAnnuler.Click += new System.EventHandler(this.btnAnnuler_Click);
            //
            // ImportDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 160);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnAjouter);
            this.Controls.Add(this.btnRemplacer);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Importer";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnRemplacer;
        private System.Windows.Forms.Button btnAjouter;
        private System.Windows.Forms.Button btnAnnuler;
    }
}
