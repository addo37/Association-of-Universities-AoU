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
    /// Interaction logic for ScholarInfo.xaml
    /// </summary>
    public partial class ScholarInfo : UserControl
    {
        Scholarship target;
        public ScholarInfo()
        {
            InitializeComponent();
        }

        public ScholarInfo(Scholarship scho)
        {
            target = scho;
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
            txbValidity.Text = "" + target.valid_until.Substring(0, target.date_submission.Length - 12);
            txbUniversity.Text = "" + target.university;
            txbFName.Text = "" + target.father_name;
            txbFWorktype.Text = "" + target.father_worktype;
            txbFStarted.Text = "" + target.father_startdate.Substring(0, target.date_submission.Length - 12);
            txbFEnded.Text = "" + target.father_enddate.Substring(0, target.date_submission.Length - 12);
            txbFIncome.Text = "" + target.father_income;
            txbMName.Text = "" + target.mother_name;
            txbMWorktype.Text = "" + target.mother_worktype;
            txbMStarted.Text = "" + target.mother_startdate.Substring(0, target.date_submission.Length - 12);
            txbMEnded.Text = "" + target.mother_enddate.Substring(0, target.date_submission.Length - 12);
            txbMIncome.Text = "" + target.mother_income;
            txbOIncome.Text = "" + target.other_income;
            txbExpenses.Text = "" + target.expenses;
            txbPercent.Text = "" + target.percentage;
            txbTI.Text = "" + (target.mother_income + target.father_income + target.other_income);
            txbDI.Text = "" + (target.mother_income + target.father_income + target.other_income - target.expenses);
            txbFDuration.Text = target.father_enddate.Substring(0, 8).Equals("1/1/0001") ? "N/A" : (DateTime.Parse(target.father_enddate) - DateTime.Parse(target.father_startdate)) + "";
            txbMDuration.Text = target.mother_enddate.Substring(0, 8).Equals("1/1/0001") ? "N/A" : (DateTime.Parse(target.mother_enddate) - DateTime.Parse(target.mother_startdate)) + "";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Scholarships());
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = Int32.Parse(txbPercent.Text);
                if (x < 1 || x > 100)
                    throw new Exception();
            }
            catch(Exception ex)
            {
                lblError.Content = "Percentage given is not valid.";
            }

            Switcher.acceptScholarship(target, Int32.Parse(txbPercent.Text));
            //from scholarship in Switcher.Scholarships where scholarship.application_id == target.application_id select scholarship;
            Switcher.Scholarships.Remove(target);
            target.status = "Accepted";
            target.percentage = Int32.Parse(txbPercent.Text);
            Switcher.Scholarships.Add(target);

            Switcher.Switch(new Scholarships());
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            Switcher.rejectScholarship(target);
            Switcher.Scholarships.Remove(target);
            target.status = "Rejected";
            Switcher.Scholarships.Add(target);

            Switcher.Switch(new Scholarships());
        }
    }
}
