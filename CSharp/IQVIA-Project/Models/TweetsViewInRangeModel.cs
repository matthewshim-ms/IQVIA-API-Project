using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IQVIA_Project.Models
{
    public class TweetsViewInRangeModel
    {
        public List<Tweet> tweetList = new List<Tweet>();

        [DataType(DataType.Date)]
        public DateTime start_date { get; set; }

        [DataType(DataType.Date)]
        public DateTime end_date { get; set; }

        public Hashtable tweetIDs = new Hashtable();

        public long tweet_count = 0;
        public long duplicate_count = 0;
    }
}
