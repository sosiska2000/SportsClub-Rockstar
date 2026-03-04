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

namespace Rockstar.Admin.WPF.Views.Directions
{
    /// <summary>
    /// Логика взаимодействия для DirectionsView.xaml
    /// </summary>
    public partial class DirectionsView : Page
    {
        private readonly Frame _mainFrame;
        public DirectionsView(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
        }
    }
}
