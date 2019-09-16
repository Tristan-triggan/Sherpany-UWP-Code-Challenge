using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sherpany_UWP_Code_Challenge.Interfaces;
using Windows.Storage;

namespace Sherpany_UWP_Code_Challenge.Services
{
    public class KeyManager: IKeyManager
    {
        private readonly string _resourceName = "Sherpany Challenge Key";
        private readonly ApplicationDataContainer _localSettings;

        public KeyManager()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public string GetEncryptionKey(bool isDemoMode)
        {
            throw new NotImplementedException();
        }

        public void SetEncryptionKey(string key)
        {
            _localSettings.Values.Add(_resourceName, key);
        }

        public bool DeleteEncryptionKey()
        {
            throw new NotImplementedException();
        }

        public bool IsKeySet()
        {
            throw new NotImplementedException();
        }
    }
}
