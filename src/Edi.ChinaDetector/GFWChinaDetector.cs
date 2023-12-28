namespace Edi.ChinaDetector;

public class GFWChinaDetector(HttpClient httpClient)
{
    public async Task<int> Detect()
    {
        int rank = 0;

        bool canConnectToGoogle = await TestConnectionAsync("https://www.google.com");
        bool canConnectToYouTube = await TestConnectionAsync("https://www.youtube.com");
        bool canConnectToTwitter = await TestConnectionAsync("https://www.twitter.com");

        rank += GetRank(canConnectToGoogle);
        rank += GetRank(canConnectToYouTube);
        rank += GetRank(canConnectToTwitter);

        return rank;
    }

    private int GetRank(bool isConnected)
    {
        return isConnected ? 0 : 1;
    }

    private async Task<bool> TestConnectionAsync(string url)
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error connecting to {url}: {e.Message}");
            return false;
        }
        catch (TaskCanceledException e)
        {
            // A TaskCanceledException often indicates the timeout was reached
            Console.WriteLine($"Connection to {url} timed out: {e.Message}");
            return false;
        }
    }
}