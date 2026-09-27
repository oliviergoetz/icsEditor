namespace icsEditor
{
    internal static class Libelles
    {
        /// <summary>
        /// Accorde un mot au nombre, sans le « (s) » des messages : 0 et 1 restent
        /// au singulier, comme le veut l'usage français.
        /// </summary>
        public static string Pluriel(int nombre, string mot)
        {
            return nombre > 1 ? mot + "s" : mot;
        }
    }
}
