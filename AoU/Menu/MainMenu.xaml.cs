using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerData;
namespace Client.Menu
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();

           if (!Switcher.Rank.Equals("Manager"))
                btnAdmin.IsEnabled = false;

            lblRank.Content = Switcher.Rank;
            lblWelcome.Content += Switcher.Name;

            txtChat.Document.Blocks.Add(new Paragraph(new Run("Start chatting with other committee members here!")));

            Switcher.myTimer = new System.Timers.Timer();
            // Tell the timer what top do when it elapses
            Switcher.myTimer.Elapsed += new ElapsedEventHandler(Switcher.updateUserList);
            // Set it to go off every two minutes
            Switcher.myTimer.Interval = 10000;//120000;
            // And start it        
            Switcher.myTimer.Enabled = true;
            this.Dispatcher.Invoke(new Action(() =>
            {
                  lstEvents.ItemsSource = Switcher.Events;
            }));
        }

        private void btnSships_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Scholarships());
        }

        private void btnExchange_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Exchanges());
        }

        private void btnFunds_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Funds());
        }

        private void btnCont_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Contributions());
        }

        private void txtType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !txtType.Text.Equals(""))
            {
                Switcher.sendChat(txtType.Text);
                txtType.Text = "";
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AdminPanel());
        }

        private void lstEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Event nevent = (Event) lstEvents.SelectedItem;

            Switcher.Switch(new EventInfo(nevent));
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Login());
            Switcher.Events.Clear();
            Switcher.Contributions.Clear();
            Switcher.Scholarships.Clear();
            Switcher.Users.Clear();
            Switcher.Funds.Clear();
        }
    }
}
