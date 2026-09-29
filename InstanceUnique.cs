using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace icsEditor
{
    /// <summary>
    /// Une seule fenêtre ICS Editor à la fois. Une seconde instance, lancée par
    /// exemple par un double-clic sur un .ics, passe le fichier à la première
    /// par WM_COPYDATA puis se termine.
    /// </summary>
    internal static class InstanceUnique
    {
        public const string NomMutex = @"Local\icsEditor.InstanceUnique";
        public const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(int processId);

        /// <summary>
        /// Chemin vide : l'instance existante se contente de passer au premier plan.
        /// </summary>
        public static void TransmettreAInstanceExistante(string fichier)
        {
            // Le répertoire courant de cette instance n'est pas celui de l'autre :
            // un chemin relatif n'y aurait aucun sens.
            string chemin = string.IsNullOrEmpty(fichier) ? string.Empty : Path.GetFullPath(fichier);

            Process courant = Process.GetCurrentProcess();
            foreach (Process autre in Process.GetProcessesByName(courant.ProcessName))
            {
                if (autre.Id == courant.Id || autre.MainWindowHandle == IntPtr.Zero)
                    continue;

                // Windows n'autorise une fenêtre à passer au premier plan que si le
                // processus qui a la main (celui-ci) le lui permet.
                AllowSetForegroundWindow(autre.Id);

                COPYDATASTRUCT donnees = new COPYDATASTRUCT
                {
                    dwData = IntPtr.Zero,
                    cbData = (chemin.Length + 1) * 2,
                    lpData = chemin
                };
                SendMessage(autre.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, ref donnees);
                return;
            }
        }
    }
}
