using Reminder_WPF.Models;
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
using System.Windows.Shapes;

namespace Reminder_WPF.Views
{
    /// <summary>
    /// Interaction logic for CredentialsDialog.xaml
    /// </summary>
    public partial class CredentialsDialog : Window
    {
        public LoginModel Credentials;

        public CredentialsDialog(LoginModel login)
        {
            InitializeComponent();
            Credentials = login;
            DataContext = Credentials;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Credentials.password = Password.Password;
            MessageBox.Show($"{Credentials.UserName} => {Credentials.password}");
            this.DialogResult = true;
        }
    }
}
