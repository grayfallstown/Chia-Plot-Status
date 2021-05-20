using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ChiaPlotStatus.GUI.Models
{
    public static class Translation
    {
        public static Dictionary<string, Language> LoadLanguages()
        {
            var deserializer = new DeserializerBuilder().Build();
            Dictionary<string, Language> langs = new();
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            foreach (var lang2 in new string[] { "en" })
            {
                System.IO.Stream filestream = assets.Open(new Uri("avares://ChiaPlotStatus/GUI/Assets/" + lang2 + ".yaml"));
                StreamReader reader = new StreamReader(filestream);
                string yaml = reader.ReadToEnd();
                var lang = deserializer.Deserialize<Language>(yaml);
                langs.Add(lang.Name, lang);
            }
            return langs;
        }

    }


    public class Language
    {
        public string Name { get; set; }
        public Tooltips Tooltips { get; set; }
        public Columns Columns { get; set; }
        public Fields Fields { get; set; }
        public Buttons Buttons { get; set; }
    }


    public class Fields
    {
        public string Search { get; set; } = "";
        public string Light { get; set; } = "";
        public string Dark { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string RawExport { get; set; } = "";
        public string HideHealthy { get; set; } = "";
        public string HideFinished { get; set; } = "";
        public string HidePossiblyDead { get; set; } = "";
        public string HideConfirmedDead { get; set; } = "";
    }


    public class Buttons
    {
        public string Add { get; set; } = "";
        public string Remove { get; set; } = "";
        public string Json { get; set; } = "";
        public string Yaml { get; set; } = "";
        public string CSV { get; set; } = "";
        public string MarkAsDead { get; set; } = "";
        public string UnmarkAsDead { get; set; } = "";
        public string Abort { get; set; } = "";
        public string Copy { get; set; } = "";
    }

    public class Tooltips
    {
        public string JsonExport { get; set; } = "";
        public string YamlExport { get; set; } = "";
        public string CsvExport { get; set; } = "";
        public string RawExport { get; set; } = "";
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        public string DestDrive { get; set; } = "";
        public string Errors { get; set; } = "";
        public string Progress { get; set; } = "";
        public string TimeRemaining { get; set; } = "";
        public string RunTimeSeconds { get; set; } = "";
        public string ETA { get; set; } = "";
        public string CurrentTable { get; set; } = "";
        public string CurrentBucket { get; set; } = "";
        public string CurrentPhase { get; set; } = "";
        public string Phase1Cpu { get; set; } = "";
        public string Phase2Cpu { get; set; } = "";
        public string Phase3Cpu { get; set; } = "";
        public string Phase4Cpu { get; set; } = "";
        public string Phase1Seconds { get; set; } = "";
        public string Phase2Seconds { get; set; } = "";
        public string Phase3Seconds { get; set; } = "";
        public string Phase4Seconds { get; set; } = "";
        public string CopyTimeSeconds { get; set; } = "";
        public string TotalSeconds { get; set; } = "";
        public string PlotSize { get; set; } = "";
        public string Threads { get; set; } = "";
        public string Buffer { get; set; } = "";
        public string Buckets { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string FinishDate { get; set; } = "";
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";
        public string Health { get; set; } = "";
        public string LastLogLine { get; set; } = "";
    }

    public class Columns
    {
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        public string DestDrive { get; set; } = "";
        public string Errors { get; set; } = "";
        public string Progress { get; set; } = "";
        public string TimeRemaining { get; set; } = "";
        public string RunTimeSeconds { get; set; } = "";
        public string ETA { get; set; } = "";
        public string CurrentTable { get; set; } = "";
        public string CurrentBucket { get; set; } = "";
        public string CurrentPhase { get; set; } = "";
        public string Phase1Cpu { get; set; } = "";
        public string Phase2Cpu { get; set; } = "";
        public string Phase3Cpu { get; set; } = "";
        public string Phase4Cpu { get; set; } = "";
        public string Phase1Seconds { get; set; } = "";
        public string Phase2Seconds { get; set; } = "";
        public string Phase3Seconds { get; set; } = "";
        public string Phase4Seconds { get; set; } = "";
        public string CopyTimeSeconds { get; set; } = "";
        public string TotalSeconds { get; set; } = "";
        public string PlotSize { get; set; } = "";
        public string Threads { get; set; } = "";
        public string Buffer { get; set; } = "";
        public string Buckets { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string FinishDate { get; set; } = "";
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";
        public string Health { get; set; } = "";
        public string LastLogLine { get; set; } = "";
    }
}
