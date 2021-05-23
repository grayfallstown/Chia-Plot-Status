using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ChiaPlotStatus.Logic.Utils;
using ChiaPlotStatusGUI.GUI.Utils;
using ChiaPlotStatusGUI.GUI.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ChiaPlotStatus.Views
{
    public class StatisticsDialog : Window
    {
        public Language Language { get; set; }
        public List<(PlottingStatisticsFull, PlottingStatisticsFullReadable)> StatsTuples { get; set; }
        public ObservableCollection<PlottingStatisticsFullReadable> Stats { get; set; } = new();
        public string SortProperty { get; set; } = "Tmp1Drive";
        public bool SortAsc { get; set; } = true;


        public StatisticsDialog()
        {
        }

        public StatisticsDialog(List<(PlottingStatisticsFull, PlottingStatisticsFullReadable)> statsTuples, Language language, string theme)
        {
            this.DataContext = this;
            this.Language = language;
            this.StatsTuples = statsTuples;
            Sorter.Sort(SortProperty, SortAsc, StatsTuples);
            Stats.Clear();
            foreach (var tuple in statsTuples)
                Stats.Add(tuple.Item2);
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            KeepGridScrollbarOnScreen();
            this.WindowState = WindowState.Maximized;
            Utils.SetTheme(this, theme);
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void DataGridHeaderClick(object sender, RoutedEventArgs e)
        {
            Button header = ((Button)sender);
            string headerText = (string)header.Content;
            var oldSortProperty = SortProperty;
            foreach (var property in typeof(Columns).GetProperties())
            {
                string translation = (string)property.GetValue(Language.Columns);
                if (string.Equals(headerText, translation))
                {
                    SortProperty = property.Name;
                    break;
                }
            }
            if (string.Equals(SortProperty, oldSortProperty))
                SortAsc = !SortAsc;
            Sorter.Sort(SortProperty, SortAsc, StatsTuples);
            Stats.Clear();
            foreach (var tuple in StatsTuples)
                Stats.Add(tuple.Item2);
            // Debug.WriteLine("SearchProperty: " + SortProperty + ", ASC: " + SortAsc);
        }

        private void KeepGridScrollbarOnScreen()
        {
            this.WhenAnyValue(x => x.Height)
                .Subscribe(x =>
                {
                    var dataGrid = this.Find<DataGrid>("StatsDataGrid");
                    if (dataGrid != null)
                        dataGrid.Height = x - 100;
                });
        }

    }
}
