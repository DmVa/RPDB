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
    public partial class DatabasesWindow : Window
    {
        private DatabasesWindowViewModel _vm;
        public DatabasesWindow()
        {
            _vm = new DatabasesWindowViewModel();
            _vm.Window = this;
            this.DataContext = _vm;
            InitializeComponent();
            _vm.LoadData();
        }
    }
}
