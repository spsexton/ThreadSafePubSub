using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ThreadSafePubSubUI
{
    public partial class Form3 : Form
    {
        private enum AppStates { Idle, ReadingFile };

        private BackgroundWorker _worker;

        public Form3()
        {
            InitializeComponent();

            // Set up initial state
            SetAppState(AppStates.Idle, null);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
	    ofd.Filter = "Text Files (*.txt)|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                SetAppState(AppStates.ReadingFile, fi.Name);

                try
                {
                    // Set up background worker object & hook up handlers
                    _worker = new BackgroundWorker();
                    _worker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
                    _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
                    _worker.WorkerReportsProgress = true;
                    _worker.WorkerSupportsCancellation = true;
                    _worker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);

                    // Launch background thread to do the work of reading the file.  This will
                    // trigger BackgroundWorker.DoWork().  Note that we pass the filename to 
                    // process as a parameter.  
                    _worker.RunWorkerAsync(ofd.FileName);
                }
                catch
                {
                    SetAppState(AppStates.Idle, null);
                    throw;
                }
            }
        }

        // Get info on progress of file-read operation (% complete)
        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Just update progress bar with % complete
            pbProgress.Value = e.ProgressPercentage;
        }

        private void btnTellJoke_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Did you hear about the cross-eyed teacher who always has trouble with her pupils?", "Bad joke");
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            lblResults.Text = "";
        }

        // Do work--runs on a background thread
        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Note about exceptions:  If an exception originates anywhere in 
            // this method, or methods that it calls, the BackgroundWorker will
            // automatically populate the Error property of the RunWorkerCompletedEventArgs
            // parameter that gets passed into the RunWorkerCompleted event handler.
            // So we can handle the exception in that method.

            FileReader3 fr = new FileReader3();

            // Filename to process was passed to RunWorkerAsync(), so it's available
            // here in DoWorkEventArgs object.
            BackgroundWorker bw = sender as BackgroundWorker;
            string sFileToRead = (string)e.Argument;

            e.Result = fr.ReadTheFile(bw, sFileToRead);

            // If operation was cancelled (triggered by CancellationPending), 
            // we bailed out of ReadTheFile() early.  But still need to set
            // Cancel flag, because RunWorkerCompleted event will still fire.
            if (bw.CancellationPending)
                e.Cancel = true;
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, "Error During File Read");
                }
                else if (e.Cancelled)
                {
                    lblResults.Text = "** Cancelled **";
                }
                else
                {
                    int numLines = (int)e.Result;
                    lblResults.Text = string.Format("We read {0} lines", numLines.ToString());
                }
            }
            finally
            {
                // State now goes back to idle
                SetAppState(AppStates.Idle, null);
            }
        }

        // Set new application state, handling button sensitivity, labels, etc.
        private void SetAppState(AppStates newState, string filename)
        {
            switch (newState)
            {
                case AppStates.Idle:
                    // Hide progress widgets
                    SetFileReadWidgetsVisible(false);
                    btnSelect.Enabled = true;
                    break;

                case AppStates.ReadingFile:
                    // Display progress widgets & file info
                    SetFileReadWidgetsVisible(true);
                    lblProgress.Text = string.Format("Reading file: {0}", filename);
                    pbProgress.Value = 0;
                    lblResults.Text = "";
                    btnSelect.Enabled = false;
                    break;
            }
        }

        private void SetFileReadWidgetsVisible(bool visible)
        {
            lblProgress.Visible = visible;
            pbProgress.Visible = visible;
            btnCancel.Visible = visible;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _worker.CancelAsync();
        }
    }
}
