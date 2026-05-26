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

            // Build display rows
            var rows = TD.Select(x => new
            {
                x.Year,
                x.Evidence,
                x.EvidenceType,
                EvidencePoint = helper.GetEvidencePointById(x.Evidence),
                Respondents = x.RespondentsDisplay,
                Legislation = string.Join(", ", x.Legislation.Select(l => l + " ⓘ")),
                x.LegislationDescription,
                x.Rating
            }).ToList();

            TimelineGrid.DataSource = rows;

            // Disable autosizing for columns so manual widths work
            TimelineGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Make EvidencePoint column wider
            var colEP = TimelineGrid.Columns["EvidencePoint"];
            colEP.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colEP.Width = 400;

            // Make Respondents column wider
            var colResp = TimelineGrid.Columns["Respondents"];
            colResp.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colResp.Width = 300;

            // Make Legislation column wider
            var colLeg = TimelineGrid.Columns["Legislation"];
            colLeg.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colLeg.Width = 300;

            // Enable wrapping + auto row height
            TimelineGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            TimelineGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            foreach (DataGridViewColumn colu in TimelineGrid.Columns)
            {
                Console.WriteLine(colu.Name);
            }
            foreach (DataGridViewRow row in TimelineGrid.Rows)
            {
                string desc = row.Cells["LegislationDescription"].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(desc))
                {
                    // Apply tooltip to the Legislation column
                    row.Cells["Legislation"].ToolTipText = desc;
                }
            }

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
