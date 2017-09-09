using System;
using System.Diagnostics;
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

namespace PerMonWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void categoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriesListBox.SelectedItem != null)
            {
                try
                {
                    PerfMonManager.Counters counters = new PerfMonManager.Counters();
                    PerfMonManager.Categories categories = new PerfMonManager.Categories();
                    PerformanceCounterCategory pcc =
                        (PerformanceCounterCategory)categoriesListBox.SelectedItem;
                    categoryHelpTextBox.Text = pcc.CategoryHelp;
                    categoryTypeLabel.Content = pcc.CategoryType;
                    machineNameLabel.Content = pcc.MachineName;

                    if (pcc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
                    {
                        countersDataGrid.ItemsSource = counters.list(pcc.CategoryName);
                    }
                    else
                    {
                        string[] instances = categories.getInstanceNames(pcc.CategoryName);

                        if (instances.Length > 0)
                        {
                            countersDataGrid.ItemsSource =
                                counters.list(pcc.CategoryName, instances[0]);
                            instanceNamesListBox.ItemsSource = instances;
                        }
                        else
                        {
                            categoryHelpTextBox.Text = 
                                $"CATEGORY INSTANCES COULD NOT BE RETRIEVED! {pcc.CategoryHelp}";
                            countersDataGrid.ItemsSource =
                                 counters.list(pcc.CategoryName, "*");
                        }
                    }

                    instanceNamesListBox.Items.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("",
                        System.ComponentModel.ListSortDirection.Ascending));

                    switch (pcc.CategoryName)
                    {
                        case "Processor":
                            MessageBox.Show("Processor!");
                            break;
                        case "Process":
                            
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
