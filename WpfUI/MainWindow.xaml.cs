using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       readonly AppConnector _app;

        public MainWindow()
        {
            _app = new AppConnector(Client.Client.WCFclient());
            InitializeComponent();
            lbContactsList.ItemsSource = _app.ViewAll();
        }

        private void MnuSave_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && result.ToString() == "OK")
            {
                string path = folderBrowser.SelectedPath;
                _app.SaveAllContacts(path);
            }
            lbContactsList.ItemsSource = _app.ViewAll();
        }

        private void MnuOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog {
                FileName = "",
                DefaultExt = ".vcf",
                Multiselect = false,
                Title = "Open ",
                InitialDirectory = @"D:\",
                Filter = "VCard files (.vcf)|*.vcf"
            };

            if (dlg.ShowDialog() == true ? true : false)
            {
                string filename = dlg.FileName;
                _app.LoadNew(filename);
            }
            lbContactsList.ItemsSource = _app.ViewAll();
        }

        private void MnuLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog {
                FileName = "",
                DefaultExt = ".vcf",
                Multiselect = false,
                Title = "Open ",
                InitialDirectory = @"D:\",
                Filter = "VCard files (.vcf)|*.vcf"
            };

            if (dlg.ShowDialog() == true ? true : false)
            {
                string filename = dlg.FileName;
                _app.LoadNew(filename);
            }
            lbContactsList.ItemsSource = _app.ViewAll();
        }

        private void MnuView_Click(object sender, RoutedEventArgs e)
        {
            lbContactsList.ItemsSource = _app.ViewAll();
        }

        //TODO: CheckBox Logic for Name and Surname
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchText = txtbSearch.Text;
            
            if (chbByName.IsChecked == true && chbBySurname.IsChecked == true)
            {
                lbContactsList.ItemsSource = _app.SurnameSearch(searchText);
            }
            else if (chbByName.IsChecked == true)
            {
                lbContactsList.ItemsSource = _app.NameSearch(searchText);
            }
            else if (chbBySurname.IsChecked == true)
            {
                lbContactsList.ItemsSource = _app.SurnameSearch(searchText);
            }
            else if (chbByPhone.IsChecked == true)
            {
                lbContactsList.ItemsSource = _app.PhoneSearch(searchText);
            }
            else if (chbByEmail.IsChecked == true)
            {
                lbContactsList.ItemsSource = _app.EmailSearch(searchText);
            }
            
        }
    }
}



