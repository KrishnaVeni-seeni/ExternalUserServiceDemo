# External User Service Demo

This project has a .NET service component that integrates with a public API (`https://reqres.in`) to fetch and handle user information.

---

##Project Structure

| Project                   | Description                                  |
|---------------------------|----------------------------------------------|
| `ExternalUserService`     | Core class library with client/service/models|
| `ExternalUserService.Tests` | Unit test project                          |
| `Main_Console`            | Simple console app demo                      |

--------------------------------------------------------- Design Explanation  --------------------------------------------------------------
1) Separate models, services, and exceptions, so each piece can have a single responsibility.
2) Used HttpClient and IOptions<UserServiceOptions> via constructor — this makes the service easily testable and DI-ready.
3) Added custom exceptions for clean error handling (ExternalApiException, NetworkException).
4) Simple in-memory caching added to simulate real scenarios.
--------------------------------------------------------------------------------------------------------------------------------------------

## How to Build and Run

```bash
git clone https://github.com/KrishnaVeni-seeni/ExternalUserServiceDemo.git
cd ExternalUserServiceDemo
API_project is my class library which interacts with the API and gets user information.
I have created API_pr

# Restore & Build
dotnet restore
dotnet build

# Run Console App
cd API_Console
dotnet run
I have added a getting a input to decide whether to show all users or only specific users based on userids.

# Additional MVC Application
cd API_MVC
dotnet run
This is a MVC web application with a frontend to show the values in a website

# Run Unit Tests
cd ..
dotnet test


