using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Sherpany_UWP_Code_Challenge.Interfaces;

namespace Sherpany_UWP_Code_Challenge.ViewModel.Pages
{
    public class SherpanyValuesPageViewModel: ViewModelBase
    {
        private readonly IDummyApiService _apiService;

        public SherpanyValuesPageViewModel(IDummyApiService apiService)
        {
            _apiService = apiService;
        }
    }
}
