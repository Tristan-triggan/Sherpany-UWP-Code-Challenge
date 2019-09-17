using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sherpany_UWP_Code_Challenge.Interfaces
{
    public interface IKeyManager
    {
        string GetEncryptionKey(bool isDemoMode = false);
        void SetEncryptionKey(string key);
        bool DeleteEncryptionKey();
        bool IsKeySet();
    }
}
