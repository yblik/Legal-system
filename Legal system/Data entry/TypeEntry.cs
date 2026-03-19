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
    public partial class TypeEntry : Form
    {
        public TypeEntry()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var helper = new DatabaseHelper("legal.db");

            helper.AddEvidenceType(textBox1.Text);            
            new TypeEntry().Show();
            this.Close();
        }
    }
}
