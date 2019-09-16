using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sherpany_UWP_Code_Challenge.Model;

namespace Sherpany_UWP_Code_Challenge.Interfaces
{
    public interface IDummyApiService
    {
        Task<IEnumerable<SherpanyValueModel>> GetValueModelsAsync();
    }
}
