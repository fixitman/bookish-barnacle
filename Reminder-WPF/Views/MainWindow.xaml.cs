using Reminder_WPF.Models;
using Reminder_WPF.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Reminder_WPF.Views;


public partial class MainWindow : Window
{
    public MainWindowVM VM { get; }

    public MainWindow(MainWindowVM vM)
    {
        //if the window doesn't have a hwnd, TBMenu crashes
        var helper = new WindowInteropHelper(this);
        helper.EnsureHandle();
        
        InitializeComponent();
        DataContext = vM;
        VM = vM;
        VM.MainWindow = this;
        ReminderList.Items.SortDescriptions.Clear();
        ReminderList.Items.SortDescriptions.Add(new SortDescription("ReminderTime", ListSortDirection.Ascending));
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var lv = (ListView)sender;
        VM.SelectedItem = (Reminder)lv.SelectedItem;
    }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if(e.ChangedButton == System.Windows.Input.MouseButton.Left)
        {
            var lv = (ListView)sender;
            Reminder? selected = lv.SelectedItem as Reminder;
            if(selected != null)
            {
                //var dlg = new TestDialog();
                //dlg.Owner = this;                
                //if (dlg.ShowDialog() == true)
                //{
                //    var newText = dlg.Reply;
                //    selected.ReminderText = newText;
                //    VM.UpdateReminder(selected);
                //    CollectionViewSource.GetDefaultView(VM.Reminders).Refresh();
                //}

                VM.EditClicked();
            }
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        this.Hide();
        e.Cancel = true;
    }
}
