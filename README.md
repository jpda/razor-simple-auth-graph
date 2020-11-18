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
- an Azure AD directory (tenant)