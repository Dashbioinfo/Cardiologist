using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CardiologistV2.Models
{
    public class Patient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfbirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Job { get; set; }
        public string Smoker { get; set; }
        public string Alcoholic { get; set; }

        public virtual ICollection<Consultation> Consultations { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<History> Histories { get; set; }
    }
}