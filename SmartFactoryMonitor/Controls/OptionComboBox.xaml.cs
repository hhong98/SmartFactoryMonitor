using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartFactoryMonitor.Controls
{
    /// <summary>
    /// Interaction logic for DropDownToggle.xaml
    /// </summary>
    public partial class OptionComboBox : UserControl
    {
        public OptionComboBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable<string>),
                typeof(OptionComboBox));

        public IEnumerable<string> ItemsSource
        {
            get => (IEnumerable<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedOptionProperty =
            DependencyProperty.Register(
                "SelectedOption",
                typeof(string),
                typeof(OptionComboBox),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SelectedOption
        {
            get => (string)GetValue(SelectedOptionProperty);
            set => SetValue(SelectedOptionProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
             DependencyProperty.Register(
                 "Placeholder",
                 typeof(string),
                 typeof(OptionComboBox),
                 new PropertyMetadata("Placeholder"));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty DisplaySizeProperty =
            DependencyProperty.Register(
                "DisplaySize",
                typeof(int),
                typeof(OptionComboBox),
                new PropertyMetadata(12));

        public int DisplaySize
        {
            get => (int)GetValue(DisplaySizeProperty);
            set => SetValue(DisplaySizeProperty, value);
        }

        public static readonly DependencyProperty OptionSizeProperty =
           DependencyProperty.Register(
               "OptionSize",
               typeof(int),
               typeof(OptionComboBox),
               new PropertyMetadata(12));

        public int OptionSize
        {
            get => (int)GetValue(OptionSizeProperty);
            set => SetValue(OptionSizeProperty, value);
        }

        private void OnOptionClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SelectedOption = button.Content.ToString();
                FilterToggle.IsChecked = false;
            }
        }
    }
}