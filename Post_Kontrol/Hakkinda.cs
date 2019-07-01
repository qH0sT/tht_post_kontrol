using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Post_Kontrol
{
    public partial class Hakkinda : Form
    {
        public Hakkinda()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(label3.Text);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(label3.Text);
        }
    }
}
