using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Notebook.Interfaces;
using System.Collections.ObjectModel;
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
        public string Title { get; }
        private ObservableCollection<IContactInfo> __listCond;
        private ObservableCollection<IContactInfo> _contacts;

        public MainViewModel()
        {
            _app = new AppConnector(Client.Client.WCFclient());
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                this.Title = "Notebook(Disign mode)";
            }
            else
            {
                // Create run time view services and models
                this.Title = "Notebook";
            }

            this.Contacts = new ObservableCollection<IContactInfo>(_app.ViewAll());
            __listCond = _contacts;

            Messenger.Default.Register<ContactInfo>(this, "ContactAdded", (p) => {
                __listCond.Add(p);
                this.Contacts = __listCond;
            });
            Messenger.Default.Register<System.Collections.Generic.List<IContactInfo>>(this, "SearchedContacts", (p) => {
                this.Contacts = new ObservableCollection<IContactInfo>(p);
            });
            Messenger.Default.Send(_app, "GetAppConnector");
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