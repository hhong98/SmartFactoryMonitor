using SmartFactoryMonitor.Model;
using SmartFactoryMonitor.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SmartFactoryMonitor
{
    /// <summary>
    /// EquipList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipList : UserControl
    {
        public EquipList()
        {
            InitializeComponent();
        }

        private void FilterCheckChanged(object sender, RoutedEventArgs e)
        {
            CheckBox filterBox = (CheckBox)sender;
            string filterTag = filterBox.Tag.ToString();

            bool isFilterActive = filterBox.IsChecked ?? false;

            ICollectionView dataView = CollectionViewSource.GetDefaultView(EquipLV.ItemsSource);
            if (dataView is null) return;

            if (!isFilterActive)
            {
                dataView.Filter = null;
            }
            else
            {
                dataView.Filter = (obj) =>
                {
                    var equip = obj as Equipment;
                    if (equip == null) return false;

                    var propValue = equip.GetType().GetProperty(filterTag)?.GetValue(equip, null);

                    return propValue != null && propValue.ToString().Equals("Y");
                };
            }
        }

        private void FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox filterBox = (TextBox)sender;
            string filterTag = filterBox.Tag.ToString();
            string filterText = filterBox.Text.ToLower().Trim();

            ICollectionView dataView = CollectionViewSource.GetDefaultView(EquipLV.ItemsSource);
            if (dataView is null) return;

            if (string.IsNullOrEmpty(filterText)) { dataView.Filter = null; }
            else
            {
                
                dataView.Filter = (obj) =>
                {
                    var equip = obj as Equipment;
                    if (equip == null) { return false; }
                    
                    var propValue = equip.GetType().GetProperty(filterTag)?.GetValue(equip, null);

                    return propValue == null
                        ? string.IsNullOrEmpty(filterText)
                        : propValue.ToString().ToLower().Contains(filterText);
                };
            }
        }

        private void EquipRow_Click(object sender, MouseButtonEventArgs e)
        {
            var row = sender as ListViewItem;

            if (row != null)
            {
                var selectedEquip = row.Content as Equipment;

                EquipUpdateWindow updateWindow = new EquipUpdateWindow(selectedEquip)
                {
                    Owner = Application.Current.MainWindow
                };

                updateWindow.DataContext = DataContext;

                updateWindow.ShowDialog();
            }
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private void SortingHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(EquipLV.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
