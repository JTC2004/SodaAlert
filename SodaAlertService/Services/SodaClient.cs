//All communication with SODA lives in one class (this class).
namespace SodaAlertService.Services;

public class SodaClient
{
    public Task<string> GetLatestPermitsAsync()
    {
        return Task.FromResult("Hello from SodaClient!");
    }
}