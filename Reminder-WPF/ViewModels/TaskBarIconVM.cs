using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Reminder_WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reminder_WPF.ViewModels
{
    internal partial class TaskBarIconVM
    {
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

    }
}
