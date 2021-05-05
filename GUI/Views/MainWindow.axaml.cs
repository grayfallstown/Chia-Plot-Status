using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Views
{
    public class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public Func<string, bool> BtnClickWorkaround { get; set; }
        public Func<string, bool> TextChangeWorkaround { get; set; }
        public Func<string, bool> SortChangeWorkaround { get; set; }
        public Func<string, bool> ThemeSwitchWorkaround { get; set; }

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ThemeSwitcher();
            this.WindowState = WindowState.Maximized;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }



        // FIXME: RemoveFolderCommand does not trigger. Why is button.Command null?
        public void RemoveFolderWorkaround(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string folder = (string)button.CommandParameter;
            ReactiveCommand<string, Unit> command = (ReactiveCommand<string, Unit>)button.Command;
            if (command == null)
            {
                command = (ReactiveCommand<string, Unit>)button.Tag;
            }
            // button.Command.Execute(folder);
            BtnClickWorkaround.Invoke(folder);
        }


        // FIXME: apparently avalonia cannot tell me when a textbox text changes in my MainWindowViewModel
        public void OnKeyPressUp(object sender, KeyEventArgs e)
        {
            TextChangeWorkaround(((TextBox)sender).Text);
        }

        public void LogDataGridHeaderClick(object sender, RoutedEventArgs e)
        {
            Button header = ((Button)sender);
            string headerText = (string) header.Content;
            SortChangeWorkaround(headerText);
        }

        public void ThemeSwitcher()
        {
            var themes = this.Find<ComboBox>("Themes");
            themes.SelectionChanged += (sender, e) =>
            {
                switch (themes.SelectedIndex)
                {
                    case 1:
                        ThemeSwitchWorkaround("Dark");
                        break;
                    default:
                        ThemeSwitchWorkaround("Light");
                        break;
                }
            };
        }

        public void OpenLogViewerWindow(object sender, RoutedEventArgs e)
        {
            var dialog = new LogViewerWindow(@"C:\Users\mk\.chia\mainnet\plotter\6ce9d6ea-cba0-4ec5-87c6-53d06a420cb2.log");
            _ = dialog.ShowDialog<bool>(this);
        }

    }
}
