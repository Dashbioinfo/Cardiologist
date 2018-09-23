using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Models.DomainModels
{
   public partial class Point
    {
        public int index;
        public double value;

        public Point()
        {
            this.index = 0;
            this.value = 0.0;
        }
        public Point(int index, double value)
        {
            this.index = index;
            this.value = value;
        }

    }
}
