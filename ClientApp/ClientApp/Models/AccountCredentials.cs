using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    class AccountCredentials
    {
        public string username;
        public string password;

        public AccountCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;

        }
    }

}
