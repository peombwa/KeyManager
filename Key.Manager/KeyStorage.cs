namespace Key.Manager
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
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

        /// <summary>
        /// Clear content from a secure store.
        /// </summary>
        public void ClearContent()
        {
            if (CommonUtils.IsLinuxPlatform())
            {
                int key = LibKeyUtils.request_key(LinuxKeyType, $"{KeyIdentifier}:{KeyStorageConfig.ClientId}", (int)KeyStorageConfig.LinuxKeyring);
                if (key != -1)
                    LibKeyUtils.keyctl("invalidate", key);
            }
        }

        /// <summary>
        /// Read a <see cref="byte[]"/> content from a secure store.
        /// </summary>
        /// <returns><see cref="byte[]"/> content.</returns>
        public byte[] ReadContent()
        {
            if (CommonUtils.IsLinuxPlatform())
                return ReadLinuxContent();
            else if (CommonUtils.IsMacPlatform())
                return ReadMacOSContent();

            return ReadWindowsContent();
        }

        /// <summary>
        /// Write a plain content to a secure store.
        /// </summary>
        /// <param name="content">Content to store securely.</param>
        public void WriteContent(byte[] content)
        {
            if (CommonUtils.IsLinuxPlatform())
                WriteLinuxContent(content);
            else if (CommonUtils.IsMacPlatform())
                WriteMacOSContent(content);

            WriteWindowsContent(content);
        }

        /// <summary>
        /// Get stored content using Linux Kernel Key management API.
        /// see: https://www.kernel.org/doc/html/latest/security/keys/core.html#id2
        /// </summary>
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

        /// <summary>
        /// Not implemented.
        /// </summary>
        private byte[] ReadMacOSContent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads secure contents from a file and decrypts them using Windows DPAPI.
        /// </summary>
        private byte[] ReadWindowsContent()
        {
            if (!File.Exists(KeyStorageConfig.CacheFilePath))
                return new byte[0];

            return ProtectedData.Unprotect(File.ReadAllBytes(KeyStorageConfig.CacheFilePath), null, DataProtectionScope.CurrentUser);
        }

        /// <summary>
        /// Store content securely using Linux Kernel Key management API.
        /// see: https://www.kernel.org/doc/html/latest/security/keys/core.html#id2
        /// </summary>
        private void WriteLinuxContent(byte[] content)
        {
            if (content != null && content.Length > 0)
            {
                string encodedContent = Convert.ToBase64String(content);
                int key = LibKeyUtils.add_key(LinuxKeyType, $"{KeyIdentifier}:{KeyStorageConfig.ClientId}", encodedContent, encodedContent.Length, (int)KeyStorageConfig.LinuxKeyring);
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        private void WriteMacOSContent(byte[] content)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encrypt and write contents to a file using Windows DPAPI.
        /// </summary>
        private void WriteWindowsContent(byte[] content)
        {
            byte[] secureData = ProtectedData.Protect(content, null, scope: DataProtectionScope.CurrentUser);

            if (!Directory.Exists(KeyStorageConfig.CacheDirectory))
                Directory.CreateDirectory(KeyStorageConfig.CacheDirectory);

            File.WriteAllBytes(KeyStorageConfig.CacheFilePath, secureData);
        }
    }
}
