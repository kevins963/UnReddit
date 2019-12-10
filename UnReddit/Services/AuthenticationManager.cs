using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnReddit.Services
{
    public class AuthenticationManager
    {
        private const string StorageKeyAccessRefreshToken = "reddit_access_refesh_token";

        private readonly string mServiceName;

        private Windows.Storage.ApplicationDataContainer mStorage;
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime AccessTokenExpire { get; private set; }

        public AuthenticationManager(String serviceName)
        {
            mServiceName = serviceName;

            mStorage = Windows.Storage.ApplicationData.Current.LocalSettings;
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            AccessTokenExpire = DateTime.Now;
        }

        public void ResetToken()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            AccessTokenExpire = DateTime.Now;

            WriteStorageValue(StorageKeyAccessRefreshToken, string.Empty);
        }

        public bool GetIsLoginRequired()
        {
            if (!string.IsNullOrEmpty(RefreshToken))
            {
                return false;
            }

            RefreshToken = ReadStorageValueOrDefault(StorageKeyAccessRefreshToken);
            if (!string.IsNullOrEmpty(RefreshToken))
            {
                return false;
            }

            return true;
        }
        public bool GetIsExpired()
        {
            return AccessTokenExpire < DateTime.Now;
        }

        public void SetRefreshToken(string refreshToken, string accessToken, int expireSeconds)
        {
            WriteStorageValue(StorageKeyAccessRefreshToken, refreshToken);

            SetAccessToken(accessToken, expireSeconds);
        }

        public void SetAccessToken(string accessToken, int expireSeconds)
        {
            AccessToken = accessToken;
            AccessTokenExpire = DateTime.Now.AddSeconds(expireSeconds);
        }

        private string GetStoragePrefixKey()
        {
            return "auth_manager_" + mServiceName;
        }

        private string ReadStorageValueOrDefault(string key, string orDefault = "")
        {
            var val = mStorage.Values[GetStoragePrefixKey() + key];

            if (val == null)
            {
                return orDefault;
            }

            return val.ToString();
        }
        private void WriteStorageValue(string key, string value)
        {
            mStorage.Values[GetStoragePrefixKey() + key] = value;
        }
    }
}
