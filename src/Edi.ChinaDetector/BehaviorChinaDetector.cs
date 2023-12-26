using Microsoft.Win32;
using System.Runtime.InteropServices;
#pragma warning disable CA1416

namespace Edi.ChinaDetector;

public class BehaviorChinaDetector
{
    public int Detect()
    {
        var rank = 0;

        // Test famous Chinese apps
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var x64ProgramPath = Environment.GetEnvironmentVariable("ProgramFiles");
            var x86ProgramPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            string[] appPaths = {
                Path.Join(x64ProgramPath, "Tencent"),
                Path.Join(x64ProgramPath, "360"),
                Path.Join(x86ProgramPath, "SogouInput"),
                Path.Join(userProfilePath, @"AppData\Roaming\baidu")
            };

            var hasChineseApps = appPaths.Any(Directory.Exists);

            if (!hasChineseApps)
            {
                // User is extremely cunning and sly, try more sick method to detect if he/she is from China
                var installedPrograms = InstalledPrograms.GetInstalledPrograms();
                string[] chineseProgramKeyWords = { "tencent", "baidu", "360" };

                hasChineseApps = installedPrograms.Any(p => chineseProgramKeyWords.Any(k => p.ToLower().Contains(k)));

                if (!hasChineseApps)
                {
                    // Check `installedPrograms` for Chinese characters
                    hasChineseApps = installedPrograms.Any(p => p.Any(c => c >= 0x4e00 && c <= 0x9fff));
                }
            }

            if (hasChineseApps)
            {
                rank++;
            }
        }

        // Test user name, if it looks like a Chinese name, then it's probably a Chinese
        if (Environment.UserName.Length > 1)
        {
            if (Environment.UserName[0] >= 0x4e00 && Environment.UserName[0] <= 0x9fff)
            {
                rank++;
            }
        }

        return rank;
    }
}

public class InstalledPrograms
{
    private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

    public static List<string> GetInstalledPrograms()
    {
        var result = new List<string>();
        using (var key = Registry.LocalMachine.OpenSubKey(RegistryKey))
        {
            if (key != null)
            {
                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    using var subkey = key.OpenSubKey(subkeyName);
                    var displayName = subkey?.GetValue("DisplayName");
                    if (displayName != null)
                    {
                        result.Add(displayName.ToString());
                    }
                }
            }
        }

        // 对于32位应用程序，在64位系统上还需要检查以下额外的注册表路径
        if (Environment.Is64BitOperatingSystem)
        {
            using var key = Registry.LocalMachine.OpenSubKey(RegistryKey.Replace(@"SOFTWARE\", @"SOFTWARE\WOW6432Node\"));
            if (key != null)
            {
                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    using var subkey = key.OpenSubKey(subkeyName);
                    if (subkey == null) continue;
                    var displayName = subkey.GetValue("DisplayName");
                    if (displayName != null)
                    {
                        result.Add(displayName.ToString());
                    }
                }
            }
        }

        return result;
    }
}