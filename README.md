# razor pages auth and graph

This app is a simple example of using the Microsoft identity platform for:

- adding Azure AD organizational sign-in to a Razor Pages app
- getting and requesting an `access_token` to call the Microsoft Graph API to get user data, and
- *incremental consent*, a concept of asking for additional permissions when needed, instead of upfront
- retrieving an email message list from graph

## instructions

You'll need a few things to start:

- [dotnet 5](https://get.dot.net)
- [visual studio code](https://code.visualstudio.com) or an editor/IDE of your choosing
- In Visual Studio Code, [install the C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- an Azure AD directory (tenant).
- If you don't have an Azure AD directory, or you don't have enough access to one, you can get a free one [here](https://aka.ms/425show/freem365)

### 1 - Install Developer Certificate

- Open a command prompt
- Run `dotnet dev-certs https`
- Run `dotnet dev-certs https --trust`

### 2 - Create .NET 5 Web App

- Open your preferred command prompt
- Create a new directory and change to it
  - `mkdir <yourDirectoryName>`
  - `cd <yourDirectoryName>`
- Create a new Razor Pages app. You can also use mvc here by substituting `webapp` with `mvc` in the command: `dotnet new webapp --auth SingleOrg`
- Open this project in VS Code
  - In the command prompt, type `code .`

### 3 - Configure Azure AD

First we need to register an application. The application registration is how we configure Azure AD to allow our app to sign users in and use other services protected by Azure AD. If you are using a free directory or sample directory, you may want to use a private browser window to avoid any conflicts with your work or school account. Make sure you sign-in with the correct account! If you created a test or developer tenant, make sure to use the account in that directory!

- Navigate to the [Azure Portal](https://portal.azure.com). Check which account and directory you are signed in with - in the top right corner, where your name and picture are located, you can see which directory 'context' you are within. If it's not your test directory, click the `Subscription + directory` filter button (next to the notification bell in the Azure portal), then choose 'All Directories' and pick the correct one.

> Tip: you can use your tenant/directory name in the URL to make sure you sign-in to the correct directory, e.g., if your directory is named contoso.onmicrosoft.com, you can navigate to `https://portal.azure.com/contoso.onmicrosoft.com` to sign-in to that directory directly.

- Next, get to the Azure Active Directory blade. You can search for 'Azure Active Directory' in the top search bar to get there faster.
- Now find the **App Registrations** blade
- Click the **New registration** button in the top menu
- Give your app a Name. This is just a display name, so make sure it's something you'll remember. You can change it later.
- Leave the account type to the default value (single-tenant, may also say My Organization or similar)
- Add a redirect URI of `https://localhost:5001/signin-oidc`
- Now we need to copy out a few pieces of information:
  - Application (client) ID
  - Tenant ID
- Next we need to create a client secret
  - Go to **Certificates and secrets**
  - Create a new secret. 1 year validity is fine
  - Copy the value somewhere safe - this is the only time you'll see it!

> Tip: if you lose the secret, that's ok - you can create a new one and delete the old one. For development and test, a secret is fine. When you move your app to production, we'd suggest using a Client Certificate instead of a secret.

### 4- Update your application code

Next we need to update some settings in our application. Go back to your editor.

- Open `appsettings.json`
  - Update the values for Tenant and Client ID to match what you copied earlier
  - Update the value for domain to use your test tenant `<YourTenantName>.onmicrosoft.com`
  - Add a new parameter called: `ClientSecret`
  - Paste the secret value from previous step
- Next Open `Startup.cs`
  - Go to line 34
  - Add the following code:

```csharp
services.Configure<MicrosoftIdentityOptions>(options => options.ResponseType = OpenIdConnectResponseType.Code);
```

- Use Ctrl + . to prompt IntelliSense and resolve the missing dependencies, or add `using using Microsoft.AspNetCore.Authentication.OpenIdConnect;` to the top of your file.

### 6 - Run the app

- (option 1) Press F5  VS Code
  - If prompted, select Environment -> .NET Core
  - Press F5 again
- (option 2) Type the following in the command line, in the folder where your app is `dotnet run`

> Tip: you can access a terminal directly in Visual Studio Code! Press `CTRL+~' in Windows, or click View --> Terminal from the top menu

- Next, let's navigate to the app! If you pressed F5, your browser may have already opened.
  - Navigate to `https://localhost:5001`
- Sign in with your test tenant account. If you can sign-in, you've successfully added authentication to your app!
- Sign out

> Tip: Sign-out is a two-step process. You can sign-out of just your app, or you can sign-out of both your app **and** the identity provider (in this case, Azure AD). When you click the sign out link, you'll be taken to a page asking you which account to sign out of - clicking your account will sign you out of both the application and Azure AD.

### 7 - add Microsoft Graph

- Open `Startup.cs`
- Add the following code
  - Line 33 -> add `services.AddHttpClient();
  - Replace this code
- Update `Index.cshtml.cs`
- Copy paste the following code

> Make sure to update the Namespace to your project

- Update `Index.cshtml`
- Paste the following code into the page

### 8 - Get to know Graph Explorer

- Open the broswer
- Navigate to `aka.ms/ge`

### 9 - add more MS Graph data

- Open `Index.cshtml.cs`
- Update the code with the following:

- Update `Index.cshtml`
- Paste the following code:
- Save and run the project
