using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTP
{
    public partial class Form1 : Form
    {
        private Jadro _jadro;
        public Form1()
        {
            InitializeComponent();
            _jadro = new Jadro();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = _jadro.StringToHex(textBox3.Text);
            _jadro.PoctajSifru(textBox1.Text);
        }
    }
}
