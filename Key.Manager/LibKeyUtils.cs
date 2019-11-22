namespace Key.Manager
{
    using System;
    using System.Runtime.InteropServices;
    internal class LibKeyUtils
    {
#pragma warning disable SA1300 // disable lowercase function name warning.
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("libkeyutils.so.1", CallingConvention = CallingConvention.StdCall)]
        public static extern int add_key(string type, string description, string payload, int plen, int keyring);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("libkeyutils.so.1", CallingConvention = CallingConvention.StdCall)]
        public static extern int request_key(string type, string description, int dest_keyring);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("libkeyutils.so.1", CallingConvention = CallingConvention.StdCall)]
        public static extern int request_key(string type, string description, string callout_info, int dest_keyring);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("libkeyutils.so.1", CallingConvention = CallingConvention.StdCall)]
        public static extern long keyctl_read_alloc(int key, out IntPtr buffer);

#pragma warning restore SA1300 // restore lowercase function name warning.
    }
}
