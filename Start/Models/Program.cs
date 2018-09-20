using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Web;

namespace Start.Models
{

    public class Program
    {
        public int ID { get; set; }
            
        public String Title { get; set; }
            
        public String Path { get; set; }

        public String IconPath { get; set; } = "";

        public int Count { get; set; } = 0;
            
    }
}