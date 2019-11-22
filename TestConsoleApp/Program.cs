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
            var keyStorageConfig = new KeyStorageConfig { ClientId = "myClientId", LinuxKeyring = KeyringType.KEY_SPEC_USER_KEYRING };
            string sampleData = "The quick brown fox jumped over the lazy dog.";


            TestLinuxKeyManager(keyStorageConfig, sampleData);

            CallGraphAsync(keyStorageConfig).GetAwaiter().GetResult(); ;
        }

        private static void TestLinuxKeyManager(KeyStorageConfig keyStorageConfig, string sampleData)
        {
            KeyStorage keyStorage = new KeyStorage(keyStorageConfig);
            byte[] data = Encoding.UTF8.GetBytes(sampleData);

            Console.WriteLine($"Writing data to keyring: {data.Length}");
            keyStorage.WriteContent(data);

            byte[] readData = keyStorage.ReadContent();
            Console.WriteLine($"Reading data from keyring: {readData.Length}");
            string originalContent = Encoding.UTF8.GetString(readData);
            Console.WriteLine($"Original data from keyring: {originalContent}");
        }

        static async Task CallGraphAsync(KeyStorageConfig keyStorageConfig)
        {
            IPublicClientApplication publicClientApp = PublicClientApplicationBuilder
            .Create("ClientID")
            .Build();

            keyStorageConfig.ClientId = publicClientApp.AppConfig.ClientId;
            KeyStorage keyStorage = new KeyStorage(keyStorageConfig);

            publicClientApp.UserTokenCache.SetBeforeAccess((args) => {
                Console.WriteLine($"Before access, the store has changed");
                var cachedStoreData = keyStorage.ReadContent();
                Console.WriteLine($"Read '{cachedStoreData?.Length}' bytes from storage");
                args.TokenCache.DeserializeMsalV3(cachedStoreData, shouldClearExistingCache: true);
            });

            publicClientApp.UserTokenCache.SetAfterAccess((args) => {
                if (args.HasStateChanged)
                {
                    Console.WriteLine($"Before Write Store");
                    byte[] data = args.TokenCache.SerializeMsalV3();
                    Console.WriteLine($"Serializing '{data.Length}' bytes");

                    keyStorage.WriteContent(data);
                    Console.WriteLine($"After write store");
                }
            });

            var authProvider = new DeviceCodeProvider(publicClientApp, new List<string> { "user.read" });

            var graphClient = new GraphServiceClient(authProvider);
            var me = await graphClient.Me.Request().GetAsync();

            Console.WriteLine($"Id: {me.Id}, Display Name: {me.DisplayName}, UPN: {me.UserPrincipalName}");
        }
    }
}
