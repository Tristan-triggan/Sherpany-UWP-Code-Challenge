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
using Sherpany_UWP_Code_Challenge.Interfaces;
using Sherpany_UWP_Code_Challenge.Messages;

namespace Sherpany_UWP_Code_Challenge.ViewModel.Pages
{
    public class MainPageViewModel: ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IKeyManager _keyManager;
        private readonly IDialogServiceEx _dialogService;

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                ((RelayCommand<string>)TryLoginCommand).RaiseCanExecuteChanged();
            }
        }
        public bool IsPasswordSet => _keyManager.IsKeySet();
        public string PassPhrase => IsPasswordSet ? "Please confirm your pin:" : "Set a six-digit passcode:"; // Could think of a converter to switch the sentence view side


        public MainPageViewModel(INavigationService navigationService, IKeyManager keyManager, IDialogServiceEx dialogService)
        {
            _navigationService = navigationService;
            _keyManager = keyManager;
            TryLoginCommand = new RelayCommand<string>(TryLogin, IsPasswordValid);
            _dialogService = dialogService;
        }

        public ICommand CloseAppCommand => new RelayCommand(CloseApp);

        private async void CloseApp()
        {
            Messenger.Default.Send(new StartAnimationMessage());
            await Task.Delay(2000);
            Messenger.Default.Send(new CloseAppMessage());
        }

        public ICommand TryLoginCommand { get;  }

        private void TryLogin(string _)
        {
            if(IsPasswordSet)
            {
                if(!VerifyPassword(Password))
                {
                    _dialogService.ShowError("Invalid password entered", "The password is incorrect", "ok", null);
                    return;
                }
            }
            else
            {
                StorePassword(Password);
            }
            _navigationService.NavigateTo("SherpanyValuesPageView");
        }

        private void StorePassword(string password)
        {
            _keyManager.SetEncryptionKey(EncryptPassword(password));
        }

        private bool VerifyPassword(string password)
        {
            return EncryptPassword(password) == _keyManager.GetEncryptionKey();
        }

        private bool IsPasswordValid(string _)
        {
            return Password?.Length == 6 && Password.All(c => char.IsDigit(c));
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
    }
}
