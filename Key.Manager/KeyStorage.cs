namespace Key.Manager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    public sealed class KeyStorage
    {
        internal readonly KeyStorageConfig KeyStorageConfig;
        internal const string KeyIdentifier = "KeyMngr";
        internal const string LinuxKeyType = "user";

        public KeyStorage(KeyStorageConfig keyStorageConfig)
        {
            if (string.IsNullOrEmpty(keyStorageConfig.ClientId))
                throw new ArgumentException($"{nameof(keyStorageConfig.ClientId)} cannot be null or empty.");

            if (CommonUtils.IsWindowsPlatform())
            {
                if (string.IsNullOrEmpty(keyStorageConfig.CacheDirectory))
                    throw new ArgumentException($"{nameof(keyStorageConfig.CacheDirectory)} cannot be null or empty.");
                if (string.IsNullOrEmpty(keyStorageConfig.CacheFileName))
                    throw new ArgumentException($"{nameof(keyStorageConfig.CacheFileName)} cannot be null or empty.");
            }

            KeyStorageConfig = keyStorageConfig;
        }

        public void ClearContent()
        {
            if (CommonUtils.IsLinuxPlatform())
            {
                int key = LibKeyUtils.request_key(LinuxKeyType, $"{KeyIdentifier}:{KeyStorageConfig.ClientId}", (int)KeyStorageConfig.LinuxKeyring);
                if (key != -1)
                    LibKeyUtils.keyctl("invalidate", key);
            }
        }

        public byte[] ReadContent()
        {
            if (CommonUtils.IsLinuxPlatform())
                return ReadLinuxContent();
            else if (CommonUtils.IsMacPlatform())
                return ReadMacOSContent();

            return ReadWindowsContent();
        }

        public void WriteContent(byte[] content)
        {
            if (CommonUtils.IsLinuxPlatform())
                WriteLinuxContent(content);
            else if (CommonUtils.IsMacPlatform())
                WriteMacOSContent(content);

            WriteWindowsContent(content);
        }

        private byte[] ReadLinuxContent()
        {
            int key = LibKeyUtils.request_key(LinuxKeyType, $"{KeyIdentifier}:{KeyStorageConfig.ClientId}", (int)KeyStorageConfig.LinuxKeyring);
            if (key == -1)
                return new byte[0];

            long contentLength = LibKeyUtils.keyctl_read_alloc(key, out IntPtr contentPtr);
            string content = Marshal.PtrToStringAuto(contentPtr);
            Marshal.FreeHGlobal(contentPtr);

            if (String.IsNullOrEmpty(content))
                return new byte[0];

            return Convert.FromBase64String(content);
        }

        private byte[] ReadMacOSContent()
        {
            return new byte[0];
        }

        private byte[] ReadWindowsContent()
        {
            if (!File.Exists(KeyStorageConfig.CacheFilePath))
                return new byte[0];

            return File.ReadAllBytes(KeyStorageConfig.CacheFilePath);
        }

        private void WriteLinuxContent(byte[] content)
        {
            if (content != null && content.Length > 0)
            {
                string encodedContent = Convert.ToBase64String(content);
                int key = LibKeyUtils.add_key(LinuxKeyType, $"{KeyIdentifier}:{KeyStorageConfig.ClientId}", encodedContent, encodedContent.Length, (int)KeyStorageConfig.LinuxKeyring);
            }
        }

        private void WriteMacOSContent(byte[] content)
        {

        }

        private void WriteWindowsContent(byte[] content)
        {
            byte[] secureData = ProtectedData.Protect(content, null, scope: DataProtectionScope.CurrentUser);

            if (!Directory.Exists(KeyStorageConfig.CacheDirectory))
                Directory.CreateDirectory(KeyStorageConfig.CacheDirectory);

            File.WriteAllBytes(KeyStorageConfig.CacheFilePath, secureData);
        }
    }
}
