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
            //var sp = new SearchPanelViewModel();
            //lbContactsList.ItemsSource = _app.ViewAll();
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
            var list = new List<string>();

            if (chbByName.IsChecked == true && chbBySurname.IsChecked == true)
            {
                list = _app.SurnameSearch(searchText);
            }
            else if (chbByName.IsChecked == true)
            {
                list = _app.NameSearch(searchText);
            }
            else if (chbBySurname.IsChecked == true)
            {
                list = _app.SurnameSearch(searchText);
            }
            else if (chbByPhone.IsChecked == true)
            {
                list = _app.PhoneSearch(searchText);
            }
            else if (chbByEmail.IsChecked == true)
            {
                list = _app.EmailSearch(searchText);
            }

            if (list.Count == 0)
            {
                list.Add("Search didn't give results");
            }

            lbContactsList.ItemsSource = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _app.AddNew(tbName.Text, tbSurname.Text, tbNickname.Text, tbBirthday.Text, tbPhone.Text, tbEmail.Text, tbMailer.Text, tbNote.Text);
            tbName.Text = ""; tbSurname.Text = ""; tbNickname.Text = ""; tbBirthday.Text = ""; tbPhone.Text = ""; tbEmail.Text = ""; tbMailer.Text = ""; tbNote.Text = "";
            lbContactsList.ItemsSource = _app.ViewAll();
        }

        private void chbBy_Checked(object sender, RoutedEventArgs e)
        {
            if (chbByName.IsChecked == true || chbBySurname.IsChecked == true || chbByPhone.IsChecked == true || chbByEmail.IsChecked == true)
            {
                btnSearch.IsEnabled = true;
            }
            else
            {
                btnSearch.IsEnabled = false;
            }

            if (chbByEmail.IsChecked == true)
            {
                chbByName.IsEnabled = false;
                chbByPhone.IsEnabled = false;
                chbBySurname.IsEnabled = false;
            }
            else if (chbByPhone.IsChecked == true)
            {
                chbByName.IsEnabled = false;
                chbByEmail.IsEnabled = false;
                chbBySurname.IsEnabled = false;
            }
            else if (chbByName.IsChecked == true && chbBySurname.IsChecked == true)
            {
                chbByPhone.IsEnabled = false;
                chbByEmail.IsEnabled = false;
            }
            else if (chbByName.IsChecked == true || chbBySurname.IsChecked == true)
            {
                chbByPhone.IsEnabled = false;
                chbByEmail.IsEnabled = false;
            }
            else
            {
                chbBySurname.IsEnabled = true;
                chbByPhone.IsEnabled = true;
                chbByName.IsEnabled = true;
                chbByEmail.IsEnabled = true;
            }
        }
    }
}



