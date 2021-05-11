using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatus.Logic.Utils;
using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChiaPlotStatus.CLI
{
    public class ChiaPlotStatusCLI
    {
        public static void Main(string[] args)
        {
            Parser parser = new((parserSettings) => {
                parserSettings.CaseSensitive = false;
                parserSettings.AutoVersion = false;
                parserSettings.AutoHelp = true;
                parserSettings.HelpWriter = Console.Out;
            });
            parser.ParseArguments<CliOptions>(args)
                   .WithParsed<CliOptions>(options =>
                   {
                       do
                       {
                           GenerateReport(options);
                           Console.Out.WriteLine("File '" + options.File + "' written");
                           if (options.KeepUpdating)
                           {
                               int seconds = 10;
                               Console.Out.WriteLine("Updating file in " + seconds + " seconds...");
                               Thread.Sleep(seconds * 1000);
                           }
                       }
                       while (options.KeepUpdating);
                   });
        }

        public static void GenerateReport(CliOptions options)
        {
            ChiaPlotStatus PlotManager = SetupChiaPlotStatus(options);
            Filter filter = SetupFilter(options);
            string sortProperty = options.SortProperty;
            if (string.IsNullOrEmpty(sortProperty))
                sortProperty = "Progress";
            Console.Out.WriteLine("Sorting by " + sortProperty);
            List<(PlotLog, PlotLogReadable)> plotLogs = PlotManager.PollPlotLogs(options.SortProperty, options.SortAsc, options.Search, filter);
            ExportToFile(options, plotLogs);
        }

        private static ChiaPlotStatus SetupChiaPlotStatus(CliOptions options)
        {
            var configFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
            Settings Settings = new Settings(configFolder + "ChiaPlotStatu.config.json");
            Settings.Load();
            ChiaPlotStatus PlotManager = new(Settings);
            List<string> folders = options.LogFolders.ToList();
            if (folders.Count() == 0)
            {
                if (PlotManager.Settings.LogDirectories.Count == 0)
                    PlotManager.AddDefaultLogFolder();
            }
            else
            {
                // override LogFolders from settings file
                PlotManager.Settings.LogDirectories.Clear();
                foreach (var folder in folders)
                    PlotManager.AddLogFolder(folder);
            }

            return PlotManager;
        }

        private static Filter SetupFilter(CliOptions options)
        {
            Filter filter = new();
            filter.HideHealthy = options.HideHealthy;
            filter.HideFinished = options.HideFinished;
            filter.HidePossiblyDead = options.HidePossiblyDead;
            filter.HideConfirmedDead = options.HideConfirmedDead;
            return filter;
        }

        private static void ExportToFile(CliOptions options, List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            Exporter exporter = new Exporter(plotLogs);
            try
            {
                switch (options.Format.ToLower())
                {
                    case "json":
                        exporter.ToJson(options.File, options.Raw);
                        break;
                    case "yaml":
                        exporter.ToYaml(options.File, options.Raw);
                        break;
                    case "csv":
                        exporter.ToCsv(options.File, options.Raw);
                        break;
                    default:
                        throw new NotImplementedException("File format '" + options.Format + "' not supported");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not write file '" + options.File + "': " + e.Message);
                Console.Error.WriteLine(e.StackTrace);
                System.Environment.Exit(1);
            }
        }
    }
}
