using Legal_system.Data_entry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Legal_system
{
    public partial class Timeline : Form
    {
        public List<TimelineData> TD;

        private DatabaseHelper db;

        private Dictionary<int, string> legislationMap;
        private Dictionary<int, string> respondentMap;

        private List<TimelineFilter> activeFilters =
            new List<TimelineFilter>();

        private FlowLayoutPanel filterPanel;

        public Timeline()
        {
            InitializeComponent();

            db = new DatabaseHelper("legal.db");

            LoadFilterData();

            SetupFilterControls();

            LoadTimeline();

            DisplayTimeline();
        }

        // ============================================================
        // LOAD FILTER DATA
        // ============================================================

        private void LoadFilterData()
        {
            legislationMap = db.GetLegislation();
            respondentMap = db.GetRespondents();
        }

        // ============================================================
        // SETUP FILTER CONTROLS
        // ============================================================

        private void SetupFilterControls()
        {
            // comboBox1 = your existing filter type dropdown
            comboBox1.Items.Clear();

            comboBox1.Items.Add("Legislation");
            comboBox1.Items.Add("Respondent");

            comboBox1.DropDownStyle =
                ComboBoxStyle.DropDownList;

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            // --------------------------------------------------------
            // button1 = your existing Add button
            // --------------------------------------------------------

            button1.Click -= button1_Click;
            button1.Click -= AddFilter_Click;
            button1.Click += AddFilter_Click;

            // --------------------------------------------------------
            // IMPORTANT:
            //
            // Do NOT look for a tab named "Filter".
            //
            // comboBox1 is already on the correct tab, so its Parent
            // is the correct container.
            // --------------------------------------------------------

            Control parent = comboBox1.Parent;

            if (parent == null)
                return;

            // --------------------------------------------------------
            // Create dynamic filter panel
            // --------------------------------------------------------

            filterPanel = new FlowLayoutPanel();

            filterPanel.Name =
                "DynamicFilterPanel";

            filterPanel.FlowDirection =
                FlowDirection.TopDown;

            filterPanel.WrapContents =
                false;

            filterPanel.AutoScroll =
                true;

            filterPanel.Location =
                new Point(
                    comboBox1.Left,
                    Math.Max(
                        comboBox1.Bottom,
                        button1.Bottom) + 10);

            filterPanel.Size =
                new Size(
                    Math.Max(
                        450,
                        parent.ClientSize.Width -
                        filterPanel.Left -
                        20),

                    Math.Max(
                        100,
                        parent.ClientSize.Height -
                        filterPanel.Top -
                        20));

            filterPanel.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            parent.Controls.Add(filterPanel);

            filterPanel.BringToFront();
        }

        // ============================================================
        // ADD FILTER BUTTON
        // ============================================================

        private void AddFilter_Click(
            object sender,
            EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
                return;

            string filterType =
                comboBox1.SelectedItem.ToString();

            AddFilter(filterType);
        }

        // ============================================================
        // ADD FILTER
        // ============================================================

        private void AddFilter(
            string filterType)
        {
            if (filterPanel == null)
                return;

            TimelineFilter filter =
                new TimelineFilter();

            filter.Type = filterType;
            filter.ValueId = -1;
            filter.ValueName = "";

            // --------------------------------------------------------
            // ROW
            // --------------------------------------------------------

            Panel row =
                new Panel();

            row.Height = 36;

            row.Width =
                Math.Max(
                    500,
                    filterPanel.ClientSize.Width - 30);

            row.Margin =
                new Padding(3);

            row.Tag = filter;

            // --------------------------------------------------------
            // TYPE LABEL
            // --------------------------------------------------------

            Label typeLabel =
                new Label();

            typeLabel.Text =
                filterType + ":";

            typeLabel.AutoSize =
                true;

            typeLabel.Location =
                new Point(5, 9);

            row.Controls.Add(typeLabel);

            // --------------------------------------------------------
            // VALUE COMBOBOX
            // --------------------------------------------------------

            ComboBox valueCombo =
                new ComboBox();

            valueCombo.DropDownStyle =
                ComboBoxStyle.DropDownList;

            valueCombo.Width =
                280;

            valueCombo.Location =
                new Point(110, 4);

            valueCombo.DisplayMember =
                "Name";

            // --------------------------------------------------------
            // LEGISLATION
            // --------------------------------------------------------

            if (filterType == "Legislation")
            {
                foreach (
                    KeyValuePair<int, string> item
                    in legislationMap.OrderBy(x => x.Value))
                {
                    valueCombo.Items.Add(
                        new FilterValue
                        {
                            Id = item.Key,
                            Name = item.Value
                        });
                }
            }

            // --------------------------------------------------------
            // RESPONDENT
            // --------------------------------------------------------

            if (filterType == "Respondent")
            {
                foreach (
                    KeyValuePair<int, string> item
                    in respondentMap.OrderBy(x => x.Value))
                {
                    valueCombo.Items.Add(
                        new FilterValue
                        {
                            Id = item.Key,
                            Name = item.Value
                        });
                }
            }

            row.Controls.Add(valueCombo);

            // --------------------------------------------------------
            // REMOVE BUTTON
            // --------------------------------------------------------

            Button removeButton =
                new Button();

            removeButton.Text =
                "Remove";

            removeButton.Width =
                75;

            removeButton.Height =
                25;

            removeButton.Location =
                new Point(405, 3);

            removeButton.Tag =
                row;

            removeButton.Click +=
                RemoveFilter_Click;

            row.Controls.Add(removeButton);

            // --------------------------------------------------------
            // VALUE CHANGED
            // --------------------------------------------------------

            valueCombo.SelectedIndexChanged +=
                delegate
                {
                    FilterValue selected =
                        valueCombo.SelectedItem as FilterValue;

                    if (selected == null)
                    {
                        filter.ValueId = -1;
                        filter.ValueName = "";
                    }
                    else
                    {
                        filter.ValueId =
                            selected.Id;

                        filter.ValueName =
                            selected.Name;
                    }

                    ApplyFilters();
                };

            // --------------------------------------------------------
            // ADD ROW TO PANEL
            // --------------------------------------------------------

            filterPanel.Controls.Add(row);

            activeFilters.Add(filter);

            // --------------------------------------------------------
            // Automatically choose first value
            // --------------------------------------------------------

            if (valueCombo.Items.Count > 0)
            {
                valueCombo.SelectedIndex = 0;
            }
        }

        // ============================================================
        // REMOVE FILTER
        // ============================================================

        private void RemoveFilter_Click(
            object sender,
            EventArgs e)
        {
            Button button =
                sender as Button;

            if (button == null)
                return;

            Panel row =
                button.Tag as Panel;

            if (row == null)
                return;

            TimelineFilter filter =
                row.Tag as TimelineFilter;

            if (filter != null)
            {
                activeFilters.Remove(filter);
            }

            filterPanel.Controls.Remove(row);

            row.Dispose();

            ApplyFilters();
        }

        // ============================================================
        // LOAD TIMELINE
        // ============================================================

        public void LoadTimeline()
        {
            TD =
                db.GetTimelineData();

            if (TD == null)
            {
                TD =
                    new List<TimelineData>();
            }

            // DEFAULT ORDER:
            // Year ascending

            TD =
                TD
                    .OrderBy(x => x.Year)
                    .ToList();
        }

        // ============================================================
        // APPLY FILTERS
        // ============================================================

        private void ApplyFilters()
        {
            if (TD == null)
                return;

            IEnumerable<TimelineData> filtered =
                TD;

            foreach (
                TimelineFilter filter
                in activeFilters)
            {
                if (filter == null)
                    continue;

                if (filter.ValueId < 0)
                    continue;

                if (string.IsNullOrWhiteSpace(
                    filter.ValueName))
                {
                    continue;
                }

                string wanted =
                    filter.ValueName.Trim();

                // ----------------------------------------------------
                // RESPONDENT FILTER
                // ----------------------------------------------------

                if (filter.Type == "Respondent")
                {
                    filtered =
                        filtered.Where(
                            x =>
                                ContainsRespondent(
                                    x,
                                    wanted));
                }

                // ----------------------------------------------------
                // LEGISLATION FILTER
                // ----------------------------------------------------

                else if (
                    filter.Type == "Legislation")
                {
                    filtered =
                        filtered.Where(
                            x =>
                                ContainsLegislation(
                                    x,
                                    wanted));
                }
            }

            // Always year ascending

            List<TimelineData> result =
                filtered
                    .OrderBy(x => x.Year)
                    .ToList();

            DisplayTimeline(result);
        }

        // ============================================================
        // RESPONDENT MATCH
        // ============================================================

        private bool ContainsRespondent(
            TimelineData data,
            string wanted)
        {
            if (data == null)
                return false;

            if (string.IsNullOrWhiteSpace(
                data.RespondentsDisplay))
            {
                return false;
            }

            string[] respondents =
                data.RespondentsDisplay.Split(
                    new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);

            foreach (
                string respondent
                in respondents)
            {
                if (string.Equals(
                    respondent.Trim(),
                    wanted,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // LEGISLATION MATCH
        // ============================================================

        private bool ContainsLegislation(
            TimelineData data,
            string wanted)
        {
            if (data == null)
                return false;

            if (string.IsNullOrWhiteSpace(
                data.LegislationDisplay))
            {
                return false;
            }

            // Each respondent's legislation group
            // is separated by |

            string[] groups =
                data.LegislationDisplay.Split(
                    new[] { '|' },
                    StringSplitOptions.RemoveEmptyEntries);

            foreach (
                string group
                in groups)
            {
                string[] laws =
                    group.Split(
                        new[] { ',' },
                        StringSplitOptions.RemoveEmptyEntries);

                foreach (
                    string law
                    in laws)
                {
                    if (string.Equals(
                        law.Trim(),
                        wanted,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // ============================================================
        // DISPLAY TIMELINE
        // ============================================================

        public void DisplayTimeline()
        {
            DisplayTimeline(TD);
        }

        public void DisplayTimeline(
            IEnumerable<TimelineData> source)
        {
            if (source == null)
            {
                TimelineGrid.DataSource =
                    null;

                return;
            }

            DatabaseHelper helper =
                new DatabaseHelper("legal.db");

            var rows =
                source
                    .OrderBy(x => x.Year)
                    .Select(x =>
                    {
                        string ep =
                            helper.GetEvidencePointById(
                                x.Evidence);

                        // ------------------------------------------------
                        // RESPONDENTS
                        // ------------------------------------------------

                        string[] respondentGroups =
                            (x.RespondentsDisplay ?? "")
                                .Split(
                                    new[] { ',' },
                                    StringSplitOptions.RemoveEmptyEntries);

                        // ------------------------------------------------
                        // LEGISLATION
                        // ------------------------------------------------

                        string[] legislationGroups =
                            (x.LegislationDisplay ?? "")
                                .Split(
                                    new[] { '|' },
                                    StringSplitOptions.RemoveEmptyEntries);

                        string respondentsBlock =
                            "";

                        for (
                            int i = 0;
                            i < respondentGroups.Length;
                            i++)
                        {
                            string resp =
                                respondentGroups[i].Trim();

                            respondentsBlock +=
                                "• Respondent: " +
                                resp +
                                "\n";

                            if (
                                i <
                                legislationGroups.Length)
                            {
                                string[] laws =
                                    legislationGroups[i]
                                        .Split(
                                            new[] { ',' },
                                            StringSplitOptions.RemoveEmptyEntries);

                                foreach (
                                    string law
                                    in laws)
                                {
                                    respondentsBlock +=
                                        "    - " +
                                        law.Trim() +
                                        "\n";
                                }
                            }

                            respondentsBlock +=
                                "\n";
                        }

                        string timelineText =
                            "Year: " +
                            x.Year +
                            "\n" +
                            "Evidence: " +
                            x.Evidence +
                            "\n" +
                            "Evidence Point: " +
                            ep +
                            "\n\n" +
                            respondentsBlock;

                        return new
                        {
                            Timeline =
                                timelineText.Trim()
                        };
                    })
                    .ToList();

            // --------------------------------------------------------
            // GRID
            // --------------------------------------------------------

            TimelineGrid.DataSource =
                rows;

            if (
                TimelineGrid.Columns["Timeline"]
                != null)
            {
                DataGridViewColumn col =
                    TimelineGrid.Columns["Timeline"];

                col.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;

                col.Width =
                    Math.Max(
                        100,
                        TimelineGrid.Width - 40);

                col.SortMode =
                    DataGridViewColumnSortMode.NotSortable;
            }

            TimelineGrid.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            TimelineGrid.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            TimelineGrid.RowHeadersVisible =
                false;

            TimelineGrid.DefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10);

            TimelineGrid.ReadOnly =
                true;

            TimelineGrid.SelectionMode =
                DataGridViewSelectionMode.CellSelect;

            TimelineGrid.DefaultCellStyle.SelectionBackColor =
                TimelineGrid.DefaultCellStyle.BackColor;

            TimelineGrid.DefaultCellStyle.SelectionForeColor =
                TimelineGrid.DefaultCellStyle.ForeColor;

            TimelineGrid.ClearSelection();
        }

        // ============================================================
        // EXISTING DESIGNER EVENTS
        // ============================================================

        private void label1_Click(
            object sender,
            EventArgs e)
        {
        }

        // IMPORTANT:
        // Your Designer may have this wired to button1.
        // Keep the method.

        private void button1_Click(
            object sender,
            EventArgs e)
        {
            // The actual filter functionality is attached
            // dynamically in SetupFilterControls().
        }

        // IMPORTANT:
        // Your Designer expects this method.
        // Keep it even though filtering does not require
        // anything to happen here.

        private void comboBox1_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            // Filter type changed.
            //
            // We intentionally do not add a filter here.
            // The user chooses:
            //
            // Legislation / Respondent
            //
            // and then presses Add.
        }

        private void TimelineTxt_TextChanged(
            object sender,
            EventArgs e)
        {
        }

        private void TimelineGrid_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
        }
    }

    // ================================================================
    // FILTER OBJECT
    // ================================================================

    public class TimelineFilter
    {
        public string Type { get; set; }

        public int ValueId { get; set; }

        public string ValueName { get; set; }
    }

    // ================================================================
    // FILTER DROPDOWN ITEM
    // ================================================================

    public class FilterValue
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}