using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Main : Form
    {
        PatientService.PatientServiceSoapClient client;
        public Main()
        {
            InitializeComponent();
            client = new PatientService.PatientServiceSoapClient();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //Check Username & Password
            //Retrieve ID
            if (txtUsername.Text != "" && txtPassword.Text != "")
            {
                var acc = new PatientService.AccountCredentials();
                acc.username = txtUsername.Text;
                acc.password = txtPassword.Text;
                string response = client.Login(acc);
                string[] tokens = response.Split('|');
                if (tokens[0] == "200")
                {
                    //if Authenticated..
                    int ID = 0;
                    this.Hide();
                    var form1 = new Form1(Convert.ToInt32(tokens[1]));
                    form1.Closed += (s, args) => this.Close();
                    form1.Show();
                }
                else
               if(tokens[0]=="422") 
               {
                   MessageBox.Show(tokens[1]+"\nUsername/Password Mismatch!");
               }
               else
               {
                   MessageBox.Show(tokens[1]+"\nPlease Register First!");
               }
            }
            else
            {
                MessageBox.Show("Username/ Password could not be empty!");
            }
            //
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            var registration = new Registration();
            registration.Closed += (s, args) => this.Close();
            registration.Show();
        }
    }
}
