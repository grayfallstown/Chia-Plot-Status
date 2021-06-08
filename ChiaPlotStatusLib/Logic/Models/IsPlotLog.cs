using ChiaPlotStatus.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatusLib.Logic.Models
{
    public abstract class IsPlotLog
    {
        public abstract int GetCurrentPhase();
        public abstract HealthIndicator GetHealth();
    }
}
