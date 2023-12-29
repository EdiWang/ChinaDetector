using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
#pragma warning disable CA1416

namespace Edi.ChinaDetector;

public class BehaviorChinaDetector
{
    static readonly string[] ChineseProgramKeyWords =
    {
        "tencent", "baidu", "360",
        "腾讯", "百度", "阿里", "网易", "迅雷", "搜狗", "钉钉",
        "安全", "杀毒", "管家", "金山", "卫士", "爱奇艺", "优酷",
        "好压", "浏览器", "向日葵", "远控", "微信", "WPS", "直播",
        "游戏", "视频", "音乐", "影音", "影视", "播放器", "输入法",
        "拼音", "助手", "看图", "翻译", "抖音"
    };

    public int Detect()
    {
        var rank = 0;

        // Test famous Chinese apps
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            rank = DetectWindowsPrograms(rank);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            rank = DetectLinuxPrograms(rank);
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

    private static int DetectWindowsPrograms(int rank)
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
            var installedPrograms = InstalledPrograms.GetWindowsPrograms();

            hasChineseApps = installedPrograms.Any(p => ChineseProgramKeyWords.Any(k => p.ToLower().Contains(k)));

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

        return rank;
    }

    private static int DetectLinuxPrograms(int rank)
    {
        var installedPrograms = InstalledPrograms.GetLinuxPrograms();
        var hasChineseApps = installedPrograms.Any(p => ChineseProgramKeyWords.Any(k => p.ToLower().Contains(k)));

        if (!hasChineseApps)
        {
            // Check `installedPrograms` for Chinese characters
            hasChineseApps = installedPrograms.Any(p => p.Any(c => c >= 0x4e00 && c <= 0x9fff));
        }

        if (hasChineseApps)
        {
            rank++;
        }

        return rank;
    }
}

public class InstalledPrograms
{
    private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

    public static List<string> GetWindowsPrograms()
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

    public static List<string> GetLinuxPrograms()
    {
        // This list will hold the names of all installed packages
        var installedPackages = new List<string>();

        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = "-c \"dpkg --get-selections | grep -v deinstall\"", // This command lists installed packages, filtering out deinstalled ones
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        using (var reader = process.StandardOutput)
        {
            while (reader.ReadLine() is { } line)
            {
                // Each line contains the package name followed by a tab and the word "install"
                // So we split the line and get the first part, which is the package name
                var packageName = line.Split(['\t'], 2)[0];
                installedPackages.Add(packageName);
            }
        }

        process.WaitForExit();
        return installedPackages;
    }
}