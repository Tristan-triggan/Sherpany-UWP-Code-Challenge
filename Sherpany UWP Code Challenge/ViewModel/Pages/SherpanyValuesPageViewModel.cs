using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Sherpany_UWP_Code_Challenge.Interfaces;
using Sherpany_UWP_Code_Challenge.Model;

namespace Sherpany_UWP_Code_Challenge.ViewModel.Pages
{
    public class SherpanyValuesPageViewModel: ViewModelBase
    {
        private readonly IDummyApiService _apiService;

        public ObservableCollection<SherpanyValueModel> Values { get; } = new ObservableCollection<SherpanyValueModel>();
        private SherpanyValueModel _detailedValue;

        public SherpanyValueModel DetailedValue
        {
            get { return _detailedValue; }
            set
            {
                if (_detailedValue != value)
                {
                    _detailedValue = value;
                    RaisePropertyChanged();
                }
            }
        }


        public ICommand GetValuesCommand { get; }
        private bool _valuesRetrievalInProgress = false;

        public bool ValuesRetrievalInProgress
        {
            get
            {
                return _valuesRetrievalInProgress;
            }
            set
            {
                if(value != _valuesRetrievalInProgress)
                {
                    _valuesRetrievalInProgress = value;
                    ((RelayCommand)GetValuesCommand).RaiseCanExecuteChanged();
                }
            }
        }


        public SherpanyValuesPageViewModel(IDummyApiService apiService)
        {
            _apiService = apiService;
            GetValuesCommand = new RelayCommand(GetValues, () => !ValuesRetrievalInProgress);
        }

        private async void GetValues()
        {
            ValuesRetrievalInProgress = true;

            Values.Clear();
            foreach(var value in (await _apiService.GetValueModelsAsync()).OrderBy(e => e.Order))
            {
                Values.Add(value);
            }

            ValuesRetrievalInProgress = false;
        }
    }
}
