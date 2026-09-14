using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace icsEditor
{
    public partial class MainForm : MetroForm
    {
        private List<CalendarEvent> events;

        /// <summary>
        /// Événements supprimés qui avaient déjà été publiés : ils restent dans le
        /// fichier exporté avec STATUS:CANCELLED pour que le calendrier destinataire
        /// les retire au lieu de conserver une copie orpheline.
        /// </summary>
        private List<CalendarEvent> cancelledEvents = new List<CalendarEvent>();
        private int editingIndex = -1;
        private bool hasUnsavedChanges = false;

        public MainForm()
        {
            InitializeComponent();

            // Charger l'icône depuis les ressources embarquées
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("icsEditor.icsEditor.ico"))
                {
                    if (stream != null)
                    {
                        this.Icon = new System.Drawing.Icon(stream);
                    }
                }
            }
            catch { }

            events = new List<CalendarEvent>();
            UpdateEventsList();

            // Initialiser les heures à 00:00
            // Une case unique pilote les deux heures. Les cases intégrées aux
            // DateTimePicker disaient la même chose, mais personne ne connaît cette
            // convention Windows — et leur état initial ne tenait pas au démarrage.
            dtpHeureDebut.Value = DateTime.Today; // 00:00
            dtpHeureFin.Value = DateTime.Today;   // 00:00
            chkJourneeEntiere.Checked = true;
            AppliquerJourneeEntiere();

            toolTipAide.SetToolTip(chkJourneeEntiere,
                "Coché, l'événement occupe la journée entière et ne porte aucune heure."
                + Environment.NewLine + "Décochez pour saisir une heure de début et de fin.");

            // Ajouter des gestionnaires d'événements pour détecter les modifications
            txtLibelle.TextChanged += (s, e) => MarkAsChanged();
            dtpDateDebut.ValueChanged += (s, e) => MarkAsChanged();
            dtpDateFin.ValueChanged += (s, e) => MarkAsChanged();
            dtpHeureDebut.ValueChanged += (s, e) => MarkAsChanged();
            dtpHeureFin.ValueChanged += (s, e) => MarkAsChanged();
            chkJourneeEntiere.CheckedChanged += (s, e) => { AppliquerJourneeEntiere(); MarkAsChanged(); };
            txtLieu.TextChanged += (s, e) => MarkAsChanged();
            txtDescription.TextChanged += (s, e) => MarkAsChanged();

            // Mettre à jour l'état des boutons
            UpdateButtonStates();
        }

        /// <summary>
        /// Les champs heure n'ont de sens que pour un événement qui n'occupe pas
        /// la journée entière : on les grise plutôt que de laisser saisir une valeur
        /// qui serait ignorée à l'export.
        /// </summary>
        private void AppliquerJourneeEntiere()
        {
            bool avecHeures = !chkJourneeEntiere.Checked;

            lblHeureDebut.Enabled = avecHeures;
            dtpHeureDebut.Enabled = avecHeures;
            lblHeureFin.Enabled = avecHeures;
            dtpHeureFin.Enabled = avecHeures;
        }

        private void MarkAsChanged()
        {
            if (editingIndex >= 0)
            {
                hasUnsavedChanges = true;
                UpdateButtonStates();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                CalendarEvent newEvent = CreateEventFromInputs();
                events.Add(newEvent);
                ClearInputs();
                UpdateEventsList();
                hasUnsavedChanges = false;
                MessageBox.Show("Événement ajouté avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (editingIndex >= 0 && editingIndex < events.Count && ValidateInputs())
            {
                CalendarEvent updatedEvent = CreateEventFromInputs();

                // Conserver l'identité de l'événement et incrémenter SEQUENCE :
                // le client calendrier met alors à jour l'événement existant
                // au lieu d'en créer un nouveau à côté.
                updatedEvent.Uid = events[editingIndex].Uid;
                updatedEvent.Sequence = events[editingIndex].Sequence + 1;

                events[editingIndex] = updatedEvent;
                UpdateEventsList();

                // Maintenir la sélection pour continuer l'édition
                eventsListBox.SelectedIndex = editingIndex;

                hasUnsavedChanges = false;
                UpdateButtonStates();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
            editingIndex = -1;
            hasUnsavedChanges = false;
            UpdateButtonStates();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (eventsListBox.SelectedIndex >= 0)
            {
                DialogResult result = MessageBox.Show(
                    "Êtes-vous sûr de vouloir supprimer cet événement ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int deletedIndex = eventsListBox.SelectedIndex;
                    CalendarEvent deletedEvent = events[deletedIndex];
                    events.RemoveAt(deletedIndex);

                    // Un événement jamais publié disparaît sans laisser de trace.
                    // Les autres sont conservés annulés : le prochain export dira au
                    // calendrier destinataire de les retirer.
                    if (deletedEvent.DejaPublie)
                    {
                        deletedEvent.EstAnnule = true;
                        deletedEvent.Sequence++;
                        cancelledEvents.Add(deletedEvent);
                    }

                    ClearInputs();
                    editingIndex = -1;
                    hasUnsavedChanges = false;
                    UpdateEventsList();

                    // Sélectionner l'événement suivant ou le dernier si on a supprimé le dernier
                    if (events.Count > 0)
                    {
                        if (deletedIndex < events.Count)
                        {
                            eventsListBox.SelectedIndex = deletedIndex;
                        }
                        else
                        {
                            eventsListBox.SelectedIndex = events.Count - 1;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un événement à supprimer.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (events.Count == 0)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer tous les événements ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Même règle que la suppression unitaire : ce qui a déjà été publié
                // part en annulation, le reste disparaît sans trace.
                foreach (CalendarEvent supprime in events)
                {
                    if (supprime.DejaPublie)
                    {
                        supprime.EstAnnule = true;
                        supprime.Sequence++;
                        cancelledEvents.Add(supprime);
                    }
                }

                events.Clear();
                ClearInputs();
                UpdateEventsList();
                editingIndex = -1;
                hasUnsavedChanges = false;
                UpdateButtonStates();
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int selectedIndex = eventsListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                // Échanger l'événement avec celui du dessus
                CalendarEvent temp = events[selectedIndex];
                events[selectedIndex] = events[selectedIndex - 1];
                events[selectedIndex - 1] = temp;

                // Mettre à jour la liste
                UpdateEventsList();

                // Réajuster les indices
                if (editingIndex == selectedIndex)
                {
                    editingIndex = selectedIndex - 1;
                }
                else if (editingIndex == selectedIndex - 1)
                {
                    editingIndex = selectedIndex;
                }

                // Resélectionner l'élément déplacé
                eventsListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int selectedIndex = eventsListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < events.Count - 1)
            {
                // Échanger l'événement avec celui du dessous
                CalendarEvent temp = events[selectedIndex];
                events[selectedIndex] = events[selectedIndex + 1];
                events[selectedIndex + 1] = temp;

                // Mettre à jour la liste
                UpdateEventsList();

                // Réajuster les indices
                if (editingIndex == selectedIndex)
                {
                    editingIndex = selectedIndex + 1;
                }
                else if (editingIndex == selectedIndex + 1)
                {
                    editingIndex = selectedIndex;
                }

                // Resélectionner l'élément déplacé
                eventsListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        /// <summary>
        /// Ancienneté au-delà de laquelle une annulation peut être oubliée sans risque :
        /// tous les destinataires ont eu le temps de la lire.
        /// </summary>
        private const int MoisAvantPurgeAnnulation = 6;

        private void btnPurgeAnnulations_Click(object sender, EventArgs e)
        {
            if (cancelledEvents.Count == 0)
            {
                MessageBox.Show(
                    "Aucune annulation en attente, rien à purger." + Environment.NewLine + Environment.NewLine
                        + "Une annulation apparaît quand vous supprimez un événement issu d'un fichier ICS, ou déjà exporté au moins une fois.",
                    "Purge",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DateTime limite = DateTime.Today.AddMonths(-MoisAvantPurgeAnnulation);
            List<CalendarEvent> anciennes = cancelledEvents.FindAll(annule => annule.DateFin.Date < limite);

            PurgeDialog purgeDialog = new PurgeDialog(cancelledEvents.Count, anciennes.Count, MoisAvantPurgeAnnulation);
            DialogResult choix = purgeDialog.ShowDialog(this);

            if (choix == DialogResult.Cancel)
                return;

            bool purgeTotale = choix == DialogResult.Yes;
            int aPurger = purgeTotale ? cancelledEvents.Count : anciennes.Count;

            if (aPurger == 0)
                return;

            string confirmation = purgeTotale
                ? $"Supprimer définitivement les {aPurger} annulation(s), y compris les plus récentes ?"
                    + Environment.NewLine + Environment.NewLine
                    + "Si ce fichier a déjà été diffusé, les événements supprimés resteront dans les calendriers qui n'ont pas encore lu l'annulation."
                : $"Supprimer définitivement {aPurger} annulation(s) de plus de {MoisAvantPurgeAnnulation} mois ?";

            DialogResult result = MessageBox.Show(
                confirmation,
                "Confirmation",
                MessageBoxButtons.YesNo,
                purgeTotale ? MessageBoxIcon.Warning : MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            if (purgeTotale)
                cancelledEvents.Clear();
            else
                cancelledEvents.RemoveAll(annule => annule.DateFin.Date < limite);

            UpdateButtonStates();

            MessageBox.Show(
                $"{aPurger} annulation(s) purgée(s). {cancelledEvents.Count} restante(s).",
                "Purge terminée",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnExportICS_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics";
            saveFileDialog.Title = "Exporter en ICS";
            saveFileDialog.FileName = "icsEditor.ics";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Les annulations accompagnent les événements actifs dans le même
                    // fichier : c'est ce qui permet au calendrier destinataire de
                    // supprimer ce qui a été supprimé ici.
                    List<CalendarEvent> aExporter = new List<CalendarEvent>(events);
                    aExporter.AddRange(cancelledEvents);

                    string icsContent = ICSManager.GenerateICS(aExporter);
                    File.WriteAllText(saveFileDialog.FileName, icsContent);

                    string messageExport = $"{events.Count} événement(s) exporté(s).";
                    if (cancelledEvents.Count > 0)
                        messageExport += Environment.NewLine + $"{cancelledEvents.Count} annulation(s) publiée(s).";

                    MessageBox.Show(messageExport, "Export terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'export ICS: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImportICS_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics";
            openFileDialog.Title = "Importer un fichier ICS";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string icsContent = File.ReadAllText(openFileDialog.FileName);
                    List<CalendarEvent> parsedEvents = ICSManager.ParseICS(icsContent);

                    // Les événements annulés du fichier ne sont pas affichés : ce sont
                    // des traces de suppression, qu'on se contente de reconduire à
                    // l'export pour ne pas les ressusciter chez le destinataire.
                    List<CalendarEvent> importedEvents = new List<CalendarEvent>();
                    List<CalendarEvent> importedCancelled = new List<CalendarEvent>();

                    foreach (CalendarEvent parsed in parsedEvents)
                    {
                        if (parsed.EstAnnule)
                            importedCancelled.Add(parsed);
                        else
                            importedEvents.Add(parsed);
                    }


                    // Un fichier ne contenant que des annulations reste exploitable :
                    // il faut pouvoir reconduire ces annulations.
                    if (importedEvents.Count > 0 || importedCancelled.Count > 0)
                    {
                        DialogResult result = DialogResult.Yes;

                        // Si la liste n'est pas vide, demander ce qu'il faut faire
                        if (events.Count > 0)
                        {
                            ImportDialog importDialog = new ImportDialog(importedEvents.Count);
                            result = importDialog.ShowDialog(this);
                        }

                        if (result == DialogResult.Yes)
                        {
                            // Remplacer (ou importer si la liste était vide)
                            events = importedEvents;
                            cancelledEvents = importedCancelled;
                            UpdateEventsList();
                            ClearInputs();
                            editingIndex = -1;
                            hasUnsavedChanges = false;

                            // Sélectionner le premier élément
                            if (eventsListBox.Items.Count > 0)
                            {
                                eventsListBox.SelectedIndex = 0;
                            }

                            MessageBox.Show("Événements importés avec succès.", "Import terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (result == DialogResult.No)
                        {
                            // Ajouter, en écartant les événements déjà présents
                            // (même UID, ou à défaut même libellé et mêmes dates)
                            List<CalendarEvent> nouveaux = new List<CalendarEvent>();
                            int doublons = 0;

                            foreach (CalendarEvent importe in importedEvents)
                            {
                                bool dejaPresent = events.Exists(existant => existant.IsSameEventAs(importe))
                                    || nouveaux.Exists(ajoute => ajoute.IsSameEventAs(importe));

                                if (dejaPresent)
                                    doublons++;
                                else
                                    nouveaux.Add(importe);
                            }

                            events.AddRange(nouveaux);

                            foreach (CalendarEvent annule in importedCancelled)
                            {
                                if (!cancelledEvents.Exists(existant => existant.IsSameEventAs(annule)))
                                    cancelledEvents.Add(annule);
                            }

                            UpdateEventsList();
                            ClearInputs();
                            editingIndex = -1;
                            hasUnsavedChanges = false;

                            // Sélectionner le premier élément
                            if (eventsListBox.Items.Count > 0)
                            {
                                eventsListBox.SelectedIndex = 0;
                            }

                            string messageAjout = $"{nouveaux.Count} événement(s) ajouté(s).";
                            if (doublons > 0)
                                messageAjout += Environment.NewLine + $"{doublons} doublon(s) ignoré(s).";

                            MessageBox.Show(messageAjout, "Import terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Aucun événement trouvé dans le fichier ICS.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'import ICS: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtLibelle.Text))
            {
                MessageBox.Show("Le libellé est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpDateFin.Value < dtpDateDebut.Value)
            {
                MessageBox.Show("La date de fin doit être supérieure ou égale à la date de début.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!chkJourneeEntiere.Checked && dtpDateDebut.Value == dtpDateFin.Value)
            {
                if (dtpHeureFin.Value.TimeOfDay <= dtpHeureDebut.Value.TimeOfDay)
                {
                    MessageBox.Show("L'heure de fin doit être supérieure à l'heure de début.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private CalendarEvent CreateEventFromInputs()
        {
            return new CalendarEvent
            {
                Libelle = txtLibelle.Text.Trim(),
                DateDebut = dtpDateDebut.Value.Date,
                DateFin = dtpDateFin.Value.Date,
                HeureDebut = chkJourneeEntiere.Checked ? null : (TimeSpan?)dtpHeureDebut.Value.TimeOfDay,
                HeureFin = chkJourneeEntiere.Checked ? null : (TimeSpan?)dtpHeureFin.Value.TimeOfDay,
                Lieu = txtLieu.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };
        }

        private void ClearInputs()
        {
            txtLibelle.Clear();
            txtLieu.Clear();
            txtDescription.Clear();
            dtpDateDebut.Value = DateTime.Today;
            dtpDateFin.Value = DateTime.Today;
            dtpHeureDebut.Value = DateTime.Today;
            dtpHeureFin.Value = DateTime.Today;
            chkJourneeEntiere.Checked = true;
            AppliquerJourneeEntiere();
        }

        private void UpdateEventsList()
        {
            eventsListBox.Items.Clear();
            foreach (CalendarEvent evt in events)
            {
                eventsListBox.Items.Add(evt.ToString());
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            int selectedIndex = eventsListBox.SelectedIndex;

            btnDeleteAll.Enabled = events.Count > 0;
            btnDelete.Enabled = selectedIndex >= 0;
            // Un export reste utile quand il ne reste que des annulations à publier.
            btnExportICS.Enabled = events.Count > 0 || cancelledEvents.Count > 0;

            // Les annulations n'apparaissent pas dans la liste : le compteur du bouton
            // et son infobulle sont les seuls endroits où l'utilisateur les voit.
            // Le bouton reste actif même sans annulation : un contrôle désactivé
            // n'affiche pas d'infobulle sous WinForms, et resterait donc muet sur
            // la raison de son état. Le clic répond à sa place.
            btnPurgeAnnulations.Text = cancelledEvents.Count > 0
                ? $"Purger annulations ({cancelledEvents.Count})"
                : "Purger annulations";
            toolTipAide.SetToolTip(btnPurgeAnnulations, cancelledEvents.Count > 0
                ? $"{cancelledEvents.Count} suppression(s) publiée(s) avec le prochain export, pour que le calendrier destinataire retire ces événements."
                : "Aucune annulation en attente : rien à purger. Supprimez un événement issu d'un fichier ICS pour en produire une.");
            btnAdd.Enabled = editingIndex < 0;
            btnUpdate.Enabled = editingIndex >= 0 && hasUnsavedChanges;

            // Boutons de réorganisation
            btnMoveUp.Enabled = selectedIndex > 0;
            btnMoveDown.Enabled = selectedIndex >= 0 && selectedIndex < events.Count - 1;
        }

        private bool HasFormChanges()
        {
            if (editingIndex < 0 || editingIndex >= events.Count)
                return false;

            CalendarEvent currentEvent = events[editingIndex];
            CalendarEvent formEvent = CreateEventFromInputs();

            return currentEvent.Libelle != formEvent.Libelle ||
                   currentEvent.DateDebut != formEvent.DateDebut ||
                   currentEvent.DateFin != formEvent.DateFin ||
                   currentEvent.HeureDebut != formEvent.HeureDebut ||
                   currentEvent.HeureFin != formEvent.HeureFin ||
                   currentEvent.Lieu != formEvent.Lieu ||
                   currentEvent.Description != formEvent.Description;
        }

        private bool ConfirmUnsavedChanges()
        {
            if (hasUnsavedChanges && editingIndex >= 0)
            {
                DialogResult result = MessageBox.Show(
                    "Voulez-vous appliquer les modifications ?",
                    "Modifications non appliquées",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Appliquer les modifications
                    if (ValidateInputs())
                    {
                        events[editingIndex] = CreateEventFromInputs();
                        hasUnsavedChanges = false;
                        return true;
                    }
                    return false; // Validation échouée
                }
                else if (result == DialogResult.No)
                {
                    hasUnsavedChanges = false;
                    return true; // Abandonner les modifications
                }
                else // Cancel
                {
                    return false; // Annuler le changement de sélection
                }
            }
            return true;
        }

        private void eventsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();

            if (eventsListBox.SelectedIndex >= 0 && eventsListBox.SelectedIndex != editingIndex)
            {
                // Sauvegarder l'index de destination
                int newSelectedIndex = eventsListBox.SelectedIndex;

                // Vérifier s'il y a des modifications non enregistrées
                bool changesSaved = ConfirmUnsavedChanges();
                if (!changesSaved)
                {
                    // Restaurer la sélection précédente
                    eventsListBox.SelectedIndexChanged -= eventsListBox_SelectedIndexChanged;
                    eventsListBox.SelectedIndex = editingIndex;
                    eventsListBox.SelectedIndexChanged += eventsListBox_SelectedIndexChanged;
                    return;
                }

                // Si des modifications ont été sauvegardées, mettre à jour la liste
                if (editingIndex >= 0)
                {
                    UpdateEventsList();
                    // Restaurer la sélection sur le nouvel index
                    eventsListBox.SelectedIndexChanged -= eventsListBox_SelectedIndexChanged;
                    eventsListBox.SelectedIndex = newSelectedIndex;
                    eventsListBox.SelectedIndexChanged += eventsListBox_SelectedIndexChanged;
                }

                editingIndex = newSelectedIndex;

                // Vérifier que l'index est valide
                if (editingIndex < 0 || editingIndex >= events.Count)
                    return;

                CalendarEvent evt = events[editingIndex];

                // Désactiver temporairement la détection de changements
                hasUnsavedChanges = false;

                txtLibelle.Text = evt.Libelle;
                dtpDateDebut.Value = evt.DateDebut;
                dtpDateFin.Value = evt.DateFin;

                chkJourneeEntiere.Checked = evt.IsAllDay();

                dtpHeureDebut.Value = evt.HeureDebut.HasValue
                    ? DateTime.Today.Add(evt.HeureDebut.Value)
                    : DateTime.Today;
                dtpHeureFin.Value = evt.HeureFin.HasValue
                    ? DateTime.Today.Add(evt.HeureFin.Value)
                    : DateTime.Today;

                AppliquerJourneeEntiere();

                txtLieu.Text = evt.Lieu;
                txtDescription.Text = evt.Description;

                // Réinitialiser après le chargement
                hasUnsavedChanges = false;
                UpdateButtonStates();
            }
        }
    }
}
