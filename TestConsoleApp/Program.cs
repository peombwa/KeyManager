using Key.Manager;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            string sampleData = "The quick brown fox jumped over the lazy dog.";


            // TestLinuxKeyManager(sampleData);

            CallGraphAsync().GetAwaiter().GetResult(); ;
        }

        private static void TestLinuxKeyManager(string sampleData)
        {
            var keyStorageConfig = new KeyStorageConfig
            {
                LinuxKeyring = KeyringType.KEY_SPEC_USER_KEYRING,
                CacheDirectory = "D:\\New folder\\tokenStore\\",
                CacheFileName = "mytoken.bin3",
                ClientId = "myId"
            };
            KeyStorage keyStorage = new KeyStorage(keyStorageConfig);
            byte[] data = Encoding.UTF8.GetBytes(sampleData);

            Console.WriteLine($"Writing data to keyring: {data.Length}");
            keyStorage.WriteContent(data);

            byte[] readData = keyStorage.ReadContent();
            Console.WriteLine($"Read data from keyring: {readData.Length}");
            string originalContent = Encoding.UTF8.GetString(readData);
            Console.WriteLine($"Original data from keyring: {originalContent}");
        }

        static async Task CallGraphAsync()
        {
            IPublicClientApplication publicClientApp = PublicClientApplicationBuilder
            .Create("clientId")
            .Build();

            var keyStorageConfig = new KeyStorageConfig
            {
                LinuxKeyring = KeyringType.KEY_SPEC_USER_KEYRING,
                CacheDirectory = "D:\\New folder\\tokenStore\\",
                CacheFileName = "mytoken.bin3",
                ClientId = publicClientApp.AppConfig.ClientId
            };

            KeyStorage keyStorage = new KeyStorage(keyStorageConfig);

            publicClientApp.UserTokenCache.SetBeforeAccess((args) => {
                var cachedStoreData = keyStorage.ReadContent();
                args.TokenCache.DeserializeMsalV3(cachedStoreData, shouldClearExistingCache: true);
            });

            publicClientApp.UserTokenCache.SetAfterAccess((args) => {
                if (args.HasStateChanged)
                {
                    byte[] data = args.TokenCache.SerializeMsalV3();
                    keyStorage.WriteContent(data);
                }
            });

            var authProvider = new DeviceCodeProvider(publicClientApp, new List<string> { "user.read" });

            var graphClient = new GraphServiceClient(authProvider);
            var me = await graphClient.Me.Request().GetAsync();

            Console.WriteLine($"Id: {me.Id}, Display Name: {me.DisplayName}, UPN: {me.UserPrincipalName}");
        }
    }
}
