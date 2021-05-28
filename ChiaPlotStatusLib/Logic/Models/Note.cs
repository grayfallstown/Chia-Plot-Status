using ChiaPlotStatus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    /**
     * A little notice or a collection of tags you can attach to a plot log <file>
     * It affects all plot logs in the same log file, which allows to add the
     * GUI Plotter Queue Name to Chia Plot Status.
     */
    public class Note
    {
        public string LogFolder { get; set; }
        public string LogFile { get; set; }
        public string text { get; set; }

        public Note() { }

        public Note(PlotLogReadable plotLogReadable)
        {
            this.LogFolder = plotLogReadable.LogFolder;
            this.LogFile = plotLogReadable.LogFile;
            this.text = plotLogReadable.Note;
        }


        public bool IsMatch(PlotLog plotLog)
        {
            if (!string.Equals(this.LogFolder, plotLog.LogFolder)) return false;
            string logFileName = plotLog.LogFile.Substring(plotLog.LogFile.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            if (!string.Equals(this.LogFile, logFileName)) return false;
            return true;
        }

        public bool IsMatch(PlotLogReadable plotLog)
        {
            if (!string.Equals(this.LogFolder, plotLog.LogFolder)) return false;
            if (!string.Equals(this.LogFile, plotLog.LogFile)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Note note &&
                   LogFolder == note.LogFolder &&
                   LogFile == note.LogFile;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LogFolder, LogFile);
        }
    }
}
