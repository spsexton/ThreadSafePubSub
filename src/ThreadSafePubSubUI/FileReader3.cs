using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadSafePubSubUI
{
    public class FileReader3
    {
        /// <summary>
        /// Read specified text file & return # lines
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int ReadTheFile(BackgroundWorker bw, string fileName)
        {
            int numLines = 0;
            FileInfo fi = new FileInfo(fileName);
            long totalBytes = fi.Length;
	    Debug.WriteLine(string.Format("Total bytes = {0}", totalBytes));
            long bytesRead = 0;

            using (StreamReader sr = new StreamReader(fileName))
            {
                // Note: When BackgroundWorker has CancellationPending set, we bail
                // out and fall back to the _DoWork method that called us.
                string nextLine;
                while (((nextLine = sr.ReadLine()) != null) &&
                       !bw.CancellationPending)
                {
		    // # bytes read includes current string + 2 bytes at end of line
		    //   This logic does NOT work, however, for final line of file if it does not include CR/LF
                    bytesRead += sr.CurrentEncoding.GetByteCount(nextLine) + 2;
                    numLines++;
                    int pctComplete = (int)(((double)bytesRead / (double)totalBytes)* 100);
		    if (pctComplete > 100)
		    {
			Debug.WriteLine(string.Format("** PCT >100.  bytesRead={0}, totalBytes={1}, pctComplete={2}", bytesRead, totalBytes, pctComplete));
			pctComplete = 100;
		    }
                    bw.ReportProgress(pctComplete);
                    Thread.Sleep(10);  // ms
                }
            }

            return numLines;
        }
    }
}
