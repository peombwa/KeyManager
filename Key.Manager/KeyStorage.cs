namespace Key.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    public sealed class KeyStorage: IKeyStorage
    {
        private const int SessionKeyring = -3;
        private const int UserKeyring = -4;
        internal readonly KeyStorageConfig keyStorageConfig;

        public KeyStorage(KeyStorageConfig keyStorageConfig)
        {
            this.keyStorageConfig = keyStorageConfig;
        }

        public void ClearContent()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadContent()
        {
            int key = LibKeyUtils.request_key("user", "myapp:12", SessionKeyring);
            if (key == -1)
            {
                return null;
            }

            long contentLength = LibKeyUtils.keyctl_read_alloc(key, out IntPtr contentPtr);
            string content = Marshal.PtrToStringAuto(contentPtr);
            Marshal.FreeHGlobal(contentPtr);

            if (String.IsNullOrEmpty(content))
            {
                return null;
            }

            return Convert.FromBase64String(content);
        }

        public void WriteContent(byte[] content)
        {
            string encodedContent = Convert.ToBase64String(content);
            int key = LibKeyUtils.add_key("user", "myapp:12", encodedContent, encodedContent.Length, SessionKeyring);
        }
    }
}
