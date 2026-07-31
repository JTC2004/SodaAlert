namespace SodaAlertService.Services;
using System.Text.Json;
using Monitor = SodaAlertService.Models.Monitor;    //This imports Monitor from Monitor.cs

//This is run every frame to check the monitors:
public class QueryMonitorService : BackgroundService
{
    private readonly SodaClient _sodaClient;

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
            //   Detect changes?        not yet
            //   Send notifications?    not yet
            foreach (Monitor monitor in monitors)
{
                var permits = await _sodaClient.GetLatestPermitsAsync(
                    monitor.BaseUrl,
                    monitor.Query);

                Console.WriteLine(
                    $"Monitor '{monitor.Query}' returned {permits?.Count ?? 0} records.");
            }
            

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}