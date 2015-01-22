using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using FillingStation.Assets;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Views
{
    public partial class CarsDBView : Window
    {
        public CarsDBView()
        {
            InitializeComponent();

            var images = getImages(new string[] {
                "/Assets/Vehicles/Cars/vh_car_1.png",
                "/Assets/Vehicles/Cars/vh_car_2.png",
                "/Assets/Vehicles/Cars/vh_car_3.png",
                "/Assets/Vehicles/Cars/vh_car_4.png",
                "/Assets/Vehicles/Cars/vh_car_5.png",
                "/Assets/Vehicles/Cars/vh_car_6.png",
                "/Assets/Vehicles/Cars/vh_car_7.png",
                "/Assets/Vehicles/Cars/vh_car_8.png",
                "/Assets/Vehicles/Cars/vh_car_9.png",
                "/Assets/Vehicles/Cars/vh_car_10.png",
                "/Assets/Vehicles/Cars/vh_car_11.png",
                "/Assets/Vehicles/Cars/vh_car_12.png",
                "/Assets/Vehicles/Cars/vh_car_13.png",
                "/Assets/Vehicles/Cars/vh_car_14.png",
                "/Assets/Vehicles/Cars/vh_car_15.png",
            });

            comboBox.ItemsSource = images;

            Loaded += (sender, args) =>
            {
                //hack to preload
                foreach (var image in images)
                {
                    root.Children.Add(image);
                }

                root.UpdateLayout();

                foreach (var image in images)
                {
                    root.Children.Remove(image);
                }

                var path = image_pathTextBox.Text;
                SetPath(path);
            };
        }

        private IList<Image> getImages(IEnumerable<string> uri)
        {
            var list = new List<Image>();
            foreach (var path in uri)
            {
                var im = new Image();

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + path, UriKind.Relative);
                image.EndInit();

                im.Source = image;
                im.Tag = path.Replace("/Assets/", "");
                im.Height = 90;
                list.Add(im);
            }
            return list;
        }

        private FillingStation_DBDataSet _fillingStationDbDataSet;
        private Car_ModelTableAdapter _fillingStationDbDataSetCarModelTableAdapter;
        private CollectionViewSource _carModelViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _fillingStationDbDataSet = ((FillingStation_DBDataSet)(FindResource("fillingStation_DBDataSet")));

                // Load data into the table Car_Model. You can modify this code as needed.
                _fillingStationDbDataSetCarModelTableAdapter = new Car_ModelTableAdapter();
                _fillingStationDbDataSetCarModelTableAdapter.Fill(_fillingStationDbDataSet.Car_Model);
                _carModelViewSource = ((CollectionViewSource)(FindResource("car_ModelViewSource")));
                _carModelViewSource.View.MoveCurrentToFirst();

                // Load data into the table Car_Type. You can modify this code as needed.
                Car_TypeTableAdapter fillingStation_DBDataSetCar_TypeTableAdapter = new Car_TypeTableAdapter();
                fillingStation_DBDataSetCar_TypeTableAdapter.Fill(_fillingStationDbDataSet.Car_Type);
                CollectionViewSource carTypeViewSource = ((CollectionViewSource)(FindResource("car_TypeViewSource")));
                carTypeViewSource.View.MoveCurrentToFirst();

                // Load data into the table Fuel_Type. You can modify this code as needed.
                Fuel_TypeTableAdapter fillingStation_DBDataSetFuel_TypeTableAdapter = new Fuel_TypeTableAdapter();
                fillingStation_DBDataSetFuel_TypeTableAdapter.Fill(_fillingStationDbDataSet.Fuel_Type);
                CollectionViewSource fuelTypeViewSource = ((CollectionViewSource)(FindResource("fuel_TypeViewSource")));
                fuelTypeViewSource.View.MoveCurrentToFirst();
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
                Close();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_carModelViewSource.View.CurrentPosition < ((CollectionView)_carModelViewSource.View).Count - 1)
                {
                    _carModelViewSource.View.MoveCurrentToNext();
                    var path = image_pathTextBox.Text;
                    SetPath(path);
                }
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_carModelViewSource.View.CurrentPosition > 0)
                {
                    _carModelViewSource.View.MoveCurrentToPrevious();
                    var path = image_pathTextBox.Text;
                    SetPath(path);
                }
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (FillingStation_DBDataSet.Car_ModelRow row in _fillingStationDbDataSet.Car_Model.Rows)
                {
                    decimal d = row.tank_volume;
                    if (d <= 0)
                    {
                        row.tank_volume = Math.Abs(row.tank_volume);
                    }
                    if (d > 200)
                    {
                        row.tank_volume = 200;
                    }
                }
                image_pathTextBox.Text = GetPath();
                _fillingStationDbDataSetCarModelTableAdapter.Update(_fillingStationDbDataSet.Car_Model);
                _fillingStationDbDataSet.Car_Model.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Searching for max ID
                short maxID = 1;
                foreach(FillingStation_DBDataSet.Car_ModelRow row in _fillingStationDbDataSet.Car_Model.Rows)
                {
                    if (row.car_model_id > maxID)
                    {
                        maxID = row.car_model_id;
                    }
                }

                // Creating a new row
                FillingStation_DBDataSet.Car_ModelRow newCar_ModelRow = _fillingStationDbDataSet.Car_Model.NewCar_ModelRow();
                newCar_ModelRow.car_model_id = (short)(maxID + 1);

                newCar_ModelRow.car_model_name = Strings.Car;
                newCar_ModelRow.image_path = Strings.Car_path;
                newCar_ModelRow.tank_volume = Strings.Car_tankVolume;
                newCar_ModelRow.car_type_id = Strings.Car_carSize;
                newCar_ModelRow.fuel_type_id = Strings.Car_fuel;

                // Adding the row to the Car_Model table
                _fillingStationDbDataSet.Car_Model.Rows.Add(newCar_ModelRow);

                // Saving the new row to the database
                _fillingStationDbDataSetCarModelTableAdapter.Update(_fillingStationDbDataSet.Car_Model);

                _carModelViewSource.View.MoveCurrentToLast();

                var path = image_pathTextBox.Text;
                SetPath(path);
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_fillingStationDbDataSet.Car_Model.Rows.Count > 1)
                {
                    // Locate the row to delete.
                    var oldCarModelRow = (FillingStation_DBDataSet.Car_ModelRow)_fillingStationDbDataSet.Car_Model.Rows[_carModelViewSource.View.CurrentPosition];

                    // Delete the row from the dataset
                    oldCarModelRow.Delete();

                    // Delete the row from the database
                    _fillingStationDbDataSetCarModelTableAdapter.Update(_fillingStationDbDataSet.Car_Model);

                    _carModelViewSource.View.Refresh();

                    var path = image_pathTextBox.Text;
                    SetPath(path);
                }
                else
                {
                    MessageDialog.ShowException(Strings.Exception_DB_singleDelete);
                }
            }
            catch (Exception ex)
            {
                MessageDialog.ShowException(Strings.Exception_DB + "\n" + ex.Message);
            }
        }

        private void SetPath(string path)
        {
            comboBox.SelectedItem = null;
            comboBox.Height = 24;
            foreach (Image item in comboBox.Items)
            {
                if ((string)item.Tag == path)
                {
                    comboBox.SelectedItem = item;
                    comboBox.Height = Math.Max(item.Height, 24);
                    break;
                }
            }
        }

        private string GetPath()
        {
            var image = comboBox.SelectedItem as Image;
            if (image != null)
            {
                return (string)image.Tag;
            }
            return null;
        }
    }
}