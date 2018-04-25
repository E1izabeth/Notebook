/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WpfUI"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.ComponentModel;
using System.Windows.Controls;

namespace WpfUI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            //SimpleIoc.Default.Register<SearchPanelViewModel>()
            //SimpleIoc.Default.Register<AddContactViewModel>(true);
            //SimpleIoc.Default.Register<MainViewModel>();

            this.Main = new MainViewModel(this);
            this.AddPanel = new AddContactViewModel(this);
            this.SearchPanel = new SearchPanelViewModel(this);
        }

        public MainViewModel Main { get; }

        public SearchPanelViewModel SearchPanel { get; }

        public AddContactViewModel AddPanel { get; }

        public static void Cleanup()
        {
            //SimpleIoc.Default.Unregister<AddContactViewModel>();
            //SimpleIoc.Default.Unregister<SearchPanelViewModel>();
            //SimpleIoc.Default.Unregister<MainViewModel>();
        }
    }
}