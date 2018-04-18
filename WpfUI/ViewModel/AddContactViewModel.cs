using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModel
{
    class AddContactViewModel : ViewModelBase
    {
        public AddContactViewModel()
        {
            this.ContactInfo = new ContactInfo();
        }
        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(this.Save);
                }
                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        public ContactInfo ContactInfo
        {
            get
            {
                return _contact;
            }
            set
            {
                _contact = value;
                RaisePropertyChanged("Contact");
            }

        }

        private ContactInfo _contact;

        void Save()
        {
            Messenger.Default.Send<ContactInfo>(this.ContactInfo, "ContactAdded");
        }
    }
}
