using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardiologistV2.Models
{
    public class Record
    {
        public int RecordID { get; set; }
        public string Data { get; set; }
        public DateTime Recdate { get; set; }
        public string Result { get; set; }
        public int PatientID { get; set; }
        //RECORD do not have Feature Id in 1:1 relationship
        public virtual Patient patients { get; set; }
        public virtual ICollection<Features> Featuress { get; set; }

    }
}