using System;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Double-clic sur un .ics associé : Windows passe son chemin en argument.
            Application.Run(new MainForm(args.Length > 0 ? args[0] : null));
        }
    }
}
