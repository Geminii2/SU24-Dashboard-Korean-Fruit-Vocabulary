using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter file image")]
        public string? Fruits_img { get; set; }
        [Required(ErrorMessage = "Please enter English")]
        public string? English { get; set; }
        [Required(ErrorMessage = "Please enter Korean")]
        public string? Korean { get; set; }
        [Required(ErrorMessage = "Please enter Vietnamese")]
        public string? Vietnamese { get; set; }
        [Required(ErrorMessage = "Please enter Spelling")]
        public string? Spelling { get; set; }
        [Required(ErrorMessage = "Please enter file Voice Vietnamese")]
        public string? Voice_VN { get; set; }
        [Required(ErrorMessage = "Please enter file Voice English")]
        public string? Voice_EN { get; set; }
        [Required(ErrorMessage = "Please enter file Voice Korean")]
        public string? Voice_KR { get; set; }
        [Required(ErrorMessage = "Please enter description Vietnamese")]
        public string? Example_VN { get; set; }
        [Required(ErrorMessage = "Please enter description English")]
        public string? Example_EN { get; set; }
        [Required(ErrorMessage = "Please enter description Koreon")]
        public string? Example_KR { get; set; }
        public int Status { get; set; }


    }
}
