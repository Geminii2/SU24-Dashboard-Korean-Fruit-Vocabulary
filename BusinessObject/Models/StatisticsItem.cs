﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class StatisticsItem
    {
        public string Label { get; set; }
        public int Total { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
    }
}
