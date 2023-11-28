namespace Edi.ChinaDetector;

public class TimeZoneChinaDetector(TimeZoneInfo timeZone)
{
    public int Detect()
    {
        timeZone ??= TimeZoneInfo.Local;

        if (timeZone.Id == "China Standard Time" ||
            timeZone.StandardName.Contains("china", StringComparison.CurrentCultureIgnoreCase))
        {
            return 1;
        }

        return 0;
    }
}