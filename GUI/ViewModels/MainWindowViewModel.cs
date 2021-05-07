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

namespace ChiaPlotStatus.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ChiaPlotStatus PlotManager { get; internal set; }

        // private readonly INotificationManager _notificationManager { get; private set; };

        public ObservableCollection<PlotLogReadable> PlotLogs { get; } = new();
        public List<(PlotLog, PlotLogReadable)> PlotLogTuples { get; set; } = new();

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

        public MainWindowViewModel()
        {
            foreach (var property in typeof(PlotLogReadable).GetProperties())
                SortProperties.Add(property.Name);
            Languages = Translation.LoadLanguages();
            Language = Languages["English"];
            InitializeChiaPlotStatus();
            InitializeButtons();
            InitializeSearchBox();
            KeepGridScrollbarOnScreen();
            InitializeThemeSwitcher();
            InitializeRefreshInterval();
            InitializeFilterUpdates();
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
                        dataGrid.Height = x - 135;
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
            double height = MainWindow.Instance.Height - 600;
            MainWindow.Instance.Find<DataGrid>("PlotLogGrid").Height = height;
        }


        public void InitializeRefreshInterval()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            timer.Tick += (sender, e) =>
            {
                // Somehow this ticks twice
                Debug.WriteLine("Refresh " + DateTime.Now);
                LoadPlotLogs();
            };
            timer.Start();
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

        /*
        public void InitializeNotifications()
        {
            _notificationManager = AvaloniaLocator.Current.GetService<INotificationManager>();
        }

        public void HandleNotification(PlotLogReadable plotLog)
        {
            try
            {
                _notificationManager.ShowNotification(new Notification
                {
                    Title = "Plot finished",
                    Body = "PlotName: " + plotLog.PlotName + "\n" +
                            "LogFolder: " + plotLog.LogFolder + "\n"+
                            "Tmp1: " + plotLog.Tmp1Drive + "\n" +
                            "Tmp2: " + plotLog.Tmp2Drive + "\n" +
                            "Dest: " + plotLog.DestDrive + "\n" +
                            "TotalTime: " + plotLog.TotalSeconds + "\n"
                });
            } catch (Exception e)
            {
                Debug.WriteLine("NotificationManager.ShowNotification failed. MacOS?", e);
            }
        }
        */

    }
}
