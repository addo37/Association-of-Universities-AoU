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
    public partial class Contributions : UserControl
    {

        public Contributions()
        {
            InitializeComponent();

            //  List<Contribution> accepted = new List<Contribution>();
            //List<Contribution> rejected = new List<Contribution>();
            List<Contribution> pending = Switcher.Contributions;
           // Contribution temp;
            /*
            for (int i = 0; i < Switcher.Contributions.Count; i++)
            {
                temp = Switcher.Contributions[i];
                if (temp.status.Equals("Awaiting Decision"))
                    pending.Add(temp);
                else if (temp.status.Equals("Accepted"))
                    accepted.Add(temp);
                else
                    rejected.Add(temp);
            }
            */
            lstPending.ItemsSource = pending;
           // lstAccepted.ItemsSource = accepted;
           // lstRejected.ItemsSource = rejected;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void lstPending_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try { Contribution Contribution = (Contribution)lstPending.SelectedItem;
                Switcher.Switch(new ContributionInfo(Contribution)); } catch (Exception ex)
            {

            }
        }
        
    }
}
