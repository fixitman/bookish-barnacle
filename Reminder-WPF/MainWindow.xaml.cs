using System.Windows;
using System.Windows.Controls;

namespace Reminder_WPF;


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
}
