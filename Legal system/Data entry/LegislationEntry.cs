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
    public partial class LegislationEntry : Form
    {
        public LegislationEntry()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var helper = new DatabaseHelper("legal.db");

            helper.AddLegislation(textBox1.Text, textBox2.Text); //+ meaning --> tool tip
            new LegislationEntry().Show();
            this.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
