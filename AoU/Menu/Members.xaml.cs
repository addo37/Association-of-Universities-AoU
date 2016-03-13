using ServerData;
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
    /// Interaction logic for Members.xaml
    /// </summary>
    public partial class Members : UserControl
    {
        public Members()
        {
            InitializeComponent();

            lstUsers.ItemsSource = Switcher.Users;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Advisor.CheckPassStrength(txbPassword.Text) < PasswordScore.Medium)
            {
                lblError.Content = "Password is weak.";
                return;
            }

            if (!Advisor.CheckEmail(txbEmail.Text))
            {
                lblError.Content = "Email is invalid.";
                return;
            }

            if (txbUsername.Text.Length < 4)
            {
                lblError.Content = "Username is too short.";
                return;
            }

            if (txbName.Text.Length < 4)
            {
                lblError.Content = "Name is too short.";
                return;
            }

            if (!(txbAtt.Text.Equals("Member") || txbAtt.Text.Equals("Student") || txbAtt.Text.Equals("Professor") || txbAtt.Text.Equals("Manager")))
            {
                lblError.Content = "Attribute is invalid.";
                return;
            }

            Switcher.addMember(txbUsername.Text, txbPassword.Text, txbEmail.Text, txbName.Text, txbAtt.Text, txbUniversity.Text);

            lblError.Content = "";
            txbAtt.Text = "";
            txbName.Text = "";
            txbUsername.Text = "";
            txbEmail.Text = "";
            txbPassword.Text = "";
            txbUniversity.Text = "";

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AdminPanel());
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try {
                User user= (User)lstUsers.SelectedItem;
                Switcher.deleteUser(user);
            } catch (Exception ex)
            {

            }
}

        private void btnDemote_Click(object sender, RoutedEventArgs e)
        {
            try {
                User user = (User)lstUsers.SelectedItem;
                if (user.attribute.Equals("Manager"))
                    Switcher.demoteUser(user);
            } catch (Exception ex)
            {

            }
}

        private void btnPromote_Click(object sender, RoutedEventArgs e)
        {
            try { 
                 User user = (User)lstUsers.SelectedItem;
                Switcher.promoteUser(user);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
