using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Reminder_WPF.Models;
using Reminder_WPF.Services;
using Reminder_WPF.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows;

namespace Reminder_WPF.ViewModels;

public partial class MainWindowVM : ObservableObject
{

    [ObservableProperty]
    public Reminder? selectedItem = null;     
    public MainWindow? MainWindow { get; set; }
    public IReminderManager Reminders { get; }    
    public bool CanDelete { get { return SelectedItem != null; } }
    public bool CanAdd { get { return true; } }
    public bool CanEdit { get { return SelectedItem != null; } }

    public MainWindowVM(IReminderManager mgr)
    {
        Reminders = mgr;        
        MainWindow = null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    public void DeleteClicked()
    {
        if (SelectedItem != null)
        {
            Reminders.RemoveReminder(SelectedItem);
        }
    }

    [RelayCommand(CanExecute = nameof (CanAdd))]
    public void AddClicked()
    {
        var addDlg = new AddEditReminderDlg();
        addDlg.Title = "Add Reminder";
        addDlg.Owner = MainWindow;
        if(addDlg.ShowDialog() == true)
        {
            AddReminder(addDlg.Reminder);
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    public void EditClicked()
    {
        var editDlg = new AddEditReminderDlg(SelectedItem);
        editDlg.Title = "Edit Reminder";
        editDlg.Owner = MainWindow;
        if (editDlg.ShowDialog() == true)
        {
            UpdateReminder(editDlg.Reminder);
        }
    }

    [RelayCommand]
    void MenuExit()
    {
        App.Current.Shutdown();
    }

    [RelayCommand]
    void MenuRefresh()
    {
        Reminders.RefreshReminders();
    }

    [RelayCommand]
    void Export(){
        
       string json = JsonConvert.SerializeObject(Reminders, Formatting.Indented );
       SaveFileDialog saveFileDialog = new SaveFileDialog();
       saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Set the default directory
        saveFileDialog.FileName = "Reminders.json"; // Set a default file name
        saveFileDialog.OverwritePrompt = true; // Prompt if the file already exists (default is true)

        // 3. Display the dialog and check the result
        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                // 4. Get the selected file name and save the content
                string fileName = saveFileDialog.FileName;
                // Example: Saving text from a RichTextBox
                // richTextBox1.SaveFile(fileName, RichTextBoxStreamType.PlainText);

                // Or, using System.IO.File for general text data:
                System.IO.File.WriteAllText(fileName, json); //
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }
    }
    [RelayCommand]
    void Import(){
       
       OpenFileDialog openFileDialog = new OpenFileDialog();
       openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
       openFileDialog.Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string remindersJson = System.IO.File.ReadAllText(openFileDialog.FileName);
                List<Reminder>? list = JsonConvert.DeserializeObject<List<Reminder>>(remindersJson);
                if(list != null)
                {
                    foreach(Reminder reminder in list){
                        Reminders.UpdateReminder(reminder);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }
        
        
        
        // 3. Display the dialog and check the result
        
    }



    partial void OnSelectedItemChanged(Reminder? value)
    {
        DeleteClickedCommand.NotifyCanExecuteChanged();
        EditClickedCommand.NotifyCanExecuteChanged();
    }

    public void UpdateReminder(Reminder r)
    {
        _ = Reminders.UpdateReminder(r);
    }

    public void AddReminder(Reminder r)
    {
        _ = Reminders.AddReminder(r);
    }

}
