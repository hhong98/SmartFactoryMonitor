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
               new FrameworkPropertyMetadata(false));

        public bool IsOpened
        {
            get => (bool)GetValue(IsOpenedProperty);
            set => SetValue(IsOpenedProperty, value);
        }

        public ToggleButton()
        {
            InitializeComponent();
        }
    }
}