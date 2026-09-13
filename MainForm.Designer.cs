namespace icsEditor
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.eventGroupBox = new System.Windows.Forms.GroupBox();
            this.eventPanel = new MetroFramework.Controls.MetroPanel();
            this.lblLibelle = new MetroFramework.Controls.MetroLabel();
            this.txtLibelle = new MetroFramework.Controls.MetroTextBox();
            this.lblDateDebut = new MetroFramework.Controls.MetroLabel();
            this.dtpDateDebut = new System.Windows.Forms.DateTimePicker();
            this.lblDateFin = new MetroFramework.Controls.MetroLabel();
            this.dtpDateFin = new System.Windows.Forms.DateTimePicker();
            this.lblHeureDebut = new MetroFramework.Controls.MetroLabel();
            this.dtpHeureDebut = new System.Windows.Forms.DateTimePicker();
            this.lblHeureFin = new MetroFramework.Controls.MetroLabel();
            this.dtpHeureFin = new System.Windows.Forms.DateTimePicker();
            this.lblLieu = new MetroFramework.Controls.MetroLabel();
            this.txtLieu = new MetroFramework.Controls.MetroTextBox();
            this.lblDescription = new MetroFramework.Controls.MetroLabel();
            this.txtDescription = new MetroFramework.Controls.MetroTextBox();
            this.btnAdd = new MetroFramework.Controls.MetroButton();
            this.btnUpdate = new MetroFramework.Controls.MetroButton();
            this.btnClear = new MetroFramework.Controls.MetroButton();
            this.eventsListBox = new System.Windows.Forms.ListBox();
            this.btnDelete = new MetroFramework.Controls.MetroButton();
            this.btnDeleteAll = new MetroFramework.Controls.MetroButton();
            this.btnMoveUp = new MetroFramework.Controls.MetroButton();
            this.btnMoveDown = new MetroFramework.Controls.MetroButton();
            this.btnExportICS = new MetroFramework.Controls.MetroButton();
            this.btnImportICS = new MetroFramework.Controls.MetroButton();
            this.btnPurgeAnnulations = new MetroFramework.Controls.MetroButton();
            this.toolTipPurge = new System.Windows.Forms.ToolTip(this.components);
            this.lblVersion = new MetroFramework.Controls.MetroLabel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.eventGroupBox.SuspendLayout();
            this.eventPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // eventGroupBox
            // 
            this.eventGroupBox.Controls.Add(this.eventPanel);
            this.eventGroupBox.Location = new System.Drawing.Point(23, 80);
            this.eventGroupBox.Name = "eventGroupBox";
            this.eventGroupBox.Size = new System.Drawing.Size(465, 380);
            this.eventGroupBox.TabIndex = 0;
            this.eventGroupBox.TabStop = false;
            this.eventGroupBox.Text = "Événement";
            // 
            // eventPanel
            // 
            this.eventPanel.Controls.Add(this.lblLibelle);
            this.eventPanel.Controls.Add(this.txtLibelle);
            this.eventPanel.Controls.Add(this.lblDateDebut);
            this.eventPanel.Controls.Add(this.dtpDateDebut);
            this.eventPanel.Controls.Add(this.lblDateFin);
            this.eventPanel.Controls.Add(this.dtpDateFin);
            this.eventPanel.Controls.Add(this.lblHeureDebut);
            this.eventPanel.Controls.Add(this.dtpHeureDebut);
            this.eventPanel.Controls.Add(this.lblHeureFin);
            this.eventPanel.Controls.Add(this.dtpHeureFin);
            this.eventPanel.Controls.Add(this.lblLieu);
            this.eventPanel.Controls.Add(this.txtLieu);
            this.eventPanel.Controls.Add(this.lblDescription);
            this.eventPanel.Controls.Add(this.txtDescription);
            this.eventPanel.Controls.Add(this.btnAdd);
            this.eventPanel.Controls.Add(this.btnUpdate);
            this.eventPanel.Controls.Add(this.btnClear);
            this.eventPanel.HorizontalScrollbarBarColor = true;
            this.eventPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.eventPanel.HorizontalScrollbarSize = 10;
            this.eventPanel.Location = new System.Drawing.Point(6, 20);
            this.eventPanel.Name = "eventPanel";
            this.eventPanel.Size = new System.Drawing.Size(459, 354);
            this.eventPanel.TabIndex = 0;
            this.eventPanel.VerticalScrollbarBarColor = true;
            this.eventPanel.VerticalScrollbarHighlightOnWheel = false;
            this.eventPanel.VerticalScrollbarSize = 10;
            // 
            // lblLibelle
            // 
            this.lblLibelle.AutoSize = true;
            this.lblLibelle.Location = new System.Drawing.Point(3, 10);
            this.lblLibelle.Name = "lblLibelle";
            this.lblLibelle.Size = new System.Drawing.Size(59, 20);
            this.lblLibelle.TabIndex = 0;
            this.lblLibelle.Text = "Libellé *";
            // 
            // txtLibelle
            // 
            // 
            // 
            // 
            this.txtLibelle.CustomButton.Image = null;
            this.txtLibelle.CustomButton.Location = new System.Drawing.Point(272, 1);
            this.txtLibelle.CustomButton.Name = "";
            this.txtLibelle.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.txtLibelle.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtLibelle.CustomButton.TabIndex = 1;
            this.txtLibelle.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtLibelle.CustomButton.UseSelectable = true;
            this.txtLibelle.CustomButton.Visible = false;
            this.txtLibelle.Lines = new string[0];
            this.txtLibelle.Location = new System.Drawing.Point(149, 10);
            this.txtLibelle.MaxLength = 32767;
            this.txtLibelle.Name = "txtLibelle";
            this.txtLibelle.PasswordChar = '\0';
            this.txtLibelle.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtLibelle.SelectedText = "";
            this.txtLibelle.SelectionLength = 0;
            this.txtLibelle.SelectionStart = 0;
            this.txtLibelle.ShortcutsEnabled = true;
            this.txtLibelle.Size = new System.Drawing.Size(296, 25);
            this.txtLibelle.TabIndex = 0;
            this.txtLibelle.UseSelectable = true;
            this.txtLibelle.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtLibelle.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // lblDateDebut
            // 
            this.lblDateDebut.AutoSize = true;
            this.lblDateDebut.Location = new System.Drawing.Point(3, 45);
            this.lblDateDebut.Name = "lblDateDebut";
            this.lblDateDebut.Size = new System.Drawing.Size(108, 20);
            this.lblDateDebut.TabIndex = 2;
            this.lblDateDebut.Text = "Date de début *";
            // 
            // dtpDateDebut
            // 
            this.dtpDateDebut.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateDebut.Location = new System.Drawing.Point(149, 45);
            this.dtpDateDebut.Name = "dtpDateDebut";
            this.dtpDateDebut.Size = new System.Drawing.Size(150, 22);
            this.dtpDateDebut.TabIndex = 1;
            // 
            // lblDateFin
            // 
            this.lblDateFin.AutoSize = true;
            this.lblDateFin.Location = new System.Drawing.Point(3, 115);
            this.lblDateFin.Name = "lblDateFin";
            this.lblDateFin.Size = new System.Drawing.Size(87, 20);
            this.lblDateFin.TabIndex = 4;
            this.lblDateFin.Text = "Date de fin *";
            // 
            // dtpDateFin
            // 
            this.dtpDateFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateFin.Location = new System.Drawing.Point(149, 115);
            this.dtpDateFin.Name = "dtpDateFin";
            this.dtpDateFin.Size = new System.Drawing.Size(150, 22);
            this.dtpDateFin.TabIndex = 3;
            // 
            // lblHeureDebut
            // 
            this.lblHeureDebut.AutoSize = true;
            this.lblHeureDebut.Location = new System.Drawing.Point(3, 80);
            this.lblHeureDebut.Name = "lblHeureDebut";
            this.lblHeureDebut.Size = new System.Drawing.Size(108, 20);
            this.lblHeureDebut.TabIndex = 6;
            this.lblHeureDebut.Text = "Heure de début";
            // 
            // dtpHeureDebut
            // 
            this.dtpHeureDebut.CustomFormat = "HH:mm";
            this.dtpHeureDebut.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHeureDebut.Location = new System.Drawing.Point(149, 80);
            this.dtpHeureDebut.Name = "dtpHeureDebut";
            this.dtpHeureDebut.ShowUpDown = true;
            this.dtpHeureDebut.Size = new System.Drawing.Size(150, 22);
            this.dtpHeureDebut.TabIndex = 2;
            // 
            // lblHeureFin
            // 
            this.lblHeureFin.AutoSize = true;
            this.lblHeureFin.Location = new System.Drawing.Point(3, 150);
            this.lblHeureFin.Name = "lblHeureFin";
            this.lblHeureFin.Size = new System.Drawing.Size(87, 20);
            this.lblHeureFin.TabIndex = 8;
            this.lblHeureFin.Text = "Heure de fin";
            // 
            // dtpHeureFin
            // 
            this.dtpHeureFin.CustomFormat = "HH:mm";
            this.dtpHeureFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHeureFin.Location = new System.Drawing.Point(149, 150);
            this.dtpHeureFin.Name = "dtpHeureFin";
            this.dtpHeureFin.ShowUpDown = true;
            this.dtpHeureFin.Size = new System.Drawing.Size(150, 22);
            this.dtpHeureFin.TabIndex = 4;
            // 
            // lblLieu
            // 
            this.lblLieu.AutoSize = true;
            this.lblLieu.Location = new System.Drawing.Point(3, 185);
            this.lblLieu.Name = "lblLieu";
            this.lblLieu.Size = new System.Drawing.Size(35, 20);
            this.lblLieu.TabIndex = 10;
            this.lblLieu.Text = "Lieu";
            // 
            // txtLieu
            // 
            // 
            // 
            // 
            this.txtLieu.CustomButton.Image = null;
            this.txtLieu.CustomButton.Location = new System.Drawing.Point(272, 1);
            this.txtLieu.CustomButton.Name = "";
            this.txtLieu.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.txtLieu.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtLieu.CustomButton.TabIndex = 1;
            this.txtLieu.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtLieu.CustomButton.UseSelectable = true;
            this.txtLieu.CustomButton.Visible = false;
            this.txtLieu.Lines = new string[0];
            this.txtLieu.Location = new System.Drawing.Point(149, 185);
            this.txtLieu.MaxLength = 32767;
            this.txtLieu.Name = "txtLieu";
            this.txtLieu.PasswordChar = '\0';
            this.txtLieu.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtLieu.SelectedText = "";
            this.txtLieu.SelectionLength = 0;
            this.txtLieu.SelectionStart = 0;
            this.txtLieu.ShortcutsEnabled = true;
            this.txtLieu.Size = new System.Drawing.Size(296, 25);
            this.txtLieu.TabIndex = 5;
            this.txtLieu.UseSelectable = true;
            this.txtLieu.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtLieu.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(3, 220);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(79, 20);
            this.lblDescription.TabIndex = 12;
            this.lblDescription.Text = "Description";
            // 
            // txtDescription
            // 
            // 
            // 
            // 
            this.txtDescription.CustomButton.Image = null;
            this.txtDescription.CustomButton.Location = new System.Drawing.Point(238, 2);
            this.txtDescription.CustomButton.Name = "";
            this.txtDescription.CustomButton.Size = new System.Drawing.Size(55, 55);
            this.txtDescription.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtDescription.CustomButton.TabIndex = 1;
            this.txtDescription.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtDescription.CustomButton.UseSelectable = true;
            this.txtDescription.CustomButton.Visible = false;
            this.txtDescription.Lines = new string[0];
            this.txtDescription.Location = new System.Drawing.Point(149, 220);
            this.txtDescription.MaxLength = 32767;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.PasswordChar = '\0';
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.SelectedText = "";
            this.txtDescription.SelectionLength = 0;
            this.txtDescription.SelectionStart = 0;
            this.txtDescription.ShortcutsEnabled = true;
            this.txtDescription.Size = new System.Drawing.Size(296, 60);
            this.txtDescription.TabIndex = 6;
            this.txtDescription.UseSelectable = true;
            this.txtDescription.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtDescription.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnAdd
            // 
            this.btnAdd.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnAdd.Location = new System.Drawing.Point(149, 295);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(99, 30);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Ajouter";
            this.btnAdd.UseSelectable = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnUpdate.Location = new System.Drawing.Point(254, 295);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(98, 30);
            this.btnUpdate.TabIndex = 8;
            this.btnUpdate.Text = "Appliquer";
            this.btnUpdate.UseSelectable = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClear
            // 
            this.btnClear.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnClear.Location = new System.Drawing.Point(358, 295);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(87, 30);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Vider";
            this.btnClear.UseSelectable = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // eventsListBox
            // 
            this.eventsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.eventsListBox.FormattingEnabled = true;
            this.eventsListBox.ItemHeight = 16;
            this.eventsListBox.Location = new System.Drawing.Point(506, 88);
            this.eventsListBox.Name = "eventsListBox";
            this.eventsListBox.Size = new System.Drawing.Size(514, 292);
            this.eventsListBox.TabIndex = 10;
            this.eventsListBox.SelectedIndexChanged += new System.EventHandler(this.eventsListBox_SelectedIndexChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnDelete.Location = new System.Drawing.Point(506, 390);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "Supprimer";
            this.btnDelete.UseSelectable = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnDeleteAll.Location = new System.Drawing.Point(616, 390);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(120, 30);
            this.btnDeleteAll.TabIndex = 12;
            this.btnDeleteAll.Text = "Supprimer tout";
            this.btnDeleteAll.UseSelectable = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnMoveUp.Location = new System.Drawing.Point(746, 390);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(60, 30);
            this.btnMoveUp.TabIndex = 13;
            this.btnMoveUp.Text = "▲";
            this.btnMoveUp.UseSelectable = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnMoveDown.Location = new System.Drawing.Point(816, 390);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(60, 30);
            this.btnMoveDown.TabIndex = 14;
            this.btnMoveDown.Text = "▼";
            this.btnMoveDown.UseSelectable = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnExportICS
            // 
            this.btnExportICS.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnExportICS.Location = new System.Drawing.Point(880, 462);
            this.btnExportICS.Name = "btnExportICS";
            this.btnExportICS.Size = new System.Drawing.Size(140, 35);
            this.btnExportICS.TabIndex = 17;
            this.btnExportICS.Text = "Exporter ICS";
            this.btnExportICS.UseSelectable = true;
            this.btnExportICS.Click += new System.EventHandler(this.btnExportICS_Click);
            // 
            // btnImportICS
            // 
            this.btnImportICS.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnImportICS.Location = new System.Drawing.Point(734, 462);
            this.btnImportICS.Name = "btnImportICS";
            this.btnImportICS.Size = new System.Drawing.Size(140, 35);
            this.btnImportICS.TabIndex = 16;
            this.btnImportICS.Text = "Importer ICS";
            this.btnImportICS.UseSelectable = true;
            this.btnImportICS.Click += new System.EventHandler(this.btnImportICS_Click);
            // 
            // btnPurgeAnnulations
            // 
            this.btnPurgeAnnulations.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnPurgeAnnulations.Location = new System.Drawing.Point(524, 462);
            this.btnPurgeAnnulations.Name = "btnPurgeAnnulations";
            this.btnPurgeAnnulations.Size = new System.Drawing.Size(200, 35);
            this.btnPurgeAnnulations.TabIndex = 18;
            this.btnPurgeAnnulations.Text = "Purger annulations";
            this.btnPurgeAnnulations.UseSelectable = true;
            this.btnPurgeAnnulations.Click += new System.EventHandler(this.btnPurgeAnnulations_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblVersion.Location = new System.Drawing.Point(5, 496);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(89, 17);
            this.lblVersion.TabIndex = 18;
            this.lblVersion.Text = "icsEditor v1.0.0";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics|Fichiers JSON (*.json)|*.json";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics|Fichiers JSON (*.json)|*.json";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 520);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnImportICS);
            this.Controls.Add(this.btnPurgeAnnulations);
            this.Controls.Add(this.btnExportICS);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.eventsListBox);
            this.Controls.Add(this.eventGroupBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1040, 520);
            this.MinimumSize = new System.Drawing.Size(1040, 520);
            this.Name = "MainForm";
            this.Resizable = false;
            this.Text = "ICS Editor";
            this.eventGroupBox.ResumeLayout(false);
            this.eventPanel.ResumeLayout(false);
            this.eventPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.GroupBox eventGroupBox;
        private System.Windows.Forms.ListBox eventsListBox;
        private MetroFramework.Controls.MetroButton btnDelete;
        private MetroFramework.Controls.MetroButton btnDeleteAll;
        private MetroFramework.Controls.MetroButton btnMoveUp;
        private MetroFramework.Controls.MetroButton btnMoveDown;
        private MetroFramework.Controls.MetroButton btnExportICS;
        private MetroFramework.Controls.MetroButton btnImportICS;
        private MetroFramework.Controls.MetroButton btnPurgeAnnulations;
        private System.Windows.Forms.ToolTip toolTipPurge;
        private MetroFramework.Controls.MetroLabel lblVersion;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private MetroFramework.Controls.MetroPanel eventPanel;
        private MetroFramework.Controls.MetroLabel lblLibelle;
        private MetroFramework.Controls.MetroTextBox txtLibelle;
        private MetroFramework.Controls.MetroLabel lblDateDebut;
        private System.Windows.Forms.DateTimePicker dtpDateDebut;
        private MetroFramework.Controls.MetroLabel lblDateFin;
        private System.Windows.Forms.DateTimePicker dtpDateFin;
        private MetroFramework.Controls.MetroLabel lblHeureDebut;
        private System.Windows.Forms.DateTimePicker dtpHeureDebut;
        private MetroFramework.Controls.MetroLabel lblHeureFin;
        private System.Windows.Forms.DateTimePicker dtpHeureFin;
        private MetroFramework.Controls.MetroLabel lblLieu;
        private MetroFramework.Controls.MetroTextBox txtLieu;
        private MetroFramework.Controls.MetroLabel lblDescription;
        private MetroFramework.Controls.MetroTextBox txtDescription;
        private MetroFramework.Controls.MetroButton btnAdd;
        private MetroFramework.Controls.MetroButton btnUpdate;
        private MetroFramework.Controls.MetroButton btnClear;
    }
}
