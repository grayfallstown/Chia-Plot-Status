using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    /**
     * This is code for the GUI, but needs to here to be part of Settings.
     * Well, it could be used to render an ascii table / cli ui, too.
     */
	public class Column
    {
        public string Name { get; set; } = "";
        public bool AlignRight { get; set; } = false;

        public Column() { }

        public Column(string name, bool alignRight) {
            this.Name = Name;
            this.AlignRight = AlignRight;
        }

        public override bool Equals(object obj)
        {
            return obj is Column column &&
                   Name == column.Name &&
                   AlignRight == column.AlignRight;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, AlignRight);
        }
    }

    public class Columns
    {
        public List<Column> cols { get; set; } = new();

        public Columns() {  }

        public void FixAddedAndRemovedColumns() {
            var defaultColumns = Default();
            // add columns that are new in this release
            foreach (var defaultCol in defaultColumns.cols)
                if (!this.cols.Contains(defaultCol))
                    this.cols.Add(defaultCol);

            // remove saved columns that were removed in this release
            List<Column> colsToRemove = new();
            foreach (var col in this.cols)
                if (!defaultColumns.cols.Contains(col))
                    colsToRemove.Add(col);
            foreach (var col in colsToRemove)
                this.cols.Remove(col);
        }

        public static Columns Default()
        {
            Columns columns = new();

            void add(string name, bool alignRight)
            {
                columns.cols.Add(new Column(name, alignRight));
            }

            add("Tmp1Drive", false);
            add("Tmp2Drive", false);
            // add("DestDrive", false);
            add("StartDate", false);
            add("FinishDate", false);
            add("Health", false);
            add("Errors", true);
            add("Progress", true);
            add("ETA", false);
            add("TimeRemaining", true);
            add("CurrentPhase", true);
            add("CurrentTable", true);
            add("CurrentBucket", true);
            add("Phase1Seconds", true);
            add("Phase2Seconds", true);
            add("Phase3Seconds", true);
            add("Phase4Seconds", true);
            add("CopyTimeSeconds", true);
            add("TotalSeconds", true);
            add("Buffer", true);
            add("Buckets", true);
            add("Threads", true);
            add("LogFolder", false);
            add("LogFile", false);
            add("ApproximateWorkingSpace", false);
            add("FinalFileSize", false);

            return columns;
        }
    }


}
