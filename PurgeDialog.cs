using System;
using System.Windows.Forms;

namespace icsEditor
{
    public partial class PurgeDialog : Form
    {
        public PurgeDialog(int total, int anciennes, int moisAnciennete)
        {
            InitializeComponent();

            lblMessage.Text =
                $"{total} annulation(s) en attente, dont {anciennes} portant sur des événements terminés depuis plus de {moisAnciennete} mois." +
                Environment.NewLine + Environment.NewLine +
                "Purger une annulation encore récente est risqué si le fichier a déjà été diffusé : " +
                "un destinataire qui n'a pas encore lu l'annulation gardera l'événement supprimé.";

            btnAnciennes.Enabled = anciennes > 0;
        }

        private void btnTout_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnAnciennes_Click(object sender, EventArgs e)
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
