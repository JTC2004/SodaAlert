//When you query the SODA API, it returns JSON.
//So, this file converts JSON into C# objects that I can work with.

namespace SodaAlertService.Models;

public class BuildingPermit
{
    public string? permit_ { get; set; }            //This creates a variable with an automatic getter and setter.
                                                    //string? means this string should never be null.
    public string? work_description { get; set; }

    public string? issue_date { get; set; }

    public string? street_number { get; set; }

    public string? street_name { get; set; }

    public string? reported_cost { get; set; }
}