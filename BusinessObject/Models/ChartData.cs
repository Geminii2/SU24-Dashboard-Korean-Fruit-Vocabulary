using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ChartData
    {
        public List<string> Labels { get; set; }
        public List<int> Total { get; set; }
        public List<int> Female { get; set; }
        public List<int> Male { get; set; }
    }
}
