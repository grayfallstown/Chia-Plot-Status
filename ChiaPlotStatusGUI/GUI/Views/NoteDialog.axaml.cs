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
using ChiaPlotStatusLib.Logic.Models;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Text.RegularExpressions;

namespace ChiaPlotStatus.Views
{
    public class NoteDialog : Window
    {
        public Settings Settings { get; set; }
        public Language Language { get; set; }
        public Action OnUpdate { get; set; }
        public PlotLogReadable plotLogReadable { get; }
        public string Note { get; set; }

        public NoteDialog()
        {
        }

        public NoteDialog(PlotLogReadable plotLogReadable, Language language, Settings settings, string theme, Action onUpdate)
        {
            this.DataContext = this;
            this.Language = language;
            this.Settings = settings;
            this.plotLogReadable = plotLogReadable;
            this.OnUpdate = onUpdate;
            this.Note = plotLogReadable.Note;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Utils.SetTheme(this, theme);
            this.Focus();
            // does not work for some reason:
            this.Find<TextBox>("Input").SelectionStart = 0;
            this.Find<TextBox>("Input").SelectionEnd = plotLogReadable.Note.Length;
            this.Find<TextBox>("Input").Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            var note = new Note(this.plotLogReadable);
            note.text = this.Note;
            if (this.Settings.Notes.Contains(note))
                this.Settings.Notes.Remove(note);
            this.Settings.Notes.Add(note);
            this.Settings.Persist();
            this.OnUpdate();
            this.Close();
        }

        public void Abort(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
