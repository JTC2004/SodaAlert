using System.Text.Json.Serialization;

namespace SodaAlertService.Models;

public class Monitor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = "";

    [JsonPropertyName("query")]
    public string Query { get; set; } = "";

    [JsonPropertyName("pollingIntervalMinutes")]
    public int PollingIntervalMinutes { get; set; }

    [JsonPropertyName("pollingIntervalSeconds")]
    public int PollingIntervalSeconds { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";
}