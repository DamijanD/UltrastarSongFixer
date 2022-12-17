namespace UltrastarSongFixer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            var folders = System.IO.Directory.EnumerateDirectories(tbPath.Text);

            progressBar1.Value = 0;
            progressBar1.Maximum = folders.Count();
            progressBar1.Step = 1;

            Application.DoEvents();

            foreach (var folder in folders)
            {
                var files = System.IO.Directory.EnumerateFiles(folder);//.Select(x => System.IO.Path.GetFileName(x));

                try
                {
                    var mainFile = files.SingleOrDefault(x => x.EndsWith(".txt"));

                    if (mainFile != null)
                    {
                        tbOutput.Text += mainFile + Environment.NewLine;

                        var mainLines = System.IO.File.ReadAllLines(mainFile);

                        /*
                         * #MP3:Adele - When we were young.mp3
#COVER:Adele - When we were young.jpg
#VIDEO:Adele - When we were young.mp4
                        E
                        */

                        var fixedLines = new List<string>();
                        bool changed = false;

                        foreach (var line in mainLines)
                        {
                            if (line.StartsWith("#MP3:"))
                            {
                                changed |= CheckAndChangeFile(files, fixedLines, line, "#MP3:");
                            }
                            else if (line.StartsWith("#COVER:"))
                            {
                                changed |= CheckAndChangeFile(files, fixedLines, line, "#COVER:");
                            }
                            else if (line.StartsWith("#VIDEO:"))
                            {
                                changed |= CheckAndChangeFile(files, fixedLines, line, "#VIDEO:");
                            }
                            else if (line.StartsWith("E")) //end
                            {
                                fixedLines.Add(line);
                                break;
                            }
                            else if (!string.IsNullOrWhiteSpace(line))
                            {
                                fixedLines.Add(line);
                            }
                        }

                        if (changed || fixedLines.Count != mainLines.Length)
                        {
                            tbOutput.Text += "Writing fixed file" + Environment.NewLine;
                            System.IO.File.Move(mainFile, mainFile + ".bak");
                            System.IO.File.WriteAllLines(mainFile, fixedLines);
                        }
                    }
                    else
                    {
                        tbOutput.Text += folder + " is empty!";
                    }
                }
                catch ( Exception exc)
                {
                    tbOutput.Text += folder + " error " + exc.Message;
                }

                tbOutput.Select(tbOutput.Text.Length - 1, 1);
                tbOutput.ScrollToCaret();
                
                progressBar1.PerformStep();
                Application.DoEvents();
            }
        }

        private bool CheckAndChangeFile(IEnumerable<string> files, List<string> fixedLines, string line, string prefix)
        {
            var contentfile = line.Substring(prefix.Length).ToLower();

            var extension = System.IO.Path.GetExtension(contentfile);

            if (files.Any(x => x.ToLower().Contains(contentfile)))
            {
                fixedLines.Add(line);
            }
            else
            {
                var foundcontentFile = files.FirstOrDefault(x => x.EndsWith(extension));
                if (foundcontentFile != null)
                {
                    var filenameOnly = System.IO.Path.GetFileName(foundcontentFile);
                    fixedLines.Add(prefix + filenameOnly);
                    tbOutput.Text += "changed "+ contentfile + " to " + filenameOnly + Environment.NewLine;
                    return true;
                }
                else
                {
                    tbOutput.Text += extension + " not found: " + contentfile + Environment.NewLine;
                    return true;
                }
            }

            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = ofd.SelectedPath;
            }
        }
    }
}