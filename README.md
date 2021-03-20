# PowerPlatform Dataverse simple client app

Small app to connect to Dataverse environment instance and call WhoAmI, using [PowerPlatform.Dataverse.ServiceClient](https://github.com/microsoft/PowerPlatform-DataverseServiceClient
) library, available as [nuget package](https://www.nuget.org/packages/Microsoft.PowerPlatform.Dataverse.Client/).

Showcases how to initialize connection and handle errors.

Console app expects 3 parameters in order (sorry, no fancy cmdline args processing here):

1. environment URL, e.g. <https://myenv.crm.dynamics.com>
2. user name or appID, e.g. me@myenv.onmicrosoft.com  or 5C813AC0-7DC2-487E-A67F-8EBD5C3CADBB
3. secret (user's password or appID's client secret)

## Setting Up Local Dev Environment

Windows, macOS or Linux:

- [git](https://git-scm.com/downloads)
- [Visual Studio Code](https://code.visualstudio.com/Download)
- alternative. on Windows: [VisualStudio 2019 Community](https://visualstudio.microsoft.com/downloads/)
- [.NET Core 5.0](https://dotnet.microsoft.com/download)
- recommended VSCode extensions:
  - [C# for VSCode (ms-dotnettools.csharp)](https://github.com/OmniSharp/omnisharp-vscode)
  - [EditorConfig for VS Code (editorconfig.editorconfig)](https://github.com/editorconfig/editorconfig-vscode)
  - [GitLens (eamodio.gitlens)](https://github.com/eamodio/vscode-gitlens)
  - [markdownlint (davidanson.vscode-markdownlint)](https://github.com/DavidAnson/vscode-markdownlint)
