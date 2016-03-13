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

namespace Client.Menu
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : UserControl
    {
        public Help()
        {
            InitializeComponent();
            Switcher.tryConnect();
        }

        private void btnToken_Click(object sender, RoutedEventArgs e)
        {
            Switcher.requestToken(txtUser.Text);
            btnToken.IsEnabled = false;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Switcher.changePass(txtUser.Text, txtToken.Text, txtPass.Text);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Login());
        }
    }
}
