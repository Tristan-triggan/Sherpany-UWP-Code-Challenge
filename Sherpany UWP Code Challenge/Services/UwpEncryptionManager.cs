﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using MetroLog;
using Sherpany_UWP_Code_Challenge.Interfaces;

namespace Sherpany_UWP_Code_Challenge.Services
{
    public class UwpEncryptionManager : IEncryptionManager
    {
        private readonly IKeyManager _keyManager;
        private static Dictionary<string, SemaphoreSlim> _semaphores;

        public UwpEncryptionManager(IKeyManager keyManager)
        {
            _keyManager = keyManager;
            _semaphores = new Dictionary<string, SemaphoreSlim>();

        }

        public async Task<byte[]> EncryptV2(byte[] bytes, bool isDemoMode)
        {
            try
            {
                var key = _keyManager.GetEncryptionKey(isDemoMode);

                return EncryptDataV2(bytes, key, key);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private byte[] EncryptDataV2(byte[] bytes, string pw, string salt)
        {
            var base64Text = Convert.ToBase64String(bytes);


            var pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
            var saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf16LE);
            var plainBuffer = CryptographicBuffer.ConvertStringToBinary(base64Text, BinaryStringEncoding.Utf16LE);


            // Derive key material for password size 32 bytes for AES256 algorithm
            var keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha1);
            // using salt and 1000 iterations
            var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

            // create a key based on original key and derivation parmaters
            var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            var derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

            // derive buffer to be used for encryption salt from derived password key 
            var saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

            // display the buffers – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
            //var keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
            //var saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

            var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            // create symmetric key from derived password key
            var symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            // encrypt data buffer using symmetric key and derived salt material
            var resultBuffer = CryptographicEngine.Encrypt(symmKey, plainBuffer, saltMaterial);
            byte[] result;
            CryptographicBuffer.CopyToByteArray(resultBuffer, out result);
            if (result.Length > 1000000)
            {
                GC.Collect();
            }
            return result;
        }

        private async Task<byte[]> DecryptDataV2(byte[] encryptedData, string pw, string salt)
        {
            //var semaphore = GetSemaphore("default");
            //await semaphore.WaitAsync();
            try
            {
                var pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
                var saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf16LE);
                var cipherBuffer = CryptographicBuffer.CreateFromByteArray(encryptedData);

                // Derive key material for password size 32 bytes for AES256 algorithm
                var keyDerivationProvider =
                    KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha1);
                // using salt and 1000 iterations
                var pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);


                // create a key based on original key and derivation parmaters
                var keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
                var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
                var derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

                // derive buffer to be used for encryption salt from derived password key 
                var saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

                // display the keys – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
                //var keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
                //var saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

                var symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                // create symmetric key from derived password material
                var symmKey = symProvider.CreateSymmetricKey(keyMaterial);

                //GC.Collect();
                // encrypt data buffer using symmetric key and derived salt material
                var resultBuffer = CryptographicEngine.Decrypt(symmKey, cipherBuffer, saltMaterial);
                //GC.Collect();
                var result = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf16LE, resultBuffer);
                //GC.Collect();
                var byteArray2 = Convert.FromBase64String(result);

                if (byteArray2?.Length > 1000000)
                {
                    GC.Collect();
                }
                return byteArray2;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //semaphore.Release();
            }
        }

        public async Task<byte[]> DecryptV2(byte[] bytes, bool isDemoMode)
        {
            try
            {
                if (bytes == null) return null;
                var key = _keyManager.GetEncryptionKey(isDemoMode);
                var decryptedBytes = await DecryptDataV2(bytes, key, key);

                return decryptedBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private SemaphoreSlim GetSemaphore(string identifier)
        {
            if (_semaphores.ContainsKey(identifier))
                return _semaphores[identifier];

            var semaphore = new SemaphoreSlim(1);
            _semaphores[identifier] = semaphore;
            return semaphore;
        }
    }
}