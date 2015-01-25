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

            IList<FuelConsumptionModel> fuelData = new FuelModelAccessor().All();

            foreach (FuelConsumptionModel model in fuelData)
            {
                if (model.Fuel == Fuel.A92)
                {
                    _92Percentage.Text = model.CarPercentage.ToString();
                    _92Volume.Text = model.FillingVolume.ToString();
                    _92Price.Text = model.Price.ToString();
                }
                if (model.Fuel == Fuel.A95)
                {
                    _95Percentage.Text = model.CarPercentage.ToString();
                    _95Volume.Text = model.FillingVolume.ToString();
                    _95Price.Text = model.Price.ToString();
                }
                if (model.Fuel == Fuel.A98)
                {
                    _98Percentage.Text = model.CarPercentage.ToString();
                    _98Volume.Text = model.FillingVolume.ToString();
                    _98Price.Text = model.Price.ToString();
                }
                if (model.Fuel == Fuel.Diesel)
                {
                    _DieselPercentage.Text = model.CarPercentage.ToString();
                    _DieselVolume.Text = model.FillingVolume.ToString();
                    _DieselPrice.Text = model.Price.ToString();
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int __92Percentage = int.Parse(_92Percentage.Text);
                int __95Percentage = int.Parse(_95Percentage.Text);
                int __98Percentage = int.Parse(_98Percentage.Text);
                int __DieselPercentage = int.Parse(_DieselPercentage.Text);
                double __92Volume = double.Parse(_92Volume.Text);
                double __95Volume = double.Parse(_95Volume.Text);
                double __98Volume = double.Parse(_98Volume.Text);
                double __DieselVolume = double.Parse(_DieselVolume.Text);
                double __92Price = double.Parse(_92Price.Text);
                double __95Price = double.Parse(_95Price.Text);
                double __98Price = double.Parse(_98Price.Text);
                double __DieselPrice = double.Parse(_DieselPrice.Text);

                if (__92Percentage < 0 || __95Percentage < 0 || __98Percentage < 0 || __DieselPercentage < 0)
                {
                    MessageDialog.ShowException("Процент должен быть неотрицательным.");
                    return;
                }
                if (!(__92Percentage + __95Percentage + __98Percentage + __DieselPercentage == 100))
                {
                    MessageDialog.ShowException("Доли автомобилей должны в сумме давать 100 %.");
                    return;
                }
                if (__92Price < 1 || __92Price > 1000 || __95Price < 1 || __95Price > 1000 || __98Price < 1 || __98Price > 1000 || __DieselPrice < 1 || __DieselPrice > 1000)
                {
                    MessageDialog.ShowException("Цена должна находиться в интервале [1; 1000].");
                    return;
                }
                if (__92Volume < 1 || __92Volume > 200 || __95Volume < 1 || __95Volume > 200 || __98Volume < 1 || __98Volume > 200 || __DieselVolume < 1 || __DieselVolume > 200)
                {
                    MessageDialog.ShowException("Объем заправляемого топлива должен находиться в интервале [1; 200].");
                    return;
                }

                FuelModelAccessor accessor = new FuelModelAccessor();

                accessor.Save(new FuelConsumptionModel(Fuel.A92, __92Percentage, __92Volume, __92Price));
                accessor.Save(new FuelConsumptionModel(Fuel.A95, __95Percentage, __95Volume, __95Price));
                accessor.Save(new FuelConsumptionModel(Fuel.A98, __98Percentage, __98Volume, __98Price));
                accessor.Save(new FuelConsumptionModel(Fuel.Diesel, __DieselPercentage, __DieselVolume, __DieselPrice));

                Close();
            }
            catch
            {
                MessageDialog.ShowException("Ошибка ввода.");
            }         
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
