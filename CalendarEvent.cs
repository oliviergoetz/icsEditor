using System;

namespace icsEditor
{
    public class CalendarEvent
    {
        public string Libelle { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public TimeSpan? HeureDebut { get; set; }
        public TimeSpan? HeureFin { get; set; }
        public string Lieu { get; set; }
        public string Description { get; set; }

        public CalendarEvent()
        {
            Libelle = string.Empty;
            DateDebut = DateTime.Today;
            DateFin = DateTime.Today;
            Lieu = string.Empty;
            Description = string.Empty;
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
