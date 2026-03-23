using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Legal_system.Data_entry
{
    public partial class TimelineEntry : Form
    {
        private LegislationPicker legislationPicker;

        public TimelineEntry()
        {
            InitializeComponent();

            legislationPicker = new LegislationPicker();
            legislationPicker.Location = new Point(350, 60);
            legislationPicker.Width = 200;
            legislationPicker.SelectionChanged += (s, e) =>
            {
                var names = legislationPicker.GetSelectedNames();
                textBox2.Text = names.Count == 0 ? "(none selected)" : string.Join(", ", names);
            };

            legislationPicker.LoadFromMap(new Dictionary<int, string>()
            {
                { 1, "Health & Safety Act" },
                { 2, "Employment Rights Act" },
                { 3, "Equality Act" },
                { 4, "Data Protection Act 2" },
                { 5, "Data Protection Act 3" },
                { 6, "Data Protection Act 4" },
                { 7, "Data Protection Act 5" },
                { 8, "Data Protection Act 6" }
            });

            tabPage1.Controls.Add(legislationPicker);
        }

        private void tabPage1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        //label2
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //delete last entry but nothins here so add it
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