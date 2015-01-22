using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using FillingStation.Core.Generators;
using FillingStation.Localization;

namespace FillingStation.Controls
{
    public partial class GeneratorControl : UserControl
    {
        public GeneratorControl()
        {
            InitializeComponent();

            normalGenerator.Visibility = Visibility.Collapsed;
            exponentialGenerator.Visibility = Visibility.Collapsed;
            uniformGenerator.Visibility = Visibility.Collapsed;

            genarationComboBox.SelectionChanged += (sender, args) =>
            {
                int x = genarationComboBox.SelectedIndex;

                determGenerator.Visibility = Visibility.Collapsed;
                normalGenerator.Visibility = Visibility.Collapsed;
                exponentialGenerator.Visibility = Visibility.Collapsed;
                uniformGenerator.Visibility = Visibility.Collapsed;

                if (x == 0)
                {
                    determGenerator.Visibility = Visibility.Visible;
                }

                if (x == 1)
                {
                    normalGenerator.Visibility = Visibility.Visible;
                }

                if (x == 2)
                {
                    exponentialGenerator.Visibility = Visibility.Visible;
                }

                if (x == 3)
                {
                    uniformGenerator.Visibility = Visibility.Visible;
                }
            };
        }

        public IGenerator Generator
        {
            get
            {
                try
                {
                    if (genarationComboBox.SelectedIndex == 0)
                    {
                        double dt = double.Parse(dtTextBox.Text.Replace(",","."), CultureInfo.InvariantCulture);

                        if (!(dt >= 2) || !(dt <= 100))
                            throw new ArgumentException(Strings.Exeption_DeterminRange);

                        return new GeneratorDetermin(dt*TimeSpan.TicksPerSecond);
                    }
                    if (genarationComboBox.SelectedIndex == 1)
                    {
                        double mx = double.Parse(mxTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                        double dx = double.Parse(dxTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                        if (!(mx >= 2) || !(mx <= 100))
                            throw new ArgumentException(Strings.Exception_NormalRange);
                        
                        mx = mx*TimeSpan.TicksPerSecond;
                        dx = dx*TimeSpan.TicksPerSecond;
                        return new GeneratorNormal(mx, dx);
                    }
                    if (genarationComboBox.SelectedIndex == 2)
                    {
                        double y = double.Parse(yTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                        if (!(y >= 0.01) || !(y <= 0.5))
                            throw new ArgumentException(Strings.Exception_ExponentialRange);

                        y = y/TimeSpan.TicksPerSecond;
                        return new GeneratorExponential(y);
                    }
                    if (genarationComboBox.SelectedIndex == 3)
                    {
                        double a = double.Parse(aTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                        double b = double.Parse(bTextBox.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                        if (!(a >= 2) || !(b <= 100))
                            throw new ArgumentException(Strings.Exception_UniformRange);

                        a = a*TimeSpan.TicksPerSecond;
                        b = b*TimeSpan.TicksPerSecond;

                        return new GeneratorUniform(a, b);
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException(Strings.Exception_Generator + "\n" + e.Message, e);
                }
                throw new ArgumentException(Strings.Exception_GeneratorNotChoosen);
            }
        }
    }
}
