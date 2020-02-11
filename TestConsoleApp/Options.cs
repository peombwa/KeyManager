using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsoleApp
{
    public class Options
    {
        [Option('c', "ClientId", HelpText = "The application id to use.")]
        public string ClientId { get; set; }

        [Option('d', "CacheDirectory", Default = "%UserProfile%\\Documents", HelpText = "Directory to save token cache file on Windows.")]
        public string CacheDirectory { get; set; }

        [Option('f', "CacheFileName", Default = "UserTokenCache", HelpText = "Token cache file name.")]
        public string CacheFileName { get; set; }

    }
}
