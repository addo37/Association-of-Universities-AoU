using ServerData;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Client.Menu
{
    /// <summary>
    /// Interaction logic for Minutes.xaml
    /// </summary>
    public partial class Minutes : UserControl
    {
        public static Dictionary<String, String> filemap;
        public Minutes()
        {
            InitializeComponent();
            filemap = new Dictionary<string, string>();
            Switcher.getLogs();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AdminPanel());
        }

        private void lstFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try { 
            txbLog.Document.Blocks.Clear();
            String filename = (String)lstFiles.SelectedItem;
            txbLog.Document.Blocks.Add(new Paragraph(new Run(filemap[filename])));
            } catch (Exception ex)
            {
            }
}
    }
}
