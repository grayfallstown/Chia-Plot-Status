using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ChiaPlotStatus.GUI.Models;
using ChiaPlotStatus.Logic.Models;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ChiaPlotStatus.Views
{
    public class DonationDialog : Window
    {
        public Language Language { get; set; }
        public string ChiaAddress { get; set; } = "xch1sxgmrrmq95klztd5796ysz8c6jattd6k70z4gxuet5a9792s24hqf4jdtn";
        public string Paypal { get; set; } = "maklemenz@googlemail.com";

        public DonationDialog() { }

        public DonationDialog(Language language)
        {
            this.DataContext = this;
            this.Language = language;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.Find<TextBlock>("Thx").IsVisible = false;
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            string text = (string)(((Button)sender).Tag);
            Application.Current.Clipboard.SetTextAsync(text);
            this.Find<TextBlock>("Thx").IsVisible = true;
        }

    }
}
