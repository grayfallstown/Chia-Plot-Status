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

namespace ChiaPlotStatus.Logic.Models
{
    public class Settings
    {
        private string SettingsFile;
        public bool? AlwaysDoFullRead { get; set; } = false;
        public string? Theme { get; set; } = "Light";
        public ObservableCollection<string> LogDirectories { get; set; } = new();
        public double? MaxHarvestLookupSeconds { get; set; } = 5d;
        public ObservableCollection<string> HarvesterLogDirectories { get; set; } = new();
        public Columns Columns { get; set; } = Columns.Default();
        public PlottingStatisticsIdRelevanceWeights Weigths { get; set; } = new();
        public Filter Filter { get; set; } = new();
        public string? SortProperty { get; set; } = "Progress";
        public bool? SortAsc { get; set; } = true;
        public List<Note>? Notes { get; set; } = new();
        public List<MarkOfDeath>? MarksOfDeath { get; set; } = new();
        // no longer supported, but needs to stay or JsonDeserializer is not happy with old config files
        public double? FontSize { get; set; } = null;

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
                    if (fromFile.SortProperty != null)
                        this.SortProperty = fromFile.SortProperty;
                    if (fromFile.SortAsc != null)
                        this.SortAsc = fromFile.SortAsc;
                    if (fromFile.Weigths != null)
                        this.Weigths = fromFile.Weigths;
                    if (fromFile.Notes != null)
                        this.Notes = fromFile.Notes;
                    if (fromFile.HarvesterLogDirectories != null)
                        this.HarvesterLogDirectories = fromFile.HarvesterLogDirectories;
                    if (fromFile.MaxHarvestLookupSeconds != null)
                        this.MaxHarvestLookupSeconds = fromFile.MaxHarvestLookupSeconds;
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
            options.AllowTrailingCommas = true;
            options.MaxDepth = 9999999;
            options.DefaultBufferSize = 64 * 1024;
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsFile, json);
        }
    }
}
