using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ThreadSafePubSubUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lblResults.Text = " ... reading the file ...";
                FileReader1 fr = new FileReader1();
                int numLines = fr.ReadTheFile(ofd.FileName);

                lblResults.Text = string.Format("We read {0} lines", numLines.ToString());
            }
        }

        private void btnTellJoke_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Did you hear about the cross-eyed teacher who always has trouble with her pupils?", "Bad joke");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblResults.Text = "";
        }
    }
}
