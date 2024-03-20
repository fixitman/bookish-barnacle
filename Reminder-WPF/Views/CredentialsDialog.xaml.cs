using Reminder_WPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace Reminder_WPF.Views
{
    /// <summary>
    /// Interaction logic for CredentialsDialog.xaml
    /// </summary>
    public partial class CredentialsDialog : Window
    {
        public CredentialsModel Credentials { get; private set; }
                

        public CredentialsDialog(CredentialsModel savedCreds)
        {
            Credentials = savedCreds;            
            InitializeComponent();
            Password.Password = Credentials.Password;
            DataContext = Credentials;
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = true;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Credentials.Password = (sender as PasswordBox)!.Password;
        }
    }
}
