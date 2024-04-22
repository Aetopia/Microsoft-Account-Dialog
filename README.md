> [!WARNING]
> All logo assets belong to Microsoft!

# Microsoft Account Dialog
An alternative way to add a Microsoft account to Windows.

## Usage
To add a Microsoft account to Windows, one has to do the following:
- Open Windows Settings.
- Accounts →  Email & accounts.
- Click <kbd>Add an account</kbd>.
- Click <kbd>Outlook.com</kbd>.

![image](https://github.com/Aetopia/MicrosoftAccountDialog/assets/41850963/45ba94ce-0f2a-4dbc-95cd-fc5048fd5a77)

This application provides the same functionality but in the form of a desktop app.<br>
This maybe used as a stand in replacement for adding Microsoft accounts to Windows in case its not possible to add through Windows Settings.

![image](https://github.com/Aetopia/MicrosoftAccountDialog/assets/41850963/7bbadd36-9bb3-4b7b-9d4b-6c3313b34817)

- Simply download the latest release from [`GitHub Releases`](https://github.com/Aetopia/MicrosoftAccountDialog/releases/latest) and run the application.

## Building
1. Download the following:
    - [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - [.NET Framework 4.8.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net481-developer-pack-offline-installer)

2. Run the following commands to compile:
    
    ```cmd
    dotnet restore
    dotnet publish -c Release
    ```