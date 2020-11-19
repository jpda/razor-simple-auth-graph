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

- Use Ctrl + . to prompt IntelliSense and resolve the missing dependencies, or add `using Microsoft.AspNetCore.Authentication.OpenIdConnect;` to the top of your file.

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

We need to add `EnabledTokenAcquisitionToCallDownstreamApi()` in `Startup.cs` which gives us a method to get our tokens in pages, and a cache for storing those tokens with `AddInMemoryCache()`.
Replace this code:

```csharp
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
```

with:

```csharp
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
```

Your `ConfigureServices` method should look like this in total:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient();
    services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddInMemoryTokenCaches()
        ;
    services.Configure<MicrosoftIdentityOptions>(options => {
        options.ResponseType = OpenIdConnectResponseType.Code;
    });
    services.AddAuthorization(options =>
    {
        // By default, all incoming requests will be authorized according to the default policy
        options.FallbackPolicy = options.DefaultPolicy;
    });
    services.AddRazorPages()
        .AddMvcOptions(options => {})
        .AddMicrosoftIdentityUI();
}
```

Next we need to update our Index.cshtml.cs page to include our new services.

First, at the top of our class, let's add `AuthorizeForScopes`, which declares which scopes or permissions we're going to need in our page.

```csharp
[AuthorizeForScopes(Scopes = new[] { "User.Read" })]
public class IndexModel : PageModel
{
  //snip
```

Now we add a couple of private variables to our class and set them in the constructor, plus we add a field called `MeData` which we'll use to show data on the page:

```csharp
private readonly ITokenAcquisition _tokenGetter;
private readonly HttpClient _httpClient;

public string MeData;

public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenGetter, IHttpClientFactory clientFactory)
{
    _logger = logger;
    _tokenGetter = tokenGetter;
    _httpClient = clientFactory.CreateClient();
}
```

Lastly, we need to actually get a token and call Microsoft Graph in the `OnGet` method. We're using Graph, but this could be any API protected with Azure AD.

```csharp
public async Task OnGet()
{
    var accessToken = await _tokenGetter.GetAccessTokenForUserAsync(new[] { "User.Read" });

    _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    var result = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");

    if (result.IsSuccessStatusCode)
    {
        MeData = await result.Content.ReadAsStringAsync();
    }
    else
    {
        MeData = $"{result.StatusCode}";
    }
}
```

Now that we're getting data from the API, let's quickly show it on our page. Open `Index.cshtml` and add this bit into the `text-center` div:

```csharp
<div>@Model.MeData</div>
```

So that our page looks like this:

```csharp
@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://docs.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
    <div>@Model.MeData</div>
</div>

```

### 8 - Get to know Graph Explorer

- Open the broswer
- Navigate to `aka.ms/ge`

### 9 - add more MS Graph data

First we need to declare the extra scopes we need - in our case, we also need the `Mail.Read` scope to read a user's mail. Add that to our existing `AuthorizeForScopes` attribute at the top of the class in `Index.cshtml.cs`.

```csharp
[AuthorizeForScopes(Scopes = new[] { "User.Read", "Mail.Read" })]
```

Then rinse and repeat - we need to add extra code for the second API call. First, a new class field to hold the new data:

```csharp
public string MailData;
```

Some new code at the end of the `OnGet` method to get the data from Graph:

```csharp
result = await _httpClient.GetAsync("https://graph.microsoft.com/beta/me/messages?$select=createdDateTime,subject,from");

if (result.IsSuccessStatusCode)
{
    MailData = await result.Content.ReadAsStringAsync();
}
else
{
    MailData = $"{result.StatusCode}";
}
```

And lastly, an extra `div` on `Index.cshtml` to show the data:

```csharp
<div>
    <h1>mail data</h1>
    <div>@Model.MailData</div>
</div>
```
