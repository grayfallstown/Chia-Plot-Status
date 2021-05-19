using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusLib.Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChiaPlotStatus.GUI.Models
{
    public class Settings
    {
        private string SettingsFile;
        public ObservableCollection<string> LogDirectories { get; set; } = new();
        public double? FontSize { get; set; } = 10d;
        public string? Theme { get; set; } = "Light";
        public List<MarkOfDeath>? MarksOfDeath { get; set; } = new();
        public bool? AlwaysDoFullRead { get; set; } = false;
        public Columns Columns { get; set; } = Columns.Default();
        public Filter Filter { get; set; } = new();

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
                    if (fromFile.LogDirectories != null)
                        this.LogDirectories = fromFile.LogDirectories;
                    if (fromFile.FontSize != null && fromFile.FontSize > 6)
                        this.FontSize = fromFile.FontSize;
                    if (fromFile.Theme != null)
                        this.Theme = fromFile.Theme;
                    if (fromFile.MarksOfDeath != null)
                        this.MarksOfDeath = fromFile.MarksOfDeath;
                    if (fromFile.AlwaysDoFullRead != null)
                        this.AlwaysDoFullRead = fromFile.AlwaysDoFullRead;
                    if (fromFile.Filter != null)
                        this.Filter = fromFile.Filter;
                    if (fromFile.Columns != null)
                    {
                        this.Columns = fromFile.Columns;
                        this.Columns.FixAddedAndRemovedColumns();
                    }
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
