using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Question
    {
        public string Id { get; set; }
        public string Test_Id { get; set; }
        public int Vocabulary_Id { get; set; }
    }
}
