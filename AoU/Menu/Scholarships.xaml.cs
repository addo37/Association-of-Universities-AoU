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
    public partial class Scholarships : UserControl
    {

        public Scholarships()
        {
            InitializeComponent();

            List<Scholarship> accepted = new List<Scholarship>();
            List<Scholarship> rejected = new List<Scholarship>();
            List<Scholarship> pending = new List<Scholarship>();
            Scholarship temp;

            for (int i = 0; i < Switcher.Scholarships.Count; i++)
            {
                temp = Switcher.Scholarships[i];
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
                Scholarship scho = (Scholarship) lstPending.SelectedItem;
                Switcher.Switch(new ScholarInfo(scho));
            } catch (Exception ex)
            {

            }
}

        private void lstAccepted_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try {
                Scholarship scho = (Scholarship)lstAccepted.SelectedItem;
                Switcher.Switch(new ScholarInfo(scho));
            } catch (Exception ex)
            {

            }
}

        private void lstRejected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try {
                Scholarship scho = (Scholarship)lstRejected.SelectedItem;
                Switcher.Switch(new ScholarInfo(scho));
            } catch (Exception ex)
            {

            }
}
    }
}
