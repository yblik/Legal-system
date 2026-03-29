using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

using Label = System.Windows.Forms.Label;

namespace Legal_system.Data_entry
{
    public partial class TimelineEntry : Form
    {
        // At the top of the class, replace the hardcoded legislationMap and add db instance
        private DatabaseHelper db = new DatabaseHelper("timeline.db");

        // Remove the hardcoded legislationMap declaration and replace with:
        private Dictionary<int, string> legislationMap;
        private Dictionary<int, string> respondentMap;
        private Dictionary<int, string> evidenceMap;
        private List<List<int>> respondentLegislation = new List<List<int>>();

        //private Dictionary<int, string> legislationMap = new Dictionary<int, string>()
        //{
        //    { 1, "Health & Safety Act" },
        //    { 2, "Employment Rights Act" },
        //    { 3, "Equality Act" },
        //    { 4, "Data Protection Act 2" },
        //    { 5, "Data Protection Act 3" },
        //    { 6, "Data Protection Act 4" },
        //    { 7, "Data Protection Act 5" },
        //    { 8, "Data Protection Act 6" }
        //};

        public TimelineEntry()
        {
            InitializeComponent();

            // Load all maps from DB
            legislationMap = db.GetLegislation();
            respondentMap = db.GetRespondents();
            evidenceMap = db.GetEvidence();

            // Populate comboBox2 (respondents)
            comboBox2.Items.Clear();
            foreach (var kv in respondentMap)
                comboBox2.Items.Add(kv.Value);

            // Populate comboBox1 (evidence)
            comboBox1.Items.Clear();
            foreach (var kv in evidenceMap)
                comboBox1.Items.Add(kv.Value);

            var addRespondentBtn = new Button();
            addRespondentBtn.Text = "+ Add Respondent";
            addRespondentBtn.AutoSize = true;
            addRespondentBtn.Location = new Point(tabControl1.Left, tabControl1.Bottom + 8);
            addRespondentBtn.Click += AddRespondentBtn_Click;
            this.Controls.Add(addRespondentBtn);
            addRespondentBtn.BringToFront();

            tabControl1.SelectedIndexChanged += (s, e) => UpdateLabel4();

            // Remove designer tab — Respondent 1 will be built fresh like all others
            tabControl1.TabPages.Remove(tabPage1);
            AddRespondentBtn_Click(null, EventArgs.Empty);
        }

        private void AttachPickerToTab(TabPage page, int respondentIndex)
        {
            // Remove respondent button
            var closeBtn = new Button();
            closeBtn.Text = "✕ Remove This Respondent";
            closeBtn.AutoSize = true;
            closeBtn.Location = new Point(10, 5);
            closeBtn.FlatStyle = FlatStyle.Flat;
            closeBtn.BackColor = Color.FromArgb(255, 220, 220);
            closeBtn.Tag = page;
            closeBtn.Click += (s, e) =>
            {
                // Prevent deleting the last remaining tab
                if (tabControl1.TabPages.Count <= 1) return;

                var targetPage = (TabPage)((Button)s).Tag;
                int idx = tabControl1.TabPages.IndexOf(targetPage);
                if (idx >= 0)
                {
                    respondentLegislation.RemoveAt(idx);
                    tabControl1.TabPages.Remove(targetPage);
                    RenumberTabs();
                }
            };
            page.Controls.Add(closeBtn);

            // Respondent dropdown
            var respondentDropdown = new ComboBox();
            respondentDropdown.Location = new Point(10, 35);
            respondentDropdown.Width = 180;
            respondentDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var item in comboBox2.Items)
                respondentDropdown.Items.Add(item);
            page.Controls.Add(respondentDropdown);

            // Legislation list label
            var listLabel = new System.Windows.Forms.Label();
            listLabel.Text = "Legislation list:";
            listLabel.Location = new Point(10, 70);
            listLabel.AutoSize = true;
            page.Controls.Add(listLabel);

            // Legislation search label
            var searchLabel = new System.Windows.Forms.Label();
            searchLabel.Text = "Legislation search:";
            searchLabel.Location = new Point(355, 40);
            searchLabel.AutoSize = true;
            page.Controls.Add(searchLabel);

            // Legislation display textbox
            var legDisplay = new TextBox();
            legDisplay.Location = new Point(10, 90);
            legDisplay.Size = new Size(310, 120);
            legDisplay.Multiline = true;
            legDisplay.ReadOnly = true;
            legDisplay.ScrollBars = ScrollBars.Vertical;
            legDisplay.BackColor = SystemColors.Window;
            page.Controls.Add(legDisplay);

