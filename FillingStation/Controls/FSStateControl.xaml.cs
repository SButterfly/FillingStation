using System.Windows.Controls;
using FillingStation.Core.Models;

namespace FillingStation.Controls
{
    public partial class FSStateControl : UserControl
    {
        public FSStateControl()
        {
            InitializeComponent();
            FSStateModel = FSStateModel.Empty;
        }

        public FSStateModel FSStateModel
        {
            get { return DataContext as FSStateModel; } 
            set { DataContext = value; }
        }
    }
}
