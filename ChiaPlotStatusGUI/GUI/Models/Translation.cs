using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using ChiaPlotStatusLib.Logic.Models.Lang;

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

}
