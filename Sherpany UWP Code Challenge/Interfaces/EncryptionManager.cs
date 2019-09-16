using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sherpany_UWP_Code_Challenge.Interfaces
{
    public interface IEncryptionManager
    {
        Task<byte[]> DecryptV2(byte[] bytes, bool isDemoMode);
        Task<byte[]> EncryptV2(byte[] bytes, bool isDemoMode);
    }
}
