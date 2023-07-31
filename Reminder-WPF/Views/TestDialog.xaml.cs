using System.Windows;

namespace Reminder_WPF.Views
{
   
    public partial class TestDialog : Window
    {

        public string Reply { get; set; } = "";
        public TestDialog()
        {
            InitializeComponent();
            txtReply.Text = Reply;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Reply = txtReply.Text;
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            txtReply.SelectAll();
            txtReply.Focus();
        }
    }
}
