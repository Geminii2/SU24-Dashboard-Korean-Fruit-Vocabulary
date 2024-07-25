using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Admin
    {
        public Admin() { }
        public Admin(int _Id, string _Email, string _Pwd, string _Fullname)
        {
            Id = _Id;
            Email = _Email;
            Pwd = _Pwd;
            Role_id = 2;
            Fullname= _Fullname;
            Avatar_img = "https://firebasestorage.googleapis.com/v0/b/su24-sep490-koreandictionary.appspot.com/o/Avatar_img%2Fbo-2.png?alt=media&token=04bf3aa0-c2d1-4141-b0b0-36f384b1f98e";
            Dob= "";
            Country="";
            Gender ="";
            Status= 1;
        }
        public int Id { get; set; }
        public int Role_id { get; set; }

        [StringLength(6, ErrorMessage = "Password minimum lenght of 6 characters")]
        [Required(ErrorMessage = "Please enter password")]
        public string? Pwd { get; set; }

        public long Phone { get; set; }

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid email format")]
        [Required(ErrorMessage = "Please enter email")]
        public string? Email { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s\u00A1-\uFFFF]*$", ErrorMessage = "Invalid full name format")]
        [Required(ErrorMessage = "Please enter full name")]
        public string? Fullname { get; set; }
        public string? Dob { get; set; }
        public string? Gender { get; set; }
        [RegularExpression(@"^[A-Z]+[a-zA-Z\s\u00A1-\uFFFF]*$", ErrorMessage = "Invalid country format")]
        public string? Country { get; set; }
        public int Status { get; set; }
        public string? Avatar_img { get; set; }
    }
}
