using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Sherpany_UWP_Code_Challenge.Messages;
using Sherpany_UWP_Code_Challenge.Services;

namespace Sherpany_UWP_Code_Challenge.ViewModel.Pages
{
    public class MainPageViewModel: ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                ((RelayCommand<string>)SetPasswordAndNavigateCommand).RaiseCanExecuteChanged();
            }
        }


        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SetPasswordAndNavigateCommand = new RelayCommand<string>(SetPasswordAndNavigate, IsPasswordValid);
        }

        public ICommand CloseAppCommand => new RelayCommand(CloseApp);

        private async void CloseApp()
        {
            Messenger.Default.Send(new StartAnimationMessage());
            await Task.Delay(2000);
            Messenger.Default.Send(new CloseAppMessage());
        }


        //TODO If no passcode is set in the vault, the user can enter one and will then be navigated to the DetailPageView
        public ICommand SetPasswordAndNavigateCommand { get;  }

        private bool IsPasswordValid(string _)
        {
            return Password?.Length == 6 && Password.All(c => char.IsDigit(c));
        }

        private void SetPasswordAndNavigate(string _)
        {
            var keyManager = new KeyManager();
            keyManager.SetEncryptionKey(EncryptPassword(Password));
            _navigationService.NavigateTo("SherpanyValuesPageView");
        }

        private string EncryptPassword(string password)
        {
            using (var sha256Hash = SHA256.Create())
            { 
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //TODO If a passcode has already been stored, use this to validate and navigate
        public ICommand ValidatePasswordAndNavigateCommand => new RelayCommand<string>(ValidatePasswordAndNavigate);

        private void ValidatePasswordAndNavigate(string password)
        {
            throw new NotImplementedException();
        }
    }
}
