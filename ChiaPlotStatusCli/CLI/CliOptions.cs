using CommandLine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus.CLI
{
    public class CliOptions
    {
        [Option('o', "outfile", Required = true, HelpText = "The file to write to")]
        public string File { get; set; }

        [Option('f', "format", Required = true, HelpText = "The format to use while writing the file. Valid values are json, yaml and csv")]
        public string Format { get; set; }

        [Option('r', "raw", Required = false, HelpText = "Write raw, unformatted values instead of human readable ones.")]
        public bool Raw { get; set; }

        [Option('l', "log-folders", Required = false, Separator = ',', HelpText = "The folders where logs can be found, comma separated. Uses default folder when empty")]
        public IEnumerable<string> LogFolders { get; set; }

        [Option('s', "sort-property", Required = false, HelpText = "The property you want the plotlogs sorted by")]
        public string SortProperty { get; set; }

        [Option('a', "sort-asc", Required = false, HelpText = "Sort ascending")]
        public bool SortAsc { get; set; }

        [Option("search", Required = false, HelpText = "Filter plotlogs by this search terms. You filter for your temp1 folder for example.")]
        public string Search { get; set; }

        [Option("hide-finished", Required = false, HelpText = "Hide finished plots")]
        public bool HideFinished { get; set; }

        [Option("hide-possibly-dead", Required = false, HelpText = "Hide possibly dead plots")]
        public bool HidePossiblyDead { get; set; }

        [Option("hide-confirmed-dead", Required = false, HelpText = "Hide confirmed dead plots")]
        public bool HideConfirmedDead { get; set; }

        [Option("hide-healthy", Required = false, HelpText = "Hide healthy plots")]
        public bool HideHealthy { get; set; }

        [Option("keep-updating", Required = false, HelpText = "Keep updating the file every 10 seconds")]
        public bool KeepUpdating { get; set; }
    }
}
