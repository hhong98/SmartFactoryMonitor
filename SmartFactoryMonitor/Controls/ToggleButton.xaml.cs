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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartFactoryMonitor.Controls
{
    /// <summary>
    /// Interaction logic for EquipListPanelToggle.xaml
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        public static readonly DependencyProperty IsOpenedProperty =
           DependencyProperty.Register(
               nameof(IsOpened),
               typeof(bool),
               typeof(ToggleButton),
               new FrameworkPropertyMetadata(
                   false,
                   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsOpened
        {
            get => (bool)GetValue(IsOpenedProperty);
            set => SetValue(IsOpenedProperty, value);
        }

        public static readonly DependencyProperty OpenBackgroundProperty =
           DependencyProperty.Register(
               nameof(OpenBackground),
               typeof(Brush),
               typeof(ToggleButton),
               new FrameworkPropertyMetadata(Brushes.LightGreen));

        public Brush OpenBackground
        {
            get => (Brush)GetValue(OpenBackgroundProperty);
            set => SetValue(OpenBackgroundProperty, value);
        }

        public static readonly DependencyProperty CloseBackgroundProperty =
            DependencyProperty.Register(
                nameof(CloseBackground),
                typeof(Brush),
                typeof(ToggleButton),
                new FrameworkPropertyMetadata(Brushes.LightPink));

        public Brush CloseBackground
        {
            get => (Brush)GetValue(CloseBackgroundProperty);
            set => SetValue(CloseBackgroundProperty, value);
        }

        public ToggleButton()
        {
            InitializeComponent();
        }

        private void ToggleClick(object sender, MouseButtonEventArgs e)
        {
            IsOpened = !IsOpened;
            e.Handled = true;
        }
    }
}