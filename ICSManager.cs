using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using ICalEvent = Ical.Net.CalendarComponents.CalendarEvent;

namespace icsEditor
{
    public static class ICSManager
    {
        public static string GenerateICS(List<CalendarEvent> events)
        {
            var calendar = new Calendar();
            calendar.ProductId = "-//icsEditor//FR";
            calendar.Version = "2.0";
            calendar.Scale = "GREGORIAN";
            calendar.Method = "PUBLISH";

            foreach (var evt in events)
            {
                // Conserver l'UID d'origine : un client calendrier identifie un événement
                // par son UID. En régénérer un à chaque export créerait des doublons
                // au lieu de mettre à jour l'événement existant.
                if (string.IsNullOrWhiteSpace(evt.Uid))
                    evt.Uid = Guid.NewGuid().ToString();

                var calendarEvent = new ICalEvent
                {
                    Uid = evt.Uid,
                    Sequence = evt.Sequence,
                    DtStamp = new CalDateTime(DateTime.UtcNow),
                    Summary = evt.Libelle
                };

                // Gestion des dates et heures
                if (evt.IsAllDay())
                {
                    // Événement all-day (sans heures)
                    // Créer des dates à minuit (00:00:00) pour éviter toute composante horaire
                    DateTime startDate = new DateTime(evt.DateDebut.Year, evt.DateDebut.Month, evt.DateDebut.Day);
                    DateTime endDate = new DateTime(evt.DateFin.Year, evt.DateFin.Month, evt.DateFin.Day).AddDays(1);

                    // Utiliser le constructeur avec hasTime = false pour indiquer que c'est un événement all-day
                    calendarEvent.Start = new CalDateTime(startDate.Year, startDate.Month, startDate.Day);
                    calendarEvent.End = new CalDateTime(endDate.Year, endDate.Month, endDate.Day);
                }
                else
                {
                    // Événement avec heures
                    DateTime startDateTime = evt.HeureDebut.HasValue
                        ? evt.DateDebut.Add(evt.HeureDebut.Value)
                        : evt.DateDebut;

                    DateTime endDateTime;
                    if (evt.FinitAMinuit())
                    {
                        // Fin à 00:00 saisie = minuit à la fin du jour de fin,
                        // soit 00:00 le lendemain dans le fichier.
                        endDateTime = evt.DateFin.AddDays(1);
                    }
                    else if (evt.HeureFin.HasValue)
                    {
                        endDateTime = evt.DateFin.Add(evt.HeureFin.Value);
                    }
                    else
                    {
                        // Si pas d'heure de fin, mettre 23:59:59 le jour de fin
                        endDateTime = evt.DateFin.AddDays(1).AddSeconds(-1);
                    }

                    calendarEvent.Start = new CalDateTime(DateTime.SpecifyKind(startDateTime, DateTimeKind.Unspecified), "Europe/Paris");
                    calendarEvent.End = new CalDateTime(DateTime.SpecifyKind(endDateTime, DateTimeKind.Unspecified), "Europe/Paris");
                }

                // Un événement supprimé est publié annulé plutôt que retiré du fichier :
                // sans cela, le client calendrier garde l'ancien événement indéfiniment.
                if (evt.EstAnnule)
                    calendarEvent.Status = EventStatus.Cancelled;

                // Propriétés optionnelles
                if (!string.IsNullOrWhiteSpace(evt.Lieu))
                    calendarEvent.Location = evt.Lieu;

                if (!string.IsNullOrWhiteSpace(evt.Description))
                    calendarEvent.Description = evt.Description;

                calendar.Events.Add(calendarEvent);

                // L'événement existe désormais dans un fichier : son annulation
                // éventuelle devra être publiée.
                evt.DejaPublie = true;
            }

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }

        public static List<CalendarEvent> ParseICS(string icsContent)
        {
            List<CalendarEvent> events = new List<CalendarEvent>();

            try
            {
                var calendar = Calendar.Load(icsContent);

                foreach (var calEvent in calendar.Events)
                {
                    var evt = new CalendarEvent
                    {
                        // Ical.Net fabrique un UID quand le fichier n'en fournit pas :
                        // la détection de doublon ne peut donc pas s'y fier seule.
                        Uid = calEvent.Uid ?? string.Empty,
                        Sequence = calEvent.Sequence,
                        EstAnnule = string.Equals(calEvent.Status, EventStatus.Cancelled, StringComparison.OrdinalIgnoreCase),
                        DejaPublie = true,
                        Libelle = calEvent.Summary ?? string.Empty,
                        Lieu = calEvent.Location ?? string.Empty,
                        Description = calEvent.Description ?? string.Empty
                    };

                    // Gestion des dates
                    if (calEvent.Start != null)
                    {
                        bool isAllDay = !calEvent.Start.HasTime;

                        if (isAllDay)
                        {
                            // Événement all-day (sans conversion UTC car ce sont des dates pures)
                            // Forcer la partie date uniquement (sans heure)
                            evt.DateDebut = calEvent.Start.Value.Date;

                            if (calEvent.End != null)
                            {
                                // ICS utilise une date exclusive pour DTEND, donc on soustrait 1 jour
                                // Forcer la partie date uniquement
                                evt.DateFin = calEvent.End.Value.Date.AddDays(-1);
                            }
                            else
                            {
                                evt.DateFin = evt.DateDebut;
                            }
                        }
                        else
                        {
                            // Événement avec heures
                            DateTime startLocal = calEvent.Start.AsUtc.ToLocalTime();
                            evt.DateDebut = startLocal.Date;
                            evt.HeureDebut = startLocal.TimeOfDay;

                            if (calEvent.End != null)
                            {
                                DateTime endLocal = calEvent.End.AsUtc.ToLocalTime();
                                evt.DateFin = endLocal.Date;
                                evt.HeureFin = endLocal.TimeOfDay;

                                // 00:00 le lendemain = minuit à la fin de la veille :
                                // c'est ainsi qu'on l'a écrit, et qu'on le saisit.
                                if (endLocal.TimeOfDay == TimeSpan.Zero && endLocal.Date > startLocal.Date)
                                    evt.DateFin = endLocal.Date.AddDays(-1);
                            }
                            else
                            {
                                evt.DateFin = evt.DateDebut;
                                evt.HeureFin = evt.HeureDebut;
                            }
                        }
                    }

                    events.Add(evt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du parsing ICS: {ex.Message}", ex);
            }

            return events;
        }
    }
}
