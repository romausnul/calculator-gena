///////////////////////////////////////////////////////////////////////////////
//
//     http://www.interestprograms.ru - программы, игры и их исходные коды
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Calc2
{
    public partial class SetDlg : Form
    {
        public SetDlg()
        {
            InitializeComponent();
        }

        public string Precision = null;
        public string SeparatorDecimalBuffer = null;
        public double FormOpacity = 1;
        double temp = 10.0;
        
        private void SetDlg_Load(object sender, EventArgs e)
        {
            string subPrecition = Precision.Substring(1, Precision.Length - 1);
            numericUpDown1.Value = int.Parse(subPrecition);
            label2.Text = temp.ToString(Precision);

            numericUpDown2.Value = (decimal)(FormOpacity * 100.0);

            textBox1.Text = SeparatorDecimalBuffer;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Precision = "F" + numericUpDown1.Value.ToString();
            label2.Text = temp.ToString(Precision);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            FormOpacity = (double)numericUpDown2.Value / 100.0;
        }
        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SeparatorDecimalBuffer = textBox1.Text;
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }
    }
}
