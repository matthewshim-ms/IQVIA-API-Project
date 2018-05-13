using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IQVIA_Project.Models;
//using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace IQVIA_Project.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> GetTweets()
        {
            // Do Later
            // HttpRequest httpRequest = new HttpRequest();

            string original_start_date = "2016-01-01T00:00:00.001Z";

            DateTime startDate = DateTime.Parse(original_start_date);
            string endDate = "2017-12-31T23%3A59%3A59.001Z";

            TweetsViewModel tweets = new TweetsViewModel();

            // Disposes of httpclient when request is completed
            using (HttpClient request = new HttpClient()) {

                request.BaseAddress = new Uri("https://badapi.iqvia.io/api/v1/Tweets");
                request.DefaultRequestHeaders.Accept.Clear();

                bool lessThan100 = false;

                while(!lessThan100)
                {

                    using (HttpResponseMessage response = await request.GetAsync("?startDate=" + startDate + "&endDate=" + endDate))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string tweetList = await response.Content.ReadAsStringAsync();
                            var listOfTweets = JsonConvert.DeserializeObject<List<Tweet>>(tweetList);

                            tweets.tweetList.AddRange(listOfTweets);

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
    }
}
//