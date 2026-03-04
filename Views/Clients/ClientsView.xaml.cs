using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Clients
{
    public partial class ClientsView : Page
    {
        private readonly Frame _mainFrame;

        public ClientsView(Frame mainFrame)
        {
            _mainFrame = mainFrame;
            InitializeComponent();
        }
    }
}