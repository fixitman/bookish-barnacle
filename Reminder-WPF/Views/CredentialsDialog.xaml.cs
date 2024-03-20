using Reminder_WPF.Models;
using System.Windows;

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
