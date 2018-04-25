using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Notebook.Interfaces;

namespace WpfUI.ViewModel
{
    public class SearchPanelViewModel : ViewModelBase
    {
        private string _searchString;
        public string SearchString {
            get
            {
                return _searchString;
            }
            set
            {
                _searchString = value;
                RaisePropertyChanged("SearchString");
                CheckAvailable();
            }
        }

        private bool _sbyname;
        public bool SByName
        {
            get
            {
                return _sbyname;
            }
            set
            {
                _sbyname = value;
                CheckAvailable();
                RaisePropertyChanged("SByName");
            }
        }
        private bool _sbysurname;
        public bool SBySurname
        {
            get
            {
                return _sbysurname;
            }
            set
            {
                _sbysurname = value;
                CheckAvailable();
                RaisePropertyChanged("SBySurname");
            }
        }
        private bool _sbyphone;
        public bool SByPhone
        {
            get
            {
                return _sbyphone;
            }
            set
            {
                _sbyphone = value;
                CheckAvailable();
                RaisePropertyChanged("SByPhone");
            }
        }
        private bool _sbyemail;
        public bool SByEmail
        {
            get
            {
                return _sbyemail;
            }
            set
            {
                _sbyemail = value;
                CheckAvailable();
                RaisePropertyChanged("SByEmail");
            }
        }

        public bool SearchAvailable { get; set; }
        public bool SByNameAvailable { get; set; }
        public bool SBySurnameAvailable { get; set; }
        public bool SByPhoneAvailable { get; set; }
        public bool SByEmailAvailable { get; set; }

        private ViewModelLocator _owner;

        public SearchPanelViewModel(ViewModelLocator owner)
        {
            _owner = owner;
            this.SByName = true;
            this.SBySurname = false;
            this.SByNameAvailable = true;
            this.SByPhoneAvailable = true;
            this.SBySurnameAvailable = true;
            this.SByEmailAvailable = true;
            //_app = new AppConnector(Client.Client.WCFclient());
            //Messenger.Default.Register<AppConnector>(_app, "GetAppConnector", (t) => {
            //    _app = t;
            //});
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand ??
                    (_searchCommand = new RelayCommand(this.BtnSearch_Click));
            }
        }

        private void BtnSearch_Click()
        {
            var searchText = this.SearchString;
            var list = new List<IContactInfo>();

            if (this.SByName == true && this.SBySurname == true)
            {
                list = _owner.Main.NameSurnameSearch(searchText);
            }
            else if (this.SByName == true)
            {
                list = _owner.Main.NameSearch(searchText);
            }
            else if (this.SBySurname == true)
            {
                list = _owner.Main.SurnameSearch(searchText);
            }
            else if (this.SByPhone == true)
            {
                list = _owner.Main.PhoneSearch(searchText);
            }
            else if (this.SByEmail == true)
            {
                list = _owner.Main.EmailSearch(searchText);
            }
            Messenger.Default.Send(list, "SearchedContacts");
        }

        public void CheckAvailable()
        {
            if ((this.SearchString != null && this.SearchString != "") &&
                (this.SByName == true || this.SBySurname == true || this.SByPhone == true || this.SByEmail == true))
            {
                this.SearchAvailable = true;
            }
            else
            {
                this.SearchAvailable = false;
            }
            RaisePropertyChanged("SearchAvailable");


            if (this.SByEmail == true)
            {
                this.SByNameAvailable = false;
                this.SByPhoneAvailable = false;
                this.SBySurnameAvailable = false;
                RaisePropertyChanged("SByNameAvailable");
                RaisePropertyChanged("SByPhoneAvailable");
                RaisePropertyChanged("SBySurnameAvailable");
            }
            else if (this.SByPhone == true)
            {
                this.SByNameAvailable = false;
                this.SByEmailAvailable = false;
                this.SBySurnameAvailable = false;
                RaisePropertyChanged("SByNameAvailable");
                RaisePropertyChanged("SByEmailAvailable");
                RaisePropertyChanged("SBySurnameAvailable");
            }
            else if (this.SByName == true && this.SBySurname == true)
            {
                this.SByPhoneAvailable = false;
                this.SByEmailAvailable = false;
                RaisePropertyChanged("SByPhoneAvailable");
                RaisePropertyChanged("SByEmailAvailable");
            }
            else if (this.SByName == true || this.SBySurname == true)
            {
                this.SByPhoneAvailable = false;
                this.SByEmailAvailable = false;
                RaisePropertyChanged("SByPhoneAvailable");
                RaisePropertyChanged("SByEmailAvailable");
            }
            else
            {
                this.SBySurnameAvailable = true;
                this.SByPhoneAvailable = true;
                this.SByNameAvailable = true;
                this.SByEmailAvailable = true;
                RaisePropertyChanged("SBySurnameAvailable");
                RaisePropertyChanged("SByPhoneAvailable");
                RaisePropertyChanged("SByNameAvailable");
                RaisePropertyChanged("SByEmailAvailable");
            }

        }
    }
}
