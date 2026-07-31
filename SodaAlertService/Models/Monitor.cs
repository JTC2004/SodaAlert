using System.Text.Json.Serialization;

namespace SodaAlertService.Models;

public class Monitor
{
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = "";

    [JsonPropertyName("query")]
    public string Query { get; set; } = "";

    [JsonPropertyName("pollingIntervalMinutes")]
    public int PollingIntervalMinutes { get; set; }
}