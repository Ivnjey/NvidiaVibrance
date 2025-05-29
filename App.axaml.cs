using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using NvidiaVibrance.ViewModels;
using NvidiaVibrance.Views;
using Avalonia.Controls;
using NvidiaVibrance.Models;
using System;

namespace NvidiaVibrance;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            NvidiaDisplayController? displayController = null;
            try
            {
                displayController = new NvidiaDisplayController();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize NvidiaDisplayController: {ex.Message}");
            }
            DataContext = new MainWindowViewModel(displayController);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            if (vm.OpenVibranceWindowCommand.CanExecute(null))
            {
                vm.OpenVibranceWindowCommand.Execute(null);
            }
        }
    }
}