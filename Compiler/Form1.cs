using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        string configuration = "Release";
        string path = "";
        string endPath = "";
        string archivePath = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int size = -1;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Файл проекта(*.sln)|*.sln";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                this.path = openFileDialog1.FileName;
                label2.Text = this.path;
            } else
            {
                label2.Text = "Ошибка";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.path != "" && this.endPath != "" && this.archivePath != "" && textBox2.Text != "")
            {
                string directory = Path.GetDirectoryName(this.path);
                string name = Path.GetFileNameWithoutExtension(this.path);
                directory = Path.Combine(directory, name, "obj", this.configuration);
                progressBar1.Value = 20;
                string resources = Path.Combine(Path.GetDirectoryName(this.path), name, "Resources");
                FileInfo resource = new FileInfo(Path.Combine(resources, "extantion.zip"));
                resource.Delete();
                File.WriteAllText(resources + @"/config.txt", textBox2.Text);
                File.Copy(this.archivePath, Path.Combine(resources, "extantion.zip"));
                progressBar1.Value += 20;
                DirectoryInfo di = new DirectoryInfo(directory);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                progressBar1.Value += 20;
                Dictionary<string, string> globalProperty = new Dictionary<string, string>()
                {
                    { "Configuration", configuration },
                    { "Platform", "Any CPU" },
                    { "VisualStudioVersion", "15.0" },
                    { "BuildInParallel", "true" },
                    { "DeployTarget", "PipelinePreDeployCopyAllFilesToOneFolder" },
                    { "_PackageTempDir", "C:\\temp"},
                    { "DeployOnBuild", "true" }
                };
                BuildManager buildManager = new BuildManager();
                BuildParameters parameters = new BuildParameters();
                BuildRequestData buildData = new BuildRequestData(path, globalProperty, null, new string[] { "Build" }, null, BuildRequestDataFlags.IgnoreExistingProjectState);
                BuildResult result = buildManager.Build(parameters, buildData);
                progressBar1.Value += 20;
                string endName = (textBox1.Text != "")?
                    Path.Combine(this.endPath, textBox1.Text + ".exe") : 
                    Path.Combine(this.endPath, name + ".exe");
                File.Copy(Path.Combine(directory, name + ".exe"), endName );
                progressBar1.Value += 20;
                MessageBox.Show(result.OverallResult.ToString(), "Результат", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Проверьте заполнение полей");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.endPath = folderBrowserDialog1.SelectedPath;
                label3.Text = this.endPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Файл мода(*.zip)|*.zip";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                this.archivePath = openFileDialog1.FileName;
                label6.Text = this.archivePath;
            }
        }
    }
}
