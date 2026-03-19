using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Legal_system.Data_entry
{
    public partial class EvidenceEntry : Form
    {
        public EvidenceEntry()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var helper = new DatabaseHelper("legal.db");

            helper.AddEvidence
                (textBox1.Text, comboBox1.SelectedIndex, trackBar1.Value, textBox2.Text, textBox3.Text);
            new EvidenceEntry().Show();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
