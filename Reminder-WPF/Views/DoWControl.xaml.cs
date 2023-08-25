using Reminder_WPF.POCOs;
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
    /// Interaction logic for DoWControl.xaml
    /// </summary>
    public partial class DoWControl : UserControl
    {
        private DaysOfWeek dow;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(DoWControl) );

        public DoWControl()
        {
            InitializeComponent();
            dow = new DaysOfWeek();
            DataContext = dow;
            
        }

        public string Text
        {
            get => dow.Text; 
            set => dow.Text = value;
        }
        
    }
}
