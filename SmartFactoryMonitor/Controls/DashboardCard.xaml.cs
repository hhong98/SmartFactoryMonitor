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
    /// Interaction logic for DashboardCard.xaml
    /// </summary>
    ///
    public partial class DashboardCard : UserControl
    {
        public static readonly DependencyProperty CardNameProperty =
            DependencyProperty.Register(
                nameof(CardName),
                typeof(string),
                typeof(DashboardCard),
                new FrameworkPropertyMetadata(
                    "카드명"));

        public string CardName
        {
            get => (string)GetValue(CardNameProperty);
            set => SetValue(CardNameProperty, value);
        }

        public static readonly DependencyProperty CardNumberProperty =
            DependencyProperty.Register(
                nameof(CardNumber),
                typeof(int),
                typeof(DashboardCard),
                new FrameworkPropertyMetadata(0));

        public int CardNumber
        {
            get => (int)GetValue(CardNumberProperty);
            set => SetValue(CardNumberProperty, value);
        }

        public static readonly DependencyProperty CardTxtColorProperty =
            DependencyProperty.Register(
                nameof(CardTextColor),
                typeof(SolidColorBrush),
                typeof(DashboardCard),
                new FrameworkPropertyMetadata(Brushes.Black));

        public SolidColorBrush CardTextColor
        {
            get => (SolidColorBrush)GetValue(CardTxtColorProperty);
            set => SetValue(CardTxtColorProperty, value);
        }

        public static readonly DependencyProperty CardBgColorProperty =
            DependencyProperty.Register(
                nameof(CardBackgroundColor),
                typeof(SolidColorBrush),
                typeof(DashboardCard),
                new FrameworkPropertyMetadata(Brushes.White));

        public SolidColorBrush CardBackgroundColor
        {
            get => (SolidColorBrush)GetValue(CardBgColorProperty);
            set => SetValue(CardBgColorProperty, value);
        }

        public DashboardCard()
        {
            InitializeComponent();
        }
    }
}