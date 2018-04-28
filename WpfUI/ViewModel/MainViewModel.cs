using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace WpfUI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>

        public readonly AppConnector _app;
        public bool IsServerConnected { get; set; }
        public string Title { get; }
        private ObservableCollection<IContactInfo> __listCond;
        private ObservableCollection<IContactInfo> _contacts;

        private ViewModelLocator _owner;

        public MainViewModel(ViewModelLocator owner)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-En");
            _owner = owner;
            this.IsServerConnected = false;

            _app = new AppConnector(Client.Client.WCFclient());
            if (IsInDesignModeStatic)
            {
                // Create design time view services and models
                this.Title = $"{WpfUI.Resources.text.Notebook}(Disign mode)";
            }
            else
            {
                // Create run time view services and models
                this.Title = $"{WpfUI.Resources.text.Notebook}";
            }

            this.Contacts = new ObservableCollection<IContactInfo>(_app.ViewAll());
            __listCond = _contacts;

            Messenger.Default.Register<ContactInfo>(this, "ContactAdded", (p) => {
                this.AddContact(p);
            });
            Messenger.Default.Register<System.Collections.Generic.List<IContactInfo>>(this, "SearchedContacts", (p) => {
                this.Contacts = new ObservableCollection<IContactInfo>(p);
            });

            Messenger.Default.Send(_app, "GetAppConnector");

            var th = new Thread(() => {
                while (true)
                {
                    this.IsServerConnected = _app.Ping();
                    RaisePropertyChanged("IsServerConnected");
                }
            });
            th.Priority = ThreadPriority.Highest;
            th.IsBackground = true;
            th.Start();
        }

        internal void AddContact(ContactInfo contact)
        {
            _app.AddNew(contact);
            __listCond.Add(contact);
            this.Contacts = __listCond;
        }

        public ObservableCollection<IContactInfo> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                RaisePropertyChanged("Contacts");
            }
        }

        private RelayCommand _viewCommand;
        public RelayCommand ViewCommand
        {
            get
            {
                return _viewCommand ??
                    (_viewCommand = new RelayCommand(this.MenuView));
            }
        }

        public void MenuView()
        {
            __listCond = new ObservableCollection<IContactInfo>(_app.ViewAll());
            this.Contacts = __listCond;
        }


        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new RelayCommand(this.MenuSave));
            }
        }

        private void MenuSave()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && result.ToString() == "OK")
            {
                string path = folderBrowser.SelectedPath;
                _app.SaveAllContacts(path);
            }
            __listCond = new ObservableCollection<IContactInfo>(_app.ViewAll());
            this.Contacts = __listCond;
        }

        internal List<IContactInfo> PhoneSearch(string searchText)
        {
            return _app.PhoneSearch(searchText);
        }

        internal List<IContactInfo> EmailSearch(string searchText)
        {
            return _app.EmailSearch(searchText);
        }

        internal List<IContactInfo> NameSurnameSearch(string searchText)
        {
            return _app.NameSurnameSearch(searchText);
        }

        internal List<IContactInfo> NameSearch(string searchText)
        {
            return _app.NameSearch(searchText);
        }

        internal List<IContactInfo> SurnameSearch(string searchText)
        {
            return _app.SurnameSearch(searchText);
        }

        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ??
                    (_openCommand = new RelayCommand(this.MenuOpen));
            }
        }

        private void MenuOpen()
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
            __listCond = new ObservableCollection<IContactInfo>(_app.ViewAll());
            this.Contacts = __listCond;
        }

        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ??
                    (_loadCommand = new RelayCommand(this.MenuLoad));
            }
        }

        private void MenuLoad()
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
            __listCond = new ObservableCollection<IContactInfo>(_app.ViewAll());
            this.Contacts = __listCond;
        }

    }
}