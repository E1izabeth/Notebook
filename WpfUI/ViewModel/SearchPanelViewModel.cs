using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WpfUI.ViewModel
{
    public class SearchPanelViewModel: ViewModelBase
    {
        public string SearchString { get; set; }
        public bool SByName { get; set; }
        public bool SBySurame { get; set; }
        public bool SByPhone { get; set; }
        public bool SByEmail { get; set; }

        public SearchPanelViewModel()
        {
            this.SByEmail = true;
            this.SByName = false;
            this.SByPhone = false;
            this.SBySurame = false;
        }
    }
}
