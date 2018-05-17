using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IQVIA_Project.Models
{
    // Individual Tweet 
    public class Tweet
    {
        public long id { get; set; }
        public DateTime stamp { get; set; }
        public string text { get; set; }
    }
}
