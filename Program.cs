// Copyright (c) David JENNI
// Licensed under the MIT License.
//
// Execute this sample with:
//      dotnet run -- -h
//      dotnet run <envUrl> [usernameOrAppId [secret]]
//

using System;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Auth;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;

namespace DataverseClient
{
    class Program
    {
        private bool IsAppUser { get; }
        private Uri EnvUrl { get; }
        private string UserNameOrAppId { get; }
        private string Secret { get; }
        private PromptBehavior PromptBehavior { get; } = PromptBehavior.Always;

        private Program(string[] args)
        {
            if (args.Length < 1 || (args.Length > 0 && string.CompareOrdinal(args[0], "-h") == 0))
            {
                Console.WriteLine($@"
Usage:
  {Process.GetCurrentProcess().ProcessName} <environmentUrl> [<usernameOrAppId> [ <secret> ]]

options:
    Without username and password cmd line arguments, MSAL's AAD login dialog will prompt interactively.
    Instead of passing the secret as the third parameter, set an environment variable 'PP_BT_ENV_SECRET' with that secret
");
                Environment.Exit(1);
            }
            EnvUrl = new Uri(args[0]);

            if (args.Length > 1)
            {
                UserNameOrAppId = args[1];
                IsAppUser = Guid.TryParse(UserNameOrAppId, out var _);
            }

            Secret = args.Length > 2 ? args[2] : Environment.GetEnvironmentVariable("PP_BT_ENV_SECRET");
            if (!string.IsNullOrWhiteSpace(Secret))
            {
                PromptBehavior = PromptBehavior.Never;
            }
        }

        static void Main(string[] args)
        {
            var success = new Program(args).Connect();
            Environment.Exit(success ? 0 : 1);
        }

        private bool Connect()
        {
            try
            {
                using var serviceClient = Create();
                Console.WriteLine($"Connected to '{EnvUrl}'");

                var response = (WhoAmIResponse)serviceClient.ExecuteOrganizationRequest(new WhoAmIRequest());
                if (response == null)
                {
                    Console.Error.WriteLine($"{Environment.NewLine}>> ERROR: Cannot execute request from '{EnvUrl}', error: {serviceClient.LastError}");
                    return false;
                }
                Console.WriteLine($"Connected as user: {serviceClient.OAuthUserId} (userId: {response.UserId}) to env: {serviceClient.ConnectedOrgUniqueName} ({response.OrganizationId}, {serviceClient.ConnectedOrgUriActual})");
            }
            catch (DataverseConnectionException ex)
            {
                Console.Error.WriteLine($"{Environment.NewLine}>> ERROR connecting to env {EnvUrl}:");
                Exception currentEx = ex;
                do
                {
                    Console.Error.WriteLine($">>++ {currentEx.Message}");
                    currentEx = currentEx.InnerException;
                }
                while (currentEx != null);
                return false;
            }
            return true;
        }

        private ServiceClient Create()
        {
            // clientID and redirect url are values configured in Microsoft's tenant that are functional for 3rd parties sample code
            // for more than sample use, please create specific clientID and redirect url in your PowerPlatform's AAD tenant
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
#if NET48
            var redirectUrl = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");
#else
            var redirectUrl = new Uri("http://localhost");
#endif

            var builder = new DbConnectionStringBuilder
            {
                { "Url", EnvUrl.AbsoluteUri },
                { "RedirectUri", redirectUrl.AbsoluteUri },
                { "LoginPrompt", PromptBehavior.ToString() }
            };

            Console.WriteLine($"Connecting to env: {EnvUrl.AbsoluteUri}...");
            if (IsAppUser)
            {
                Console.WriteLine($"... authN using appId & clientSecret - {UserNameOrAppId}");
                builder.Add("AuthType", "ClientSecret");
                builder.Add("AppId", UserNameOrAppId);
                builder.Add("ClientSecret", Secret);
            }
            else
            {
                Console.WriteLine($"... authN using username & password - {UserNameOrAppId}");
                builder.Add("AuthType", "OAuth");
                builder.Add("ClientId", clientId);
                if (!string.IsNullOrWhiteSpace(UserNameOrAppId))
                {
                    builder.Add("Username", UserNameOrAppId);
                    builder.Add("Password", Secret);
                }
            }

            Console.WriteLine(builder.ConnectionString);
            return new ServiceClient(builder.ConnectionString);
        }
    }
}
