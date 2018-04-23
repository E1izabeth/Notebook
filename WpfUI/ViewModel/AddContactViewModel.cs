using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI.ViewModel
{
    public class AddContactViewModel : ViewModelBase
    {
        private AppConnector _app;
        
        public AddContactViewModel()
        {
            this.Contact = new ContactInfo();
            Messenger.Default.Register<AppConnector>(_app, "GetAppConnector", (t) => {
                _app = t;
            });
        }

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
            _app.AddNew(this.Contact);
            Messenger.Default.Send(this.Contact, "ContactAdded");
        }
    }
}
