namespace Key.Manager
{
    using System;
    using System.Runtime.InteropServices;
    public enum KeyringType
    {
        /// <summary>
        /// Each thread may have its own keyring.
        /// This is searched first, before all others.
        /// </summary>
        KEY_SPEC_THREAD_KEYRING = -1,
        /// <summary>
        /// Each process (thread group) may have its own keyring.
        /// This is shared between all members of a group and will be searched after the thread keyring.
        /// </summary>
        KEY_SPEC_PROCESS_KEYRING = -2,
        /// <summary>
        /// Each process subscribes to a session keyring that is inherited across (v)fork, exec and clone.
        /// This is searched after the process keyring.
        /// </summary>
        KEY_SPEC_SESSION_KEYRING = -3,
        /// <summary>
        /// This keyring is shared between all the processes owned by a particular user.
        /// It isn't searched directly, but is normally linked to from the session keyring.
        /// </summary>
        KEY_SPEC_USER_KEYRING = -4,
        /// <summary>
        /// This is the default session keyring for a particular user.
        /// Login processes that change to a particular user will bind to this session until another session is set.
        /// </summary>
        KEY_SPEC_USER_SESSION_KEYRING = -5
    }
    /// <summary>
    /// An implementation of Linux Kernel Key management API https://www.kernel.org/doc/html/latest/security/keys/core.html#id2.
    /// </summary>
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
        public static extern long keyctl(string action, int key);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("libkeyutils.so.1", CallingConvention = CallingConvention.StdCall)]
        public static extern long keyctl_read_alloc(int key, out IntPtr buffer);

#pragma warning restore SA1300 // restore lowercase function name warning.
    }
}
