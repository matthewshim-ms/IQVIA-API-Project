# IQVIA-API: C#

## Prerequisites
- [Visual Studio Community 2017](https://www.visualstudio.com/downloads/?utm_source=mscom&utm_campaign=msdocs)

## Summary

This application queries the [iqvia API](https://badapi.iqvia.io/swagger/) for tweet objects between 01/01/2016 - 12/31/2017. The user may also provide a custom date range instead. Duplicate tweet objects (by ID) are detected and discarded. This project was build using Microsoft's [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-2.0) web application template. 

![dotnet-01][dotnet-01]

![dotnet-02][dotnet-02]

## To Run:

There are two ways to run this project. First, clone this repo and CD into the **CSharp** directory. From here, you can:

1) From your command line, cd into **CSharp/IQVIA-Project** enter:
```
dotnet restore

dotnet run
```
By default this will launch the application on **http://localhost:60358**. Open a web browser of your choosing and navigate to the URL. 

2) Launch in Visual Studio - from the **CSharp** directory, double click on the **.sln** file to open the project in Visual Studio. In the Solution Explorer, right click on the project and select **Build solution.** Then, hit **F5**. 

## TODO:

**Features to Implement:**
- [ ] Export data to local feature
- [ ] Add data analytics
- [ ] Chatbot interface for user-friendly querying
- [ ] Database? 

**General:
- [x] **FIX UTC DATE Offset**
- [ ] Error handling
- [ ] More testing
- [ ] Re-factor code
- [ ] Explore other front end client options


[dotnet-01]:
../media/dotnet-01.png
[dotnet-02]:
../media/dotnet-02.png