using Autodesk.Revit.DB;
using ColMarkSort.Model.Data;
using ColMarkSort.Model.Etabs;
using ColMarkSort.Model.Revit;
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

namespace ColMarkSort.View.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Document doc;
        public MainWindow(Document doc,string[] options)
        {
            InitializeComponent();
            this.doc = doc;
            // Set the DataContext of the window to the list of strings
            this.DataContext = options;
        }

        private void OldColumns_Click(object sender, RoutedEventArgs e)
        {
            ManageData.GetColumnData(doc,OptionsComboBox.SelectedItem as string);
        }

        private void EtabsColumns_Click(object sender, RoutedEventArgs e)
        {
            ManageEtabs.LinkEtabsModel();
            ColumnArrayGroup etabsColumns = ManageEtabs.GetDataFromEtabs();
            RvtUtils.SendEtabsDataToRevit(doc, etabsColumns);
            ManageData.GetColumnData(doc, OptionsComboBox.SelectedItem as string);

        }
    }
}
