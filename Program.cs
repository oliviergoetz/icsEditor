using System;
using System.Threading;
using System.Windows.Forms;

namespace icsEditor
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Double-clic sur un .ics associé : Windows passe son chemin en argument.
            string fichier = args.Length > 0 ? args[0] : null;

            // Le mutex vit tant que la fenêtre est ouverte : sa présence signale
            // aux lancements suivants qu'une instance tourne déjà.
            using (new Mutex(true, InstanceUnique.NomMutex, out bool premiereInstance))
            {
                if (!premiereInstance)
                {
                    InstanceUnique.TransmettreAInstanceExistante(fichier);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(fichier));
            }
        }
    }
}
