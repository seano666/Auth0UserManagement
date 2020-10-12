# Auth0UserManagement

Auth0UserManagement is a .Net Core wrapper library for the Auth0 User Management API V2.  
This implementation retreives the API token automatically, and then allows the user to 
Search, Insert, Update, and Delete users from an Auth0 connection.

## Installation

Open the Solution, enter your Auth0 connection details in the test project config file.

```bash
testhost.dll.config
```

Then run the NUnit tests to verify your configs are accurate.

## Usage
Build the solution, then reference the Auth0UserManagement Project or DLL

```C#
using Auth0UserManagement;
using Auth0UserManagement.Models;

var config = new Auth0UserManagementConfig(
    "mybasedomain",
    "myconnectionname",
    "myconnectionid",
    "clientid",
    "myclientsecret"
);

var mgr = new Auth0UserManager(config);

List<User> users = mgr.SearchUsers($"email:{ randomEmail }");
```