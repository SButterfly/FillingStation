using System;
using System.Globalization;
using System.Windows;
using FillingStation.Core.Vehicles;
using FillingStation.DAL;
using FillingStation.DAL.Models;
using FillingStation.Helpers;
using FillingStation.Localization;

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
                double __92Volume = double.Parse(_92VolumeTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __95Volume = double.Parse(_95VolumeTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __98Volume = double.Parse(_98VolumeTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __DieselVolume = double.Parse(_DieselVolumeTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __92Price = double.Parse(_92PriceTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __95Price = double.Parse(_95PriceTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __98Price = double.Parse(_98PriceTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double __DieselPrice = double.Parse(_DieselPriceTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                if (__92Percentage < 0 || __95Percentage < 0 || __98Percentage < 0 || __DieselPercentage < 0)
                {
                   throw new Exception(Strings.Exception_PercentageMustBeNonNegativeValue);
                }
                if (__92Percentage + __95Percentage + __98Percentage + __DieselPercentage != 100)
                {
                    throw new Exception(Strings.Exception_SumPercentage);
                }

                const double minVolume = 0d;
                const double maxVolume = 200;

                if (__92Volume <= minVolume || __92Volume > maxVolume ||
                    __95Volume <= minVolume || __95Volume > maxVolume ||
                    __98Volume <= minVolume || __98Volume > maxVolume ||
                    __DieselVolume <= minVolume || __DieselVolume > maxVolume)
                {
                    throw new Exception(string.Format(Strings.Exception_FillingVolumeFormat, minVolume, maxVolume));
                }

                const double minPrice = 0d;
                const double maxPrice = 1000d;

                if (__92Price <= minPrice || __92Price > maxPrice ||
                    __95Price <= minPrice || __95Price > maxPrice ||
                    __98Price <= minPrice || __98Price > maxPrice ||
                    __DieselPrice <= minPrice || __DieselPrice > maxPrice)
                {
                    throw new Exception(string.Format(Strings.Exception_FuelPriceFormat, minPrice, maxPrice));
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
