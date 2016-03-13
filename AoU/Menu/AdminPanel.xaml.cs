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
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : UserControl
    {
        public AdminPanel()
        {
            InitializeComponent();

            int Srejected = 0;
            int Saccepted = 0;
            int Stotal = 0;
            int Ftotal = 0;
            int Faccepted = 0;
            int Frejected = 0;
            int Etotal = 0;
            int Eaccepted = 0;
            int Erejected = 0;
            int Camount = 0;
            int Famount = 0;

            foreach (Scholarship x in Switcher.Scholarships)
            {
                Stotal++;
                if (x.status.Equals("Accepted"))
                    Saccepted++;
                else if (x.status.Equals("Rejected"))
                    Srejected++;
            }
            foreach (Exchange x in Switcher.Exchanges)
            {
                Etotal++;
                if (x.status.Equals("Accepted"))
                    Eaccepted++;
                else if (x.status.Equals("Rejected"))
                    Erejected++;
            }
            foreach (Fund x in Switcher.Funds)
            {
                Ftotal++;
                if (x.status.Equals("Accepted"))
                    Faccepted++;
                else if (x.status.Equals("Rejected"))
                    Frejected++;
                Famount += x.amount_given;
            }
            foreach (Contribution x in Switcher.Contributions)
            {
                Camount += x.amount;
            }

            txbStotal.Text += Stotal;
            txbSPending.Text += Stotal - Saccepted - Srejected;
            txbSAccepted.Text += Saccepted;
            txbSRejected.Text += Srejected;

            txbEtotal.Text += Etotal;
            txbEPending.Text += Etotal - Eaccepted - Erejected;
            txbEAccepted.Text += Eaccepted;
            txbERejected.Text += Erejected;

            txbFtotal.Text += Ftotal;
            txbFPending.Text += Ftotal - Faccepted - Frejected;
            txbFAccepted.Text += Faccepted;
            txbFRejected.Text += Frejected;

            txbFA.Text += Famount;
            txbCA.Text += Camount;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void btnMembers_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Members());
        }

        private void btnMinutes_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Minutes());
        }

        private void btnAddevent_Click(object sender, RoutedEventArgs e)
        {
            if (!txbName.Text.Equals("") && (dteStarts.SelectedDate.Value < dteExpires.SelectedDate.Value) && dteStarts.SelectedDate.Value > DateTime.Now)
            {
                DateTime start = DateTime.Parse(dteStarts.SelectedDate.Value + "");
                DateTime end = DateTime.Parse(dteExpires.SelectedDate.Value + "");
                Switcher.addEvent(txbName.Text, start.ToString("yyyy-MM-dd HH:mm:ss"), end.ToString("yyyy-MM-dd HH:mm:ss"), txbDesc.Text);
                txbName.Text = "";
                dteStarts.SelectedDate = null;
                dteExpires.SelectedDate = null;
                txbDesc.Text = "";
            }


        }
    }
}
