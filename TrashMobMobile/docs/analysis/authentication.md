# Authentication

## Identity provider

- Uses MSAL (`Microsoft.Identity.Client`) for Entra External ID (CIAM).
- `AuthConstants` defines tenant, client ID, scopes, and redirect URI.
- `USETEST` toggles between dev and prod tenants and API base URLs.

## Auth flow

- `AuthService.SignInAsync()` attempts a silent sign-in first.
- If silent sign-in fails with `MsalUiRequiredException`, it falls back to interactive sign-in.
- On success, `SetAuthenticated()` stores token data and resolves the user profile via `IUserManager`.

## User context

- `UserState.UserContext` holds the access token and email address.
- `App.CurrentUser` is set after successful authentication.

## HTTP authorization

- `BaseAddressAuthorizationMessageHandler` injects the bearer token from `UserState` into REST calls.
- `AuthHandler` is an alternate delegating handler that uses `IAuthService.GetAccessTokenAsync()`.

## Sign-out

- `AuthService.SignOutAsync()` clears MSAL accounts and sets `App.CurrentUser = null`.
