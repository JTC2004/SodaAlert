//Author:           Jared Crow
//Project name:     SodaAlert
//Created:          7/29/26
//Made for:         RankedIn Sprint #3
//Target company:   Tyler Technologies

using System.Runtime.CompilerServices;
using SodaAlertService.Services;                    //This imports SodaClient.cs from my Services dir.
var builder = WebApplication.CreateBuilder(args);

/*What this does (as of 7/30/26 @ 3:04pm):
    This program runs its own API, 
    and prints a parsed JSON message, 
    containing data from cityofchicago's SODA endpoint, 
    via a hard-coded URL & user-input SoQL query. 

*/

//Variables:
bool debug = false;

// Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

//Tell ASP.NET: "Whenever someone asks for a SodaClient, create one for them."
    builder.Services.AddHttpClient();               //This tells ASP.NET:   "I want to use HttpClient."
                                                    //This means "If any class needs an HttpClient, create one and provide it automatically."

    builder.Services.AddScoped<SodaClient>();       //Stand-in for SodaClient client = new SodaClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();  //Automatically intercepts insecure HTTP web requests and redirects them to their secure HTTPS counterparts.


app.MapGet("/permits", async (SodaClient sodaClient) =>
{   
    string baseURL = "https://data.cityofchicago.org/resource/ydr8-5enu.json";
    string query = "";

    if (debug)
        query = "$limit=5";
    else
        Console.Write("ENTER YOUR QUERY: ");
        query = Console.ReadLine() ?? "";       // ?? assigns a default value if nothing is entered.

    return await sodaClient.GetLatestPermitsAsync(baseURL, query);
});

//This makes the program actually run.
app.Run();                                                  


//SAMPLE CODE:
/*
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}*/