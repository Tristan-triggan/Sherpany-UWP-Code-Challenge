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
using WinRTXamlToolkit.Tools;

namespace Sherpany_UWP_Code_Challenge.ViewModel.Pages
{
    public class SherpanyValuesPageViewModel: ViewModelBase
    {
        private readonly IDummyApiService _apiService;
        private readonly ICachingService<List<SherpanyValueModel>> _cachingService;

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
                    if(value != null)
                    {
                        NewValue = null;
                    }
                    RaisePropertyChanged();
                }
            }
        }
        private SherpanyValueModel _newValue;
        public SherpanyValueModel NewValue
        {
            get { return _newValue; }
            set
            {
                if (_newValue != value)
                {
                    _newValue = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged();
            }
        }


        public ICommand GetValuesCommand { get; }
        public ICommand CreateNewValueCommand { get; }
        public ICommand AddValueCommand { get; }
        public ICommand DeleteValueCommand { get; }

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

        public SherpanyValuesPageViewModel(IDummyApiService apiService, ICachingService<List<SherpanyValueModel>> cachingService)
        {
            _apiService = apiService;
            _cachingService = cachingService;
            GetValuesCommand = new RelayCommand(GetValues, () => !ValuesRetrievalInProgress);
            CreateNewValueCommand = new RelayCommand(CreateNewValue, () => Values?.Any() == true);
            AddValueCommand = new RelayCommand(AddNewValue);
            DeleteValueCommand = new RelayCommand(DeleteValue);
        }

        private async void GetValues()
        {
            ValuesRetrievalInProgress = true;

            Values.CollectionChanged -= ValuesChanged;
            Values.Clear();
            var cache = await _cachingService.GetCache();
            if (cache == null)
            {
                foreach (var value in (await _apiService.GetValueModelsAsync()).OrderBy(e => e.Order))
                {
                    Values.Add(value);
                }
                _cachingService.CacheData(Values.ToList());
            }
            else
            {
                foreach(var value in cache)
                {
                    Values.Add(value);
                }
            }
            Values.CollectionChanged += ValuesChanged;
            ((RelayCommand)CreateNewValueCommand).RaiseCanExecuteChanged();

            ValuesRetrievalInProgress = false;
        }

        private void ValuesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var movedValue = e.NewItems[0] as SherpanyValueModel;
                var startingIndex = Math.Min(movedValue.Order, e.NewStartingIndex);
                var lastIndex = Math.Max(movedValue.Order, e.NewStartingIndex);
                for(int i = startingIndex; i <= lastIndex; i++)
                {
                    Values[i].Order = i;
                }
                _cachingService.CacheData(Values.ToList());
            }
        }

        private void CreateNewValue()
        {
            DetailedValue = null;
            NewValue = new SherpanyValueModel { Order = Values.Max(v => v.Order) };
        }

        private void AddNewValue()
        {
            if (IsNewValueValid())
            {
                ErrorMessage = string.Empty;
                Values.Add(NewValue);
                _cachingService.CacheData(Values.ToList());
                NewValue = null;
                DetailedValue = Values.Last();
                ((RelayCommand)CreateNewValueCommand).RaiseCanExecuteChanged();
            }
            else
            {
                ErrorMessage = "New value should have at least a title and a description!";
            }
        }

        private bool IsNewValueValid()
        {
            return !string.IsNullOrEmpty(NewValue?.Title) && !string.IsNullOrEmpty(NewValue?.Description);
        }

        private void DeleteValue()
        {
            Values.Remove(DetailedValue);
            DetailedValue = null;
        }
    }
}
