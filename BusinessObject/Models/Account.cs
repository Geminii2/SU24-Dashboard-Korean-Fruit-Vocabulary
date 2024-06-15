using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Account
    {
        public int Id { get; set; }
        public long Phone { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public int Status { get; set; }
        public string? Avatar { get; set; }
        public string Created_date { get; set; }
    }
}
