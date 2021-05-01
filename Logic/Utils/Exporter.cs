using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ChiaPlotStatus.Logic.Utils
{
    public class Exporter
    {
        private List<(PlotLog, PlotLogReadable)> plotLogTuples;
        private List<PlotLog> plotLogs = new();
        private List<PlotLogReadable> plotLogReadables = new();

        public Exporter(List<(PlotLog, PlotLogReadable)> plotLogs)
        {
            this.plotLogTuples = plotLogs;
            foreach (var tuple in this.plotLogTuples)
            {
                this.plotLogs.Add(tuple.Item1);
                this.plotLogReadables.Add(tuple.Item2);
            }
        }

        public void ToJson(string file, bool raw)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string? json = null;
            if (raw)
                json = JsonSerializer.Serialize(this.plotLogs, options);
            else
                json = JsonSerializer.Serialize(this.plotLogReadables, options);
            File.WriteAllText(file, json);
        }

        public void ToYaml(string file, bool raw)
        {
            var serializer = new SerializerBuilder().Build();
            string? yaml = null;
            if (raw)
                yaml = serializer.Serialize(this.plotLogs);
            else
                yaml = serializer.Serialize(this.plotLogReadables);
            File.WriteAllText(file, yaml);
        }

        public void ToCsv(string file, bool raw)
        {
            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            if (raw)
                csv.WriteRecords(this.plotLogs);
            else
                csv.WriteRecords(this.plotLogReadables);
        }

    }
}
