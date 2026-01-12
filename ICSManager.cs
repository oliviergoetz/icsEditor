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
                var calendarEvent = new ICalEvent
                {
                    Uid = Guid.NewGuid().ToString(),
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
                    if (evt.HeureFin.HasValue)
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

                // Propriétés optionnelles
                if (!string.IsNullOrWhiteSpace(evt.Lieu))
                    calendarEvent.Location = evt.Lieu;

                if (!string.IsNullOrWhiteSpace(evt.Description))
                    calendarEvent.Description = evt.Description;

                calendar.Events.Add(calendarEvent);
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