            // Remove last legislation button
            var removeBtn = new Button();
            removeBtn.Text = "Remove last legislation";
            removeBtn.Location = new Point(355, 175);
            removeBtn.AutoSize = true;
            removeBtn.Click += (s, e) =>
            {
                int liveIndex = tabControl1.TabPages.IndexOf(page);
                if (liveIndex < 0 || liveIndex >= respondentLegislation.Count) return;

                if (respondentLegislation[liveIndex].Count > 0)
                {
                    respondentLegislation[liveIndex].RemoveAt(respondentLegislation[liveIndex].Count - 1);
                    legDisplay.Text = string.Join(Environment.NewLine,
                        respondentLegislation[liveIndex].Select(id => legislationMap[id]));
                }
            };
            page.Controls.Add(removeBtn);

            // Legislation picker
            var picker = new LegislationPicker();
            picker.Location = new Point(350, 60);
            picker.Width = 400;
            picker.LoadFromMap(legislationMap);
            picker.SelectionChanged += (s, e) =>
            {
                int liveIndex = tabControl1.TabPages.IndexOf(page);
                if (liveIndex < 0) return;

                while (respondentLegislation.Count <= liveIndex)
                    respondentLegislation.Add(new List<int>());

                respondentLegislation[liveIndex] = picker.GetSelectedIds();
                legDisplay.Text = string.Join(Environment.NewLine, picker.GetSelectedNames());
            };
            page.Controls.Add(picker);
        }

        private void AddRespondentBtn_Click(object sender, EventArgs e)
        {
            int newIndex = tabControl1.TabPages.Count;
            var newPage = new TabPage($"Respondent {newIndex + 1}");
            respondentLegislation.Add(new List<int>());
            tabControl1.TabPages.Add(newPage);
            AttachPickerToTab(newPage, newIndex);
            tabControl1.SelectedTab = newPage;
            UpdateLabel4();
        }

        private void UpdateLabel4()
        {
            label4.Text = $"Current tab: {tabControl1.SelectedIndex + 1}";
        }

