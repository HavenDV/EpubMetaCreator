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

namespace EpubMetaCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Epub Files (.epub)|*.epub|All Files (*.*)|*.*";
            dialog.InitialDirectory = ".";
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            { 
                inputTextBox.Text = dialog.FileName;
                label.Text = "Click Start button.";
            }
        }

        private void selectDirButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                inputTextBox.Text = dialog.SelectedPath;
                label.Text = "Click Start button.";
            }
        }

        private void outputButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                outputTextBox.Text = dialog.SelectedPath;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                var inputDir = !string.IsNullOrWhiteSpace(inputTextBox.Text) ? inputTextBox.Text : ".";
                var inputFile = File.Exists(inputDir) ? inputDir : string.Empty;
                var outputDir = outputTextBox.Text;

                label.Text = "Creation started.";
                progressBar.Value = 0;

                if (!string.IsNullOrWhiteSpace(inputFile))
                {
                    Program.ReadEpub(inputFile, outputDir);
                }
                else
                {
                    var files = Directory.GetFiles(inputDir, "*.epub");
                    if (files.Length == 0)
                    {
                        label.Text = "Directory not contains .epub files.";
                        return;
                    }

                    var i = 0;
                    foreach (var file in files)
                    {
                        label.Text = $"Current file: {file}...";
                        Program.ReadEpub(file, outputDir);
                        progressBar.Value = 100 * ++i / files.Length;
                    }
                }

                progressBar.Value = 100;
                label.Text = "Creation ended.";
            }
            catch (Exception exception)
            {
                label.Text = $"Exception: {exception.Message}";
            }
        }
    }
}
