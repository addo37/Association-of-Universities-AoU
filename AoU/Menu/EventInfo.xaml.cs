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
    /// Interaction logic for Event.xaml
    /// </summary>
    public partial class EventInfo : UserControl
    {
        public EventInfo()
        {
            InitializeComponent();
        }

        public EventInfo(Event nevent)
        {
            InitializeComponent();

            txbExpire.Text = nevent.dateend;
            txbName.Text = nevent.name;
            txbStart.Text = nevent.datestart;
            txbDesc.Text = nevent.desc;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
    }
}
