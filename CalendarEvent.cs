using System;

namespace icsEditor
{
    public class CalendarEvent
    {
        public string Uid { get; set; }
        public int Sequence { get; set; }
        /// <summary>Événement supprimé, conservé pour publier son annulation (STATUS:CANCELLED).</summary>
        public bool EstAnnule { get; set; }
        /// <summary>Événement déjà présent dans un fichier ICS lu ou écrit : une annulation a un sens pour lui.</summary>
        public bool DejaPublie { get; set; }
        public string Libelle { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public TimeSpan? HeureDebut { get; set; }
        public TimeSpan? HeureFin { get; set; }
        public string Lieu { get; set; }
        public string Description { get; set; }

        public CalendarEvent()
        {
            Uid = Guid.NewGuid().ToString();
            Sequence = 0;
            EstAnnule = false;
            DejaPublie = false;
            Libelle = string.Empty;
            DateDebut = DateTime.Today;
            DateFin = DateTime.Today;
            Lieu = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Deux événements sont considérés identiques s'ils partagent le même UID,
        /// ou si tous leurs champs visibles coïncident. Le second cas couvre les
        /// fichiers ICS sans UID : Ical.Net en fabrique un nouveau à chaque lecture,
        /// on ne peut donc pas se fier à l'UID seul pour détecter un doublon.
        /// </summary>
        public bool IsSameEventAs(CalendarEvent other)
        {
            if (other == null)
                return false;

            if (!string.IsNullOrWhiteSpace(Uid)
                && !string.IsNullOrWhiteSpace(other.Uid)
                && string.Equals(Uid, other.Uid, StringComparison.OrdinalIgnoreCase))
                return true;

            return string.Equals(Libelle, other.Libelle, StringComparison.OrdinalIgnoreCase)
                && DateDebut.Date == other.DateDebut.Date
                && DateFin.Date == other.DateFin.Date
                && HeureDebut == other.HeureDebut
                && HeureFin == other.HeureFin
                && string.Equals(Lieu ?? string.Empty, other.Lieu ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Description ?? string.Empty, other.Description ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsAllDay()
        {
            return !HeureDebut.HasValue && !HeureFin.HasValue;
        }

        public DateTime GetStartDateTime()
        {
            if (HeureDebut.HasValue)
                return DateDebut.Add(HeureDebut.Value);
            return DateDebut;
        }

        public DateTime GetEndDateTime()
        {
            if (HeureFin.HasValue)
                return DateFin.Add(HeureFin.Value);
            return DateFin.AddDays(1); // Pour les événements sur toute la journée
        }

        public override string ToString()
        {
            // Vérifier si les dates sont différentes
            if (!DateDebut.Date.Equals(DateFin.Date))
            {
                // Plusieurs jours : format "libelle - DD/MM/YYYY - DD/MM/YYYY"
                string result = $"{Libelle} - {DateDebut:dd/MM/yyyy}";

                // Ajouter l'heure de début si présente
                if (HeureDebut.HasValue)
                    result += $" {HeureDebut.Value:hh\\:mm}";

                result += $" - {DateFin:dd/MM/yyyy}";

                // Ajouter l'heure de fin si présente
                if (HeureFin.HasValue)
                    result += $" {HeureFin.Value:hh\\:mm}";

                return result;
            }
            else
            {
                // Un seul jour : format "libelle - DD/MM/YYYY"
                string result = $"{Libelle} - {DateDebut:dd/MM/yyyy}";

                // Ajouter les heures si présentes
                if (HeureDebut.HasValue)
                    result += $" {HeureDebut.Value:hh\\:mm}";

                if (HeureFin.HasValue)
                    result += $" - {HeureFin.Value:hh\\:mm}";

                return result;
            }
        }
    }
}
