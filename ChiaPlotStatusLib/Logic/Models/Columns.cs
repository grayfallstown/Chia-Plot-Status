using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    /**
     * This is code for the GUI, but needs to here to be part of Settings.
     * Well, it could be used to render an ascii table / cli ui, too.
     */
    public class Columns
    {
        public List<string> Order { get; set; } = new();
        public Dictionary<string, int> Widths { get; set; } = new();

        public Columns() {  }

        public int IndexOf(string columnName)
        {
            for (int i = 0; i < Order.Count; i++)
                if (columnName == Order[i])
                    return i;
            return -1;
        }

        public void FixAddedAndRemovedColumns() {
            var defaultColumns = Default();
            // add columns that are new in this release
            foreach (var defaultCol in defaultColumns.Order)
                if (!this.Order.Contains(defaultCol))
                    this.Order.Add(defaultCol);

            // remove saved columns that were removed in this release
            List<string> colsToRemove = new();
            foreach (var col in this.Order)
                if (!defaultColumns.Order.Contains(col))
                    colsToRemove.Add(col);
            foreach (var col in colsToRemove)
                this.Order.Remove(col);
        }

        public static Columns Default()
        {
            Columns columns = new();

            columns.Order.Add("Note");
            columns.Order.Add("Tmp1Drive");
            columns.Order.Add("Tmp2Drive");
            columns.Order.Add("DestDrive");
            columns.Order.Add("StartDate");
            columns.Order.Add("FinishDate");
            columns.Order.Add("Health");
            columns.Order.Add("Errors");
            columns.Order.Add("Progress");
            columns.Order.Add("ETA");
            columns.Order.Add("TimeRemaining");
            columns.Order.Add("RunTimeSeconds");
            columns.Order.Add("CurrentPhase");
            columns.Order.Add("CurrentTable");
            columns.Order.Add("CurrentBucket");
            columns.Order.Add("Phase1Seconds");
            columns.Order.Add("Phase1Cpu");
            columns.Order.Add("Phase2Seconds");
            columns.Order.Add("Phase2Cpu");
            columns.Order.Add("Phase3Seconds");
            columns.Order.Add("Phase3Cpu");
            columns.Order.Add("Phase4Seconds");
            columns.Order.Add("Phase4Cpu");
            columns.Order.Add("CopyTimeSeconds");
            columns.Order.Add("TotalSeconds");
            columns.Order.Add("Buffer");
            columns.Order.Add("Buckets");
            columns.Order.Add("Threads");
            columns.Order.Add("LogFolder");
            columns.Order.Add("LogFile");
            columns.Order.Add("PlaceInLogFile");
            columns.Order.Add("PID");
            columns.Order.Add("ApproximateWorkingSpace");
            columns.Order.Add("FinalFileSize");
            columns.Order.Add("LastLogLine");

            return columns;
        }
    }


}
