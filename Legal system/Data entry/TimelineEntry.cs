using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Legal_system.Data_entry
{
    public partial class TimelineEntry : Form
    {
        private List<List<int>> respondentLegislation = new List<List<int>>();

        private Dictionary<int, string> legislationMap = new Dictionary<int, string>()
        {
            { 1, "Health & Safety Act" },
            { 2, "Employment Rights Act" },
            { 3, "Equality Act" },
            { 4, "Data Protection Act 2" },
            { 5, "Data Protection Act 3" },
            { 6, "Data Protection Act 4" },
            { 7, "Data Protection Act 5" },
            { 8, "Data Protection Act 6" }
        };

        public TimelineEntry()
        {
            InitializeComponent();

            // Add Respondent button placed below the TabControl
            var addRespondentBtn = new Button();
            addRespondentBtn.Text = "+ Add Respondent";
            addRespondentBtn.AutoSize = true;
            addRespondentBtn.Location = new Point(tabControl1.Left, tabControl1.Bottom + 8);
            addRespondentBtn.Click += AddRespondentBtn_Click;
            this.Controls.Add(addRespondentBtn);
            addRespondentBtn.BringToFront();

            // Set up tab 1 — designer controls already exist, just wire them up
            respondentLegislation.Add(new List<int>());
            AttachPickerToTab(tabPage1, 0);

            // Simulate clicking Add Respondent on load to create Respondent 1 tab
            AddRespondentBtn_Click(null, EventArgs.Empty);

            // Remove the designer base tab
            tabControl1.TabPages.Remove(tabPage1);
        }

        private void AttachPickerToTab(TabPage page, int respondentIndex)
        {
            if (respondentIndex > 0)
            {
                // ── New tabs: recreate all designer controls manually ──

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

                // Enter Respondent dropdown (mirrors comboBox2)
                var respondentDropdown = new ComboBox();
                respondentDropdown.Location = new Point(10, 35);
                respondentDropdown.Width = 180;
                respondentDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
                // Populate with same items as comboBox2
                foreach (var item in comboBox2.Items)
                    respondentDropdown.Items.Add(item);
                page.Controls.Add(respondentDropdown);

                // Legislation list label (mirrors label2)
                var listLabel = new Label();
                listLabel.Text = "Legislation list:";
                listLabel.Location = new Point(10, 70);
                listLabel.AutoSize = true;
                page.Controls.Add(listLabel);

                // Legislation search label (mirrors label3)
                var searchLabel = new Label();
                searchLabel.Text = "Legislation search:";
                searchLabel.Location = new Point(355, 40);
                searchLabel.AutoSize = true;
                page.Controls.Add(searchLabel);

                // Multiline readonly TextBox to display selected legislation names (mirrors textBox2)
                var legDisplay = new TextBox();
                legDisplay.Location = new Point(10, 90);
                legDisplay.Size = new Size(310, 120);
                legDisplay.Multiline = true;
                legDisplay.ReadOnly = true;
                legDisplay.ScrollBars = ScrollBars.Vertical;
                legDisplay.BackColor = SystemColors.Window;
                page.Controls.Add(legDisplay);

                // Remove last legislation button (mirrors button2)
                var removeBtn = new Button();
                removeBtn.Text = "Remove last legislation";
                removeBtn.Location = new Point(355, 175);
                removeBtn.AutoSize = true;
                removeBtn.Click += (s, e) =>
                {
                    while (respondentLegislation.Count <= respondentIndex)
                        respondentLegislation.Add(new List<int>());

                    if (respondentLegislation[respondentIndex].Count > 0)
                    {
                        respondentLegislation[respondentIndex].RemoveAt(respondentLegislation[respondentIndex].Count - 1);
                        legDisplay.Text = string.Join(Environment.NewLine,
                            respondentLegislation[respondentIndex].Select(id => legislationMap[id]));
                    }
                };
                page.Controls.Add(removeBtn);

                // Legislation picker
                var picker = new LegislationPicker();
                picker.Location = new Point(350, 60);
                picker.Width = 400;
                picker.LoadFromMap(legislationMap);

                // Capture the index at creation time — do NOT use TabPages.IndexOf(page) inside the lambda
                int capturedIndex = respondentIndex;

                picker.SelectionChanged += (s, e) =>
                {
                    // Grow the list if needed (safety net)
                    while (respondentLegislation.Count <= capturedIndex)
                        respondentLegislation.Add(new List<int>());

                    respondentLegislation[capturedIndex] = picker.GetSelectedIds();
                    legDisplay.Text = string.Join(Environment.NewLine, picker.GetSelectedNames());
                };
                page.Controls.Add(picker);
            }
            else
            {
                // ── Tab 1: designer controls already exist, just wire them up ──

                button2.Click += (s, e) =>
                {
                    if (respondentLegislation[0].Count > 0)
                    {
                        respondentLegislation[0].RemoveAt(respondentLegislation[0].Count - 1);
                        textBox2.Text = string.Join(Environment.NewLine,
                            respondentLegislation[0].Select(id => legislationMap[id]));
                    }
                };

                var picker = new LegislationPicker();
                picker.Location = new Point(350, 60);
                picker.Width = 400;
                picker.LoadFromMap(legislationMap);
                picker.SelectionChanged += (s, e) =>
                {
                    while (respondentLegislation.Count <= respondentIndex)
                        respondentLegislation.Add(new List<int>());

                    respondentLegislation[respondentIndex] = picker.GetSelectedIds();
                    textBox2.Text = string.Join(Environment.NewLine, picker.GetSelectedNames()); // textBox2 not legDisplay
                };
                tabPage1.Controls.Add(picker);
            }
        }

        private void AddRespondentBtn_Click(object sender, EventArgs e)
        {
            int newIndex = tabControl1.TabPages.Count;
            var newPage = new TabPage($"Respondent {newIndex + 1}");
            respondentLegislation.Add(new List<int>());
            tabControl1.TabPages.Add(newPage);
            AttachPickerToTab(newPage, newIndex);
            tabControl1.SelectedTab = newPage;
        }

        private void RenumberTabs()
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
                tabControl1.TabPages[i].Text = $"Respondent {i + 1}";
        }

        // ── All original stub handlers preserved to keep Designer happy ──
        private void tabPage1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e)
        {
            // delete last entry but nothing here so add it
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
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