namespace Edi.ChinaDetector;

public class TimeZoneChinaDetector(TimeZoneInfo timeZone, bool includeHKTW = false)
{
    public int Detect()
    {
        timeZone ??= TimeZoneInfo.Local;

        /* Not all regions in the +8 time zone use the name "China Standard Time" (CST).
           For example, the +8 time zone also includes the following time zone names:
           
           Australian Western Standard Time (AWST)
           Singapore Time (SGT)
           Hong Kong Time (HKT)
           Taiwan Standard Time (TST)
           Malaysia Time (MYT)
           Philippine Time (PHT)
           Western Indonesian Time (WIB) 
        */

        // Reference: https://www.iana.org/time-zones

        if (includeHKTW)
        {
            // Consider Hong Kong and Taiwan as part of China
            if (timeZone.Id == "Taipei Standard Time" || // Windows time zone
                timeZone.Id == "Taiwan Standard Time" || // https://en.wikipedia.org/wiki/Time_in_Taiwan
                timeZone.Id == "Hong Kong Time" || // https://en.wikipedia.org/wiki/Hong_Kong_Time
                timeZone.Id == "Asia/Taipei " ||
                timeZone.Id == "Asia/Hong_Kong" ||
                timeZone.StandardName.Contains("taiwan", StringComparison.CurrentCultureIgnoreCase) ||
                timeZone.StandardName.Contains("taipei", StringComparison.CurrentCultureIgnoreCase) ||
                timeZone.StandardName.Contains("hong kong", StringComparison.CurrentCultureIgnoreCase))
            {
                return 1;
            }
        }

        // Note: On Windows, Hong Kong is using "China Standard Time" time zone
        // So even if we exclude Hong Kong and Taiwan, the result will still be positive
        if (timeZone.Id == "China Standard Time" ||
            timeZone.Id == "Asia/Shanghai" ||
            timeZone.StandardName.Contains("china", StringComparison.CurrentCultureIgnoreCase))
        {
            return 1;
        }

        return 0;
    }
}