# IQVIA-API: JavaScript

## Prerequisites
- [Node.js](https://nodejs.org/en/) v.8 and greater

## Summary

This simple command line tool queries for all tweets from 01/01/2016 - 12/31/2017 from the [iqvia web API](https://badapi.iqvia.io/swagger/), and saves the information to a text file. 

![node-01][node-01]

## To Run:

Clone this repo and CD into the /Node folder using your command line. Install the node dependencies using **npm install**, and from the command line enter **node iq-get-tweets.js** as follows: 

```
npm install

node iq-get-tweets.js
```
**Note:** Running this program will create a file called **tweet-data.txt** in the current directory.

## TODO:

**Features to implement:**

For CLI:
- [ ] User prompts in CLI, specify date range, file format, etc.
- [ ] Prompt for save instead of direct download

Other:
- [ ] Create web client (React/Angular/Vue)
- [ ] Data Export feature to web client

[node-01]:
../media/node-01.png