using System.Windows.Controls;
using FillingStation.Core.Models;

namespace FillingStation.Controls
{
    public partial class FSStatisticsControl : UserControl
    {
        public FSStatisticsControl()
        {
            InitializeComponent();
            FSStatisticsModel = FSStatisticsModel.Empty;
        }

        public FSStatisticsModel FSStatisticsModel
        {
            get { return DataContext as FSStatisticsModel; }
            set { DataContext = value; }
        }
    }
}
