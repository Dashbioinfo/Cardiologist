using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardiologistV2.Models
{
    public class History
    {
        public int HistoryID { get; set; }
        public string Health { get; set; }
        public string Surgery { get; set; }
        public string Medicine { get; set; }
        public string Family { get; set; }
        public int PatientID { get; set; }

        public virtual Patient patients { get; set; }


    }
}