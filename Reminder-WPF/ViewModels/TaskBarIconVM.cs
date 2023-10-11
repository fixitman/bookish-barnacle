using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Reminder_WPF.ViewModels
{
    internal partial class TaskBarIconVM : ObservableObject
    {

        [ObservableProperty]
        public bool startMinimized;

        public TaskBarIconVM()
        {
            startMinimized = AppSettings.Default.StartMinimized;
        }

        [RelayCommand]
        void TBIDoubleClick()
        {
            MessageBox.Show("TBI Double-Clicked");
        }

        [RelayCommand]
        void MenuExit()
        {
            App.Current.Shutdown();
        }

        [RelayCommand]
        void ShowMainWindow()
        {
            Application.Current.MainWindow.Show();
        }

        [RelayCommand]
        void HideMainWindow()
        {
            Application.Current.MainWindow.Hide();
        }

        [RelayCommand]
        void ToggleStartMinimized()
        {
            StartMinimized = !StartMinimized;
        }

        partial void OnStartMinimizedChanged(bool oldValue, bool newValue)
        {
            AppSettings.Default.StartMinimized = newValue;            
        }

    }
}
