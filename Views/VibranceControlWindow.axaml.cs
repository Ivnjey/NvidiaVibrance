using Avalonia.Controls;
using Avalonia.Input;
using NvidiaVibrance.ViewModels;

namespace NvidiaVibrance.Views;

public partial class VibranceControlWindow : Window
{
    public VibranceControlWindow()
    {
        InitializeComponent();
    }

    private void OnSliderMouseWheel(object? sender, PointerWheelEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && sender is Slider slider)
        {
            int change = e.Delta.Y > 0 ? 1 : -1;
            int newValue = viewModel.CurrentVibrance + change;

            if (newValue < slider.Minimum) newValue = (int)slider.Minimum;
            if (newValue > slider.Maximum) newValue = (int)slider.Maximum;

            viewModel.CurrentVibrance = newValue;
            e.Handled = true;
        }
    }
}