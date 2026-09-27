using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace icsEditor
{
    public partial class MainForm : MetroForm
    {
        private List<CalendarEvent> events;

        /// <summary>
        /// Événements supprimés qui avaient déjà été publiés : ils restent dans le
        /// fichier enregistré avec STATUS:CANCELLED pour que le calendrier destinataire
        /// les retire au lieu de conserver une copie orpheline.
        /// </summary>
        private List<CalendarEvent> cancelledEvents = new List<CalendarEvent>();
        private int editingIndex = -1;
        private bool hasUnsavedChanges = false;

        /// <summary>
        /// Fichier ICS en cours d'édition : Enregistrer y réécrit directement.
        /// Null tant qu'aucun fichier n'a été ouvert ni enregistré.
        /// </summary>
        private string currentFilePath;

        /// <summary>
        /// La liste diffère du fichier sur disque. Distinct de hasUnsavedChanges,
        /// qui ne concerne que le formulaire de saisie de l'événement sélectionné.
        /// </summary>
        private bool isDocumentModified = false;

        /// <summary>Nom affiché tant que le document n'a pas de fichier, repris à l'enregistrement.</summary>
        private const string NomSansTitre = "Sans titre";

        public MainForm(string fichierAOuvrir = null)
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
            txtLibelle.TextChanged += (s, e) => { RetirerSautsDeLigne(txtLibelle); MarkAsChanged(); };
            // La fin suit le début (date et heure) : la plupart des événements
            // tiennent sur une journée. Le chargement d'un événement réaffecte la
            // fin juste après, sa vraie valeur est donc conservée.
            dtpDateDebut.ValueChanged += (s, e) => { dtpDateFin.Value = dtpDateDebut.Value; MarkAsChanged(); };
            dtpDateFin.ValueChanged += (s, e) => MarkAsChanged();
            dtpHeureDebut.ValueChanged += (s, e) => { dtpHeureFin.Value = dtpHeureDebut.Value; MarkAsChanged(); };
            dtpHeureFin.ValueChanged += (s, e) => MarkAsChanged();
            chkJourneeEntiere.CheckedChanged += (s, e) => { AppliquerJourneeEntiere(); MarkAsChanged(); };
            txtLieu.TextChanged += (s, e) => { RetirerSautsDeLigne(txtLieu); MarkAsChanged(); };
            txtDescription.TextChanged += (s, e) => MarkAsChanged();

            // Mettre à jour l'état des boutons
            UpdateButtonStates();
            UpdateTitle();

            FormClosing += (s, e) => e.Cancel = !ConfirmDocumentChanges();

            if (fichierAOuvrir != null)
                Load += (s, e) => OpenDocument(fichierAOuvrir);
        }

        /// <summary>
        /// Un texte collé depuis une page web ou un tableur traîne souvent un saut
        /// de ligne final. Dans un champ d'une seule ligne il est invisible mais
        /// suit chaque copie : on le remplace par un espace, ou on le retire en bout.
        /// </summary>
        private static void RetirerSautsDeLigne(MetroFramework.Controls.MetroTextBox champ)
        {
            if (champ.Text.IndexOfAny(new[] { '\r', '\n' }) < 0)
                return;

            int position = champ.SelectionStart;
            champ.Text = SansSautsDeLigne(champ.Text);
            champ.SelectionStart = Math.Min(position, champ.Text.Length);
        }

        private static string SansSautsDeLigne(string texte)
        {
            return Regex.Replace(texte, @"[ \t]*[\r\n]+[ \t]*", " ").Trim();
        }

        private void MarkDocumentModified()
        {
            isDocumentModified = true;
            UpdateTitle();
            UpdateButtonStates();
        }

        /// <summary>
        /// Un fichier sans événement ni annulation n'a rien à dire au calendrier
        /// destinataire : on ne l'écrit pas. Des annulations seules suffisent.
        /// </summary>
        private bool HasContentToSave()
        {
            return events.Count > 0 || cancelledEvents.Count > 0;
        }

        private void UpdateTitle()
        {
            string nom = currentFilePath == null ? NomSansTitre : Path.GetFileName(currentFilePath);
            Text = $"ICS Editor : {nom}{(isDocumentModified ? " *" : "")}";
            Invalidate();
        }

        /// <summary>
        /// Avant de quitter ou d'ouvrir un autre fichier : propose d'enregistrer.
        /// Retourne false si l'utilisateur annule.
        /// </summary>
        private bool ConfirmDocumentChanges()
        {
            if (!ConfirmUnsavedChanges())
                return false;

            if (!isDocumentModified || !HasContentToSave())
                return true;

            DialogResult result = MessageBox.Show(
                "Enregistrer les modifications avant de continuer ?",
                "Modifications non enregistrées",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                return SaveDocument();

            return result == DialogResult.No;
        }

        /// <summary>
        /// Les champs heure n'ont de sens que pour un événement qui n'occupe pas
        /// la journée entière : on les grise plutôt que de laisser saisir une valeur
        /// qui serait ignorée à l'enregistrement.
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
                MarkDocumentModified();
                ClearInputs();
                UpdateEventsList();
                hasUnsavedChanges = false;
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
                MarkDocumentModified();
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

        private void btnDupliquer_Click(object sender, EventArgs e)
        {
            int selectedIndex = eventsListBox.SelectedIndex;
            if (selectedIndex < 0 || !ConfirmUnsavedChanges())
                return;

            // Une copie est un nouvel événement pour le calendrier destinataire :
            // nouvel UID, jamais publié. Reprendre l'UID écraserait l'original.
            CalendarEvent source = events[selectedIndex];
            CalendarEvent copie = new CalendarEvent
            {
                Libelle = source.Libelle,
                DateDebut = source.DateDebut,
                DateFin = source.DateFin,
                HeureDebut = source.HeureDebut,
                HeureFin = source.HeureFin,
                Lieu = source.Lieu,
                Description = source.Description
            };

            events.Insert(selectedIndex + 1, copie);
            MarkDocumentModified();

            editingIndex = -1;
            hasUnsavedChanges = false;
            UpdateEventsList();

            // Sélectionner la copie pour l'ajuster aussitôt (date, en général)
            eventsListBox.SelectedIndex = selectedIndex + 1;
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
                    MarkDocumentModified();

                    // Un événement jamais publié disparaît sans laisser de trace.
                    // Les autres sont conservés annulés : le prochain enregistrement dira au
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
                MarkDocumentModified();
                ClearInputs();
                UpdateEventsList();
                editingIndex = -1;
                hasUnsavedChanges = false;
                UpdateButtonStates();
            }
        }

        private void btnClasserParDates_Click(object sender, EventArgs e)
        {
            if (!ConfirmUnsavedChanges())
                return;

            // Tri stable : deux événements au même instant gardent leur ordre.
            // Une journée entière (sans heure) passe avant les créneaux du même jour.
            CalendarEvent selection = eventsListBox.SelectedIndex >= 0 ? events[eventsListBox.SelectedIndex] : null;
            List<CalendarEvent> classes = events
                .OrderBy(evt => evt.DateDebut)
                .ThenBy(evt => evt.HeureDebut ?? TimeSpan.MinValue)
                .ToList();

            if (classes.SequenceEqual(events))
                return;

            events = classes;
            MarkDocumentModified();

            editingIndex = -1;
            hasUnsavedChanges = false;
            UpdateEventsList();

            // Garder l'événement sélectionné, à sa nouvelle place
            if (selection != null)
                eventsListBox.SelectedIndex = events.IndexOf(selection);
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
                MarkDocumentModified();

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
                MarkDocumentModified();

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
                ? (aPurger == 1
                    ? "Supprimer définitivement l'annulation en attente, même récente ?"
                    : $"Supprimer définitivement les {aPurger} annulations, y compris les plus récentes ?")
                    + Environment.NewLine + Environment.NewLine
                    + "Si ce fichier a déjà été diffusé, les événements supprimés resteront dans les calendriers qui n'ont pas encore lu l'annulation."
                : $"Supprimer définitivement {aPurger} {Libelles.Pluriel(aPurger, "annulation")} de plus de {MoisAvantPurgeAnnulation} mois ?";

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

            MarkDocumentModified();
            UpdateButtonStates();

            MessageBox.Show(
                $"{aPurger} {Libelles.Pluriel(aPurger, "annulation")} {Libelles.Pluriel(aPurger, "purgée")}. {cancelledEvents.Count} {Libelles.Pluriel(cancelledEvents.Count, "restante")}.",
                "Purge terminée",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+S fonctionne quel que soit le champ qui a le focus, mais suit
            // la même règle que le bouton : rien à enregistrer, rien ne se passe.
            if (keyData == (Keys.Control | Keys.S))
            {
                if (btnEnregistrer.Enabled)
                    SaveDocument();
                return true;
            }

            // Échap ferme l'application ; FormClosing propose d'enregistrer si besoin.
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            if (keyData == (Keys.Control | Keys.N))
            {
                btnNouveau_Click(btnNouveau, EventArgs.Empty);
                return true;
            }

            if (keyData == (Keys.Control | Keys.O))
            {
                btnOuvrir_Click(btnOuvrir, EventArgs.Empty);
                return true;
            }

            // Collage dans un champ d'une ligne : la zone de texte Windows s'arrête
            // au premier saut de ligne, et un texte qui commence par un CRLF ne
            // colle donc rien. On colle nous-mêmes le texte débarrassé des sauts.
            if (keyData == (Keys.Control | Keys.V) || keyData == (Keys.Shift | Keys.Insert))
            {
                MetroFramework.Controls.MetroTextBox champ =
                    txtLieu.ContainsFocus ? txtLieu : txtLibelle.ContainsFocus ? txtLibelle : null;

                if (champ != null && Clipboard.ContainsText())
                {
                    // MetroTextBox.SelectedText remplace tout le texte au lieu de la
                    // sélection : on reconstruit la chaîne à la position du curseur.
                    string colle = SansSautsDeLigne(Clipboard.GetText());
                    int debut = champ.SelectionStart;
                    champ.Text = champ.Text.Remove(debut, champ.SelectionLength).Insert(debut, colle);
                    champ.SelectionStart = debut + colle.Length;
                    champ.SelectionLength = 0;
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnEnregistrer_Click(object sender, EventArgs e)
        {
            // Pas de message de confirmation : le « * » qui disparaît du titre
            // et le bouton qui se grise suffisent à dire que c'est enregistré.
            SaveDocument();
        }

        /// <summary>
        /// Réécrit le fichier courant ; demande un emplacement s'il n'y en a pas encore.
        /// Retourne false si l'utilisateur annule ou si l'écriture échoue.
        /// </summary>
        private bool SaveDocument()
        {
            if (!ConfirmUnsavedChanges())
                return false;

            if (currentFilePath == null)
            {
                saveFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics";
                saveFileDialog.Title = "Enregistrer le fichier ICS";
                saveFileDialog.FileName = NomSansTitre + ".ics";

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return false;

                currentFilePath = saveFileDialog.FileName;
            }

            try
            {
                // Les annulations accompagnent les événements actifs dans le même
                // fichier : c'est ce qui permet au calendrier destinataire de
                // supprimer ce qui a été supprimé ici.
                List<CalendarEvent> aEnregistrer = new List<CalendarEvent>(events);
                aEnregistrer.AddRange(cancelledEvents);

                string icsContent = ICSManager.GenerateICS(aEnregistrer);
                File.WriteAllText(currentFilePath, icsContent);

                isDocumentModified = false;
                UpdateTitle();
                UpdateButtonStates();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement ICS: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnNouveau_Click(object sender, EventArgs e)
        {
            if (!ConfirmDocumentChanges())
                return;

            // Document vide, sans fichier : le prochain enregistrement demandera
            // où l'écrire. Les annulations appartenaient à l'ancien fichier.
            events = new List<CalendarEvent>();
            cancelledEvents = new List<CalendarEvent>();
            currentFilePath = null;
            isDocumentModified = false;

            ClearInputs();
            editingIndex = -1;
            hasUnsavedChanges = false;
            UpdateEventsList();
            UpdateTitle();
        }

        private void btnOuvrir_Click(object sender, EventArgs e)
        {
            if (!ConfirmDocumentChanges())
                return;

            openFileDialog.Filter = "Fichiers ICS (*.ics)|*.ics";
            openFileDialog.Title = "Ouvrir un fichier ICS";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            OpenDocument(openFileDialog.FileName);
        }

        private void OpenDocument(string chemin)
        {
            try
            {
                string icsContent = File.ReadAllText(chemin);
                List<CalendarEvent> parsedEvents = ICSManager.ParseICS(icsContent);

                // Les événements annulés du fichier ne sont pas affichés : ce sont
                // des traces de suppression, qu'on se contente de reconduire à
                // l'enregistrement pour ne pas les ressusciter chez le destinataire.
                events = parsedEvents.FindAll(parsed => !parsed.EstAnnule);
                cancelledEvents = parsedEvents.FindAll(parsed => parsed.EstAnnule);

                currentFilePath = chemin;
                isDocumentModified = false;

                ClearInputs();
                editingIndex = -1;
                hasUnsavedChanges = false;
                UpdateEventsList();
                UpdateTitle();

                if (eventsListBox.Items.Count > 0)
                {
                    eventsListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture ICS: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            btnDupliquer.Enabled = selectedIndex >= 0;
            btnClasserParDates.Enabled = events.Count > 1;
            // Rien à enregistrer tant que la liste et le formulaire en cours
            // d'édition sont identiques au fichier sur disque.
            btnEnregistrer.Enabled = HasContentToSave() && (isDocumentModified || hasUnsavedChanges);

            // Les annulations n'apparaissent pas dans la liste : le compteur du bouton
            // et son infobulle sont les seuls endroits où l'utilisateur les voit.
            // Purger n'a de sens que sur un fichier chargé qui en contient.
            btnPurgeAnnulations.Enabled = currentFilePath != null && cancelledEvents.Count > 0;
            btnPurgeAnnulations.Text = cancelledEvents.Count > 0
                ? $"Purger les annulations ({cancelledEvents.Count})"
                : "Purger les annulations";
            toolTipAide.SetToolTip(btnPurgeAnnulations,
                $"{cancelledEvents.Count} {Libelles.Pluriel(cancelledEvents.Count, "suppression")} {Libelles.Pluriel(cancelledEvents.Count, "publiée")} avec le prochain enregistrement, pour que le calendrier destinataire retire ces événements.");
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
                        // Même règle que btnUpdate_Click : garder l'identité, sinon
                        // le calendrier destinataire verrait un nouvel événement.
                        CalendarEvent updatedEvent = CreateEventFromInputs();
                        updatedEvent.Uid = events[editingIndex].Uid;
                        updatedEvent.Sequence = events[editingIndex].Sequence + 1;
                        events[editingIndex] = updatedEvent;
                        MarkDocumentModified();
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
