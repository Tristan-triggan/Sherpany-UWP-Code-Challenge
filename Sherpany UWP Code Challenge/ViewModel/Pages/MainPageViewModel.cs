using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Sherpany_UWP_Code_Challenge.Messages;

namespace Sherpany_UWP_Code_Challenge.ViewModel.Pages
{
    public class MainPageViewModel: ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ICommand CloseAppCommand => new RelayCommand(CloseApp);

        private async void CloseApp()
        {
            Messenger.Default.Send(new StartAnimationMessage());
            await Task.Delay(2000);
            Messenger.Default.Send(new CloseAppMessage());
        }


        //TODO If no passcode is set in the vault, the user can enter one and will then be navigated to the DetailPageView
        public ICommand SetPasswordAndNavigateCommand => new RelayCommand<string>(SetPasswordAndNavigate);

        private void SetPasswordAndNavigate(string password)
        {
            throw new NotImplementedException();
        }

        //TODO If a passcode has already been stored, use this to validate and navigate
        public ICommand ValidatePasswordAndNavigateCommand => new RelayCommand<string>(ValidatePasswordAndNavigate);

        private void ValidatePasswordAndNavigate(string password)
        {
            throw new NotImplementedException();
        }
    }
}
