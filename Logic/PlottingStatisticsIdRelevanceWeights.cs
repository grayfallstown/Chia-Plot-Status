using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus
{
    /**
     * When trying to find the best matched finished PlotLogs for a given
     * PlotLog this is used to define relevance.
     */
    class PlottingStatisticsIdRelevanceWeights
    {
        /**
         * k32, k33, k34...
         */
        public int PlotSize { get; set; } = 10000;

        /**
         * from which folder the PlogLogs originate
         */
        public int LogFolder { get; set; } = 1000;

        /**
         * Partially or exact match on tmp1 and tmp2 dirs
         */
        public int TmpDir { get; set; } = 200;

        /**
         * Similarity on Threads, Buffer, Buckets
         */
        public int ComputeConfiguration { get; set; } = 100;
    }
}
