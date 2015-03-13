using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ThreadSafePubSubUI
{
    public partial class Form2 : Form
    {
        public Form2()
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

                // Set up background worker object & hook up handlers
                BackgroundWorker bgWorker;
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

                // Launch background thread to do the work of reading the file.  This will
                // trigger BackgroundWorker.DoWork().  Note that we pass the filename to 
                // process as a parameter.  
                bgWorker.RunWorkerAsync(ofd.FileName);
            }
        }

        private void btnTellJoke_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Did you hear about the cross-eyed teacher who always has trouble with her pupils?", "Bad joke");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            lblResults.Text = "";
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileReader2 fr = new FileReader2();

            // Filename to process was passed to RunWorkerAsync(), so it's available
            // here in DoWorkEventArgs object.
            string sFileToRead = (string)e.Argument;
            e.Result = fr.ReadTheFile(sFileToRead);
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                int numLines = (int)e.Result;
                lblResults.Text = string.Format("We read {0} lines", numLines.ToString());
            }
        }
    }
}
