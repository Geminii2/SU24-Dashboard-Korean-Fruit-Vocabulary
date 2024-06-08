using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Admin
    {
        public Admin() { }
        public Admin(int _Id, string _Email, string _Pwd, string _Avatar_img, string _Fullname) 
        {
            Id = _Id;
            Email = _Email;
            Pwd = _Pwd;
            Role_id = 2;
            Avatar_img = _Avatar_img;
            Fullname= _Fullname;
        }
        public int Id { get; set; }
        public int Role_id { get; set; }
        public string? Pwd { get; set; }
        public long Phone { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public int Status { get; set; }
        public string? Avatar_img { get; set; }
    }
}
