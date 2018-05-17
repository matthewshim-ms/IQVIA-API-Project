using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IQVIA_Project.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace IQVIA_Project.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> GetTweets()
        {
            string original_start_date = "2016-01-01T00:00:00.001Z";
            DateTime original_start = DateTime.Parse(original_start_date);
            var sanitized_time = original_start.ToLocalTime();
            DateTime startDateUnspecifiedKind = sanitized_time.ToUniversalTime();
           
            DateTime startDate = DateTime.Parse(original_start_date);
            string endDate = "2017-12-31T23%3A59%3A59.001Z";

            TweetsViewModel tweets = new TweetsViewModel();

            // Performance testing between using Hashtable vs. LINQ query
            // Stopwatch sw = new Stopwatch();
            // sw.Start();

            // Disposes of httpclient when request is completed
            using (HttpClient request = new HttpClient()) {

                request.BaseAddress = new Uri("https://badapi.iqvia.io/api/v1/Tweets");
                request.DefaultRequestHeaders.Accept.Clear();

                bool lessThan100 = false;
                List<Tweet> listOfTweets = new List<Tweet>();
                while (!lessThan100)
                {
                    using (HttpResponseMessage response = await request.GetAsync("?startDate=" + startDate + "&endDate=" + endDate))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string tweetList = await response.Content.ReadAsStringAsync();
                            listOfTweets = JsonConvert.DeserializeObject<List<Tweet>>(tweetList);

                            foreach (Tweet twt in listOfTweets)
                            {
                                if (!tweets.tweetIDs.Contains(twt.id) && twt.stamp.Year >= startDateUnspecifiedKind.Year)
                                {
                                    // adds ID to hash table
                                    tweets.tweetIDs.Add(twt.id, 1);
                                    // adds tweet object to list
                                    tweets.tweetList.Add(twt);
                                    tweets.tweet_count++;
                                }
                                else if(tweets.tweetIDs.Contains(twt.id))
                                {
                                    // duplicate detected
                                    tweets.duplicate_count++;
                                }
                            }

                            // # Method 2 - adding all, then use LINQ
                            // tweets.tweetList.AddRange(listOfTweets);

                            // Get the maximum time stamp in the list of tweets, update the time stamp
                            startDate = listOfTweets.Max(t => t.stamp);

                            lessThan100 = listOfTweets.Count() < 100;
                        }
                        else // bad response
                        {
                         
                            lessThan100 = true;
                        }
                        
                    }
                }

                // # Method 2 - with LINQ
                //tweets.tweetList = (from tweet in tweets.tweetList
                //                    where tweet.stamp >= startDateUnspecifiedKind
                //                    select tweet).ToList();

            }
            // TESTING PURPOSES
            //sw.Stop();
            //var time_elapsed = sw.Elapsed;
            return View(tweets);   
        }

        [HttpPost]
        public async Task<IActionResult> GetTweetsInRange(DateTime startDate, DateTime endDate)
        {
            TweetsViewInRangeModel tweets = new TweetsViewInRangeModel();

            string start_date_string = sanitize_time_stamp(startDate);
            string end_date = sanitize_time_stamp(endDate);

            DateTime start_date = DateTime.Parse(start_date_string);

            tweets.start_date = startDate;
            tweets.end_date = endDate;

            using (HttpClient request = new HttpClient())
            {
                request.BaseAddress = new Uri("https://badapi.iqvia.io/api/v1/Tweets");
                request.DefaultRequestHeaders.Accept.Clear();

                bool lessThan100 = false;
                List<Tweet> listOfTweets = new List<Tweet>();

                while (!lessThan100)
                {
                    using (HttpResponseMessage response = await request.GetAsync("?startDate=" + start_date + "&endDate=" + end_date))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string tweetList = await response.Content.ReadAsStringAsync();
                            listOfTweets = JsonConvert.DeserializeObject<List<Tweet>>(tweetList);

                            foreach (Tweet twt in listOfTweets)
                            {
                                if (!tweets.tweetIDs.Contains(twt.id) && twt.stamp.Year >= startDate.Year)
                                {
                                    tweets.tweetIDs.Add(twt.id, 1);
                                    tweets.tweetList.Add(twt);
                                    tweets.tweet_count++;
                                }
                                else if(tweets.tweetIDs.Contains(twt.id))
                                {
                                    tweets.duplicate_count++;
                                }
                            }

                            start_date = listOfTweets.Max(t => t.stamp);

                            lessThan100 = listOfTweets.Count() < 100;
                        }
                        else // bad response
                        {
                            lessThan100 = true;
                        }
                    }
                }

            }

            return View(tweets);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // Takes a DateTime object and returns a string in the UTC format specified by IQVIA 'Bad API'
        private string sanitize_time_stamp(DateTime date)
        {
            DateTime d = date.AddDays(-31);
            return (d.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss") + ".271Z");
        }

    }
}