using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardiologistV2.Models
{
    public class Consultation
    {
        public int ConsultationID { get; set; }
        public int DoctorID { get; set; }
        public int PatientID { get; set; }

        public virtual Patient patients { get; set; }
        public virtual Doctor doctors { get; set; }
    }
}