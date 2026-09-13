namespace icsEditor
{
    partial class PurgeDialog
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
            this.btnTout = new System.Windows.Forms.Button();
            this.btnAnciennes = new System.Windows.Forms.Button();
            this.btnAnnuler = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(20, 20);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(470, 110);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // btnTout
            //
            this.btnTout.Location = new System.Drawing.Point(20, 150);
            this.btnTout.Name = "btnTout";
            this.btnTout.Size = new System.Drawing.Size(150, 30);
            this.btnTout.TabIndex = 1;
            this.btnTout.Text = "Tout purger";
            this.btnTout.UseVisualStyleBackColor = true;
            this.btnTout.Click += new System.EventHandler(this.btnTout_Click);
            //
            // btnAnciennes
            //
            this.btnAnciennes.Location = new System.Drawing.Point(185, 150);
            this.btnAnciennes.Name = "btnAnciennes";
            this.btnAnciennes.Size = new System.Drawing.Size(170, 30);
            this.btnAnciennes.TabIndex = 2;
            this.btnAnciennes.Text = "Anciennes seulement";
            this.btnAnciennes.UseVisualStyleBackColor = true;
            this.btnAnciennes.Click += new System.EventHandler(this.btnAnciennes_Click);
            //
            // btnAnnuler
            //
            this.btnAnnuler.Location = new System.Drawing.Point(370, 150);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Size = new System.Drawing.Size(110, 30);
            this.btnAnnuler.TabIndex = 3;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.UseVisualStyleBackColor = true;
            this.btnAnnuler.Click += new System.EventHandler(this.btnAnnuler_Click);
            //
            // PurgeDialog
            //
            this.AcceptButton = this.btnAnciennes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnnuler;
            this.ClientSize = new System.Drawing.Size(510, 200);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnAnciennes);
            this.Controls.Add(this.btnTout);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PurgeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Purger les annulations";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnTout;
        private System.Windows.Forms.Button btnAnciennes;
        private System.Windows.Forms.Button btnAnnuler;
    }
}
