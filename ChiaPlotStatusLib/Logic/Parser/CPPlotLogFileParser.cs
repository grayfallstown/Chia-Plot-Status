using ChiaPlotStatus;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusLib.Logic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Parser
{
    class CPPlotLogFileParser
    {
        private TailLineEmitter TailLineEmitter { get; }
        protected List<CPPlotLog> PlotLogs { get; } = new();
        public string LogFile;
        public string LogFolder;
        public bool closed = false;
        public bool firstRead = false;
        public bool lineRead = false;
        public DateTime? lastGrown;

        public CPPlotLogFileParser(string path, bool closeOnEndOfFile)
        {
            this.LogFile = path;
            this.LogFolder = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
            CurrentPlotLog().StartDate = File.GetCreationTime(path);
            CurrentPlotLog().LogFile = this.LogFile;
            CurrentPlotLog().LogFolder = this.LogFolder;
            CurrentPlotLog().CurrentPhase = 1;
            this.TailLineEmitter = new TailLineEmitter(path, closeOnEndOfFile, (line) =>
            {
                lineRead = true;
                CurrentPlotLog().LastLogLine = line.Trim();
                if (CurrentPlotLog().CurrentTable == 0)
                    CurrentPlotLog().CurrentTable = 1;
                if (!firstRead)
                    lastGrown = DateTime.Now;
                MatchCollection matches;
                switch (line)
                {
                    case var _ when threadsRg.IsMatch(line):
                        matches = threadsRg.Matches(line);
                        if (CurrentPlotLog().Threads != 0)
                            NextPlotLog();
                        CurrentPlotLog().Threads = int.Parse(matches[0].Groups[1].Value);
                        CurrentPlotLog().EnterPhase(1);
                        break;
                    case var _ when bucketsRg.IsMatch(line):
                        matches = bucketsRg.Matches(line);
                        CurrentPlotLog().Buckets = int.Parse(matches[0].Groups[1].Value);
                        break;
                    case var _ when plotNameRg.IsMatch(line):
                        matches = plotNameRg.Matches(line);
                        CurrentPlotLog().PlotName = matches[0].Groups[1].Value;
                        break;
                    case var _ when tmp1Rg.IsMatch(line):
                        matches = tmp1Rg.Matches(line);
                        CurrentPlotLog().Tmp1Drive = matches[0].Groups[1].Value;
                        break;
                    case var _ when tmp2Rg.IsMatch(line):
                        matches = tmp2Rg.Matches(line);
                        CurrentPlotLog().Tmp2Drive = matches[0].Groups[1].Value;
                        break;
                    case var _ when lossRg.IsMatch(line):
                        matches = lossRg.Matches(line);
                        long lost = long.Parse(matches[0].Groups[1].Value);
                        switch (CurrentPlotLog().CurrentTable)
                        {
                            case 1:
                                CurrentPlotLog().P1Table1Lost = lost;
                                break;
                            case 2:
                                CurrentPlotLog().P1Table2Lost = lost;
                                break;
                            case 3:
                                CurrentPlotLog().P1Table3Lost = lost;
                                break;
                            case 4:
                                CurrentPlotLog().P1Table4Lost = lost;
                                break;
                            case 5:
                                CurrentPlotLog().P1Table5Lost = lost;
                                break;
                            case 6:
                                CurrentPlotLog().P1Table6Lost = lost;
                                break;
                            case 7:
                                CurrentPlotLog().P1Table7Lost = lost;
                                break;
                        }
                        if (CurrentPlotLog().P1Table1Lost == 0)
                            CurrentPlotLog().P1Table1Lost = lost;
                        else if (CurrentPlotLog().P1Table2Lost == 0)
                            CurrentPlotLog().P1Table2Lost = lost;
                        else if (CurrentPlotLog().P1Table3Lost == 0)
                            CurrentPlotLog().P1Table3Lost = lost;
                        else if (CurrentPlotLog().P1Table4Lost == 0)
                            CurrentPlotLog().P1Table4Lost = lost;
                        else if (CurrentPlotLog().P1Table5Lost == 0)
                            CurrentPlotLog().P1Table5Lost = lost;
                        else if (CurrentPlotLog().P1Table6Lost == 0)
                            CurrentPlotLog().P1Table6Lost = lost;
                        else if (CurrentPlotLog().P1Table7Lost == 0)
                            CurrentPlotLog().P1Table7Lost = lost;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table1Rg.IsMatch(line):
                        matches = p1table1Rg.Matches(line);
                        CurrentPlotLog().P1Table1 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 2;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table2Rg.IsMatch(line):
                        matches = p1table2Rg.Matches(line);
                        CurrentPlotLog().P1Table2 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P1Table2Found = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 3;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table3Rg.IsMatch(line):
                        matches = p1table3Rg.Matches(line);
                        CurrentPlotLog().P1Table3 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P1Table3Found = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 4;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table4Rg.IsMatch(line):
                        matches = p1table4Rg.Matches(line);
                        CurrentPlotLog().P1Table4 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P1Table4Found = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 5;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table5Rg.IsMatch(line):
                        matches = p1table5Rg.Matches(line);
                        CurrentPlotLog().P1Table5 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P1Table5Found = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 6;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table6Rg.IsMatch(line):
                        matches = p1table6Rg.Matches(line);
                        CurrentPlotLog().P1Table6 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P1Table6Found = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 7;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1table7Rg.IsMatch(line):
                        matches = p1table7Rg.Matches(line);
                        CurrentPlotLog().P1Table7 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P1Table7Found = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p1timeRg.IsMatch(line):
                        matches = p1timeRg.Matches(line);
                        CurrentPlotLog().P1 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().Phase1Seconds = (int)float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().EnterPhase(2);
                        break;
                    case var _ when p2timeRg.IsMatch(line):
                        matches = p2timeRg.Matches(line);
                        CurrentPlotLog().P2 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().Phase2Seconds = (int)float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().EnterPhase(3);
                        break;
                    case var _ when p3timeRg.IsMatch(line):
                        matches = p3timeRg.Matches(line);
                        CurrentPlotLog().P3 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().Phase3Seconds = (int)float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P3Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().EnterPhase(4);
                        break;
                    case var _ when p4timeRg.IsMatch(line):
                        matches = p4timeRg.Matches(line);
                        CurrentPlotLog().P4 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().Phase4Seconds = (int)float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().FinalFileSize = (long.Parse(matches[0].Groups[2].Value) / 1024 / 1024) + " MB";
                        CurrentPlotLog().EnterPhase(5);
                        break;
                    case var _ when totaltimeRg.IsMatch(line):
                        matches = totaltimeRg.Matches(line);
                        CurrentPlotLog().TotalSeconds = (int)float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().FinishDate = CurrentPlotLog().StartDate.Value.AddSeconds(CurrentPlotLog().TotalSeconds);
                        CurrentPlotLog().EnterPhase(6);
                        // TODO:
                        //if (CurrentPlotLog().QueueSize == 1 || CurrentPlotLog().QueueSize == 0)
                        //    this.Close();
                        break;

                    // phase 2
                    case var _ when p2MaxTableSize.IsMatch(line):
                        matches = p2MaxTableSize.Matches(line);
                        CurrentPlotLog().P2MaxTableSize = long.Parse(matches[0].Groups[1].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table1scanRg.IsMatch(line):
                        matches = p2table1scanRg.Matches(line);
                        CurrentPlotLog().P2Table1Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 1;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table1rewriteRg.IsMatch(line):
                        matches = p2table1rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table1Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table2Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 1;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table2scanRg.IsMatch(line):
                        matches = p2table2scanRg.Matches(line);
                        CurrentPlotLog().P2Table2Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 2;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table2rewriteRg.IsMatch(line):
                        matches = p2table2rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table2Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table2Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 1;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table3scanRg.IsMatch(line):
                        matches = p2table3scanRg.Matches(line);
                        CurrentPlotLog().P2Table3Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 3;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table3rewriteRg.IsMatch(line):
                        matches = p2table3rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table3Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table3Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 2;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table4scanRg.IsMatch(line):
                        matches = p2table4scanRg.Matches(line);
                        CurrentPlotLog().P2Table4Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 4;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table4rewriteRg.IsMatch(line):
                        matches = p2table4rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table4Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table4Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 3;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table5scanRg.IsMatch(line):
                        matches = p2table5scanRg.Matches(line);
                        CurrentPlotLog().P2Table5Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 5;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table5rewriteRg.IsMatch(line):
                        matches = p2table5rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table5Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table5Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 4;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table6scanRg.IsMatch(line):
                        matches = p2table6scanRg.Matches(line);
                        CurrentPlotLog().P2Table6Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 6;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table6rewriteRg.IsMatch(line):
                        matches = p2table6rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table6Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table6Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 5;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table7scanRg.IsMatch(line):
                        matches = p2table7scanRg.Matches(line);
                        CurrentPlotLog().P2Table7Scan = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().CurrentTable = 7;
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p2table7rewriteRg.IsMatch(line):
                        matches = p2table7rewriteRg.Matches(line);
                        CurrentPlotLog().P2Table7Rewrite = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P2Table7Lost = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().CurrentTable = 6;
                        CurrentPlotLog().NextPhasePart();
                        break;


                    // phase 3
                    case var _ when p31table2Rg.IsMatch(line):
                        matches = p31table2Rg.Matches(line);
                        CurrentPlotLog().P31Table2 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table2Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 3;
                        break;
                    case var _ when p31table3Rg.IsMatch(line):
                        matches = p31table3Rg.Matches(line);
                        CurrentPlotLog().P31Table3 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table3Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 4;
                        break;
                    case var _ when p31table4Rg.IsMatch(line):
                        matches = p31table4Rg.Matches(line);
                        CurrentPlotLog().P31Table4 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table4Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 5;
                        break;
                    case var _ when p31table5Rg.IsMatch(line):
                        matches = p31table5Rg.Matches(line);
                        CurrentPlotLog().P31Table5 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table5Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 6;
                        break;
                    case var _ when p31table6Rg.IsMatch(line):
                        matches = p31table6Rg.Matches(line);
                        CurrentPlotLog().P31Table6 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table6Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 7;
                        break;
                    case var _ when p31table7Rg.IsMatch(line):
                        matches = p31table7Rg.Matches(line);
                        CurrentPlotLog().P31Table7 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table7Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 1;
                        break;
                    case var _ when p31table2Rg.IsMatch(line):
                        matches = p31table2Rg.Matches(line);
                        CurrentPlotLog().P31Table2 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P31Table2Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p32table2Rg.IsMatch(line):
                        matches = p32table2Rg.Matches(line);
                        CurrentPlotLog().P32Table2 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P32Table2Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p32table3Rg.IsMatch(line):
                        matches = p32table3Rg.Matches(line);
                        CurrentPlotLog().P32Table3 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P32Table3Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p32table4Rg.IsMatch(line):
                        matches = p32table4Rg.Matches(line);
                        CurrentPlotLog().P32Table4 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P32Table4Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p32table5Rg.IsMatch(line):
                        matches = p32table5Rg.Matches(line);
                        CurrentPlotLog().P32Table5 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P32Table5Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p32table6Rg.IsMatch(line):
                        matches = p32table6Rg.Matches(line);
                        CurrentPlotLog().P32Table6 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P32Table6Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        break;
                    case var _ when p32table7Rg.IsMatch(line):
                        matches = p32table7Rg.Matches(line);
                        CurrentPlotLog().P32Table7 = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                        CurrentPlotLog().P32Table7Entries = long.Parse(matches[0].Groups[2].Value);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 1;
                        break;
                    case var _ when p4start1Rg.IsMatch(line):
                        matches = p4start1Rg.Matches(line);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 1;
                        break;
                    case var _ when p4finish1Rg.IsMatch(line):
                        matches = p4finish1Rg.Matches(line);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 2;
                        break;
                    case var _ when p4start2Rg.IsMatch(line):
                        matches = p4start2Rg.Matches(line);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 2;
                        break;
                    case var _ when p4finish2Rg.IsMatch(line):
                        matches = p4finish2Rg.Matches(line);
                        CurrentPlotLog().NextPhasePart();
                        CurrentPlotLog().CurrentTable = 1;
                        break;

                    // errors
                    case var _ when segFaultRg.IsMatch(line):
                    case var _ when coreDumpRg.IsMatch(line):
                    case var _ when catchExceptionRg.IsMatch(line):
                    case var _ when invalidArgumentRg.IsMatch(line):
                    case var _ when terminateRg.IsMatch(line):
                    case var _ when failedRg.IsMatch(line):
                        CurrentPlotLog().CaughtPlottingError = true;
                        CurrentPlotLog().Health = new ConfirmedDead(false);
                        break;
                }
                CurrentPlotLog().UpdateProgress();
            });
        }


        /**
         * Can be called as often as needed as it does not reparse what was already processed.
         */
        public List<CPPlotLog> ParseCPPlotLog()
        {
            if (closed)
                return PlotLogs;
            lineRead = false;
            this.TailLineEmitter.ReadMore();
            CurrentPlotLog().FileLastWritten = File.GetLastWriteTime(this.LogFile);
            if (CurrentPlotLog().FileLastWritten != null && ((DateTime.Now - (DateTime)CurrentPlotLog().FileLastWritten).TotalDays > 1d))
                Close();
            if (lineRead && !firstRead)
                lastGrown = DateTime.Now;
            if (lastGrown != null)
                CurrentPlotLog().FileLastWritten = lastGrown;
            firstRead = false;

            return PlotLogs;
        }


        private CPPlotLog CurrentPlotLog()
        {
            if (PlotLogs.Count == 0)
                PlotLogs.Add(new());
            return PlotLogs[PlotLogs.Count - 1];
        }

        private CPPlotLog NextPlotLog()
        {
            var oldPlotLog = CurrentPlotLog();
            oldPlotLog.IsLastInLogFile = false;
            var newPlotLog = new CPPlotLog();
            // when plot create --num n is used parameters stay the same
            newPlotLog.Buckets = oldPlotLog.Buckets;
            newPlotLog.Threads = oldPlotLog.Threads;
            newPlotLog.Buffer = oldPlotLog.Buffer;
            newPlotLog.Tmp1Drive = oldPlotLog.Tmp1Drive;
            newPlotLog.Tmp2Drive = oldPlotLog.Tmp2Drive;
            newPlotLog.DestDrive = oldPlotLog.DestDrive;
            newPlotLog.LogFile = oldPlotLog.LogFile;
            newPlotLog.LogFolder = oldPlotLog.LogFolder;
            newPlotLog.PlaceInLogFile = oldPlotLog.PlaceInLogFile + 1;
            newPlotLog.QueueSize = oldPlotLog.QueueSize;
            newPlotLog.StartDate = oldPlotLog.StartDate.Value.AddSeconds(oldPlotLog.TotalSeconds);
            PlotLogs.Add(newPlotLog);
            return newPlotLog;
        }


        private void Close()
        {
            if (closed)
                return;
            Debug.WriteLine("CPPlotLog " + LogFile + " has not been updated for more than a day. Closing file.");
            this.TailLineEmitter.Close();
            closed = true;
        }

        static Regex threadsRg = new Regex("Number of Threads: (\\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex bucketsRg = new Regex("Number of Sort Buckets: 2\\^\\d+ \\((\\d+)\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex plotNameRg = new Regex("Plot Name: (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex tmp1Rg = new Regex("Working Directory:\\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex tmp2Rg = new Regex("Working Directory 2:\\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table1Rg = new Regex("\\[P1\\] Table 1 took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table2Rg = new Regex("\\[P1\\] Table 2 took ([0-9.]+) sec, found (\\d+) matches", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table3Rg = new Regex("\\[P1\\] Table 3 took ([0-9.]+) sec, found (\\d+) matches", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table4Rg = new Regex("\\[P1\\] Table 4 took ([0-9.]+) sec, found (\\d+) matches", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table5Rg = new Regex("\\[P1\\] Table 5 took ([0-9.]+) sec, found (\\d+) matches", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table6Rg = new Regex("\\[P1\\] Table 6 took ([0-9.]+) sec, found (\\d+) matches", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1table7Rg = new Regex("\\[P1\\] Table 7 took ([0-9.]+) sec, found (\\d+) matches", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex lossRg = new Regex("\\[P1\\] Lost (\\d+) matches due to 32-bit overflow.", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p1timeRg = new Regex("Phase 1 took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2timeRg = new Regex("Phase 2 took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p3timeRg = new Regex("Phase 3 took ([0-9.]+) sec, wrote (\\d+) entries to final plot", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p4timeRg = new Regex("Phase 4 took ([0-9.]+) sec, final plot size is (\\d+) bytes", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex totaltimeRg = new Regex("Total plot creation time was ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2MaxTableSize = new Regex("\\[P2\\] max_table_size = (\\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table1scanRg = new Regex("\\[P2\\] Table 1 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table2scanRg = new Regex("\\[P2\\] Table 2 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table3scanRg = new Regex("\\[P2\\] Table 3 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table4scanRg = new Regex("\\[P2\\] Table 4 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table5scanRg = new Regex("\\[P2\\] Table 5 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table6scanRg = new Regex("\\[P2\\] Table 6 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table7scanRg = new Regex("\\[P2\\] Table 7 scan took ([0-9.]+) sec", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table1rewriteRg = new Regex("\\[P2\\] Table 1 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table2rewriteRg = new Regex("\\[P2\\] Table 2 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table3rewriteRg = new Regex("\\[P2\\] Table 3 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table4rewriteRg = new Regex("\\[P2\\] Table 4 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table5rewriteRg = new Regex("\\[P2\\] Table 5 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table6rewriteRg = new Regex("\\[P2\\] Table 6 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p2table7rewriteRg = new Regex("\\[P2\\] Table 7 rewrite took ([0-9.]+) sec, dropped (\\d+) entries \\(([0-9.]+) %\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p31table2Rg = new Regex("\\[P3-1\\] Table 2 took ([0-9.]+) sec, wrote (\\d+) right entries", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p31table3Rg = new Regex("\\[P3-1\\] Table 3 took ([0-9.]+) sec, wrote (\\d+) right entries", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p31table4Rg = new Regex("\\[P3-1\\] Table 4 took ([0-9.]+) sec, wrote (\\d+) right entries", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p31table5Rg = new Regex("\\[P3-1\\] Table 5 took ([0-9.]+) sec, wrote (\\d+) right entries", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p31table6Rg = new Regex("\\[P3-1\\] Table 6 took ([0-9.]+) sec, wrote (\\d+) right entries", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p31table7Rg = new Regex("\\[P3-1\\] Table 7 took ([0-9.]+) sec, wrote (\\d+) right entries", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p32table2Rg = new Regex("\\[P3-2\\] Table 2 took ([0-9.]+) sec, wrote (\\d+) left entries, (\\d+) final", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p32table3Rg = new Regex("\\[P3-2\\] Table 3 took ([0-9.]+) sec, wrote (\\d+) left entries, (\\d+) final", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p32table4Rg = new Regex("\\[P3-2\\] Table 4 took ([0-9.]+) sec, wrote (\\d+) left entries, (\\d+) final", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p32table5Rg = new Regex("\\[P3-2\\] Table 5 took ([0-9.]+) sec, wrote (\\d+) left entries, (\\d+) final", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p32table6Rg = new Regex("\\[P3-2\\] Table 6 took ([0-9.]+) sec, wrote (\\d+) left entries, (\\d+) final", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p32table7Rg = new Regex("\\[P3-2\\] Table 7 took ([0-9.]+) sec, wrote (\\d+) left entries, (\\d+) final", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p4start1Rg = new Regex("\\[P4\\] Starting to write C1 and C3 tables", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p4finish1Rg = new Regex("\\[P4\\] Finished to write C1 and C3 tables", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p4start2Rg = new Regex("\\[P4\\] Starting to write C2 tables", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex p4finish2Rg = new Regex("\\[P4\\] Finished to write C2 tables", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // errors
        static Regex segFaultRg = new Regex("(Segmentation fault|SIGSEGV|segfault)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex coreDumpRg = new Regex("(core dumped|coredump)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex catchExceptionRg = new Regex("Error", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex invalidArgumentRg = new Regex("Invalid", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex terminateRg = new Regex("terminat", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex failedRg = new Regex("failed", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
