using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NvidiaVibrance.Models;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NvidiaVibrance.Views;
using Avalonia.Platform;

namespace NvidiaVibrance.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private NvidiaDisplayController? _displayController;
    private bool _isMonochrome;
    private VibranceControlWindow? _vibranceWindow;

    [ObservableProperty]
    private int _currentVibrance;

    public MainWindowViewModel(NvidiaDisplayController? displayController)
    {
        _displayController = displayController;
        InitializeVibrance();
        IsMonochrome = false;
    }

    public MainWindowViewModel()
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
        {
            try
            {
                _displayController = new NvidiaDisplayController();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating NvidiaDisplayController in default constructor: {ex.Message}");
                _displayController = null;
            }
        }
        InitializeVibrance();
        IsMonochrome = false;
    }

    private void InitializeVibrance()
    {
        if (_displayController != null)
        {
            try
            {
                CurrentVibrance = _displayController.GetDigitalVibrance();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting initial vibrance: {ex.Message}");
            }
        }
        else
        {
            CurrentVibrance = 50;
            Console.WriteLine("Display controller not available during InitializeVibrance.");
        }
    }

    partial void OnCurrentVibranceChanged(int value)
    {
        SetVibranceInternal(value);
    }

    private void SetVibranceInternal(int level)
    {
        if (_displayController == null)
        {
            Console.WriteLine("Display controller is not initialized.");
            return;
        }
        try
        {
            _displayController.SetDigitalVibrance(level);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex.Message);
            InitializeVibrance();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting vibrance: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ResetVibrance()
    {
        if (_displayController == null)
        {
            Console.WriteLine("Display controller is not initialized for reset.");
            return;
        }
        try
        {
            _displayController.SetDigitalVibranceDefault();
            CurrentVibrance = _displayController.GetDigitalVibrance();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resetting vibrance: {ex.Message}");
        }
    }

    public bool IsMonochrome
    {
        get => _isMonochrome;
        set
        {
            if (SetProperty(ref _isMonochrome, value))
            {
                Console.WriteLine($"Monochrome mode set to: {IsMonochrome}");
                _displayController?.SetMonochrome(IsMonochrome);
                if (_displayController != null) CurrentVibrance = _displayController.GetDigitalVibrance();
            }
        }
    }

    [RelayCommand]
    private void ToggleMonochrome()
    {
        IsMonochrome = !IsMonochrome;
    }

    [RelayCommand]
    private void OpenVibranceWindow()
    {
        if (_vibranceWindow == null) // Проверяем, что окно не создано или было закрыто (и _vibranceWindow установлено в null)
        {
            if (_displayController != null)
            {
                try
                {
                    CurrentVibrance = _displayController.GetDigitalVibrance();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting vibrance before opening window: {ex.Message}");
                }
            }

            _vibranceWindow = new VibranceControlWindow
            {
                DataContext = this
            };
            _vibranceWindow.Closed += (s, e) => _vibranceWindow = null;
            _vibranceWindow.Deactivated += (s, e) => _vibranceWindow?.Close();

            // Позиционирование в правом нижнем углу
            if (_vibranceWindow.Screens?.Primary is Screen primaryScreen)
            {
                double windowWidth = _vibranceWindow.Width;
                double windowHeight = _vibranceWindow.Height;

                // Если DesignWidth используется как реальная ширина:
                // Для VibranceControlWindow d:DesignWidth="220" d:DesignHeight="80"
                // SizeToContent="Height" значит ширина будет DesignWidth
                // windowWidth = 220; // Возьмем из XAML пока

                var x = primaryScreen.WorkingArea.Right - windowWidth - 10; // 10px отступ
                var y = primaryScreen.WorkingArea.Bottom - windowHeight - 10; // 10px отступ
                _vibranceWindow.Position = new PixelPoint((int)x, (int)y);
            }

            _vibranceWindow.Show();
            _vibranceWindow.Activate();
        }
        else
        {
            _vibranceWindow.Activate();
        }
    }

    [RelayCommand]
    private void CloseVibranceWindow()
    {
        _vibranceWindow?.Close();
    }

    [RelayCommand]
    private static void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime appLifetime)
        {
            appLifetime.Shutdown();
        }
    }
}
