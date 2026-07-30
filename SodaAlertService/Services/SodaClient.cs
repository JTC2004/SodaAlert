//All communication with SODA lives in one class (this class).
namespace SodaAlertService.Services;

public class SodaClient
{
   //Make it so Soda Client receives the HttpClient registered in Program.cs:
    private readonly HttpClient _httpClient;

    //This is another example of dependency injection. ASP.NET automatically creates the HttpClient and passes it into your class.
    public SodaClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //Access 5 items from the City of Chicago's endpoint:
    //Returns an unparsed JSON string.
    public async Task<string> GetLatestPermitsAsync()
    {
        string url =
            "https://data.cityofchicago.org/resource/ydr8-5enu.json?$limit=5";

        return await _httpClient.GetStringAsync(url);
    }

    //Test task to make sure this program's local API is working:
    /*public Task<string> GetLatestPermitsAsync()
    {
        return Task.FromResult("Hello from SodaClient!");
    }*/
}