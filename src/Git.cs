using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Track
{

    public class Git
    {
        string FilePath;
        string FileName;
        string Directory;


        public Git(string filePath) {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Directory = Path.GetDirectoryName(filePath);
        }

        public string[] ReadProcess(string command, string arguments = "")
        {

            string output;
            using (Process p = new Process())
            {
                p.StartInfo.WorkingDirectory = Directory;
                p.StartInfo.FileName = command;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                output = p.StandardOutput.ReadToEnd();

                p.WaitForExit();
            }

            Debug.WriteLine(output);
            //System.Windows.MessageBox.Show(output);

            // Split into lines
            string[] lines = output.Split(new Char[] { '\n' });

            if (lines.Last() == "")
            {
                Array.Resize(ref lines, lines.Length - 1);
            }

            return lines;
        }

        public void Checkout(string state, bool appendFile = false)
        {
            string str = state;

            if (appendFile)
            {
                str += " -- " + FileName;
            }

            // Updates files in the working tree to match the version in the index or the specified tree. 
            // https://git-scm.com/docs/git-checkout
            ReadProcess("git", "checkout " + str);
        }

        public void Reset(string state, bool appendFile = false)
        {
            string str = state;

            if (appendFile)
            {
                str += " " + FileName;
            }
            
            // Git reset to previous state
            // https://git-scm.com/docs/git-reset
            ReadProcess("git", "reset " + str);
        }

        public Dictionary<string, string> Log()
        {
            // Get the commit hashes then the commit subjects that apply to the file
            // https://git-scm.com/docs/git-log#_pretty_formats
            string[] hashes = ReadProcess("git", "log --format=%h -- " + FileName);
            string[] subjects = ReadProcess("git", "log --format=%s -- " + FileName);

            // Create a dictionary from the commit hashes and hash subjects
            Dictionary<string, string> commits = hashes.Zip(subjects, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.k + " " + x.v);

            return commits;
        }

    }
}
