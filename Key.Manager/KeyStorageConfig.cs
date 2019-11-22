using System.IO;

namespace Key.Manager
{
    public class KeyStorageConfig
    {
        private const int SessionKeyring = -3;
        private const int UserKeyring = -4;
        public string CacheFilePath => Path.Combine(CacheDirectory, CacheFileName);
        public string CacheDirectory { get; set; }
        public string CacheFileName { get; set; }
        public string ClientId { get; set; }
        public KeyringType LinuxKeyring { get; set; } = KeyringType.KEY_SPEC_USER_KEYRING;
    }
}