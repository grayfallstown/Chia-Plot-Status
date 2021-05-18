using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using ChiaPlotStatus;
using ChiaPlotStatus.Logic.Utils;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ChiaPlotStatus.Logic.Models;
using Avalonia;
using System.Runtime.InteropServices;
using ChiaPlotStatusGUI.GUI.Models;

namespace ChiaPlotStatus.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ChiaPlotStatus PlotManager { get; internal set; }

        public ObservableCollection<PlotLogReadable> PlotLogs { get; } = new();
        public List<(PlotLog, PlotLogReadable)> PlotLogTuples { get; set; } = new();
        public PlotCounts PlotCounts { get; set; }

        public string? Search { get; set; } = null;
        public Filter Filter { get; } = new();

        public string SortProperty { get; set; } = "Progress";
        public bool SortAsc { get; set; } = true;
        public ObservableCollection<string> SortProperties = new();

        public Language Language { get; set; }
        public Dictionary<string, Language> Languages { get; set; }

        public ReactiveCommand<Unit, Unit> ExportJsonCommand { get; set; }
        public bool RawExport { get; set; } = false;
        public ReactiveCommand<Unit, Unit> ExportYamlCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExportCsvCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AddFolderCommand { get; set; }
        public ReactiveCommand<string, Unit> RemoveFolderCommand { get; set; }
        public ReactiveCommand<Unit, Unit> IncreaseFontSizeCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DecreaseFontSizeCommand { get; set; }
        public ReactiveCommand<PlotLogReadable, Unit> MarkAsDeadCommand { get; set; }
        public DispatcherTimer RefreshTimer { get; set; }

        public MainWindowViewModel()
        {
            foreach (var property in typeof(PlotLogReadable).GetProperties())
                SortProperties.Add(property.Name);
            if (MainWindow.Instance.Find<Button>("RefreshPauseButton") == null)
            {
                // TODO: rewrite in proper MVVM or similar pattern to get rid of this
                return;
            }
            Languages = Translation.LoadLanguages();
            Language = Languages["English"];
            InitializeChiaPlotStatus();
            InitializeButtons();
            InitializeSearchBox();
            KeepGridScrollbarOnScreen();
            InitializeThemeSwitcher();
            InitializeRefreshInterval();
            InitializeRefreshPauseButton();
            InitializeFilterUpdates();
            SetGridHeight();
            SortColumns();
        }


        private void SortColumns()
        {
            var logDataGrid = MainWindow.Instance.Find<DataGrid>("LogDataGrid");
            List<DataGridColumn> columns = new();
            foreach (var col in logDataGrid.Columns)
                columns.Add(col);

            logDataGrid.Columns.Clear();

            columns.Sort((a, b) =>
            {
                return PlotManager.Settings.Columns.IndexOf(a.SortMemberPath).CompareTo(
                    PlotManager.Settings.Columns.IndexOf(b.SortMemberPath));
            });

            for (int i = 0; i < columns.Count; i++)
            {
                columns[i].DisplayIndex = i;
                logDataGrid.Columns.Add(columns[i]);
            }
        }

        private void InitializeRefreshPauseButton()
        {
            var button = MainWindow.Instance.Find<Button>("RefreshPauseButton");
            if (button != null)
            {
                button.Click += (sender, e) =>
                {
                    if (RefreshTimer.IsEnabled)
                    {
                        Debug.WriteLine("pausing refresh");
                        button.Content = "Refresh ■";
                        RefreshTimer.Stop();
                    } else
                    {
                        Debug.WriteLine("continuing refresh");
                        button.Content = "Refresh ▶";
                        RefreshTimer.Start();
                        LoadPlotLogs();
                    }
                };
            }
        }

        private void InitializeThemeSwitcher()
        {
            MainWindow.Instance.ThemeSwitchWorkaround = (theme) =>
            {
                PlotManager.Settings.Theme = theme;
                PlotManager.Settings.Persist();
                var light = new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
                {
                    Source = new Uri("resm:Avalonia.Themes.Default.Accents.BaseLight.xaml?assembly=Avalonia.Themes.Default")
                };
                var dark = new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
                {
                    Source = new Uri("resm:Avalonia.Themes.Default.Accents.BaseDark.xaml?assembly=Avalonia.Themes.Default")
                };

                switch (theme)
                {
                    case "Dark":
                        MainWindow.Instance.Styles[0] = dark;
                        MainWindow.Instance.Find<ComboBox>("Themes").SelectedIndex = 1;
                        MainWindow.Instance.Find<TextBox>("SearchBox").Foreground = Avalonia.Media.Brushes.LightGray;
                        MainWindow.Instance.Find<ComboBox>("Themes").Foreground = Avalonia.Media.Brushes.LightGray;
                        break;
                    default:
                        MainWindow.Instance.Styles[0] = light;
                        MainWindow.Instance.Find<ComboBox>("Themes").SelectedIndex = 0;
                        MainWindow.Instance.Find<TextBox>("SearchBox").Foreground = Avalonia.Media.Brushes.Black;
                        MainWindow.Instance.Find<ComboBox>("Themes").Foreground = Avalonia.Media.Brushes.Black;
                        break;
                }
                MainWindow.Instance.Find<TextBox>("SearchBox").FontSize = (double) PlotManager.Settings.FontSize;
                return true;
            };
            Dispatcher.UIThread.InvokeAsync(() =>
            {
               MainWindow.Instance.ThemeSwitchWorkaround(PlotManager.Settings.Theme);
           });
        }

        private void KeepGridScrollbarOnScreen()
        {
            MainWindow.Instance.WhenAnyValue(x => x.Height)
                .Subscribe(x =>
                {
                    var dataGrid = MainWindow.Instance.Find<DataGrid>("LogDataGrid");
                    if (dataGrid != null)
                        dataGrid.Height = x - 145;
                });
        }

        public void InitializeChiaPlotStatus()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
            Settings Settings = new Settings(folder + "ChiaPlotStatu.config.json");
            Settings.Load();
            PlotManager = new(Settings);
            if (PlotManager.Settings.LogDirectories.Count == 0)
                PlotManager.AddDefaultLogFolder();
            LoadPlotLogs();
        }

        public void LoadPlotLogs()
        {
            PlotLogs.Clear();
            PlotLogTuples = new();
            foreach (var plotLog in PlotManager.PollPlotLogs(SortProperty, SortAsc, Search, Filter))
            {
                PlotLogs.Add(plotLog.Item2);
                PlotLogTuples.Add(plotLog);
            }
            PlotCounts = new(PlotLogTuples);
            this.RaisePropertyChanged("PlotCounts");
        }


        public void InitializeButtons()
        {
            AddFolderCommand = ReactiveCommand.Create(AddFolder);
            RemoveFolderCommand = ReactiveCommand.Create<string>(RemoveFolder);
            MainWindow.Instance.BtnClickWorkaround = (folder) =>
            {
                RemoveFolder(folder);
                return true;
            };

            MainWindow.Instance.SortChangeWorkaround = (headerText) =>
            {
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
                // Debug.WriteLine("SearchProperty: " + SortProperty + ", ASC: " + SortAsc);
                LoadPlotLogs();
                return true;
            };

            IncreaseFontSizeCommand = ReactiveCommand.Create(() =>
            {
                this.PlotManager.Settings.FontSize += 0.3;
                this.PlotManager.Settings.Persist();
            });

            DecreaseFontSizeCommand = ReactiveCommand.Create(() =>
            {
                this.PlotManager.Settings.FontSize -= 0.3;
                this.PlotManager.Settings.Persist();
            });

            ExportJsonCommand = ReactiveCommand.Create(() =>
            {
                Exporter exporter = new Exporter(PlotLogTuples);
                SaveFileDialog picker = new SaveFileDialog();
                picker.Title = "Save as Json";
                if (RawExport)
                    picker.InitialFileName = "ChiaPlotStatus-raw.json";
                else
                    picker.InitialFileName = "ChiaPlotStatus.json";
                picker.Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                        Name = "Json (.json)", Extensions = new List<string> {"json"}
                    },
                    new FileDialogFilter
                    {
                        Name = "All files",
                        Extensions = new List<string> {"*"}
                    }
                };
                Task.Run(async () =>
                {
                    var result = await picker.ShowAsync(MainWindow.Instance);
                    if (result != null)
                        exporter.ToJson(result, RawExport);
                });
            });

            ExportYamlCommand = ReactiveCommand.Create(() =>
            {
                Exporter exporter = new Exporter(PlotLogTuples);
                SaveFileDialog picker = new SaveFileDialog();
                picker.Title = "Save as Yaml";
                if (RawExport)
                    picker.InitialFileName = "ChiaPlotStatus-raw.yaml";
                else
                    picker.InitialFileName = "ChiaPlotStatus.yaml";
                picker.Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                        Name = "Yaml (.yaml)", Extensions = new List<string> {"yaml"}
                    },
                    new FileDialogFilter
                    {
                        Name = "All files",
                        Extensions = new List<string> {"*"}
                    }
                };
                Task.Run(async () =>
                {
                    var result = await picker.ShowAsync(MainWindow.Instance);
                    if (result != null)
                        exporter.ToYaml(result, RawExport);
                });
            });

            ExportCsvCommand = ReactiveCommand.Create(() =>
            {
                Exporter exporter = new Exporter(PlotLogTuples);
                SaveFileDialog picker = new SaveFileDialog();
                picker.Title = "Save as Csv";
                if (RawExport)
                    picker.InitialFileName = "ChiaPlotStatus-raw.csv";
                else
                    picker.InitialFileName = "ChiaPlotStatus.csv";
                picker.Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                        Name = "Yaml (.csv)", Extensions = new List<string> {"csv"}
                    },
                    new FileDialogFilter
                    {
                        Name = "All files",
                        Extensions = new List<string> {"*"}
                    }
                };
                Task.Run(async () =>
                {
                    var result = await picker.ShowAsync(MainWindow.Instance);
                    if (result != null)
                        exporter.ToCsv(result, RawExport);
                });
            });

            MarkAsDeadCommand = ReactiveCommand.Create<PlotLogReadable>((plotLogReadable) =>
            {
                var dialog = new MarkOfDeathDialog(plotLogReadable, this.Language, this.PlotManager.Settings, LoadPlotLogs);
                dialog.Show();
            });

            var donateButton = MainWindow.Instance.Find<MenuItem>("DonateButton");
            if (donateButton != null)
                donateButton.Command = ReactiveCommand.Create(() =>
                {
                    var dialog = new DonationDialog(this.Language);
                    dialog.Show();
                });

            var updateButton = MainWindow.Instance.Find<MenuItem>("UpdateButton");
            if (updateButton != null)
                updateButton.Command = ReactiveCommand.Create(() =>
                {
                    var dialog = new UpdateDialog(this.Language);
                    dialog.Show();
                });
        }

        public void InitializeSearchBox()
        {
            MainWindow.Instance.TextChangeWorkaround = (text) =>
            {
                Search = text;
                LoadPlotLogs();
                return true;
            };
        }

        public async void AddFolder()
        {
            OpenFolderDialog picker = new OpenFolderDialog();
            var result = await picker.ShowAsync(MainWindow.Instance);
            AddFolder(result);
        }

        public void AddFolder(string folder)
        {
            if (Directory.Exists(folder) && !PlotManager.Settings.LogDirectories.Contains(folder))
            {
                PlotManager.AddLogFolder(folder);
                PlotManager.Settings.Persist();
                LoadPlotLogs();
            }
        }

        public void RemoveFolder(string folder)
        {
            PlotManager.RemoveLogFolder(folder);
            PlotManager.Settings.Persist();
            LoadPlotLogs();
        }

        public void SetGridHeight()
        {
            if (MainWindow.Instance.Find<DataGrid>("PlotLogGrid") != null)
            {
                double height = MainWindow.Instance.Height - 600;
                MainWindow.Instance.Find<DataGrid>("PlotLogGrid").Height = height;
            }
        }


        public void InitializeRefreshInterval()
        {
            RefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            RefreshTimer.Tick += (sender, e) =>
            {
                // Somehow this ticks twice
                Debug.WriteLine("Refresh " + DateTime.Now);
                LoadPlotLogs();
            };
            RefreshTimer.Start();
        }


        public void InitializeFilterUpdates()
        {
            /*
            this.WhenAnyValue(x => x.Filter.HideHealthy)
                .Subscribe((x) => LoadPlotLogs());
            this.WhenAnyValue(x => x.Filter.HidePossiblyDead)
                .Subscribe((x) => LoadPlotLogs());
            this.WhenAnyValue(x => x.Filter.HideConfirmedDead)
                .Subscribe((x) => LoadPlotLogs());
            */
            var checkBox = MainWindow.Instance.Find<CheckBox>("HideConfirmedDead");
            if (checkBox != null)
            {
                checkBox.WhenAnyValue(x => x.IsChecked)
                    .Subscribe((x) => LoadPlotLogs());
                MainWindow.Instance.Find<CheckBox>("HidePossiblyDead").WhenAnyValue(x => x.IsChecked)
                    .Subscribe((x) => LoadPlotLogs());
                MainWindow.Instance.Find<CheckBox>("HideHealthy").WhenAnyValue(x => x.IsChecked)
                    .Subscribe((x) => LoadPlotLogs());
                MainWindow.Instance.Find<CheckBox>("HideFinished").WhenAnyValue(x => x.IsChecked)
                    .Subscribe((x) => LoadPlotLogs());
            }
        }
    }
}
