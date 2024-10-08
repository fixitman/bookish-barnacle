﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;

namespace Reminder_WPF.ViewModels
{
    internal partial class TaskBarIconVM : ObservableObject
    {

        [ObservableProperty]
        public bool startMinimized;

        [ObservableProperty]
        public bool runAtStartup;

        public TaskBarIconVM( )
        {
            startMinimized = AppSettings.Default.StartMinimized;
            runAtStartup = AppSettings.Default.RunAppOnWindowsStart;
        }

        [RelayCommand]
        void TBIDoubleClick()
        {
            //MessageBox.Show("TBI Double-Clicked");
            ShowMainWindow();
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
            //Application.Current.MainWindow.Topmost = true;  
            Application.Current.MainWindow.Activate();
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
            AppSettings.Default.StartMinimized = StartMinimized;
            AppSettings.Default.Save();
        }

        [RelayCommand]
        void ToggleRunAtStartup()
        {
            RunAtStartup = !RunAtStartup;            
        }

        partial void OnStartMinimizedChanged(bool oldValue, bool newValue)
        {
            AppSettings.Default.StartMinimized = newValue;   
            AppSettings.Default.Save();
        }

        partial void OnRunAtStartupChanged(bool oldValue, bool newValue)
        {
            AppSettings.Default.RunAppOnWindowsStart = RunAtStartup;
            AppSettings.Default.Save();

            string? selfPath = Process.GetCurrentProcess().MainModule?.FileName;
            if (selfPath != null)
            {
                string subKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
                string valueName = "Reminders";
                if (RunAtStartup)
                {
                    Registry.CurrentUser.OpenSubKey(subKey, true)?.SetValue(valueName, selfPath);
                }
                else
                {
                    Registry.CurrentUser.OpenSubKey(subKey, true)?.DeleteValue(valueName);
                }
            }
        }

        
    }
}
