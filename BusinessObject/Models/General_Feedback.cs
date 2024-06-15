﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class General_Feedback
    {
        public string Id { get; set; }
        public int Account_Id { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Created_date { get; set; }
        public int Status { get; set; }

    }
}
