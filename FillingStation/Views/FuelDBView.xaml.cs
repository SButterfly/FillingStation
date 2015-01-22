using System;
using System.Windows;
using System.Windows.Data;
using FillingStation.Assets;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Views
{
    public partial class FuelDBView : Window
    {
        public FuelDBView()
        {
            InitializeComponent();
        }

        private FillingStation_DBDataSet _fillingStationDbDataSet;
        private Fuel_TypeTableAdapter _fillingStationDbDataSetFuelTypeTableAdapter;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _fillingStationDbDataSet = ((FillingStation_DBDataSet)(FindResource("fillingStation_DBDataSet")));

                // Load data into the table Fuel_Type. You can modify this code as needed.
                _fillingStationDbDataSetFuelTypeTableAdapter = new Fuel_TypeTableAdapter();
                _fillingStationDbDataSetFuelTypeTableAdapter.Fill(_fillingStationDbDataSet.Fuel_Type);
                CollectionViewSource fuelTypeViewSource = ((CollectionViewSource)(FindResource("fuel_TypeViewSource")));
                fuelTypeViewSource.View.MoveCurrentToFirst();
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
                Close();
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (FillingStation_DBDataSet.Fuel_TypeRow row in _fillingStationDbDataSet.Fuel_Type.Rows)
                {
                    decimal d = row.price;
                    if (d <= 0)
                    {
                        row.price = Math.Abs(row.price);
                    }
                    if (d > 100)
                    {
                        row.price = 100; 
                    }
                }
                _fillingStationDbDataSetFuelTypeTableAdapter.Update(_fillingStationDbDataSet.Fuel_Type);
                _fillingStationDbDataSet.Fuel_Type.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
            }
        }
    }
}
