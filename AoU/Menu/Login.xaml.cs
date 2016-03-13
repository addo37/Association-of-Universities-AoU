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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl, ISwitchable
    {
        public Login()
        {
            InitializeComponent();
            FocusManager.SetFocusedElement(this, txtUser);
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Login(txtUser.Text, txtPass.Password);
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Switcher.Login(txtUser.Text, txtPass.Password);
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Help());
        }
    }
}
