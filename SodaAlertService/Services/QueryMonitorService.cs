namespace SodaAlertService.Services;
using System.Text.Json;
using Monitor = SodaAlertService.Models.Monitor;    //This imports Monitor from Monitor.cs
using SodaAlertService.Models;

//This is run every frame to check the monitors:
public class QueryMonitorService : BackgroundService
{
    private readonly SodaClient _sodaClient;
    private Dictionary<string, List<BuildingPermit>> previousMonitorsPermits = new();    //This is a dictionary that tracks the previous json calls.

    public QueryMonitorService(SodaClient sodaClient)
    {
        _sodaClient = sodaClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("\n\nChecking monitors...");

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
            //   Send notifications?    WIP. Not perfect yet.
            int i = 0;
            int pollingSeconds = 0;
            foreach (Monitor monitor in monitors)
            {
                string URL = monitor.BaseUrl + monitor.Query;                   //The full URL plus the query.
                if(pollingSeconds == 0)
                {
                    pollingSeconds = monitor.PollingIntervalSeconds;    //How long to wait before polling again.
                }
                var currentPermits = await _sodaClient.GetLatestPermitsAsync(monitor.BaseUrl, monitor.Query);

                //Account for null:
                if (currentPermits == null)
                {
                    Console.WriteLine($"Monitor {i} returned no data.");
                    i++;
                    continue;
                }

                //If this monitor's parsed JSON isn't already in the dictionary, add it, with the URL as the key.
                if (!previousMonitorsPermits.ContainsKey(URL))
                {
                    previousMonitorsPermits[URL] = currentPermits;
                    Console.WriteLine($"Now observing with monitor {i}.");
                    i++;
                    continue;
                }
                //If this URL's raw JSON is different from what is recorded in the dictionary, notify the user and update the dictionary.
                else if (previousMonitorsPermits[URL] != currentPermits)
                {

                    //Compare the two and report what changed:
                    var previousPermits = previousMonitorsPermits[URL];

                    int numChanges = 0;
                    List<string> updates = [];

                    //Since every permit has a unique permit number, build dictionaries:
                    //Note: ! is a null-forgiving operator here. It means: "I know this value won't be null here."
                    var currentDict = currentPermits.Where(p => p.PermitNumber != null).ToDictionary(p => p.PermitNumber!);     
                    var previousDict = previousPermits.Where(p => p.PermitNumber != null).ToDictionary(p => p.PermitNumber!);      //Filter out if null.

                    //Search the permits in the monitor, check each property, and see what changed:
                    foreach (var permit in currentPermits)
                    {
                        if (permit.PermitNumber == null)
                        {
                            continue;
                        }
                        //Add a message if a permit was added:
                        if (!previousDict.TryGetValue(permit.PermitNumber, out var oldPermit))
                        {
                            updates.Add($"New permit added: {permit.PermitNumber}");
                            numChanges++;
                            continue;
                        }

                        //Add a message if a permit property was changed:
                        foreach (var property in typeof(BuildingPermit).GetProperties())
                        {
                            object? oldValue = property.GetValue(oldPermit);
                            object? newValue = property.GetValue(permit);

                            if (!Equals(oldValue, newValue))
                            {
                                updates.Add($"Permit # {permit.PermitNumber}: {property.Name} changed from '{oldValue}' to '{newValue}'");
                                numChanges++;
                            }
                        }
                    }
                    //Add a message if a permit was removed:
                    foreach (var permitNumber in previousDict.Keys)
                    {
                        if (!currentDict.ContainsKey(permitNumber))
                        {
                            updates.Add($"Removed permit: {permitNumber}");
                            numChanges++;
                        }
                    }

                    //Print the update messages if there was a difference detected:
                    if (numChanges > 0)
                    {
                        Console.WriteLine($"Monitor {i} changed!");

                        //SIMULATED EMAIL NOTIFICATION:
                        Console.WriteLine("\t================ EMAIL =================");
                        Console.WriteLine($"\tto: {monitor.Email}");
                        Console.WriteLine($"\tSubject: SodaAlert - Chicago Building Permits Updated");
                        Console.WriteLine($"\t");
                        Console.WriteLine($"\t{numChanges} changes detected in monitor \"{monitor.Name}\":");
                        foreach (var update in updates)
                        {
                            Console.WriteLine("\t\t• " + update);
                        }
                        Console.WriteLine($"\tAt endpoint: {monitor.BaseUrl}");
                        Console.WriteLine($"\tNumber of permits returned: {currentDict.Count}");
                        Console.WriteLine($"\t");
                        Console.WriteLine($"\tAI Summary (example):");
                        Console.WriteLine($"\tFive changes were detected in the monitored electrical work permits.");
                        Console.WriteLine($"\tThree new permits were issued, primarily involving commercial renovations, ");
                        Console.WriteLine($"\tresidential electrical service upgrades, and life-safety system installations.");
                        Console.WriteLine($"\tTwo permits that previously matched the monitoring criteria are no longer present in the current query results, indicating they may have ");
                        Console.WriteLine($"\tbeen completed, updated so they no longer satisfy the query conditions, or are no longer included in the dataset returned by the monitor.");
                        Console.WriteLine($"\t\n");
                        Console.WriteLine($"\tSincerely,");
                        Console.WriteLine($"\tSodaAlert.");
                        Console.WriteLine($"\t");
                        Console.WriteLine("\t=========================================");
                    }
                    //Else, print there was nothing to report.
                    else
                    {
                        Console.WriteLine($"Nothing to report with Monitor {i}.");
                    }
                    
                    previousMonitorsPermits[URL] = currentPermits;
                }
                //Else, nothing changed.
                else
                {
                    Console.WriteLine($"No changes with monitor {i}.");
                }
                i++;
            }


            
            Console.WriteLine($"Polling monitors again in {pollingSeconds} seconds.");
            await Task.Delay(TimeSpan.FromSeconds(pollingSeconds), stoppingToken);
        }
    }
}