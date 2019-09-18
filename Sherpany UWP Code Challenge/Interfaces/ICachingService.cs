using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sherpany_UWP_Code_Challenge.Interfaces
{
    public interface ICachingService<T>
    {
        Task<T> GetCache();
        void CacheData(T values);
    }
}
