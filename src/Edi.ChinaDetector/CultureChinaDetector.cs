using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Edi.ChinaDetector;

public class CultureChinaDetector(CultureInfo culture = null, CultureInfo uiCulture = null)
{
    public async Task<int> Detect()
    {
        culture ??= CultureInfo.CurrentCulture;
        uiCulture ??= CultureInfo.CurrentUICulture;

        int rank = 0;

        if (culture.Name == "zh-CN" ||
            culture.Name == "zh-Hans" ||
            culture.Name == "zh-Hans-CN" ||
            culture.EnglishName.Contains("china", StringComparison.InvariantCultureIgnoreCase))
        {
            rank++;
        }

        if (uiCulture.Name == "zh-CN" ||
            uiCulture.Name == "zh-Hans" ||
            uiCulture.Name == "zh-Hans-CN" ||
            uiCulture.EnglishName.Contains("china", StringComparison.InvariantCultureIgnoreCase))
        {
            rank++;
        }

        if (Environment.MachineName.Any(c => c >= 0x4e00 && c <= 0x9fff))
        {
            rank++;
        }

        if (rank == 0)
        {
            // User is extremely cunning and sly, try more sick method to detect if he/she is from China

            // Try using DISM to get system locale info (Windows only)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "dism.exe",
                    Arguments = "/online /get-intl",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using Process process = new();
                process.StartInfo = psi;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    if (output.Contains("zh-CN", StringComparison.InvariantCultureIgnoreCase) ||
                        output.Contains("zh-Hans", StringComparison.InvariantCultureIgnoreCase))
                    {
                        rank++;
                    }
                }
                else
                {
                    // DISM failed (mostly because program is not running as admin)
                    // A lot of Chinese people like to run programs as admin and disable UAC, so this is a good indicator
                    Debug.WriteLine($"DISM failed with exit code {process.ExitCode}");
                }
            }
        }

        return rank;
    }

}