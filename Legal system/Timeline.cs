using Legal_system.Data_entry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Legal_system
{
    public partial class Timeline : Form
    {
        public List<TimelineData> TD;
        public Timeline()
        {
            InitializeComponent();
            LoadTimeline();
             DisplayTimeline();


        }
        public void LoadTimeline()
        {
            var helper = new DatabaseHelper("legal.db");

            TD = helper.GetTimelineData();
        }
        public void DisplayTimeline()
        {
            if (TD == null || TD.Count == 0)
            {
                MessageBox.Show("No timeline data loaded.");
                return;
            }

            var helper = new DatabaseHelper("legal.db");

            var rows = TD.Select(x =>
            {
                string ep = helper.GetEvidencePointById(x.Evidence);

                // Split respondents by comma
                var respondentGroups = x.RespondentsDisplay
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Split legislation groups by | (each respondent’s set)
                var legislationGroups = x.LegislationDisplay
                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                string respondentsBlock = "";

                for (int i = 0; i < respondentGroups.Length; i++)
                {
                    string resp = respondentGroups[i].Trim();
                    respondentsBlock += $"• Respondent: {resp}\n";

                    // Each respondent’s legislation list
                    if (i < legislationGroups.Length)
                    {
                        var laws = legislationGroups[i]
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var law in laws)
                        {
                            respondentsBlock += $"    - {law.Trim()}\n";
                        }
                    }

                    respondentsBlock += "\n";
                }

                string timelineText =
                    $"Year: {x.Year}\n" +
                    //$"Evidence: {x.Evidence}\n" +
                    $"Evidence Point: {ep}\n\n" +
                    $"{respondentsBlock}";

                return new
                {
                    Timeline = timelineText.Trim()
                };
            }).ToList();

            TimelineGrid.DataSource = rows;

            var col = TimelineGrid.Columns["Timeline"];
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            col.Width = TimelineGrid.Width - 40;

            TimelineGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            TimelineGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            TimelineGrid.RowHeadersVisible = false;
            TimelineGrid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            col.SortMode = DataGridViewColumnSortMode.NotSortable;

            // Disable highlighting
            TimelineGrid.ReadOnly = true;
            TimelineGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            TimelineGrid.DefaultCellStyle.SelectionBackColor = TimelineGrid.DefaultCellStyle.BackColor;
            TimelineGrid.DefaultCellStyle.SelectionForeColor = TimelineGrid.DefaultCellStyle.ForeColor;
            TimelineGrid.ClearSelection();
        }




        //var filtered = TD.Where(x => x.Year == "2020").ToList();
        //TimelineGrid.DataSource = filtered;
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void TimelineTxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void TimelineGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
