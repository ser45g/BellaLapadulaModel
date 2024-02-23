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

namespace MultipleUserLoginForm.Views
{
    /// <summary>
    /// Interaction logic for UserCabinet.xaml
    /// </summary>
    public partial class Admin : UserControl
    {
        public Admin()
        {
            InitializeComponent();
            
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        
        //void SetAccessMatrics()
        //{
        //    IEnumerable<string> source = (IEnumerable<string>)datgrdAccessMatrice.ItemsSource;
        //    foreach (string item in source)
        //    {
        //        DataGridTextColumn column = new DataGridTextColumn() { Header="jj"};
        //    }
        //}

    }
}
