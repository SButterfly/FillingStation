using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FillingStation.Core.Properties;
using FillingStation.Extensions;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Controls
{
    public partial class PropertyControl : UserControl
    {
        public PropertyControl()
        {
            InitializeComponent();
            Property = null;
        }

        #region Properties

        public static readonly DependencyProperty HeaderProperty =
                          DependencyProperty.Register("Property", typeof(IProperty), typeof(PropertyControl), new PropertyMetadata(null));

        private IProperty _property;
        public IProperty Property
        {
            get { return _property; }
            set
            {
                if (_property != null)
                {
                    DisposeTurn(_property as ITurnProperty);
                }

                _property = value;

                IsEnabled = value != null;
                nameTextBlock.Text = "";
                root.Children.Clear();
                turnPanel.Visibility = Visibility.Collapsed;

                if (_property != null)
                {
                    InitializeName(_property);
                    InitilizeTurn(_property as ITurnProperty);
                    InitializeDynamicProperties(_property);
                }
            }
        }

        private void InitializeDynamicProperties(IProperty property)
        {
            if (property == null) return;

            var properties = property.GetType().GetProperties().ToArray();
            foreach (var info in properties)
            {
                //values region
                {
                    var nameProperty = properties.FirstOrDefault(prop => prop.Name == info.Name + "Name" && prop.PropertyType == typeof(string));
                    var serviceType = properties.FirstOrDefault(prop => IsAssignableToGenericType(prop.PropertyType, typeof (EnumService<>), info.PropertyType));
                    var enumService = serviceType != null ? serviceType.GetValue(property) : null;

                    if (enumService != null && nameProperty != null)
                    {
                        AddValuesField(info, new EnumServiceWrapper(enumService), nameProperty, property);
                    }
                }

                if (info.PropertyType == typeof(int))
                {
                    var nameProperty = properties.FirstOrDefault(prop => prop.Name == info.Name + "Name" && prop.PropertyType == typeof(string));
                    if (nameProperty != null)
                    {
                        AddIntField(info, nameProperty, property);
                    }
                }
            }
        }

        private void AddIntField(PropertyInfo intProperty, PropertyInfo nameProperty, IProperty property)
        {
            TextBlock textBlock;
            TextBox textBox;

            AddAnswerBlock(out textBlock, out textBox);

            textBlock.Text = nameProperty.GetValue(property).ToString();
            textBox.Text = intProperty.GetValue(property).ToString();

            property.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameProperty.Name)
                {
                    var text = nameProperty.GetValue(property).ToString();
                    if (textBlock.Text != text)
                    {
                        textBlock.Text = text;
                    }
                }

                if (args.PropertyName == intProperty.Name)
                {
                    var text = intProperty.GetValue(property).ToString();
                    if (textBox.Text != text)
                    {
                        textBox.Text = text;
                    }
                }
            };

            textBox.TextChanged += (sender, args) =>
            {
                var newValue = textBox.Text;
                int result;
                if (int.TryParse(newValue, out result))
                {
                    if ((int)intProperty.GetValue(property) != result)
                    {
                        intProperty.SetValue(property, result);
                    }
                }
            };
        }

        private void AddValuesField(PropertyInfo enumProperty, EnumServiceWrapper enumService, PropertyInfo nameProperty, IProperty property)
        {
            TextBlock textBlock;
            ComboBox comboBox;

            AddChooseBlocks(out textBlock, out comboBox);

            textBlock.Text = nameProperty.GetValue(property).ToString();
            comboBox.ItemsSource = enumService.AllItems().Select(enumService.ToObject);
            comboBox.SelectedItem = enumService.ToObject(enumProperty.GetValue(property));

            property.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameProperty.Name)
                {
                    var text = (string)nameProperty.GetValue(property);
                    if (textBlock.Text != text)
                    {
                        textBlock.Text = text;
                    }
                }

                if (args.PropertyName == enumProperty.Name)
                {
                    var obj = enumService.ToObject(enumProperty.GetValue(property));
                    if (!Equals(obj, comboBox.SelectedItem))
                    {
                        comboBox.SelectedItem = obj;
                    }
                }
            };

            comboBox.SelectionChanged += (sender, args) =>
            {
                var newValue = enumService.ToEnum(comboBox.SelectedItem);
                var propertyValue = enumProperty.GetValue(property);
                if (!Equals(newValue, propertyValue))
                {
                    enumProperty.SetValue(property, newValue);
                }
            };
        }

        private void AddAnswerBlock(out TextBlock textBlock, out TextBox textBox)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(0,5,0,5);
            stackPanel.Orientation = Orientation.Vertical;

            textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBox = new TextBox();

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);

            root.Children.Add(stackPanel);
        }

        private void AddChooseBlocks(out TextBlock textBlock, out ComboBox comboBox)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(0, 5, 0, 5);
            stackPanel.Orientation = Orientation.Vertical;

            textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            comboBox = new ComboBox();

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(comboBox);

            root.Children.Add(stackPanel);
        }

        #endregion

        #region Pattern Name

        private void InitializeName(IProperty property)
        {
            nameTextBlock.Text = Strings.PropertyName + property.PatternName;
        }

        #endregion

        #region Turn

        private void InitilizeTurn(ITurnProperty property)
        {
            if (property == null)
            {
                turnPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                turnPanel.Visibility = Visibility.Visible;

                leftRotationButton.Click += LeftRotationButtonOnClick;
                rightRotationButton.Click += RightRotationButtonOnClick;
            }
        }

        private void DisposeTurn(ITurnProperty property)
        {
            if (property != null)
            {
                leftRotationButton.Click -= LeftRotationButtonOnClick;
                rightRotationButton.Click -= RightRotationButtonOnClick;
            }
        }

        private void LeftRotationButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var turnProperty = Property as ITurnProperty;
            if (turnProperty != null)
            {
                var rotation = turnProperty.Angle;
                turnProperty.Angle = rotation.Sum(Rotation.Rotate270);
            }
        }

        private void RightRotationButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var turnProperty = Property as ITurnProperty;
            if (turnProperty != null)
            {
                var rotation = turnProperty.Angle;
                turnProperty.Angle = rotation.Sum(Rotation.Rotate90);
            }
        }

        #endregion

        #region Enum Service Helper

        private static bool IsAssignableToGenericType(Type givenType, Type baseType, params Type[] genericTypes)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes.Concat(new[] { givenType }))
            {
                if (!it.IsGenericType || it.GetGenericTypeDefinition() != baseType ||
                    genericTypes.Length != it.GenericTypeArguments.Length) continue;

                bool isGood = true;
                for (int i = 0, n = genericTypes.Length; i < n; i++)
                {
                    if (genericTypes[i] != it.GenericTypeArguments[i])
                    {
                        isGood = false;
                        break;
                    }
                }
                if (isGood) return true;
            }

            var type = givenType.BaseType;
            return type != null && IsAssignableToGenericType(type, baseType, genericTypes);
        }

        private class EnumServiceWrapper : EnumService<object>
        {
            private object EnumService { get; set; }
            private Type EnumServiceType { get { return EnumService.GetType(); } }

            public EnumServiceWrapper(object enumService)
            {
                if (typeof(EnumService<>).IsAssignableFrom(enumService.GetType()))
                    throw new ArgumentException("enumService must be EnumService type");

                EnumService = enumService;
            }

            public override IEnumerable<object> AllItems()
            {
                return ((IEnumerable)EnumServiceType.GetMethod("AllItems").Invoke(EnumService, null)).Cast<object>();
            }

            public override object ToEnum(object value)
            {
                return EnumServiceType.GetMethod("ToEnum").Invoke(EnumService, new[] { value });
            }

            public override object ToObject(object value)
            {
                return EnumServiceType.GetMethod("ToObject").Invoke(EnumService, new[] { value });
            }
        }

        #endregion
    }
}
