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
        public Dictionary<string, bool> ShowColumn { get; set; } = new();
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
            {
                if (!this.Order.Contains(defaultCol))
                    this.Order.Add(defaultCol);
                if (!this.ShowColumn.ContainsKey(defaultCol))
                    this.ShowColumn.Add(defaultCol, true);
            }

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

            void Add(string col) {
                columns.Order.Add(col);
                if (!columns.ShowColumn.ContainsKey(col))
                    columns.ShowColumn.Add(col, true);
            }

            Add("Note");
            Add("Tmp1Drive");
            Add("Tmp2Drive");
            Add("DestDrive");
            Add("StartDate");
            Add("FinishDate");
            Add("Health");
            Add("Errors");
            Add("Progress");
            Add("ETA");
            Add("TimeRemaining");
            Add("RunTimeSeconds");
            Add("CurrentPhase");
            Add("CurrentTable");
            Add("CurrentBucket");
            Add("Phase1Seconds");
            Add("Phase1Cpu");
            Add("Phase2Seconds");
            Add("Phase2Cpu");
            Add("Phase3Seconds");
            Add("Phase3Cpu");
            Add("Phase4Seconds");
            Add("Phase4Cpu");
            Add("CopyTimeSeconds");
            Add("TotalSeconds");
            Add("Buffer");
            Add("Buckets");
            Add("Threads");
            Add("LogFolder");
            Add("LogFile");
            Add("PlaceInLogFile");
            Add("PID");
            Add("ApproximateWorkingSpace");
            Add("FinalFileSize");
            Add("LastLogLine");
            Add("PoolPuzzleHash");

            return columns;
        }
    }


}
