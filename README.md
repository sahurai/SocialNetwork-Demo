# Social Network API Documentation

This project provides a well-structured API for a social network application, leveraging clean architecture principles with ASP.NET Core, Entity Framework, and PostgreSQL. Key features include user authorization, friendships, post interactions, group management, and more. The project also employs a **cookie-based 2-token authorization** mechanism for enhanced security and token management.

---

## Requirements

Before starting, ensure you have the following installed:
- [.NET SDK 6.0 or later](https://dotnet.microsoft.com/download/dotnet/6.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- A compatible IDE, such as Visual Studio 2022 or Visual Studio Code

## Installation

1. **Clone the Project**

   Clone the repository using Git:
   ```bash
   git clone https://github.com/sahurai/SocialNetwork-Demo-API.git
   cd social-network-api
   ```

2. **Set Up PostgreSQL Database**

   1. Make sure PostgreSQL is running.
   2. Create a new database for the application:
      ```sql
      CREATE DATABASE SocialNetworkDB;
      ```
   3. In `appsettings.json`, update the connection string to match your database configuration:
      ```json
      {
        "Logging": {
          "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
          }
        },
        "Jwt": {
          "Key": "your_key",
          "Issuer": "localhost",
          "Audience": "localhost"
        },
        "ConnectionStrings": {
          "SocialNetworkDbContext": "Host=localhost; Database=SocialNetworkDB; Username=your_username; Password=your_password"
        }
      }
      ```

3. **Install Dependencies**

   Run the following command to restore all necessary dependencies:
   ```bash
   dotnet restore
   ```

4. **Run Database Migrations**

   This project uses Entity Framework Core for database migrations. Run the migrations with:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Start the Application**

   Run the application with:
   ```bash
   dotnet run
   ```
   The API will be available at `http://localhost:5000` or `https://localhost:5001`.

6. **Access API Documentation**

   Swagger UI is enabled in this project for easy testing and documentation. Access it at:
   ```
   https://localhost:5000
   or
   https://localhost:5001
   ```

---

## Authorization - Cookie-Based 2-Token System

This API implements a secure **cookie-based 2-token authorization** system that combines access and refresh tokens:

1. **Access Token**: Used for user authentication in short-lived sessions, embedded in the cookies for frontend authorization.
2. **Refresh Token**: Long-lived token stored securely in HttpOnly cookies. When the access token expires, the refresh token can request a new access token without requiring the user to re-authenticate.

To enable this, ensure you have the JWT configurations set up correctly in `appsettings.json`, particularly for the `"Jwt"` settings, and configure middleware in the `Startup.cs` or `Program.cs` file to manage token validation and cookie settings.

---

## Project Features

1. **User Authorization**: Supports user registration, login, and token management.
2. **Friendship Management**: Users can send and receive friend requests.
3. **Posts, Likes, and Comments**: Users can create posts, comment, and like them.
4. **Group Creation and Role Management**: Users can create groups with role-based permissions.
5. **Blacklisting**: Both users and groups have the ability to block others.
6. **Private Messaging**: Users can send, edit, delete messages, mark messages as read, and remove conversations entirely.

## Role-Based Endpoints

This API supports role-based access control, with distinct endpoints and Swagger schemas for user and admin roles to ensure proper access and functionality based on role privileges.

---

The project utilizes Dependency Injection (DI) for dependency inversion, resulting in modular, testable code.
