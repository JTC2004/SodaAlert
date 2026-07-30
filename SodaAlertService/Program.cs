//Author:           Jared Crow
//Project name:     SodaAlert
//Created:          7/29/26
//Made for:         RankedIn Sprint #3
//Target company:   Tyler Technologies

using SodaAlertService.Services;                    //This imports SodaClient.cs from my Services dir.
var builder = WebApplication.CreateBuilder(args);

/*What it do (as of 7/30/26 @ 1:46pm):
    This program runs its own API, 
    and prints a hello message from SodaClient.cs in the /permits directory of localhost. 

*/

// Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

//Tell ASP.NET: "Whenever someone asks for a SodaClient, create one for them."
    builder.Services.AddHttpClient();               //This tells ASP.NET:   "I want to use HttpClient."

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
    return await sodaClient.GetLatestPermitsAsync();
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