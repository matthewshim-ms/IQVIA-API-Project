using System;
using System.Collections.Generic;
using System.Collections;
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
            Hashtable tweetIDs = new Hashtable();

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
                                if (!tweetIDs.Contains(twt.id) && twt.stamp.Year >= startDateUnspecifiedKind.Year)
                                {
                                    tweetIDs.Add(twt.id, 1);
                                    tweets.tweetList.Add(twt);
                                    tweets.tweet_count++;
                                }
                                else
                                {
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
    }
}