using System.Net.Http.Json;

namespace Edi.ChinaDetector;

public class IPChinaDetector(HttpClient httpClient)
{
    public async Task<(int Rank, string IPAddress)> Detect()
    {
        int rank = 0;
        string ip = null;

        try
        {
            const string geoIpServiceUrl = "http://ip-api.com/json/";
            var response = await httpClient.GetFromJsonAsync<GeoIPResult>(geoIpServiceUrl);

            ip = response.Query;
            if (response.CountryCode == "CN") rank++;
        }
        catch (HttpRequestException)
        {
            rank++;
        }

        return (rank, ip);
    }
}