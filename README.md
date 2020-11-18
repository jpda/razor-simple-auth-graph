# razor pages auth and graph
This app is a simple example of using the Microsoft identity platform for:
- adding Azure AD organizational sign-in to a Razor Pages app
- getting and requesting an `access_token` to call the Microsoft Graph API to get user data, and
- *incremental consent*, a concept of asking for additional permissions when needed, instead of upfront
- retrieving an email message list from graph