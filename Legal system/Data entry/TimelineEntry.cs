using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Label = System.Windows.Forms.Label;

namespace Legal_system.Data_entry
{
    public partial class TimelineEntry : Form
    {
        private DatabaseHelper db = new DatabaseHelper("legal.db");

        private Dictionary<int, string> legislationMap;
        private Dictionary<int, string> respondentMap;
        private Dictionary<int, string> evidenceMap;

        private List<List<int>> respondentLegislation = new List<List<int>>();

        public TimelineEntry()
        {
            InitializeComponent();

            // ---------------------------------------------------------
            // LOAD DATA
            // ---------------------------------------------------------

            legislationMap = db.GetLegislation();
            respondentMap = db.GetRespondents();
            evidenceMap = db.GetEvidence();

            // ---------------------------------------------------------
            // RESPONDENT LIST
            // ---------------------------------------------------------

            comboBox2.Items.Clear();

            foreach (KeyValuePair<int, string> kv in respondentMap)
            {
                comboBox2.Items.Add(kv.Value);
            }

            // ---------------------------------------------------------
            // EVIDENCE LIST
            // IMPORTANT:
            // We store the ACTUAL database ID in EvidenceItem.Id.
            // Do NOT use SelectedIndex as the database ID.
            // ---------------------------------------------------------

            comboBox1.Items.Clear();

            comboBox1.Items.Add(new EvidenceItem
            {
                Id = -1,
                Name = "Select evidence..."
            });

            foreach (KeyValuePair<int, string> kv in evidenceMap)
            {
                comboBox1.Items.Add(new EvidenceItem
                {
                    Id = kv.Key,
                    Name = kv.Value
                });
            }

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedIndex = 0;

            comboBox1.IntegralHeight = false;
            comboBox1.MaxDropDownItems = 8;
            comboBox1.DropDownHeight = 180;

            // ---------------------------------------------------------
            // ADD RESPONDENT BUTTON
            // ---------------------------------------------------------

            Button addRespondentBtn = new Button();
            addRespondentBtn.Text = "+ Add Respondent";
            addRespondentBtn.AutoSize = true;
            addRespondentBtn.Location =
                new Point(tabControl1.Left, tabControl1.Bottom + 8);

            addRespondentBtn.Click += AddRespondentBtn_Click;

            this.Controls.Add(addRespondentBtn);
            addRespondentBtn.BringToFront();

            tabControl1.SelectedIndexChanged += delegate
            {
                UpdateLabel4();
            };

            // Remove designer tab.
            tabControl1.TabPages.Remove(tabPage1);

            // Create first respondent.
            AddRespondentBtn_Click(null, EventArgs.Empty);
        }

        // =============================================================
        // RESPONDENT TAB
        // =============================================================

