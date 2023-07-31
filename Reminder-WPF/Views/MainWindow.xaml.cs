using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Reminder_WPF.Models;
using Reminder_WPF.ViewModels;
using System.Windows.Data;

namespace Reminder_WPF.Views;


public partial class MainWindow : Window
{
    public MainWindow(MainWindowVM vM)
    {
        InitializeComponent();
        DataContext = vM;
        VM = vM;
    }

    public MainWindowVM VM { get; }

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
                var dlg = new TestDialog();
                dlg.Owner = this;
                if (dlg.ShowDialog() == true)
                {
                    var newText = dlg.Reply;
                    selected.ReminderText = newText;
                    VM.UpdateReminder(selected);
                    CollectionViewSource.GetDefaultView(VM.Reminders).Refresh();
                }
            }
        }
    }

    
}
