using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace MD5_hasher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string substr = Application.StartupPath + @"\";

        private string ComputeMD5Checksum(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                return BitConverter.ToString(md5.ComputeHash(fileData)).Replace("-", String.Empty);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (string mfile in Directory.GetFiles(Application.StartupPath, "*.*", SearchOption.AllDirectories))
            {
                string lfile = mfile;
                int n = lfile.IndexOf(substr);
                lfile = lfile.Remove(n, substr.Length);
                UpdateTextBox(lfile + ";" + ComputeMD5Checksum(mfile) + "\n");
            }
        }

        public void UpdateTextBox(string ab)
        {
            if (InvokeRequired)
                Invoke(new Action<string>((s) => richTextBox1.Text = richTextBox1.Text + s), ab);
            else
                richTextBox1.Text = richTextBox1.Text + ab;
        }

        OpenFileDialog ofd = new OpenFileDialog();
        SaveFileDialog sfd = new SaveFileDialog();
        string f1;
        bool isfileloaded = false;

        private void DeleteLine(int a_line, RichTextBox textBox)
        {
            int start_index = textBox.GetFirstCharIndexFromLine(a_line);
            int count = textBox.Lines[a_line].Length;

            if (a_line < textBox.Lines.Length - 1)
            {
                count += textBox.GetFirstCharIndexFromLine(a_line + 1) -
                    ((start_index + count - 1) + 1);
            }

            textBox.Text = textBox.Text.Remove(start_index, count);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter ad = new StreamWriter(sfd.FileName);
                ad.Write(richTextBox1.Text);
                ad.Close();
                MessageBox.Show("Done!");
            }
        }

        private void getHashesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            backgroundWorker1.RunWorkerAsync();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void browsePreviousHashFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader fr1 = new StreamReader(ofd.FileName);
                f1 = fr1.ReadToEnd();
                fr1.Close();
                isfileloaded = true;
            }
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isfileloaded == false)
            {
                MessageBox.Show("Please, browse the previous hash file to compare!");
            }
            else
            {
                richTextBox2.Text = f1;
                for (int i = 0; i < richTextBox1.Lines.Length; i++)
                {
                    for (int a = 0; a < richTextBox2.Lines.Length; a++)
                    {
                        if (richTextBox2.Lines[a] == richTextBox1.Lines[i])
                        {
                            DeleteLine(i, richTextBox1);
                            DeleteLine(a, richTextBox2);
                            a--;
                        }
                        else
                        {
                            string s = null;
                            char[] ac = richTextBox1.Lines[i].ToCharArray();
                            for (int b = 0; b < ac.Length; b++)
                            {
                                if (ac[b] == ';')
                                {
                                    break;
                                }
                                else
                                {
                                    s = s + Convert.ToString(ac[b]);
                                }
                            }
                            if (richTextBox2.Lines[a].StartsWith(s))
                            {
                                DeleteLine(a, richTextBox2);
                                a--;
                            }
                        }
                    }
                }
                MessageBox.Show("Done!");
            }
        }

        private void createAnUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(@"Patch"))
            {
                DialogResult ac = MessageBox.Show("The directory Patch exists. Do you want to delete it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (ac == DialogResult.Yes)
                {
                    Directory.Delete(@"Patch");
                }
                else
                {
                    return;
                }
            }
            Directory.CreateDirectory(@"Patch");
            string file = "";
            char[] ab;
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                ab = richTextBox1.Lines[i].ToCharArray();
                for (int a = 0; a < ab.Length; a++)
                {
                    if (ab[a] == ';')
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(@"Patch\" + file));
                        File.Copy(substr + file, @"Patch\" + file, true);
                        file = "";
                        Array.Clear(ab, 0, ab.Length);
                        break;
                    }
                    else
                    {
                        file = file + ab[a];
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("NADevs MD5 Hasher alpha\nC0D3D by RadioMan\nhttp://ilyadud.ru\nhttp://vk.com/ilyadud\nEnjoy my soft :P", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            fileToolStripMenuItem.Enabled = true;
            comparingToolStripMenuItem.Enabled = true;
        }
    }
}
