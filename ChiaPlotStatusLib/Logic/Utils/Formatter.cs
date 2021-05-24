using ChiaPlotStatus.Logic.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Utils
{
    public class Formatter
    {

        public static string formatCpuUsage(double usage)
        {
            if (usage < 0.01d)
                return "";
            else
                return usage.ToString("0.0 '%'");
        }

        public static string formatSeconds(int seconds)
        {
            if (seconds == 0)
                return "";
            else if (seconds > 24 * 60 * 60)
                return TimeSpan.FromSeconds(seconds).ToString(@"dd\d\ hh\h\ mm\m\ ss\s");
            else if (seconds > 60 * 60)
                return TimeSpan.FromSeconds(seconds).ToString(@"hh\h\ mm\m\ ss\s");
            else if (seconds > 60)
                return TimeSpan.FromSeconds(seconds).ToString(@"mm\m\ ss\s");
            else
                return TimeSpan.FromSeconds(seconds).ToString(@"ss\s");
        }

        public static string formatDateTime(DateTime? dateTime)
        {
            if (dateTime == null)
                return "";
            else {
                CultureInfo culture = CultureInfo.CurrentCulture;
                // forced to cast it to a non nullable or it does not find ToString(format)
                return ((DateTime)dateTime).ToString("m", culture) + " " + ((DateTime)dateTime).ToString("t", culture);
            }
        }

        public static string formatHealth(HealthIndicator health)
        {
            switch (health)
            {
                case Healthy:
                    return "✓";
                case TempError:
                    return "⚠ Temp Errors";
                case Concerning c:
                    return "⚠ Slow " + c.Minutes + " / " + c.ExpectedMinutes + "m";
                case PossiblyDead p:
                    return "⚠ Dead? " + p.Minutes + " / " + p.ExpectedMinutes + "m";
                case ConfirmedDead c:
                    return "✗ Dead" + (c.Manual ? " (m)" : "");
                default:
                    throw new NotImplementedException("formatHealth (HealthIndicator " + health + ")");
            }
        }

    }
}
