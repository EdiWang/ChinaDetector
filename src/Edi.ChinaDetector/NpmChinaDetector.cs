using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Edi.ChinaDetector;

public class NpmChinaDetector
{
    public async Task<int> Detect()
    {
        try
        {
            int rank = 0;

            string source = await GetNpmSource();

            if (source.Contains("taobao") || source.Contains("tencent"))
            {
                rank++;
            }

            return rank;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    private async Task<string> GetNpmSource()
    {
        Process process = new Process();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c npm config get registry";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = "-c \"npm config get registry\"";
        }

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return output;
    }
}