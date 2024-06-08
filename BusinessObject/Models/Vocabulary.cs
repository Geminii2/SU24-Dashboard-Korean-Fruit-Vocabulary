using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }
        public string? Fruits_img { get; set; }
        public string? English { get; set; }
        public string? Korean { get; set; }
        public string? Vietnamese { get; set; }
        public string? Spelling { get; set; }
        public string? Voice_VN { get; set; }
        public string? Voice_EN { get; set; }
        public string? Voice_KR { get; set; }
        public string? Example_VN { get; set; }
        public string? Example_EN { get; set; }
        public string? Example_KR { get; set; }
        public int Status { get; set; }
        

    }
}
