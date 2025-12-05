# TrashMob.Shared.Tests

## Overview

The `TrashMob.Shared.Tests` project contains automated tests for the shared business logic and utilities in the TrashMob platform. Its purpose is to ensure the reliability and correctness of the code in the `TrashMob.Shared` project by validating core features, integrations, and helper methods.

## Key Contents

- **Unit Tests**: Verifies the behavior of managers, services, and utility classes.
- **Mocking**: Uses Moq to simulate dependencies and isolate test scenarios.
- **Test Framework**: Built with xUnit for test organization and execution.
- **Code Coverage**: Integrated with Coverlet for measuring test coverage.

## Technologies

- .NET 9.0
- xUnit (test framework)
- Moq (mocking library)
- Coverlet (code coverage)
- Microsoft.NET.Test.Sdk (test runner)

## Usage

1. Restore dependencies:
    ```sh
    dotnet restore
    ```
2. Run all tests:
    ```sh
    dotnet test
    ```
3. View code coverage (optional):
    ```sh
    dotnet test --collect:"XPlat Code Coverage"
    ```

## Helpful Links

- [TrashMob.Shared README](../TrashMob.Shared/README.md)
- [Mobile App User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/MobileAppUserStories.md)
- [Website User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/WebsiteUserStories.md)

---

*For questions or contributions, please refer to the main repository [TrashMob-eco/TrashMob](https://github.com/TrashMob-eco/TrashMob).*