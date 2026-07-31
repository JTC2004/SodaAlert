namespace SodaAlertService.Services;
using System.Text.Json;
using Monitor = SodaAlertService.Models.Monitor;    //This imports Monitor from Monitor.cs

//This is run every frame to check the monitors:
public class QueryMonitorService : BackgroundService
{
    private readonly SodaClient _sodaClient;
    private Dictionary<string, string> previousJson = new();    //This is a dictionary that tracks the previous json calls.

    public QueryMonitorService(SodaClient sodaClient)
    {
        _sodaClient = sodaClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Checking monitors...");

            List<Monitor> monitors = new();

            //Populate the list of monitors:
            foreach (string file in Directory.GetFiles("JSONs", "*.json"))
            {
                string json = File.ReadAllText(file);

                Monitor? monitor = JsonSerializer.Deserialize<Monitor>(json);

                if (monitor != null)
                    monitors.Add(monitor);
            }

            

            //For each monitor,
            //   Execute SoQL query?    yes
            //   Detect changes?        yes
            //   Print messages?        yes
            //   Send notifications?    not yet
            int i = 0;
            foreach (Monitor monitor in monitors)
            {
                string URL = monitor.BaseUrl + monitor.Query;           //The full URL plus the query.

                var rawJSON = await _sodaClient.GetRawJsonAsync(monitor.BaseUrl, monitor.Query);

                //If this monitor's raw JSON isn't already in the dictionary, add it, with the URL as the key.
                if (!previousJson.ContainsKey(URL))
                {
                    previousJson[URL] = rawJSON;
                }
                //If this URL's raw JSON is different from what is recorded in the dictionary, notify the user and update the dictionary.
                else if (previousJson[URL] != rawJSON)
                {
                    Console.WriteLine($"MONITOR {i} CHANGED!!");

                    var parsedJSON = await _sodaClient.GetLatestPermitsAsync(monitor.BaseUrl, monitor.Query);
                    Console.WriteLine($"Monitor '{monitor.Query}' returned {parsedJSON?.Count ?? 0} records.");

                    previousJson[URL] = rawJSON;
                }
                //Else, nothing changed.
                else
                {
                    Console.WriteLine($"No changes with monitor {i}.");
                }
                i++;
            }
            

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}