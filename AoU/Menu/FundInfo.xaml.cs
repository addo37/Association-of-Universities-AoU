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
    /// Interaction logic for FundInfo.xaml
    /// </summary>
    public partial class FundInfo : UserControl
    {
        Fund target;
        public FundInfo()
        {
            InitializeComponent();
        }

        public FundInfo(Fund Fund)
        {
            target = Fund;
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
            txbPlannedterm.Text = "" + target.planned_term;
            txbItemized.Text = "" + target.itemized_expenses;
            txbReasons.Text = "" + target.reasons;
            txbAmount.Text = "" + target.amount;
            txbAmountgiven.Text = "" + target.amount_given;
            txbOInfo.Text = "" + target.other_info;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Funds());
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = Int32.Parse(txbAmountgiven.Text);
                if (x < 0)
                    throw new Exception();
            }
            catch (Exception ex)
            {
                lblError.Content = "Amount given is not valid.";
            }

            Switcher.acceptFund(target, Int32.Parse(txbAmountgiven.Text));
            Switcher.Funds.Remove(target);
            target.status = "Accepted";
            target.amount = Int32.Parse(txbAmountgiven.Text);
            Switcher.Funds.Add(target);

            Switcher.Switch(new Funds());
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            Switcher.rejectFund(target);
            Switcher.Funds.Remove(target);
            target.status = "Rejected";
            Switcher.Funds.Add(target);

            Switcher.Switch(new Funds());
        }
    }
}
