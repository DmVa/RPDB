using RPDB.ViewModel;
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
using System.Windows.Shapes;

namespace RPDB.View
{
    /// <summary>
    /// Interaction logic for ServerSetting.xaml
    /// </summary>
    public partial class SearchFoldersWindow : Window
    {
        private SearchFoldersWindowViewModel _vm;
        public SearchFoldersWindow()
        {
            _vm = new SearchFoldersWindowViewModel();
            _vm.Window = this;
            this.DataContext = _vm;
            InitializeComponent();
            _vm.LoadData();
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
