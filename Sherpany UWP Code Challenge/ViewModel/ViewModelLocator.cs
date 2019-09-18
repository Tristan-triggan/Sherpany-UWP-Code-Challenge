using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight.Views;
using LightInject;
using CommonServiceLocator;
using LightInject.ServiceLocation;
using MetroLog;
using Sherpany_UWP_Code_Challenge.Interfaces;
using Sherpany_UWP_Code_Challenge.Services;
using Sherpany_UWP_Code_Challenge.View.Pages;
using Sherpany_UWP_Code_Challenge.View.Dialog;
using Sherpany_UWP_Code_Challenge.ViewModel.Pages;
using Sherpany_UWP_Code_Challenge.Model;

namespace Sherpany_UWP_Code_Challenge.ViewModel
{
    public class ViewModelLocator
    {
        private readonly ServiceContainer _container;

        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            _container = new ServiceContainer();
            RegisterServices();
            SetLocatorProvider();
        }

        public void SetLocatorProvider()
        {
            ServiceLocator.SetLocatorProvider(() => new LightInjectServiceLocator(_container));
        }

        //the new pages
        public MainPageViewModel MainPage => ServiceLocator.Current.GetInstance<MainPageViewModel>();
        public SherpanyValuesPageViewModel ValuesPage => ServiceLocator.Current.GetInstance<SherpanyValuesPageViewModel>();


        private void RegisterServices()
        {
            //_container.Invalidate();
            _container.SetDefaultLifetime<PerContainerLifetime>();
            _container.Register(f => CreateNavigationService());
            _container.Register(f => CreateDialogService());
            _container.Register(f => LogManagerFactory.DefaultLogManager);

            // Registration of the pages
            _container.Register<MainPageViewModel>();
            _container.Register<SherpanyValuesPageViewModel>();

            //Services
            _container.Register<IDummyApiService, DummyApiService>();
            _container.Register<IKeyManager, KeyManager>();
            _container.Register<IEncryptionManager, UwpEncryptionManager>();
            _container.Register<ICachingService<List<SherpanyValueModel>>, CachingService<List<SherpanyValueModel>>>();
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();

            // New pages
            navigationService.Configure("MainPageView", typeof(MainPageView));
            navigationService.Configure("SherpanyValuesPageView", typeof(SherpanyValuesPageView));

            return navigationService;
        }

        private IDialogServiceEx CreateDialogService()
        {
            var dialogService = new DialogServiceEx();
            dialogService.Configure("ConfirmActionDialog", typeof(ConfirmActionDialog));
            return dialogService;
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
