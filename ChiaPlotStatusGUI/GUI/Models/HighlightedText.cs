using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus.GUI.Models
{
    public class HighlightedText
    {
        public string Text { get; set; }
        public bool Level0 { get; set; }
        public bool Level1 { get; set; }
        public bool Level2 { get; set; }
        public bool Level3 { get; set; }
        public bool Level4 { get; set; }
        public bool Level5 { get; set; }

        public HighlightedText(string Text, int level)
        {
            this.Text = Text;
            this.Level0 = level == 0;
            this.Level1 = level == 1;
            this.Level2 = level == 2;
            this.Level3 = level == 3;
            this.Level4 = level == 4;
            this.Level5 = level == 5;
        }
    }
}
