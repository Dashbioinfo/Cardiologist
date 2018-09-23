using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CardiologistV2.Models
{
    public class Features
    {   
        public int FeaturesID { get; set; }
        public string Value { get; set; }
        public int RecordID { get; set; }
        public virtual Record Records { get; set; }

    }
}