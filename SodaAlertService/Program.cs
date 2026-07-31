//Author:           Jared Crow
//Project name:     SodaAlert
//Created:          7/29/26
//Made for:         RankedIn Sprint #3
//Target company:   Tyler Technologies

using System.Text.Json;
using System.Runtime.CompilerServices;
using SodaAlertService.Services;                    //This imports SodaClient.cs from my Services dir.
using Monitor = SodaAlertService.Models.Monitor;    //This imports Monitor from Monitor.cs
var builder = WebApplication.CreateBuilder(args);

/*What this does (as of 7/31/26 @ 2:00pm):
    This program runs its own API, 
    prints a parsed JSON building permits from cityofchicago's SODA endpoint to localhost, 
    via URL + query combinations from JSON files,
    automatically queries again every 3 seconds,
    and lets the user know whether the data has changed or not. 
*/

//Variables:
    //bool debug = false;
    //string baseURL = "https://data.cityofchicago.org/resource/ydr8-5enu.json";
    List<Monitor> monitors = new();

    //Populate the list of monitors:
    foreach (string file in Directory.GetFiles("JSONs", "*.json"))
    {
        string json = File.ReadAllText(file);

        Monitor? monitor = JsonSerializer.Deserialize<Monitor>(json);

        if (monitor != null)
            monitors.Add(monitor);
    }


// Add services to the program's order of operations:.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    //Tell ASP.NET: "Whenever someone asks for a SodaClient, create one for them."
    builder.Services.AddHttpClient();               //This tells ASP.NET:   "I want to use HttpClient."
                                                    //This means "If any class needs an HttpClient, create one and provide it automatically."

    builder.Services.AddTransient<SodaClient>();       //Stand-in for SodaClient client = new SodaClient();

    builder.Services.AddHostedService<QueryMonitorService>();   //After web server is started, 

//Build the app:
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();  //Automatically intercepts insecure HTTP web requests and redirects them to their secure HTTPS counterparts.

//What happens when the webpage is accessed:
    int i = 0;
    foreach (Monitor monitor in monitors)
    {   
        //Making legal copies, just to be safe:
        var currentMonitor = monitor;
        int currentIndex = i;

        //Needs to return data:
        app.MapGet($"/monitor{currentIndex}", async (SodaClient sodaClient) =>
        {   
            var permits = sodaClient.GetLatestPermitsAsync(currentMonitor.BaseUrl, currentMonitor.Query);
            return await permits;
        });

        i++;
    }



//This makes the program actually run.
//FYI, ap.Run() never returns. It starts the web server and blocks the main thread until the application shuts down.
app.Run();