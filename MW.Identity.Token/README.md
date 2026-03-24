# MW.Identity.Token

A lightweight .NET 8 library for extracting and managing user identity information from JWT claims in ASP.NET Core applications. It provides a clean `ICurrentUser` abstraction, ready-to-use `ClaimsPrincipal` extension methods, and predefined constants for claim types and system roles.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [API Reference](#api-reference)
  - [ServiceRegistration](#serviceregistration)
  - [ICurrentUser / CurrentUser](#icurrentuser--currentuser)
  - [ClaimsPrincipalExtensions](#claimsprincipalextensions)
  - [ClaimConstants](#claimconstants)
  - [SystemRole](#systemrole)
- [Usage Examples](#usage-examples)
- [Dependencies](#dependencies)
- [License](#license)

## Installation

Add a project reference to `MW.Identity.Token`:

```bash
dotnet add reference path/to/MW.Identity.Token/MW.Identity.Token.csproj
```

## Quick Start

### 1. Register Services

In your `Program.cs` (or `Startup.cs`), call `AddUserTokenManager()`:

```csharp
using MW.Identity.Token.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUserTokenManager();
```

This registers `IHttpContextAccessor` and `ICurrentUser` (as a scoped service) in the DI container.

### 2. Inject and Use `ICurrentUser`

```csharp
using MW.Identity.Token.Contracts;

public class OrderService
{
    private readonly ICurrentUser _currentUser;

    public OrderService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public void CreateOrder()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var userId = _currentUser.UserId;
        var email = _currentUser.Email;
        var roles = _currentUser.Roles;

        // ... create order logic
    }
}
```

## API Reference

### ServiceRegistration

**Namespace:** `MW.Identity.Token.DependencyInjection`

| Method | Description |
|---|---|
| `AddUserTokenManager(this IServiceCollection)` | Registers `IHttpContextAccessor` and `ICurrentUser` (scoped) in the DI container. |

---

### ICurrentUser / CurrentUser

**Namespace:** `MW.Identity.Token.Contracts` / `MW.Identity.Token.Services`

The `ICurrentUser` interface provides access to the current authenticated user's identity information. The `CurrentUser` class implements this interface by reading claims from the HTTP context.

#### ICurrentUser Properties

| Property | Type | Description |
|---|---|---|
| `IsAuthenticated` | `bool` | Whether the current user is authenticated. |
| `ClaimsPrincipal` | `ClaimsPrincipal?` | The claims principal, or `null` if not authenticated. |
| `UserId` | `string?` | The user's unique identifier from claims. |
| `Email` | `string?` | The user's email address from claims. |
| `Roles` | `IList<string>` | List of roles assigned to the user. |

#### ICurrentUser Methods

| Method | Return Type | Description |
|---|---|---|
| `IsInRole(string role)` | `bool` | Checks if the user belongs to the specified role. |

#### CurrentUser Additional Members

The `CurrentUser` service class provides extra methods beyond the `ICurrentUser` interface:

| Member | Type | Description |
|---|---|---|
| `IsSuperAdmin` | `bool` | Whether the current user has the `SuperAdmin` role. |
| `Get(string type)` | `string?` | Gets a claim value by its type. |
| `GetJson<T>(string type)` | `T?` | Gets a claim value deserialized from JSON. |
| `HasClaimType(string type)` | `bool` | Checks whether the specified claim type exists. |
| `CheckUserAuthorize(string userId)` | `bool` | Returns `true` if the user is a SuperAdmin or the `userId` matches the current user. |
| `GetHeaderValue(string key)` | `string?` | Gets an HTTP request header value by key. |

---

### ClaimsPrincipalExtensions

**Namespace:** `MW.Identity.Token.Extensions`

Extension methods for `ClaimsPrincipal` to simplify access to common JWT claims and role checks.

#### Claim Accessors

| Method | Return Type | Description |
|---|---|---|
| `Get(string type)` | `string?` | Gets a claim value by type. |
| `GetUserId()` | `string` | Gets the user ID. Throws `UnauthorizedAccessException` if missing. |
| `GetUserName()` | `string?` | Gets the username. |
| `GetUserEmail()` | `string?` | Gets the email address. |
| `GetUserDisplayName()` | `string?` | Gets the display name. |
| `GetUserPhone()` | `string?` | Gets the phone number. |
| `GetUserCreatedDate()` | `string?` | Gets the account creation date. |
| `GetTelegramChatId()` | `string?` | Gets the Telegram chat ID. |
| `GetSystemId()` | `string?` | Gets the system ID. |
| `GetExpiration()` | `string?` | Gets the token expiration value. |
| `GetShopPointId()` | `string?` | Gets the shop point ID. |
| `GetShopPointName()` | `string?` | Gets the shop point name. |
| `GetIsTemporaryPassword()` | `string?` | Gets the temporary password flag. |
| `GetCreatedMarketPlaceUserId()` | `string?` | Gets the marketplace user ID of the creator. |
| `GetName()` | `string?` | Gets the user's name. |

#### Role Checks

| Method | Return Type | Description |
|---|---|---|
| `IsSuperAdmin()` | `bool` | Checks for the `SuperAdmin` role. |
| `IsProductAdmin()` | `bool` | Checks for the `ProductAdmin` role. |
| `IsMarketPlaceAdmin()` | `bool` | Checks for the `MarketPlaceAdmin` role. |
| `IsPublicShopAdmin()` | `bool` | Checks for the `PublicShopAdmin` role. |
| `IsExternalSystem()` | `bool` | Checks for the `ExternalSystem` role. |
| `IsUser()` | `bool` | Checks for the `User` role. |
| `IsAdminPanelUser()` | `bool` | Checks for `SuperAdmin` **or** `ProductAdmin` role. |

---

### ClaimConstants

**Namespace:** `MW.Identity.Token.Constants`

String constants for JWT claim type names.

| Constant | Value | Description |
|---|---|---|
| `UserId` | `"userId"` | User's unique identifier |
| `UserName` | `"userName"` | Username |
| `DisplayName` | `"displayName"` | Display name |
| `Phone` | `"phone"` | Phone number |
| `Name` | `"name"` | User's name |
| `Email` | `"email"` | Email address |
| `Expiration` | `"expiration"` | Token expiration |
| `SystemId` | `"systemId"` | System identifier |
| `Role` | `"role"` | Single role |
| `Roles` | `"roles"` | Roles collection |
| `TelegramChatId` | `"telegramChatId"` | Telegram chat identifier |
| `CreatedDate` | `"createdDate"` | Account creation date |
| `ShopPointId` | `"shopPointId"` | Shop point identifier |
| `ShopPointName` | `"shopPointName"` | Shop point name |
| `IsTemporaryPassword` | `"isTemporaryPassword"` | Temporary password flag |
| `CreatedMarketPlaceUserId` | `"createdMarketPlaceUserId"` | Marketplace creator user ID |

---

### SystemRole

**Namespace:** `MW.Identity.Token.Constants`

String constants for system role names used in authorization.

| Constant | Value | Description |
|---|---|---|
| `SuperAdmin` | `"SuperAdmin"` | Full system access |
| `User` | `"User"` | Standard user |
| `ExternalSystem` | `"ExternalSystem"` | Service-to-service communication |
| `ProductAdmin` | `"ProductAdmin"` | Product management |
| `MarketPlaceAdmin` | `"MarketPlaceAdmin"` | Marketplace administration |
| `PublicShopAdmin` | `"PublicShopAdmin"` | Public shop administration |

## Usage Examples

### Role-Based Authorization in a Controller

```csharp
using MW.Identity.Token.Contracts;
using MW.Identity.Token.Constants;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public ProductsController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_currentUser.IsInRole(SystemRole.SuperAdmin))
            return Forbid();

        // ... delete product
        return NoContent();
    }
}
```

### Using ClaimsPrincipal Extensions in Middleware

```csharp
using MW.Identity.Token.Extensions;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetUserId();
            var email = context.User.GetUserEmail();
            var isSuperAdmin = context.User.IsSuperAdmin();

            // ... log audit info
        }

        await _next(context);
    }
}
```

### Deserializing JSON Claims

> **Note:** `GetJson<T>` is available on the concrete `CurrentUser` class, not on the `ICurrentUser` interface.

```csharp
// Inject CurrentUser directly when you need methods beyond ICurrentUser
var currentUser = serviceProvider.GetRequiredService<ICurrentUser>() as CurrentUser;

var permissions = currentUser?.GetJson<List<string>>("permissions");
```

### Checking User Authorization

> **Note:** `CheckUserAuthorize` is available on the concrete `CurrentUser` class, not on the `ICurrentUser` interface.

```csharp
public class UserService
{
    private readonly CurrentUser _currentUser;

    // Inject the concrete CurrentUser when you need CheckUserAuthorize
    public UserService(ICurrentUser currentUser)
    {
        _currentUser = (CurrentUser)currentUser;
    }

    public UserProfile GetProfile(string userId)
    {
        // Returns true if the user is a SuperAdmin or if userId matches the current user
        if (!_currentUser.CheckUserAuthorize(userId))
            throw new UnauthorizedAccessException();

        // ... fetch profile
    }
}
```

## Project Structure

```
MW.Identity.Token/
├── Constants/
│   ├── ClaimConstants.cs          # JWT claim type name constants
│   └── SystemRole.cs              # System role name constants
├── Contracts/
│   └── ICurrentUser.cs            # Current user abstraction interface
├── DependencyInjection/
│   └── ServiceRegistration.cs     # DI extension method
├── Extensions/
│   └── ClaimsPrincipalExtensions.cs  # ClaimsPrincipal extension methods
└── Services/
    └── CurrentUser.cs             # ICurrentUser implementation
```

## Dependencies

- [.NET 8.0](https://dotnet.microsoft.com/)
- [Microsoft.AspNetCore.App](https://www.nuget.org/packages/Microsoft.AspNetCore.App) (framework reference)
- [Newtonsoft.Json 13.0.3](https://www.nuget.org/packages/Newtonsoft.Json/) (for JSON claim deserialization)

## License

This project is open source. See the repository for license details.