        private void RenumberTabs()
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
                tabControl1.TabPages[i].Text = $"Respondent {i + 1}";
            UpdateLabel4();
        }

        // ── Designer stubs ──


        // ── All original stub handlers preserved to keep Designer happy ──
        private void tabPage1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { } // Respondent dropdown: comboBox2
        private void label3_Click(object sender, EventArgs e) { }

        /// <summary>
        /// Save to db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Year
            if (!int.TryParse(textBox2.Text, out int year))
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Evidence ID from comboBox1
            int evidenceId = evidenceMap.Keys.ElementAtOrDefault(comboBox1.SelectedIndex);

            // Respondents CSV from each tab's comboBox2-populated dropdown
            var respondentNames = new List<string>();
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                TabPage page = tabControl1.TabPages[i];
                var dropdown = page.Controls.OfType<ComboBox>().FirstOrDefault();
                string name = dropdown?.SelectedItem?.ToString() ?? "";
                respondentNames.Add(name);
            }
            string respondentsCSV = string.Join(",", respondentNames);

            // 2D legislation list per respondent
            var allLegislation = new List<List<string>>();
            for (int i = 0; i < respondentLegislation.Count; i++)
            {
                List<string> legNames = respondentLegislation[i]
                    .Select(id => legislationMap[id])
                    .ToList();
                allLegislation.Add(legNames);
            }
            string respondentsLegalString = string.Join("|", allLegislation
                .Select(group => string.Join(",", group)));

            // Save
            try
            {
                int newEventId = db.AddCaseEvent(year, evidenceId, respondentsCSV, respondentsLegalString);
                MessageBox.Show($"Saved (ID: {newEventId})", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //evidence combo drop down: comboBox1
        }
    }

    public class LegislationPicker : UserControl
    {
        private TextBox searchBox;
        private ListBox suggestionList;
        private FlowLayoutPanel tagPanel;
        private Dictionary<int, string> _map = new Dictionary<int, string>();
        private Dictionary<int, string> _selected = new Dictionary<int, string>();

        public event EventHandler SelectionChanged;

        private const string Placeholder = "Search legislation...";

        public LegislationPicker()
        {
            Width = 400;
            AutoSize = true;

            tagPanel = new FlowLayoutPanel();
            tagPanel.AutoSize = true;
            tagPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tagPanel.MinimumSize = new Size(Width, 30);
            tagPanel.WrapContents = true;
            tagPanel.Dock = DockStyle.Top;
            tagPanel.BackColor = Color.White;
            tagPanel.BorderStyle = BorderStyle.FixedSingle;
            tagPanel.Padding = new Padding(3);

            suggestionList = new ListBox();
            suggestionList.Dock = DockStyle.Top;
            suggestionList.Height = 120;
            suggestionList.Visible = false;
            suggestionList.Click += SuggestionList_Click;
            suggestionList.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) SelectCurrent();
                if (e.KeyCode == Keys.Escape) HideSuggestions();
            };

            searchBox = new TextBox();
            searchBox.Dock = DockStyle.Top;
            searchBox.TextChanged += SearchBox_TextChanged;
            searchBox.KeyDown += SearchBox_KeyDown;
            searchBox.Enter += (s, e) =>
            {
                if (searchBox.ForeColor == Color.Gray)
                {
                    searchBox.TextChanged -= SearchBox_TextChanged;
                    searchBox.Text = "";
                    searchBox.ForeColor = Color.Black;
                    searchBox.TextChanged += SearchBox_TextChanged;
                }
            };
            searchBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    ResetSearch();
                    HideSuggestions();
                }
            };
            var lbl = new System.Windows.Forms.Label();

            ResetSearch();

            Controls.Add(tagPanel);
            Controls.Add(suggestionList);
            Controls.Add(searchBox);
        }

        private void ResetSearch()
        {
            if (searchBox == null) return;
            searchBox.TextChanged -= SearchBox_TextChanged;
            searchBox.Text = Placeholder;
            searchBox.ForeColor = Color.Gray;
            searchBox.TextChanged += SearchBox_TextChanged;
            HideSuggestions();
        }

        private void HideSuggestions()
        {
            if (suggestionList == null) return;
            suggestionList.Visible = false;
            suggestionList.Items.Clear();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.ForeColor == Color.Gray) return;

            string filter = searchBox.Text.Trim();

            if (string.IsNullOrEmpty(filter))
            {
                HideSuggestions();
                return;
            }

            var matches = _map
                .Where(kv => !_selected.ContainsKey(kv.Key) &&
                             kv.Value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (matches.Count == 0)
            {
                HideSuggestions();
                return;
            }

            suggestionList.Items.Clear();
            foreach (var kv in matches)
                suggestionList.Items.Add(kv);

            suggestionList.DisplayMember = "Value";
            suggestionList.Visible = true;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!suggestionList.Visible) return;

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
            }
        }

        private void SuggestionList_Click(object sender, EventArgs e) => SelectCurrent();

        private void SelectCurrent()
        {
            if (suggestionList.SelectedItem == null) return;

            var kv = (KeyValuePair<int, string>)suggestionList.SelectedItem;
            AddTag(kv.Key, kv.Value);
            ResetSearch();
            searchBox.Focus();
        }

        private void AddTag(int id, string name)
        {
            if (_selected.ContainsKey(id)) return;
            _selected[id] = name;

            var tag = new Panel();
            tag.AutoSize = true;
            tag.BackColor = Color.FromArgb(220, 235, 255);
            tag.Margin = new Padding(2);
            tag.Padding = new Padding(4, 2, 4, 2);
            tag.Tag = id;

            var lbl = new Label();
            lbl.Text = name;
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Left;
            lbl.TextAlign = ContentAlignment.MiddleLeft;

            var btn = new Button();
            btn.Text = "×";
            btn.Width = 18;
            btn.Height = 18;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Dock = DockStyle.Right;
            btn.Cursor = Cursors.Hand;
            btn.Tag = id;
            btn.Click += (s, e) =>
            {
                int removeId = (int)((Button)s).Tag;
                _selected.Remove(removeId);
                tagPanel.Controls.Remove(tag);
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            };

            tag.Controls.Add(lbl);
            tag.Controls.Add(btn);
            tagPanel.Controls.Add(tag);

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void LoadFromMap(Dictionary<int, string> map)
        {
            _map = map;
            _selected.Clear();
            tagPanel.Controls.Clear();
            ResetSearch();
        }

        public List<int> GetSelectedIds() => _selected.Keys.ToList();
        public List<string> GetSelectedNames() => _selected.Values.ToList();
    }
}