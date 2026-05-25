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
                x.EvidenceID,
                EvidencePoint = helper.GetEvidencePointById(x.EvidenceID),
                Respondents = x.RespondentsDisplay,
                Legislation = x.LegislationDisplay
            }).ToList();

            TimelineGrid.DataSource = rows;

            foreach (DataGridViewColumn col in TimelineGrid.Columns)
            {
                Console.WriteLine(col.Name);
            }

            // Make EvidencePoint column wider
            //TimelineGrid.Columns["EvidencePoint"].Width = 400;
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
