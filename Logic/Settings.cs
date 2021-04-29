using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChiaPlottStatus.GUI.Models
{
    public class Settings
    {
        private string SettingsFile;
        public ObservableCollection<string> LogDirectories { get; set; } = new();
        public double FontSize { get; set; } = 10d;

        public Settings()
        {
            this.SettingsFile = "ChiaPlotStatusSettings";
        }

        public Settings(string settingsFile)
        {
            this.SettingsFile = settingsFile;
        }

        public bool Load()
        {
            if (File.Exists(this.SettingsFile))
            {
                string json = File.ReadAllText(this.SettingsFile);
                Settings? fromFile = JsonSerializer.Deserialize<Settings>(json);
                if (fromFile != null)
                {
                    this.LogDirectories = fromFile.LogDirectories;
                    this.FontSize = fromFile.FontSize;
                    return true;
                }
            }
            return false;
        }

        public void Persist()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsFile, json);
        }
    }
}
