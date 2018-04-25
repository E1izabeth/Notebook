using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI.ViewModel
{
    public class NewContactViewModel : DependencyObject
    {

        public ContactInfo Info { get; private set; }
        private AddContactViewModel _addContactViewModel;

        public NewContactViewModel(ContactInfo contact, AddContactViewModel addContactViewModel)
        {
            this.Info = contact;
            this._addContactViewModel = addContactViewModel;
        }

        public string Phone
        {
            get { return (string)GetValue(PhoneProperty); }
            set { SetValue(PhoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Phone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhoneProperty =
            DependencyProperty.Register("Phone", typeof(string), typeof(NewContactViewModel), new PropertyMetadata(string.Empty), o => ContactInfo.ValidNumber(o.ToString()));
                
        public string FirstName
        {
            get { return (string)GetValue(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstNameProperty =
            DependencyProperty.Register("FirstName", typeof(string), typeof(NewContactViewModel), new PropertyMetadata(string.Empty), o => ContactInfo.ValidName(o.ToString()));

        public string LastName
        {
            get { return (string)GetValue(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastNameProperty =
            DependencyProperty.Register("LastName", typeof(string), typeof(NewContactViewModel), new PropertyMetadata(string.Empty), o => ContactInfo.ValidName(o.ToString()));

        public string Birthday
        {
            get { return (string)GetValue(BirthdayProperty); }
            set { SetValue(BirthdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BirthdayProperty =
            DependencyProperty.Register("Birthday", typeof(string), typeof(NewContactViewModel), new PropertyMetadata(string.Empty), o => ContactInfo.ValidDate(o.ToString()));

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(NewContactViewModel), new PropertyMetadata(string.Empty), o => ContactInfo.ValidEmail(o.ToString()));

        
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == PhoneProperty)
            {
                this.Info.Phone = e.NewValue.ToString();
            }
            else if (e.Property == FirstNameProperty)
            {
                this.Info.FirstName = e.NewValue.ToString();
            }
            else if (e.Property == LastNameProperty)
            {
                this.Info.LastName = e.NewValue.ToString();
            }
            else if (e.Property == EmailProperty)
            {
                this.Info.Email = e.NewValue.ToString();
            }
            else if (e.Property == BirthdayProperty)
            {
                this.Info.Birthday = e.NewValue.ToString();
            }

            if (this.Birthday!=String.Empty && this.Email != String.Empty && this.Phone != String.Empty &&
                this.LastName != String.Empty && this.FirstName != String.Empty)
            {
                _addContactViewModel.IsAddAvailable = true;
            }
            else
            {
                _addContactViewModel.IsAddAvailable = false;
            }
            //PropertyChanged(_addContactViewModel.IsAddAvailable, new PropertyChangedEventArgs("IsAddAvailable"));
            base.OnPropertyChanged(e);
        }
    }

    public class AddContactViewModel : ViewModelBase
    {
        // private AppConnector _app;
        private bool _isAddAvailable;
        public bool IsAddAvailable
        {
            get { return _isAddAvailable; }
            set
            {
                _isAddAvailable = value;
                RaisePropertyChanged("IsAddAvailable");
            }
        }

        private ViewModelLocator _owner;

        public AddContactViewModel(ViewModelLocator owner)
        {
            _owner = owner;
            this.Contact = new ContactInfo();
            //_app = new AppConnector(Client.Client.WCFclient());
            //Messenger.Default.Register<AppConnector>(_app, "GetAppConnector", (t) => {
            //    _app = t;
            //});

            this.ContactView = new NewContactViewModel(this.Contact, this);
        }

        public NewContactViewModel ContactView { get; private set; }

        public ContactInfo Contact { get; private set; }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new RelayCommand(this.Save));
            }
        }


        private void Save()
        {
            //_owner.Main.AddContact(this.Contact);
            //CommonServiceLocator.ServiceLocator.Current.GetInstance<MainViewModel>().AddContact(this.Contact);
            Messenger.Default.Send(this.Contact, "ContactAdded");
        }
    }
}
