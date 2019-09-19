using System;
using System.ComponentModel.Design;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;
using LightInject.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sherpany_UWP_Code_Challenge.Interfaces;
using Sherpany_UWP_Code_Challenge.ViewModel;
using Sherpany_UWP_Code_Challenge.ViewModel.Pages;

namespace Sherpany_UWP_Tests
{
    [TestClass]
    public class MainPageViewModelTests
    {
        Mock<INavigationService> _navigationService;
        Mock<IKeyManager> _keyManager;
        Mock<IDialogServiceEx> _dialogService;
        MainPageViewModel _mainViewModel;

        [TestInitialize]
        public void Init()
        {
            _navigationService = new Mock<INavigationService>();
            _keyManager = new Mock<IKeyManager>();
            _dialogService = new Mock<IDialogServiceEx>();
            _mainViewModel = new MainPageViewModel(_navigationService.Object, _keyManager.Object, _dialogService.Object);
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("abcd")]
        [DataRow("12345")]
        [DataRow("aaaaaa")]
        [DataRow("12345a")]
        [DataRow("123456a")]
        [DataRow("1234567")]
        [TestMethod]
        public void TryLoginCommandCannotExecuteIfPasswordIsNotValid(string password)
        {
            var vm = _mainViewModel;
            vm.Password = password;

            var result = vm.TryLoginCommand.CanExecute(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryLoginCommandCanExecuteIfPasswordIsValid()
        {
            var vm = _mainViewModel;
            var numberPassword = new Random().Next(100000, 999999);
            vm.Password = numberPassword.ToString();

            var result = vm.TryLoginCommand.CanExecute(null);

            Assert.IsTrue(result);
        }

        [DataRow("123456", "654321")]
        [DataRow("654321", "123456")]
        [DataRow("354268", "159648")]
        [TestMethod]
        public void TryLoginCommandShouldShowErrorIfPasswordSetAndNotValid(string password, string storedPassword)
        {
            var vm = _mainViewModel;
            vm.Password = password;
            _keyManager.Setup(km => km.IsKeySet()).Returns(true);
            var encryptedPassword = vm.EncryptPassword(storedPassword);
            _keyManager.Setup(km => km.GetEncryptionKey(It.IsAny<bool>())).Returns(encryptedPassword);
            _dialogService.Setup(dlg => dlg.ShowError(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action>())).Verifiable();

            vm.TryLoginCommand.Execute(null);

            _dialogService.Verify();
        }

        [DataRow("123456")]
        [DataRow("654321")]
        [DataRow("354268")]
        [TestMethod]
        public void TryLoginCommandShouldNavigateToValuePageIfPasswordSetAndValid(string password)
        {
            var vm = _mainViewModel;
            vm.Password = password;
            _keyManager.Setup(km => km.IsKeySet()).Returns(true);
            var encryptedPassword = vm.EncryptPassword(password);
            _keyManager.Setup(km => km.GetEncryptionKey(It.IsAny<bool>())).Returns(encryptedPassword);
            _navigationService.Setup(nav => nav.NavigateTo("SherpanyValuesPageView")).Verifiable();

            vm.TryLoginCommand.Execute(null);

            _navigationService.Verify();
        }

        [DataRow("123456")]
        [DataRow("654321")]
        [DataRow("354268")]
        [TestMethod]
        public void TryLoginCommandShouldNavigateAndStoreIfPasswordNotSet(string password)
        {
            var vm = _mainViewModel;
            vm.Password = password;
            _keyManager.Setup(km => km.IsKeySet()).Returns(false);
            var encryptedPassword = vm.EncryptPassword(password);
            _keyManager.Setup(km => km.SetEncryptionKey(encryptedPassword)).Verifiable();
            _navigationService.Setup(nav => nav.NavigateTo("SherpanyValuesPageView")).Verifiable();

            vm.TryLoginCommand.Execute(null);

            _keyManager.Verify();
            _navigationService.Verify();
        }
    }
}
