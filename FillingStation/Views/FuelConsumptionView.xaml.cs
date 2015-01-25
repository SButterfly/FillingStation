using System;
using System.Windows;
using FillingStation.DAL;
using FillingStation.DAL.Models;
using FillingStation.Helpers;

namespace FillingStation.Views
{
    public partial class FuelConsumptionView : Window
    {
        public FuelConsumptionView()
        {
            InitializeComponent();

            var fuelData = new FuelModelAccessor().All();

            foreach (var model in fuelData)
            {
                if (model.Fuel == Fuel.A92)
                {
                    _92PercentageTextBox.Text = model.CarPercentage.ToString();
                    _92VolumeTextBox.Text = model.FillingVolume.ToString();
                    _92PriceTextBox.Text = model.Price.ToString();
                }
                if (model.Fuel == Fuel.A95)
                {
                    _95PercentageTextBox.Text = model.CarPercentage.ToString();
                    _95VolumeTextBox.Text = model.FillingVolume.ToString();
                    _95PriceTextBox.Text = model.Price.ToString();
                }
                if (model.Fuel == Fuel.A98)
                {
                    _98PercentageTextBox.Text = model.CarPercentage.ToString();
                    _98VolumeTextBox.Text = model.FillingVolume.ToString();
                    _98PriceTextBox.Text = model.Price.ToString();
                }
                if (model.Fuel == Fuel.Diesel)
                {
                    _DieselPercentageTextBox.Text = model.CarPercentage.ToString();
                    _DieselVolumeTextBox.Text = model.FillingVolume.ToString();
                    _DieselPriceTextBox.Text = model.Price.ToString();
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int __92Percentage = int.Parse(_92PercentageTextBox.Text);
                int __95Percentage = int.Parse(_95PercentageTextBox.Text);
                int __98Percentage = int.Parse(_98PercentageTextBox.Text);
                int __DieselPercentage = int.Parse(_DieselPercentageTextBox.Text);
                double __92Volume = double.Parse(_92VolumeTextBox.Text);
                double __95Volume = double.Parse(_95VolumeTextBox.Text);
                double __98Volume = double.Parse(_98VolumeTextBox.Text);
                double __DieselVolume = double.Parse(_DieselVolumeTextBox.Text);
                double __92Price = double.Parse(_92PriceTextBox.Text);
                double __95Price = double.Parse(_95PriceTextBox.Text);
                double __98Price = double.Parse(_98PriceTextBox.Text);
                double __DieselPrice = double.Parse(_DieselPriceTextBox.Text);

                if (__92Percentage < 0 || __95Percentage < 0 || __98Percentage < 0 || __DieselPercentage < 0)
                {
                   throw new Exception("Процент должен быть неотрицательным.");
                }
                if (__92Percentage + __95Percentage + __98Percentage + __DieselPercentage != 100)
                {
                    throw new Exception("Доли автомобилей должны в сумме давать 100 %.");
                }
                if (__92Price < 1 || __92Price > 1000 || __95Price < 1 || __95Price > 1000 || __98Price < 1 || __98Price > 1000 || __DieselPrice < 1 || __DieselPrice > 1000)
                {
                    throw new Exception("Цена должна находиться в интервале [1; 1000].");
                }
                if (__92Volume < 1 || __92Volume > 200 || __95Volume < 1 || __95Volume > 200 || __98Volume < 1 || __98Volume > 200 || __DieselVolume < 1 || __DieselVolume > 200)
                {
                    throw new Exception("Объем заправляемого топлива должен находиться в интервале [1; 200].");
                }

                var accessor = new FuelModelAccessor();

                accessor.Save(new FuelConsumptionModel(Fuel.A92, __92Percentage, __92Volume, __92Price));
                accessor.Save(new FuelConsumptionModel(Fuel.A95, __95Percentage, __95Volume, __95Price));
                accessor.Save(new FuelConsumptionModel(Fuel.A98, __98Percentage, __98Volume, __98Price));
                accessor.Save(new FuelConsumptionModel(Fuel.Diesel, __DieselPercentage, __DieselVolume, __DieselPrice));

                Close();
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(ex);
            }         
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
