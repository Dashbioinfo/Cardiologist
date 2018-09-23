using ClientApp.Models;
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
    public partial class Registration : Form
    {
        PatientService.PatientServiceSoapClient client;
        public Registration()
        {
            InitializeComponent();
            client = new PatientService.PatientServiceSoapClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text != "" && comboBox1.Text!="" && txtUser.Text != "" && txtPass.Text != "")
            {
                if (txtPass.Text.Length >= 6)
                {
                    var p = new PatientService.Patient();
                   p.Name = txtName.Text;
                   p.DOB = dateTimePicker1.Value;
                   p.Gender = comboBox1.Text;
                   p.Address = rtbAddress.Text;
                   p.Job = txtJob.Text;

                           if (checkedListBox1.GetItemChecked(0))
                           {
                               p.Smoker = "Smoker";
                           }
                           else
                           {
                               p.Smoker = "Non-Smoker";
                           }


                 if (checkedListBox1.GetItemChecked(1))
                     {
                         p.Alcoholic = "Alcoholic";
                     } 
                  else
                     {
                        p.Alcoholic = "Non-Alcoholic";
                     }
                 var acc = new PatientService.AccountCredentials();
                 acc.username = txtUser.Text;
                 acc.password = txtPass.Text;

                //TODO: Send acc and p to the server (DONE)
                string response =  client.register( p, acc);
                if (response == "200")
                {
                    //Close form
                    this.Hide();
                    var main = new Main(); // (ID);
                    main.Closed += (s, args) => this.Close();
                    main.Show();
                }

                else
                {
                    MessageBox.Show("Error Occured\nPlease Try Again Later");
                }


                }
                else
                {
                    MessageBox.Show("Please Enter a Password that has at least 6 Characters!");
                }
            }
            else
            {
                MessageBox.Show("Please Enter At Least the Following Required Fields:\nFull Name\nGender\nUsername\nPassword");
            }



        }

    }
}
