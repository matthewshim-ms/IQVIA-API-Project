
var request = require('request');
var fs = require('fs');
    
let baseURL = `https://badapi.iqvia.io/api/v1/Tweets`;
let startDate = "2016-01-01T00:00:00.001Z";
let endDate = "2017-12-31T23%3A59%3A59.001Z";

let tweet_list = [];
let Hashmap_Tweet_Ids = {};
let duplicateCount = 0;

let url = `${baseURL}?startDate=${startDate}&endDate=${endDate}`;

function getTweets(){

    console.log("\nFetching Tweets from Bad API ---- one moment...");

    return new Promise((resolve, reject) => {
        getTweetsFromIQVIA(url);
    })
}

function getTweetsFromIQVIA(url){

    request.get({
        url: url,
        json: true
    }, function(err, res, body){

        if(!err && res.statusCode == 200){
            let tweets = body;
            let dates = [];

            // convert every string timestamp to actual timestamp
            tweets.forEach(t => {
                t.stamp = new Date(t.stamp);

                // if ID is not in object, add it
                if(!(t.id in Hashmap_Tweet_Ids)){
                    
                    // add id to Hashmap, value not important
                    Hashmap_Tweet_Ids[t.id] = true;

                    // add tweet to list
                    tweet_list.push(t);

                    dates.push(t.stamp);
                }else if(t.id in Hashmap_Tweet_Ids){
                    duplicateCount++;
                }

                // sort dates low --> high
                dates = dates.sort(sortDates);

            });
            
            // get the next date from the sorted date list, convert to UTC timestamp
            let nextDate = dates[dates.length - 1].toUTCString();

            // more pages to get if we get 100 tweets
            if(tweets.length == 100){
                nextUrl = `${baseURL}?startDate=${nextDate}&endDate=${endDate}`

                // console.log(nextUrl);
                
                getTweetsFromIQVIA(nextUrl);
            }else{
                // done, display
                displayToConsole();

                // save to .txt file
                var file = fs.createWriteStream('tweet-data.txt');
                file.on('error', function(err){console.log(err)});
                tweet_list.forEach(function(t){file.write(`id: ${t.id}\r\ntext: ${t.text}\r\nstamp: ${t.stamp}\r\n`+ '\r\n')});
                console.log("Results saved to tweet-data.txt");
                file.end();
            }
        }else{
            console.log(err);
            throw err;
        }

    });
}

function sortDates(a, b){
    return a.getTime() - b.getTime();
}

function displayToConsole(){
    console.log("\n======== TWEETS ==============");
    var tweet_count = tweet_list.length;

    console.log(tweet_list);
    console.log("\n----------------------------------\n");
    console.log(`Tweets Found: ${tweet_count}, Duplicates Detected: ${duplicateCount}\n`);
    console.log("----------------------------------");
}

// GET TWEETS
getTweets();    




  