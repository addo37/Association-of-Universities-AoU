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
using ServerData;

namespace Client.Menu
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Exchanges : UserControl
    {

        public Exchanges()
        {
            InitializeComponent();

            List<Exchange> accepted = new List<Exchange>();
            List<Exchange> rejected = new List<Exchange>();
            List<Exchange> pending = new List<Exchange>();
            Exchange temp;

            for (int i = 0; i < Switcher.Exchanges.Count; i++)
            {
                temp = Switcher.Exchanges[i];
                if (temp.status.Equals("Awaiting Decision"))
                    pending.Add(temp);
                else if (temp.status.Equals("Accepted"))
                    accepted.Add(temp);
                else
                    rejected.Add(temp);
            }

            lstPending.ItemsSource = pending;
            lstAccepted.ItemsSource = accepted;
            lstRejected.ItemsSource = rejected;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void lstPending_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try { 
                Exchange exchange = (Exchange) lstPending.SelectedItem;
                Switcher.Switch(new ExchangeInfo(exchange));
            } catch (Exception ex)
            {

            }
}

        private void lstRejected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try { 
                Exchange exchange = (Exchange)lstRejected.SelectedItem;
                Switcher.Switch(new ExchangeInfo(exchange));
             } catch (Exception ex)
            {

            }
}

        private void lstAccepted_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try {
                Exchange exchange = (Exchange)lstAccepted.SelectedItem;
                Switcher.Switch(new ExchangeInfo(exchange));
            } catch (Exception ex)
            {

            }
}
    }
}
