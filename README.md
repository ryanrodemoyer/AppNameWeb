# AppNameWeb

## Your next webapp
* C# / .NET Core / ASP.NET Core 2.1
* GraphQL + GraphQL Playground
* Active Directory Domain Authentication + Cookies

### Usage
1. Clone the app.
1. Build Solution via Visual Studio 2017.
1. Play.
1. Logins:  
   * domainuser/*anything* for full access (including GraphQL playground).
   * diffuser/*anything* for limited access (does not include GraphQL Playground)

### Current Limitations
* All repository data is currently in-memory. In other words, there's currently no auxillary data store configured in the app.
* Domain authentication needs more testing. Currently no good environment to test against.
* GraphQL route protection could use some improvement but for now it works.
* Code documentation needs added.