using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using ChiaPlotStatus.GUI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ChiaPlotStatus.Views
{
    public class LogViewerWindow : Window
    {
        public static LogViewerWindow? Instance { get; private set; }
        public TailLineEmitter TailLineEmitter { get; private set; }
        public StringBuilder htmlBlock = new();
        public DispatcherTimer Timer { get; private set; }
        public string Path{ get; private set; }

        public StackPanel LogLines;

        public LogViewerWindow()
        {
        }

        public LogViewerWindow(string path)
        {
            this.Path = path;
            Instance = this;
            DataContext = this;
            InitializeTailLineEmitter();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            InitializeResizer();
            InitializeRefresh();
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            // this.LogLines = this.Find<StackPanel>("LogLines");
        }

        private void InitializeTailLineEmitter()
        {
            this.TailLineEmitter = new TailLineEmitter(this.Path, (line) =>
            {
                StringBuilder htmlLine = new StringBuilder("<p class=\"line\">");

                void highlight(int level, string text)
                {
                    htmlLine.Append("<span class=\"level" + level + "\">");
                    htmlLine.Append(HttpUtility.HtmlEncode(text));
                    htmlLine.Append("</span>");
                }

                MatchCollection matches;
                if (PlotLogFileParser.plotSizeRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.plotSizeRg.Matches(line);
                    highlight(1, "Plot size is: ");
                    highlight(2, matches[0].Groups[1].Value);
                } else if (PlotLogFileParser.bufferSizeRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.bufferSizeRg.Matches(line);
                    highlight(1, "Buffer size is: ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (PlotLogFileParser.bucketsRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.bucketsRg.Matches(line);
                    highlight(1, "Using ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " buckets");
                }
                else if (PlotLogFileParser.threadsRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.threadsRg.Matches(line);
                    highlight(1, "Using ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " threads");
                }
                else if (PlotLogFileParser.threadsRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.threadsRg.Matches(line);
                    highlight(1, "Using ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " threads");
                }
                else if (PlotLogFileParser.startDateRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.startDateRg.Matches(line);
                    highlight(1, "Starting phase 1/4: Forward Propagation into tmp files... ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (PlotLogFileParser.phase1Rg.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase1Rg.Matches(line);
                    highlight(1, "Time for phase 1 = ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " seconds");
                }
                else if (PlotLogFileParser.phase2Rg.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase2Rg.Matches(line);
                    highlight(1, "Time for phase 2 = ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " seconds");
                }
                else if (PlotLogFileParser.phase3Rg.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase3Rg.Matches(line);
                    highlight(1, "Time for phase 3 = ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " seconds");
                }
                else if (PlotLogFileParser.phase4Rg.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase4Rg.Matches(line);
                    highlight(1, "Time for phase 4 = ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " seconds");
                }
                else if (PlotLogFileParser.totalTimeRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.totalTimeRg.Matches(line);
                    highlight(1, "Total time = ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " seconds");
                }
                else if (PlotLogFileParser.plotNameRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.plotNameRg.Matches(line);
                    var parts = line.Split(matches[0].Groups[1].Value);
                    highlight(1, parts[0]);
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, parts[1]);
                }
                else if (PlotLogFileParser.currentBucketRg.IsMatch(line))
                {
                    matches = PlotLogFileParser.currentBucketRg.Matches(line);
                    highlight(1, "            Bucket ");
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(0, line.Split(matches[0].Groups[1].Value)[1]);
                }
                else if (PlotLogFileParser.phase1Table.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase1Table.Matches(line);
                    highlight(1, "Computing table ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (PlotLogFileParser.phase2Table.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase2Table.Matches(line);
                    highlight(1, "scanned table ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (PlotLogFileParser.phase3Table.IsMatch(line))
                {
                    matches = PlotLogFileParser.phase3Table.Matches(line);
                    highlight(1, "Compressing tables ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (PlotLogFileParser.tmpFolders.IsMatch(line))
                {
                    matches = PlotLogFileParser.tmpFolders.Matches(line);
                    var parts = line.Split(matches[0].Groups[1].Value);
                    highlight(1, parts[0]);
                    highlight(2, matches[0].Groups[1].Value);
                    highlight(1, " and ");
                    highlight(2, matches[0].Groups[2].Value);
                }
                else if (PlotLogFileParser.writePloblemRg.IsMatch(line))
                {
                    highlight(2, line);
                }
                else if (PlotLogFileParser.readPloblemRg.IsMatch(line))
                {
                    highlight(2, line);
                }
                else if (PlotLogFileParser.approximateWorkingSpace.IsMatch(line))
                {
                    matches = PlotLogFileParser.approximateWorkingSpace.Matches(line);
                    var parts = line.Split(matches[0].Groups[1].Value);
                    highlight(1, "Approximate working space used (without final file): ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (PlotLogFileParser.finalFileSize.IsMatch(line))
                {
                    matches = PlotLogFileParser.finalFileSize.Matches(line);
                    highlight(1, "Final File size: ");
                    highlight(2, matches[0].Groups[1].Value);
                }
                else if (forwardPropagationTime.IsMatch(line))
                {
                    highlight(1, line);
                }
                else if (scannedTime.IsMatch(line))
                {
                    highlight(1, line);
                }
                else if (sortTime.IsMatch(line))
                {
                    highlight(1, line);
                }
                else if (scannedTable.IsMatch(line))
                {
                    highlight(1, line);
                }



                else
                {
                    highlight(0, line);
                }

                htmlLine.Append("</span>");
                htmlBlock.Append(htmlLine).Append('\n');
            });

            TailLineEmitter.ReadMore();
        }

        public static Regex forwardPropagationTime = new Regex("^Forward propagation ", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex scannedTable = new Regex("^scanned table ", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex scannedTime = new Regex("^scanned time ", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex sortTime = new Regex("^sort time ", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        void InitializeResizer()
        {
            this.WhenAnyValue(x => x.Height)
                .Subscribe(x => this.Find<ScrollViewer>("Log").Height = x - 40);
            this.WhenAnyValue(x => x.Width)
                .Subscribe(x => this.Find<ScrollViewer>("Log").Width = x - 10);
        }

        public void InitializeRefresh()
        {
            Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            Timer.Tick += (sender, e) =>
            {
                Debug.WriteLine("Refresh LogViewer" + DateTime.Now);
                TailLineEmitter.ReadMore();
            };
            Timer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Timer.Stop();
            base.OnClosing(e);
        }

    }
}
