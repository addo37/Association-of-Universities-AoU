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
    /// Interaction logic for ContributionInfo.xaml
    /// </summary>
    public partial class ContributionInfo : UserControl
    {
        Contribution target;
        public ContributionInfo()
        {
            InitializeComponent();
        }

        public ContributionInfo(Contribution Contribution)
        {
            target = Contribution;
            InitializeComponent();

            if (!Switcher.Rank.Equals("Manager"))
            {
                //btnAccept.IsEnabled = false;
               // btnReject.IsEnabled = false;
            }
            txbappID.Text = "" + target.application_id;
            txbName.Text = "" + target.name;
            txbUsername.Text = "" + target.username;
            txbSubmitted.Text = "" + target.date_submission.Substring(0, target.date_submission.Length - 11);
            txbUniversity.Text = "" + target.university;
            txbAmount.Text = "" + target.amount;
            txbAttribute.Text = "" + target.attribute;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Contributions());
        }
        /*
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            Switcher.acceptContribution(target);
            Switcher.Contributions.Remove(target);
            target.status = "Accepted";
            Switcher.Contributions.Add(target);

            Switcher.Switch(new Contributions());
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            Switcher.rejectContribution(target);
            Switcher.Contributions.Remove(target);
            target.status = "Rejected";
            Switcher.Contributions.Add(target);

            Switcher.Switch(new Contributions());
        }*/
    }
}
