using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CardiologistV2.Models
{
    public class Doctor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public int Phone { get; set; }
        public string address { get; set; }
        public string email { get; set; }

        public virtual ICollection<Consultation> Consulations { get; set; }

    }
}