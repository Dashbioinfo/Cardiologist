using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CardiologistV2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace CardiologistV2.View_Models
{
    public class RegisterPatient
    {
        public RegisterPatient()
            {
                this.ThePatient = new Patient();
                this.RM = new RegisterModel();
            }

            public Patient ThePatient { set; get; }

            public RegisterModel RM { set; get; }
    }
}