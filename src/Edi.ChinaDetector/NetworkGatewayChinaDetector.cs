using System.Diagnostics;

namespace Edi.ChinaDetector;

/// <summary>
/// Detects if the network gateway is a China brand.
/// Note: This is not a perfect solution, many China brands are used overseas.
/// Please DO NOT rely on this only to determine if the user is in China.
/// </summary>
public class NetworkGatewayChinaDetector
{
    private readonly string[] _chinaGatewayKeywords = { "huawei", "china", "cmcc", "xiaomi" };

    public int Detect(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(domain));
        }

        var nslookupOutput = RunNslookup(domain);
        if (string.IsNullOrEmpty(nslookupOutput))
        {
            return 0;
        }

        var server = ParseServerValue(nslookupOutput);
        if (server == null)
        {
            return 0;
        }

        return _chinaGatewayKeywords.Any(keyword => server.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ? 1 : 0;
    }

    public string RunNslookup(string domain)
    {
        // This may only works on Windows
        var startInfo = new ProcessStartInfo("nslookup")
        {
            Arguments = domain,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process != null)
        {
            using var reader = process.StandardOutput;
            return reader.ReadToEnd();
        }

        return null;
    }

    public string ParseServerValue(string nslookupOutput)
    {
        var lines = nslookupOutput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            if (line.StartsWith("Server:"))
            {
                // The server's name is the second part of the line, after "Server:"
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    return parts[1];
                }
            }
        }

        return null;
    }
}