using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Legal_system.Data_entry;

namespace Legal_system
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new TimelineEntry().Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            new EvidenceEntry().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new LegislationEntry().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new RespondentEntry
                ().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new TypeEntry().Show();
        }
    }
}
