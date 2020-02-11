using CommandLine;
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
    public class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => CallGraphAsync(opts.ClientId, opts.CacheDirectory, opts.CacheFileName).GetAwaiter().GetResult())
                .WithNotParsed((errs) => HandleError(errs));
        }

        static async Task CallGraphAsync(string clientId, string CacheDirectory, string CacheFileName)
        {
            IPublicClientApplication publicClientApp = PublicClientApplicationBuilder
            .Create(clientId)
            .Build();

            var keyStorageConfig = new KeyStorageConfig
            {
                LinuxKeyring = KeyringType.KEY_SPEC_USER_KEYRING,
                CacheDirectory = CacheDirectory,
                CacheFileName = $"{CacheFileName}.bin3",
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
            var currentUser = await graphClient.Users[me.Id].Request().GetAsync();

            Console.WriteLine($"Id: {currentUser.Id}, Display Name: {currentUser.DisplayName}, UPN: {currentUser.UserPrincipalName}");
        }

        private static void HandleError(IEnumerable<CommandLine.Error> errors)
        {
            foreach (CommandLine.Error item in errors)
            {
                Console.Write(item.ToString());
            }
        }
    }
}
