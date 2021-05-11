using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Logic.Models
{
    public interface HealthIndicator
    {
        public string Name { get; }
        public int SortIndex { get; }
    }

    public class Healthy : HealthIndicator
    {
        public string Name { get; } = "Healthy";
        public int SortIndex { get; } = 1;
        private Healthy() { }
        public static Healthy Instance = new();
    }

    public class TempError : HealthIndicator
    {
        public string Name { get; } = "TempError";
        public int SortIndex { get; } = 2;
        private TempError() { }
        public static TempError Instance = new();
    }

    public class Concerning : HealthIndicator
    {
        public string Name { get; } = "Concerning";
        public int SortIndex { get; } = 3;
        public float Minutes { get; set; }
        public float ExpectedMinutes { get; set; }

        public Concerning(float minutes, float expectedMinutes) {
            this.Minutes = minutes;
            this.ExpectedMinutes = expectedMinutes;
        }
    }

    public class PossiblyDead : HealthIndicator
    {
        public string Name { get; } = "PossiblyDead";
        public int SortIndex { get; } = 4;
        public float Minutes { get; set; }
        public float ExpectedMinutes { get; set; }

        public PossiblyDead(float minutes, float expectedMinutes)
        {
            this.Minutes = minutes;
            this.ExpectedMinutes = expectedMinutes;
        }
    }

    public class ConfirmedDead : HealthIndicator
    {
        public string Name { get; } = "ConfirmedDead";
        public int SortIndex { get; } = 5;
        public bool Manual { get; set; }
        public ConfirmedDead(bool manual)
        {
            this.Manual = manual;
        }
    }



    /**
     * // apparently csharp cannot do this? Which lang did tho? Scala?
     * public enum Health
        {
            HEALTHY,
            TEMP_ERROR,
            CONCERNING(float minutes),
            POSSIBLY_DEAD(float minutes),
            CONFIRMED_DEAD(bool manual)
        }
*/
}
