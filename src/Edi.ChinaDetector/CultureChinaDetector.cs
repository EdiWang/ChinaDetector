using System.Globalization;

namespace Edi.ChinaDetector;

public class CultureChinaDetector(CultureInfo culture = null, CultureInfo uiCulture = null)
{
    public int Detect()
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

        return rank;
    }
}