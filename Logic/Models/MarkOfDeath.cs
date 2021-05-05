using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Logic.Models
{
    /**
     * Manually marks a PlogLog as dead, so it can be hidden
     */
    public class MarkOfDeath
    {
        public string LogFolder { get; set; }
        public string LogFile { get; set; }
        public int PlaceInLogFile { get; set; }

        public MarkOfDeath() { }

        public MarkOfDeath(PlotLogReadable plotLogReadable) {
            this.LogFolder = plotLogReadable.LogFolder;
            this.LogFile = plotLogReadable.LogFile;
            this.PlaceInLogFile = plotLogReadable.PlaceInLogFile;
        }

        public bool IsMatch(PlotLog plotLog)
        {
            if (!string.Equals(this.LogFolder, plotLog.LogFolder)) return false;
            if (!string.Equals(this.LogFile, plotLog.LogFile)) return false;
            if (!string.Equals(this.PlaceInLogFile, plotLog.PlaceInLogFile)) return false;
            return true;
        }

        public bool IsMatch(PlotLogReadable plotLog)
        {
            if (!string.Equals(this.LogFolder, plotLog.LogFolder)) return false;
            if (!string.Equals(this.LogFile, plotLog.LogFile)) return false;
            if (!string.Equals(this.PlaceInLogFile, plotLog.PlaceInLogFile)) return false;
            return true;
        }
    }
}
