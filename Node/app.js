var express = require('express');
var path = require('path');
var bodyParser = require('body-parser');
var request = require('request');
// var async = require('async')

var handleBars = require('express-handlebars').create({
    defaultLayout: 'main',
    layoutsDir: path.join(__dirname, "views/layouts"),
});

// Server setup **EXPRESS**
var app = express();
app.set('port', 9876);
var server = app.listen( process.env.PORT || 9876 || app.get('port'), function(){
    console.log('Listening on port ' + server.address().port);
});
app.use(express.static(__dirname + '/public'));
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

// handlebars config
app.engine('handlebars', handleBars.engine);
app.set('view engine', 'handlebars');
app.set('views', path.join(__dirname, "views"));


// Page Routes
app.get('/', function(req, res){
    return res.render('index');
});

app.get('/getTweets', function(req, res, next){
    
    // Source: https://stackoverflow.com/questions/48339532/how-to-make-multiple-paginated-get-requests-to-an-api-in-while-loop-with-node-js

    let baseURL = `https://badapi.iqvia.io/api/v1/Tweets`;
    let startDate = "2016-01-01T00:00:00.001Z";
    let endDate = "2017-12-31T23%3A59%3A59.001Z";
 
    let context = {};
    context.tweet_list = [];
    let Hashmap_Tweet_Ids = {};
    let duplicateCount = 0;


    let url = `${baseURL}?startDate=${startDate}&endDate=${endDate}`;

    function getTweetsFromIQVIA(url, callback){

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
                        context.tweet_list.push(t);

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
                    
                    getTweetsFromIQVIA(nextUrl, callback);
                }else{
                    // **CALLBACK 
                    displayToConsole();
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
        console.log("======== TWEETS ==============");
        var tweet_count = context.tweet_list.length;
        console.log(`Tweets Found: ${tweet_count}, Duplicates Detected: ${duplicateCount}\n`);
        console.log("----------------------------------")
        console.log(context.tweet_list);
    }

    getTweetsFromIQVIA(url).then;

});



  