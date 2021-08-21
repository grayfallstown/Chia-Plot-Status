using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatusGUI.GUI.Utils;
using ChiaPlotStatusLib.Logic.Models.Lang;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ChiaPlotStatus.Views
{
    public class MarkOfDeathDialog : Window
    {
        public Settings Settings { get; set; }
        public Language Language { get; set; }
        public Action OnUpdate { get; set; }
        public bool IsDead { get; set; }
        public bool IsAlive { get; set; }
        public PlotLogReadable plotLogReadable { get; }

        public MarkOfDeathDialog()
        {
        }

        public MarkOfDeathDialog(PlotLogReadable plotLogReadable, Language language, Settings settings, string theme, Action onUpdate)
        {
            this.DataContext = this;
            this.Language = language;
            this.Settings = settings;
            this.plotLogReadable = plotLogReadable;
            this.OnUpdate = onUpdate;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            foreach (var markOfDeath in Settings.MarksOfDeath)
                if (markOfDeath.IsMatch(plotLogReadable))
                    this.IsDead = true;
            this.IsAlive = !this.IsDead;
            Utils.SetTheme(this, theme);
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void MarkAsDead(object sender, RoutedEventArgs e)
        {
            var mark = new MarkOfDeath(this.plotLogReadable);
            if (!this.Settings.MarksOfDeath.Contains(mark))
            {
                this.Settings.MarksOfDeath.Add(mark);
                this.Settings.Persist();
                this.OnUpdate();
            }
            this.Close();
        }

        public void UnmarkAsDead(object sender, RoutedEventArgs e)
        {
            var mark = new MarkOfDeath(this.plotLogReadable);
            if (this.Settings.MarksOfDeath.Contains(mark))
            {
                this.Settings.MarksOfDeath.Remove(mark);
                this.Settings.Persist();
                this.OnUpdate();
            }
            this.Close();
        }

        public void Abort(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
