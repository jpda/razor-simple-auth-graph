# razor pages auth and graph
This app is a simple example of using the Microsoft identity platform for:
- adding Azure AD organizational sign-in to a Razor Pages app
- getting and requesting an `access_token` to call the Microsoft Graph API to get user data, and
- *incremental consent*, a concept of asking for additional permissions when needed, instead of upfront
- retrieving an email message list from graph


# Workshop Instructions

## Prerequsites 
Everyone should have a tenant - start with this
	- Open a private window
	- Use this URI 
	- Press **Join** now
	- Sign in with an MSA account

## Exercise 1 - install the dependencies
	1. Download .NET 5 SDK
		a. Go to https://dotnet.microsoft.com/download
	2. Download VS Code 
		a. Go to aka.ms/vscode 
	3. In VS Code go to extensions
		a. Install the C# Extension

## Exercise 2 - Install Developer Certificate
		a. Open a command prompt
		b. Run `dotnet dev-certs https`
		c. Run `dotnet dev-certs https --trust`
	
## Exercise 3 - Create .NET 5 Web App
	1. Open your preferred command prompt
	2. Create a new directory
		a. `mkdir <yourDirectoryName>`
		b. `cd <yourDirectoryName>`
	3. Type `dotnet new webapp --auth SingleOrg` and press enter
	4. Open this project in VS Code
		a. In the command prompt, type `code .`

## Exercise 4 - Configure Azure AD
	1. Register an app
		a. Open the Azure AD portal in Azure 
			i. Navigate to `portal.azure.com`
			ii. Sign in with your test tenant account
		b. Open your **Azure Active Directory** tenant
		c. Click on the **App Registrations** tab
		d. Click the **New Registration** button (at the top)
		e. Give your app a **Name**
		f. Add Redirect URI -> `https://localhost:5001/signin-oidc`
		g. For Account Types -> Leave the default option (single organization)
		h. Press Register (bottom)
		i. Copy the Client ID
		j. Copy the Tenant ID
		k. Create a Client Secret
			i. Go to the **Certificates and Secrets** tab
			ii. Create a new Secret
			iii. Set expiration date to 1 year
			iv. Copy the value

## Exercise 5 - Update your application code
	1. Open `AppSettings.json`
		a. Update the values for Tenant and Client ID
		b. Update the value for domain to use your test tenant `<YourTenantName>.onmicrosoft.com`
		c. Add a new parameter called: `ClientSecret`
		d. Paste the secret value from previous step
	2. Open `Startup.cs`
	3. Go to line 34
		a. Add the following code: 
		  ```
      services.Configure<MicrosoftIdentityOptions>(options => options.ResponseType = OpenIdConnectResponseType.Code);
      ```
		b. Use Ctrl + . to prompt IntelliSence and resolve the missing dependencies
	
## Exercise 6 - Run the app
	1. (option 1) Press F5  VS Code
		a. If prompted, select Environment -> .NET
		b. Press F5 again
	2. (option 2) Type the following in the command line `dotnet run`
		a. In your browser 
		b. Navigate to https://localhost:5001 
	3. Sign in with your test tenant account
		a. Sign out 

## Exercise 7 - add Microsoft Graph
	- Open `Startup.cs`
	- Add the following code
		○ Line 33 -> add `services.AddHttpClient();
		○ Replace this code 
		○ ```
		○ ```
		○ With this code
		○ ```
		○ ```
	- Update `Index.cshtml.cs`
	- Copy paste the following code
    ```
    ```
	> Make sure to update the Namespace to your project
	- Update `Index.cshtml`
	- Paste the following code into the page
    ```
    ```

## Exercise 9 - Get to know Graph Explorer
	- Open the broswer
	- Navigate to `aka.ms/ge`
	
## Exercise 10 - add more MS Graph data
	- Open `Index.cshtml.cs`
	- Update the code with the following:
    ```
    ```
  - Update `Index.cshtml`
  - Paste the following code:
    ```
    ```
  - Save and run the project
