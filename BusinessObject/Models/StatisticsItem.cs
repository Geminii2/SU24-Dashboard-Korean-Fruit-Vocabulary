using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class StatisticsItem
    {
        public int Month { get; set; }
        public int Total { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
    }
}
