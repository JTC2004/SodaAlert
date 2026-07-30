//When you query the SODA API, it returns JSON.
//So, this file converts JSON into C# objects that I can work with.
using System.Text.Json.Serialization;

namespace SodaAlertService.Models;

public class BuildingPermit
{
    //[JsonPropertyName] tells the JSON parser: "When you see the JSON field permit_, put its value into my PermitNumber property."
    //This lets me keep nice C# property names while still matching the SODA JSON.
    [JsonPropertyName("permit_")]           
    public string? PermitNumber { get; set; }

    [JsonPropertyName("work_description")]
    public string? WorkDescription { get; set; }

    [JsonPropertyName("issue_date")]
    public DateTime? IssueDate { get; set; }

    [JsonPropertyName("reported_cost")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]     //Tell the JSON serializer that numbers can be encoded as strings.
    public decimal? ReportedCost { get; set; }
}

/*public class BuildingPermit
{
    public string? permit_ { get; set; }            //This creates a variable with an automatic getter and setter.
                                                    //string? means this string should never be null.
    public string? work_description { get; set; }

    public string? issue_date { get; set; }

    public string? street_number { get; set; }

    public string? street_name { get; set; }

    public string? reported_cost { get; set; }
}*/