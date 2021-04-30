using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ChiaPlottStatus.GUI.Models
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
                System.IO.Stream filestream = assets.Open(new Uri("avares://ChiaPlotStatus/Assets/" + lang2 + ".yaml"));
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
        public string Columns { get; set; }
    }

    public class Tooltips
    {
        public string Tmp1Drive { get; set; } = "";
        public string Tmp2Drive { get; set; } = "";
        public string Errors { get; set; } = "";
        public string Progress { get; set; } = "";
        public string ETA { get; set; } = "";
        public string CurrentTable { get; set; } = "";
        public string CurrentBucket { get; set; } = "";
        public string CurrentPhase { get; set; } = "";
        public string Phase1Time { get; set; } = "";
        public string Phase2Time { get; set; } = "";
        public string Phase3Time { get; set; } = "";
        public string Phase4Time { get; set; } = "";
        public string TotalTime { get; set; } = "";
        public string PlotSize { get; set; } = "";
        public string Threads { get; set; } = "";
        public string Buffer { get; set; } = "";
        public string Buckets { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string PlotName { get; set; } = "";
        public string LogFolder { get; set; } = "";
        public string LogFile { get; set; } = "";
        public string ApproximateWorkingSpace { get; set; } = "";
        public string FinalFileSize { get; set; } = "";
    }
}
