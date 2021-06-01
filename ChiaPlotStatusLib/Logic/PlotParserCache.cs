using ChiaPlotStatus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic
{

    public class PlotParserCache
    {
        public Dictionary<string, List<PlotLog>> CachedPlots { get; set; } = new();
        private readonly object lockObject = new object();
        private string path;


        public PlotParserCache(string path)
        {
            this.path = path;
            try
            {
                Debug.WriteLine("Trying to read cache");
                if (File.Exists(path))
                    CachedPlots = JsonSerializer.Deserialize<Dictionary<string, List<PlotLog>>>(File.ReadAllText(path));
                Debug.WriteLine("Loaded " + CachedPlots.Count + " entries from cache");
            } catch (Exception e)
            {
                Debug.WriteLine("Could not load PlotParserCache: " + e);
            }
        }

        private string GetKey(PlotLogFileParser parser)
        {
            return parser.LogFolder + " # " + parser.LogFile;
        }

        public void Add(PlotLogFileParser parser, List<PlotLog> plotLogs)
        {
            lock(lockObject)
            {
                CachedPlots[GetKey(parser)] = plotLogs;
            }
        }

        public List<PlotLog>? Get(PlotLogFileParser parser)
        {
            lock (lockObject)
            {
                List<PlotLog> result;
                if (CachedPlots.TryGetValue(GetKey(parser), out result))
                    return result;
                return null;
            }
        }

        public void Persist()
        {
            lock (lockObject)
            {
                try
                {
                    Debug.WriteLine("Persisting Cache");
                    string json = JsonSerializer.Serialize(CachedPlots);
                    File.WriteAllText(path, json);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Could not persist PlotParserCache: " + e);
                }
            }
        }

    }
}