        private void AttachPickerToTab(TabPage page, int respondentIndex)
        {
            // ---------------------------------------------------------
            // REMOVE RESPONDENT BUTTON
            // ---------------------------------------------------------

            Button closeBtn = new Button();
            closeBtn.Text = "✕ Remove This Respondent";
            closeBtn.AutoSize = true;
            closeBtn.Location = new Point(10, 5);
            closeBtn.FlatStyle = FlatStyle.Flat;
            closeBtn.BackColor = Color.FromArgb(255, 220, 220);
            closeBtn.Tag = page;

            closeBtn.Click += delegate (object sender, EventArgs e)
            {
                if (tabControl1.TabPages.Count <= 1)
                    return;

                Button button = sender as Button;

                if (button == null)
                    return;

                TabPage targetPage = button.Tag as TabPage;

                if (targetPage == null)
                    return;

                int idx = tabControl1.TabPages.IndexOf(targetPage);

                if (idx >= 0 && idx < respondentLegislation.Count)
                {
                    respondentLegislation.RemoveAt(idx);
                    tabControl1.TabPages.Remove(targetPage);
                    RenumberTabs();
                }
            };

            page.Controls.Add(closeBtn);

            // ---------------------------------------------------------
            // RESPONDENT DROPDOWN
            // ---------------------------------------------------------

            ComboBox respondentDropdown = new ComboBox();
            respondentDropdown.Location = new Point(10, 35);
            respondentDropdown.Width = 180;
            respondentDropdown.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (object item in comboBox2.Items)
            {
                respondentDropdown.Items.Add(item);
            }

            page.Controls.Add(respondentDropdown);

            // ---------------------------------------------------------
            // LEGISLATION LABEL
            // ---------------------------------------------------------

            Label listLabel = new Label();
            listLabel.Text = "Legislation list:";
            listLabel.Location = new Point(10, 70);
            listLabel.AutoSize = true;

            page.Controls.Add(listLabel);

            // ---------------------------------------------------------
            // SEARCH LABEL
            // ---------------------------------------------------------

            Label searchLabel = new Label();
            searchLabel.Text = "Legislation search:";
            searchLabel.Location = new Point(355, 40);
            searchLabel.AutoSize = true;

            page.Controls.Add(searchLabel);

            // ---------------------------------------------------------
            // LEGISLATION DISPLAY
            // ---------------------------------------------------------

            TextBox legDisplay = new TextBox();
            legDisplay.Location = new Point(10, 90);
            legDisplay.Size = new Size(310, 120);
            legDisplay.Multiline = true;
            legDisplay.ReadOnly = true;
            legDisplay.ScrollBars = ScrollBars.Vertical;
            legDisplay.BackColor = SystemColors.Window;

            page.Controls.Add(legDisplay);

            // ---------------------------------------------------------
            // REMOVE LAST LEGISLATION
            // ---------------------------------------------------------

            Button removeBtn = new Button();
            removeBtn.Text = "Remove last legislation";
            removeBtn.Location = new Point(355, 175);
            removeBtn.AutoSize = true;

            removeBtn.Click += delegate
            {
                int liveIndex = tabControl1.TabPages.IndexOf(page);

                if (liveIndex < 0 ||
                    liveIndex >= respondentLegislation.Count)
                    return;

                if (respondentLegislation[liveIndex].Count > 0)
                {
                    respondentLegislation[liveIndex].RemoveAt(
                        respondentLegislation[liveIndex].Count - 1);

                    legDisplay.Text = string.Join(
                        Environment.NewLine,
                        respondentLegislation[liveIndex]
                            .Select(id => legislationMap[id])
                    );
                }
            };

            page.Controls.Add(removeBtn);

            // ---------------------------------------------------------
            // LEGISLATION PICKER
            // ---------------------------------------------------------

            LegislationPicker picker = new LegislationPicker();

            picker.Location = new Point(350, 60);
            picker.Width = 400;

            picker.LoadFromMap(legislationMap);

            picker.SelectionChanged += delegate
            {
                int liveIndex = tabControl1.TabPages.IndexOf(page);

                if (liveIndex < 0)
                    return;

                while (respondentLegislation.Count <= liveIndex)
                {
                    respondentLegislation.Add(new List<int>());
                }

                respondentLegislation[liveIndex] =
                    picker.GetSelectedIds();

                legDisplay.Text = string.Join(
                    Environment.NewLine,
                    picker.GetSelectedNames()
                );
            };

            page.Controls.Add(picker);
        }

        private void AddRespondentBtn_Click(object sender, EventArgs e)
        {
            int newIndex = tabControl1.TabPages.Count;

            TabPage newPage =
                new TabPage("Respondent " + (newIndex + 1));

            respondentLegislation.Add(new List<int>());

            tabControl1.TabPages.Add(newPage);

            AttachPickerToTab(newPage, newIndex);

            tabControl1.SelectedTab = newPage;

            UpdateLabel4();
        }

        private void UpdateLabel4()
        {
            label4.Text =
                "Current tab: " +
                (tabControl1.SelectedIndex + 1);
        }

