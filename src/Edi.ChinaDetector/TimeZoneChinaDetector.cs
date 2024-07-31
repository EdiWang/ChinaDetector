namespace Edi.ChinaDetector;

public class TimeZoneChinaDetector(TimeZoneInfo timeZone)
{
    public int Detect()
    {
        timeZone ??= TimeZoneInfo.Local;

        /* Not all regions in the +8 time zone use the name "China Standard Time" (CST).
           For example, the +8 time zone also includes the following time zone names:
           
           Australian Western Standard Time (AWST)
           Singapore Time (SGT)
           Hong Kong Time (HKT)
           Malaysia Time (MYT)
           Philippine Time (PHT)
           Western Indonesian Time (WIB) */

        if (timeZone.Id == "China Standard Time" ||
            timeZone.Id == "Hong Kong Time (HKT)" || // Hong Kong is considered part of China
            timeZone.Id == "Asia/Shanghai" ||
            timeZone.StandardName.Contains("china", StringComparison.CurrentCultureIgnoreCase))
        {
            return 1;
        }

        return 0;
    }
}