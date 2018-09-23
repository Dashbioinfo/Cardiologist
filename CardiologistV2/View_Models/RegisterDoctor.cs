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
    public class RegisterDoctor
    {
        public RegisterDoctor()
            {
                this.TheDoctor = new Doctor();
                this.RM = new RegisterModel();
            }

            public Doctor TheDoctor { set; get; }

            public RegisterModel RM { set; get; }
    }
}