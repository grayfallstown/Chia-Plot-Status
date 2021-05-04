using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Logic.Models
{
    public class Filter
    {
        public bool HidePossiblyDead { get; set; } = false;
        public bool HideConfirmedDead { get; set; } = false;
        public bool HideHealthy { get; set; } = false;
    }
}
