using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IQVIA_Project.Models
{
    // List of Tweets
    public class TweetsViewModel
    {
        public List<Tweet> tweetList = new List<Tweet>();
        public long tweet_count = 0;
        public long duplicate_count = 0;
    }
}
