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
            dtpHeureDebut.ShowCheckBox = true;
            dtpHeureDebut.Checked = false;
            dtpHeureDebut.Value = DateTime.Today; // 00:00
            dtpHeureFin.ShowCheckBox = true;
            dtpHeureFin.Checked = false;
            dtpHeureFin.Value = DateTime.Today; // 00:00

            // Ajouter des gestionnaires d'événements pour détecter les modifications
            txtLibelle.TextChanged += (s, e) => MarkAsChanged();
            dtpDateDebut.ValueChanged += (s, e) => MarkAsChanged();
            dtpDateFin.ValueChanged += (s, e) => MarkAsChanged();
            dtpHeureDebut.ValueChanged += (s, e) => MarkAsChanged();
            dtpHeureFin.ValueChanged += (s, e) => MarkAsChanged();
            txtLieu.TextChanged += (s, e) => MarkAsChanged();
            txtDescription.TextChanged += (s, e) => MarkAsChanged();

            // Mettre à jour l'état des boutons
            UpdateButtonStates();
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
                events[editingIndex] = CreateEventFromInputs();
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
                    events.RemoveAt(deletedIndex);
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

        private void btnExportICS_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics";
            saveFileDialog.Title = "Exporter en ICS";
            saveFileDialog.FileName = "icsEditor.ics";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string icsContent = ICSManager.GenerateICS(events);
                    File.WriteAllText(saveFileDialog.FileName, icsContent);
                    MessageBox.Show("Fichier ICS généré avec succès.", "Export terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    List<CalendarEvent> importedEvents = ICSManager.ParseICS(icsContent);

                    if (importedEvents.Count > 0)
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
                            // Ajouter
                            events.AddRange(importedEvents);
                            UpdateEventsList();
                            ClearInputs();
                            editingIndex = -1;
                            hasUnsavedChanges = false;

                            // Sélectionner le premier élément
                            if (eventsListBox.Items.Count > 0)
                            {
                                eventsListBox.SelectedIndex = 0;
                            }

                            MessageBox.Show("Événements ajoutés avec succès.", "Import terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            if (dtpHeureDebut.Checked && dtpHeureFin.Checked && dtpDateDebut.Value == dtpDateFin.Value)
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
                HeureDebut = dtpHeureDebut.Checked ? (TimeSpan?)dtpHeureDebut.Value.TimeOfDay : null,
                HeureFin = dtpHeureFin.Checked ? (TimeSpan?)dtpHeureFin.Value.TimeOfDay : null,
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
            dtpHeureDebut.Checked = false;
            dtpHeureFin.Checked = false;
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
            btnExportICS.Enabled = events.Count > 0;
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

                if (evt.HeureDebut.HasValue)
                {
                    dtpHeureDebut.Checked = true;
                    dtpHeureDebut.Value = DateTime.Today.Add(evt.HeureDebut.Value);
                }
                else
                {
                    dtpHeureDebut.Checked = false;
                }

                if (evt.HeureFin.HasValue)
                {
                    dtpHeureFin.Checked = true;
                    dtpHeureFin.Value = DateTime.Today.Add(evt.HeureFin.Value);
                }
                else
                {
                    dtpHeureFin.Checked = false;
                }

                txtLieu.Text = evt.Lieu;
                txtDescription.Text = evt.Description;

                // Réinitialiser après le chargement
                hasUnsavedChanges = false;
                UpdateButtonStates();
            }
        }
    }
}
