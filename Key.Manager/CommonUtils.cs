namespace Key.Manager
{
    using System;
    using System.Runtime.InteropServices;
    internal class CommonUtils
    {
        public static bool IsWindowsPlatform()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        public static bool IsMacPlatform()
        {
#if NET45
            //See http://www.mono-project.com/docs/faq/technical/
            return Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix;
#else
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
        }

        public static bool IsLinuxPlatform()
        {
#if NET45
            return Environment.OSVersion.Platform == PlatformID.Unix;
#else
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif
        }
    }
}
