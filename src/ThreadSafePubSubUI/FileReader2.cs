using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadSafePubSubUI
{
    public class FileReader2
    {
        /// <summary>
        /// Read specified text file & return # lines
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int ReadTheFile(string fileName)
        {
            int numLines = 0;

            using (StreamReader sr = new StreamReader(fileName))
            {
                string nextLine;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    numLines++;
                }
            }

            Thread.Sleep(3000);     // Simulate lengthy operation

            return numLines;
        }
    }
}
