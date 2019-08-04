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
                p.StartInfo.WorkingDirectory = Directory; // "C:\\Users\\elsdonl0213\\Repos\\DynamoExamples\\Latest";
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

        // """Review changes in the latest commit. Prepare a list of changed Dynamo and Grasshopper graphs."""
        public List<string> Review()
        {

            // Get all files changed in the last commit
            string[] files = ReadProcess("git", "diff --name-only HEAD HEAD~1");

            Debug.WriteLine("Files changed in last commit: " + files);
            // # files = ['example-1.3.4.dyn', 'example-2.0.2.dyn', 'example.gh', 'foo.json', 'bar.md']

            List<string> changed = new List<string>();

            // # Check if any changed files are Dynamo or Grasshopper files
            foreach (string file in files)
            {
                string pattern = @"\\.(dyn|gh|ghx)$";
                Match m = Regex.Match(file, pattern);
                //while (m.Success)
                //{
                //    Debug.WriteLine("'{0}' found at position {1}", m.Value, m.Index);
                //    m = m.NextMatch();
                //    changed.Append(file);
                //}

                if (m.Success)
                {
                    changed.Add(file);
                }
            }

            Debug.WriteLine("Graphs changed in last commit:", string.Join("\n", changed));
            return changed;
        }



        // """Review changes in the latest commit. Prepare a list of changed Dynamo and Grasshopper graphs."""
        public void Checkout(string state)
        {
            //# Copy the latest commit
            //            copyfile(file, latest)

            //    # Checkout the previous commit
            //        subprocess.call(['git', 'checkout', 'HEAD~1', file])

            //# Copy the previous commit
            //    copyfile(file, previous)

            //    # Reset git 
            //        subprocess.call(['git', 'reset', 'HEAD', file])
            //    subprocess.call(['git', 'checkout', '--', file])

            //    # Compute differences
            //# subprocess.call([p('VVD-build', 'diffgh.cmd'), previous, latest, previous + '.diff'])
            //        diff(file, previous, latest)

            ReadProcess("git", "checkout " + state);

            // File is checked out 
            // Read it now...
            //ReadProcess("git", "checkout reset HEAD file");
            //ReadProcess("git", "checkout -- file");
        }

        public Dictionary<string, string> Log()
        {
            // https://git-scm.com/docs/git-log#_pretty_formats
            //string[] commitStrings = ReadProcess("git", "log --format='%h %s' -- " + file);

            //Dictionary<string, string> commits = new Dictionary<string, string>();

            //// @todo There's probably a better way to do this
            //foreach (string commit in commitStrings)
            //{
            //    string[] parts = commit.Split(new Char[] { '\n' });
            //    commits.Add(parts[0], parts[1]);
            //}


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
