using Reminder_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reminder_WPF.Views
{
    /// <summary>
    /// Interaction logic for TBMenu.xaml
    /// </summary>
    public partial class TBMenu : UserControl
    {
        public TBMenu()
        {
            InitializeComponent();
            this.DataContext = new TaskBarIconVM();
        }
    }
}
