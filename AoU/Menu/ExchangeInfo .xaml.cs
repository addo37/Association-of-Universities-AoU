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
    /// Interaction logic for ExchangeInfo.xaml
    /// </summary>
    public partial class ExchangeInfo : UserControl
    {
        Exchange target;
        public ExchangeInfo()
        {
            InitializeComponent();
        }

        public ExchangeInfo(Exchange exchange)
        {
            target = exchange;
            InitializeComponent();

            if (!Switcher.Rank.Equals("Manager"))
            {
                btnAccept.IsEnabled = false;
                btnReject.IsEnabled = false;
            }
            txbappID.Text = "" + target.application_id;
            txbName.Text = "" + target.name;
            txbUsername.Text = "" + target.username;
            txbSubmitted.Text = "" + target.date_submission.Substring(0, target.date_submission.Length - 12);
            txbValidity.Text = "" + target.valid_until;
            txbUniversity.Text = "" + target.university;
            txbSubjects.Text = "" + target.subjects;
            txbClasshours.Text = "" + target.class_hours;
            txbOInfo.Text = "" + target.other_info;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Exchanges());
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Switcher.acceptExchange(target);
            //from Exchange in Switcher.Exchanges where Exchange.application_id == target.application_id select Exchange;
            Switcher.Exchanges.Remove(target);
            target.status = "Accepted";
            Switcher.Exchanges.Add(target);

            Switcher.Switch(new Exchanges());
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            Switcher.rejectExchange(target);
            Switcher.Exchanges.Remove(target);
            target.status = "Rejected";
            Switcher.Exchanges.Add(target);

            Switcher.Switch(new Exchanges());
        }
    }
}
