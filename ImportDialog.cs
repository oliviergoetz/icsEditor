using System;
using System.Windows.Forms;

namespace icsEditor
{
    public partial class ImportDialog : Form
    {
        public ImportDialog(int eventCount)
        {
            InitializeComponent();
            lblMessage.Text = $"{eventCount} événement(s) trouvé(s). Voulez-vous remplacer les événements actuels ou les ajouter?";
        }

        private void btnRemplacer_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnAjouter_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