        private void RenumberTabs()
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages[i].Text =
                    "Respondent " + (i + 1);
            }

            UpdateLabel4();
        }

        // =============================================================
        // DESIGNER STUBS
        // =============================================================

        private void tabPage1_Click(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void button2_Click(object sender, EventArgs e) { }

        private void comboBox2_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e) { }

        // =============================================================
        // SAVE
        // =============================================================

        private void button1_Click(object sender, EventArgs e)
        {
            // ---------------------------------------------------------
            // YEAR
            // ---------------------------------------------------------

            int year;

            if (!int.TryParse(textBox1.Text, out year))
            {
                MessageBox.Show(
                    "Please enter a valid year.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // ---------------------------------------------------------
            // EVIDENCE
            //
            // THIS IS THE IMPORTANT PART.
            //
            // Do NOT do:
            //
            // evidenceMap.Keys.ElementAt(comboBox1.SelectedIndex)
            //
            // because SelectedIndex is a UI index.
            //
            // Instead retrieve the EvidenceItem that was actually
            // selected and use its Id.
            // ---------------------------------------------------------

            EvidenceItem selectedEvidence =
                comboBox1.SelectedItem as EvidenceItem;

            if (selectedEvidence == null ||
                selectedEvidence.Id < 1)
            {
                MessageBox.Show(
                    "Please select evidence.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int evidenceId = selectedEvidence.Id;

            // ---------------------------------------------------------
            // DEBUG - REMOVE THIS MESSAGEBOX AFTER TESTING
            // ---------------------------------------------------------

            // This lets you CONFIRM exactly what is being sent.
            //
            // If you select evidence ID 5, this will say:
            //
            // Selected evidence:
            // Name = whatever
            // ID = 5
            //
            // This proves the ComboBox is no longer converting it
            // to an index.

            // MessageBox.Show(
            //     "Selected evidence:\r\n\r\n" +
            //     "Name = " + selectedEvidence.Name + "\r\n" +
            //     "ID = " + selectedEvidence.Id);

            // ---------------------------------------------------------
            // RESPONDENTS
            // ---------------------------------------------------------

            List<string> respondentNames =
                new List<string>();

            for (int i = 0;
                 i < tabControl1.TabPages.Count;
                 i++)
            {
                TabPage page = tabControl1.TabPages[i];

                ComboBox dropdown =
                    page.Controls
                        .OfType<ComboBox>()
                        .FirstOrDefault();

                string name =
                    dropdown != null &&
                    dropdown.SelectedItem != null
                        ? dropdown.SelectedItem.ToString()
                        : "";

                respondentNames.Add(name);
            }

            string respondentsCSV =
                string.Join(",", respondentNames);

            // ---------------------------------------------------------
            // LEGISLATION
            // ---------------------------------------------------------

            List<List<string>> allLegislation =
                new List<List<string>>();

            for (int i = 0;
                 i < respondentLegislation.Count;
                 i++)
            {
                List<string> legNames =
                    respondentLegislation[i]
                        .Select(id => legislationMap[id])
                        .ToList();

                allLegislation.Add(legNames);
            }

            string respondentsLegalString =
                string.Join(
                    "|",
                    allLegislation.Select(
                        group => string.Join(",", group)
                    )
                );

            // ---------------------------------------------------------
            // SAVE
            // ---------------------------------------------------------

            try
            {
                int newEventId =
                    db.AddCaseEvent(
                        year,
                        evidenceId,
                        respondentsCSV,
                        respondentsLegalString);

                MessageBox.Show(
                    "Saved successfully.\r\n\r\n" +
                    "Event ID: " + newEventId + "\r\n" +
                    "Evidence ID: " + evidenceId,
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error:\r\n\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            // Nothing required here.
            //
            // button1_Click reads comboBox1.SelectedItem
            // and gets the actual EvidenceItem.Id.
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }

    // =================================================================
    // EVIDENCE ITEM
    // =================================================================

    public class EvidenceItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    // =================================================================
    // LEGISLATION PICKER
    // =================================================================

    public class LegislationPicker : UserControl
    {
        private TextBox searchBox;
        private ListBox suggestionList;
        private FlowLayoutPanel tagPanel;

        private Dictionary<int, string> _map =
            new Dictionary<int, string>();

        private Dictionary<int, string> _selected =
            new Dictionary<int, string>();

        public event EventHandler SelectionChanged;

        private const string Placeholder =
            "Search legislation...";

        public LegislationPicker()
        {
            Width = 400;
            AutoSize = true;

            // ---------------------------------------------------------
            // TAG PANEL
            // ---------------------------------------------------------

            tagPanel = new FlowLayoutPanel();

            tagPanel.AutoSize = true;
            tagPanel.AutoSizeMode =
                AutoSizeMode.GrowAndShrink;

            tagPanel.MinimumSize =
                new Size(Width, 30);

            tagPanel.WrapContents = true;
            tagPanel.Dock = DockStyle.Top;
            tagPanel.BackColor = Color.White;
            tagPanel.BorderStyle =
                BorderStyle.FixedSingle;

            tagPanel.Padding = new Padding(3);

            // ---------------------------------------------------------
            // SUGGESTION LIST
            // ---------------------------------------------------------

            suggestionList = new ListBox();

            suggestionList.Dock = DockStyle.Top;
            suggestionList.Height = 100;

            suggestionList.ScrollAlwaysVisible = true;
            suggestionList.HorizontalScrollbar = true;
            suggestionList.HorizontalExtent = 600;

            suggestionList.Visible = false;
            suggestionList.IntegralHeight = false;

            suggestionList.Click +=
                SuggestionList_Click;

            suggestionList.KeyDown += delegate (
                object sender,
                KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SelectCurrent();
                    e.Handled = true;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    HideSuggestions();
                    e.Handled = true;
                }
            };

            // ---------------------------------------------------------
            // SEARCH BOX
            // ---------------------------------------------------------

            searchBox = new TextBox();

            searchBox.Dock = DockStyle.Top;

            searchBox.TextChanged +=
                SearchBox_TextChanged;

            searchBox.KeyDown +=
                SearchBox_KeyDown;

            searchBox.Enter += delegate
            {
                if (searchBox.ForeColor == Color.Gray)
                {
                    searchBox.TextChanged -=
                        SearchBox_TextChanged;

                    searchBox.Text = "";
                    searchBox.ForeColor = Color.Black;

                    searchBox.TextChanged +=
                        SearchBox_TextChanged;
                }
            };

            searchBox.Leave += delegate
            {
                if (string.IsNullOrWhiteSpace(
                    searchBox.Text))
                {
                    ResetSearch();
                    HideSuggestions();
                }
            };

            ResetSearch();

            Controls.Add(tagPanel);
            Controls.Add(suggestionList);
            Controls.Add(searchBox);
        }

        // -------------------------------------------------------------
        // RESET SEARCH
        // -------------------------------------------------------------

        private void ResetSearch()
        {
            if (searchBox == null)
                return;

            searchBox.TextChanged -=
                SearchBox_TextChanged;

            searchBox.Text = Placeholder;
            searchBox.ForeColor = Color.Gray;

            searchBox.TextChanged +=
                SearchBox_TextChanged;

            HideSuggestions();
        }

        // -------------------------------------------------------------
        // HIDE SUGGESTIONS
        // -------------------------------------------------------------

        private void HideSuggestions()
        {
            if (suggestionList == null)
                return;

            suggestionList.Visible = false;
            suggestionList.Items.Clear();
        }

        // -------------------------------------------------------------
        // SEARCH
        // -------------------------------------------------------------

        private void SearchBox_TextChanged(
            object sender,
            EventArgs e)
        {
            if (searchBox.ForeColor == Color.Gray)
                return;

            string filter =
                searchBox.Text.Trim();

            if (string.IsNullOrEmpty(filter))
            {
                HideSuggestions();
                return;
            }

            List<KeyValuePair<int, string>> matches =
                _map
                    .Where(kv =>
                        !_selected.ContainsKey(kv.Key) &&
                        kv.Value.IndexOf(
                            filter,
                            StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            if (matches.Count == 0)
            {
                HideSuggestions();
                return;
            }

            suggestionList.Items.Clear();

            foreach (KeyValuePair<int, string> kv in matches)
            {
                suggestionList.Items.Add(kv);
            }

            suggestionList.DisplayMember = "Value";
            suggestionList.Visible = true;
        }

        // -------------------------------------------------------------
        // SEARCH KEYBOARD
        // -------------------------------------------------------------

        private void SearchBox_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (!suggestionList.Visible)
                return;

            if (e.KeyCode == Keys.Down)
            {
                suggestionList.Focus();

                if (suggestionList.Items.Count > 0)
                    suggestionList.SelectedIndex = 0;

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (suggestionList.Items.Count > 0)
                {
                    suggestionList.SelectedIndex = 0;
                    SelectCurrent();
                }

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideSuggestions();
                e.Handled = true;
            }
        }

        private void SuggestionList_Click(
            object sender,
            EventArgs e)
        {
            SelectCurrent();
        }

        // -------------------------------------------------------------
        // SELECT
        // -------------------------------------------------------------

        private void SelectCurrent()
        {
            if (suggestionList.SelectedItem == null)
                return;

            KeyValuePair<int, string> kv =
                (KeyValuePair<int, string>)
                    suggestionList.SelectedItem;

            AddTag(kv.Key, kv.Value);

            ResetSearch();
            searchBox.Focus();
        }

        // -------------------------------------------------------------
        // ADD TAG
        // -------------------------------------------------------------

        private void AddTag(int id, string name)
        {
            if (_selected.ContainsKey(id))
                return;

            _selected[id] = name;

            Panel tag = new Panel();

            tag.AutoSize = true;
            tag.BackColor =
                Color.FromArgb(220, 235, 255);

            tag.Margin = new Padding(2);
            tag.Padding =
                new Padding(4, 2, 4, 2);

            tag.Tag = id;

            Label lbl = new Label();

            lbl.Text = name;
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Left;
            lbl.TextAlign =
                ContentAlignment.MiddleLeft;

            Button btn = new Button();

            btn.Text = "×";
            btn.Width = 18;
            btn.Height = 18;

            btn.FlatStyle =
                FlatStyle.Flat;

            btn.FlatAppearance.BorderSize = 0;

            btn.Dock = DockStyle.Right;

            btn.Cursor =
                Cursors.Hand;

            btn.Tag = id;

            btn.Click += delegate (
                object sender,
                EventArgs e)
            {
                Button button =
                    sender as Button;

                if (button == null)
                    return;

                int removeId =
                    (int)button.Tag;

                _selected.Remove(removeId);

                tagPanel.Controls.Remove(tag);

                if (SelectionChanged != null)
                {
                    SelectionChanged(
                        this,
                        EventArgs.Empty);
                }
            };

            tag.Controls.Add(lbl);
            tag.Controls.Add(btn);

            tagPanel.Controls.Add(tag);

            if (SelectionChanged != null)
            {
                SelectionChanged(
                    this,
                    EventArgs.Empty);
            }
        }

        // -------------------------------------------------------------
        // LOAD MAP
        // -------------------------------------------------------------

        public void LoadFromMap(
            Dictionary<int, string> map)
        {
            _map = map ??
                   new Dictionary<int, string>();

            _selected.Clear();

            tagPanel.Controls.Clear();

            ResetSearch();
        }

        // -------------------------------------------------------------
        // GET SELECTED
        // -------------------------------------------------------------

        public List<int> GetSelectedIds()
        {
            return _selected.Keys.ToList();
        }

        public List<string> GetSelectedNames()
        {
            return _selected.Values.ToList();
        }
    }
}