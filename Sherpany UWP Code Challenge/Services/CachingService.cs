using Sherpany_UWP_Code_Challenge.Interfaces;
using Sherpany_UWP_Code_Challenge.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Sherpany_UWP_Code_Challenge.Services
{
    public class CachingService<T> : ICachingService<T>
    {
        private readonly string _fileName = "tmpData";
        private StorageFolder _folder;

        private IEncryptionManager _encryptionManager;

        public CachingService(IEncryptionManager encryptionManager)
        {
            _encryptionManager = encryptionManager;
            _folder = ApplicationData.Current.LocalFolder;
        }

        public async Task<bool> CacheExists()
        {
            return await _folder.TryGetItemAsync(_fileName) != null;
        }

        public async Task<T> GetCache()
        {
            T data = default;
            if (await _folder.TryGetItemAsync(_fileName) is StorageFile file)
            {
                var encryptedData = await GetBytesAsync(file);
                var serializedData = await _encryptionManager.DecryptV2(encryptedData, false);
                data = Deserialize(Encoding.UTF8.GetString(serializedData));
            }
            return data;
        }

        public async void CacheData(T data)
        {
            if (data == null) return;

            var serializedData = Encoding.UTF8.GetBytes(Serialize(data).OuterXml);
            var encryptedData = await _encryptionManager.EncryptV2(serializedData, true);

            await SetBytesAsync(encryptedData);
        }

        public XmlDocument Serialize(T data)
        {               
            var xmlData = new XmlDocument();
            using (var _writer = new StringWriter())
            {
                var ser = new XmlSerializer(data.GetType());
                ser.Serialize(_writer, data);
                xmlData.LoadXml(_writer.ToString());
            }
            return xmlData;
        }

        public T Deserialize(string serializedData)
        {
            T data = default;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(serializedData))
            {
                data = (T)ser.Deserialize(reader);
            }

            return data;
        }
        public async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            if (file == null) return null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
        private async Task<StorageFile> SetBytesAsync(byte[] bytes)
        {
            var file = await _folder.CreateFileAsync(_fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, bytes);
            return file;
        }
    }
}
