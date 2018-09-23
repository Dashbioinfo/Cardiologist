using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
//using ClientApp.PatientService;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        static string path = null;
        static int ID = -1;
        FileSystemWatcher m_Watcher = null;
        static int num = 0;
        PatientService.PatientServiceSoapClient client;
        string fullPath = null;
        public int numTries = 0;
        string response;
        
        public void init()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            //TODO:Include them for the client (Done)
            client = new PatientService.PatientServiceSoapClient();
            client.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);

        }
        public Form1()
        {
            //InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;

            //client = new PatientService.PatientServiceSoapClient();
            //client.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
            init();
        }
        public Form1(int Id)
        {
            init();
            ID = Id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(m_Watcher !=null)
            {
                m_Watcher.EnableRaisingEvents = false;
                m_Watcher = null;
            }

            using (FolderBrowserDialog dlgOpenDir = new FolderBrowserDialog())
            {
                DialogResult resDialog = dlgOpenDir.ShowDialog();
                if (resDialog.ToString() == "OK")
                {
                    path = dlgOpenDir.SelectedPath;
                    textBox1.Text = path;
                }
            
            }
            richTextBox1.Text += "\nA new Path has been Set\nClick \"Sync\" to Synchronize ...";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                if (path != null)
                {
                    m_Watcher = new System.IO.FileSystemWatcher();
                    m_Watcher.Filter = "*.txt"; //Watchout all Text Files
                    m_Watcher.Path = path + "\\"; //watchout this Directory
                    m_Watcher.NotifyFilter = NotifyFilters.LastWrite;

                    m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
                    // m_Watcher.Created += new FileSystemEventHandler(OnChanged);
                    m_Watcher.EnableRaisingEvents = true;
                    richTextBox1.Text += "\n Directory" + path.Substring(path.LastIndexOf("\\")) + " is now Synchronized !";
                }
                else
                {
                    MessageBox.Show("Please Choose a Directory, First!");
                }
            }
            else
            {
                MessageBox.Show("Please Choose the Machine's Frequence Fist!");
            }
          
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            num = num + 1;
            //if(num %6 ==0)
            //{
                HandleNewFile();
          //  }
        }

        private void HandleNewFile()
        {
            richTextBox1.Clear();
            string[] filenames = Directory.GetFiles(path, "*.txt").Select(p => Path.GetFileName(p))
                                               .ToArray();

            richTextBox1.Text = "File " + filenames[filenames.Length - 1] + " Has Been Added !";
            string filename = filenames[filenames.Length -1];
          string Data =  File.ReadAllText(path + "\\" + filename, Encoding.ASCII);
            //Plug-in Webservice Call, here:
       //  Data 
          int fs = 0;
          if (comboBox1.Text == "EU (250 Hz)")
          {
              fs = 250;
          }
          else
          {
              fs = 370;
          }

           //  string response = client.Upload(Data); //client.Upload(Data);
            
         //if (str == "200")
         //{ }
            string response="";
            do
            {
                numTries++;
                richTextBox1.Text += "\nTry "+numTries+" : Connecting to Server ...";
                richTextBox1.Text += "\nConnected!\nSending File" + filename + " to Server ...";

                 response = client.Upload(ID, filename, Data, fs);
                 string[] messagechunks = response.Split('|');
            } while (messagechunks[0] != "200");

            richTextBox1.Text +="\n"+messagechunks[1];


        }

    }
}
