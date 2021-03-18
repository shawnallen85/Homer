using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Homer.ViewModels;
using Homer.Views;
using LibVLCSharp.Shared;

namespace Homer
{
    public class App : Application
    {
        public override void Initialize()
        {
            Core.Initialize();
            AvaloniaXamlLoader.Load(this);
        }
        
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }
            
            base.OnFrameworkInitializationCompleted();
        }
    }
}