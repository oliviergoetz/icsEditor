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

        /// <summary>
        /// Une heure de fin à 00:00 signifie minuit à la fin du jour de fin :
        /// « 21/03 19:00 → 21/03 00:00 » se termine le 22/03 à 00:00.
        /// </summary>
        public bool FinitAMinuit()
        {
            return HeureDebut.HasValue && HeureFin == TimeSpan.Zero;
        }

        public DateTime GetEndDateTime()
        {
            if (FinitAMinuit())
                return DateFin.AddDays(1);
            if (HeureFin.HasValue)
                return DateFin.Add(HeureFin.Value);
            return DateFin.AddDays(1); // Pour les événements sur toute la journée
        }

        public override string ToString()
        {
            // Date en tête : la liste se lit comme un agenda. Le « : » sépare les
            // dates du libellé sans se confondre avec le « - » d'une plage.
            string result = $"{DateDebut:dd/MM/yyyy}";

            if (HeureDebut.HasValue)
                result += $" {HeureDebut.Value:hh\\:mm}";

            if (!DateDebut.Date.Equals(DateFin.Date))
            {
                // Plusieurs jours : "DD/MM/YYYY [hh:mm] - DD/MM/YYYY [hh:mm]"
                result += $" - {DateFin:dd/MM/yyyy}";

                if (HeureFin.HasValue)
                    result += $" {HeureFin.Value:hh\\:mm}";
            }
            else if (HeureFin.HasValue)
            {
                // Un seul jour : "DD/MM/YYYY hh:mm - hh:mm"
                result += $" - {HeureFin.Value:hh\\:mm}";
            }

            return $"{result} : {Libelle}";
        }
    }
}
