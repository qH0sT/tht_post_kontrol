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
    public partial class Karsilama : Form
    {
        public Karsilama()
        {
            InitializeComponent();
        }
        int i = 4;
        private void timer1_Tick(object sender, EventArgs e)
        {
            i--;
            label1.Text += "●";
            if (i == 0) {

                Hide();
                new Form1().Show();

            }
        }
    }
}
