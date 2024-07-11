using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class History
    {
        public string Id { get; set; }
        public string Question_Id { get; set; }
        public int Account_Id { get; set; }
        public string Answer { get; set; }
        public bool Result { get; set; }
    }
}
