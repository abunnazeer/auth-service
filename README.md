
# AuthService – JWT Authentication API (.NET 10)

A production-style authentication service built with **ASP.NET Core**, **JWT**, **PostgreSQL**, and **Clean Architecture** principles.

This service provides secure user registration, login, and authenticated user profile retrieval using **stateless JWT authentication**.



# Features

* User Registration
* Secure Password Hashing (BCrypt)
* User Login with JWT Token Issuance
* JWT Authentication Middleware
* Protected API Endpoints
* PostgreSQL Database with Entity Framework Core
* Database Migrations
* Clean Architecture (Domain / Application / Infrastructure / API)



# Architecture

The project follows a **layered architecture** separating concerns.

```
AuthService
│
├── AuthService.Domain
│   ├── Entities
│   └── Enums
│
├── AuthService.Application
│   ├── Interfaces
│   └── Services
│
├── AuthService.Infrastructure
│   ├── Auth
│   ├── Configuration
│   ├── Database
│   ├── Persistence
│   └── Security
│
└── AuthService.API
    ├── Contracts
    └── Program.cs
```

### Layer Responsibilities

**Domain**

* Core entities
* Business rules
* Enums and models

**Application**

* Interfaces
* Use cases
* Authentication service logic

**Infrastructure**

* JWT generation
* Database access
* Password hashing
* Repository implementations

**API**

* HTTP endpoints
* Request contracts
* Dependency injection
* Authentication middleware



# Tech Stack

* **.NET 10**
* **ASP.NET Core Minimal APIs**
* **PostgreSQL**
* **Entity Framework Core**
* **JWT (System.IdentityModel.Tokens.Jwt)**
* **BCrypt.Net**



# Database

PostgreSQL is used for persistent storage.

Example table created via migration:

```
users
```

Fields include:

* Id
* FirstName
* LastName
* Email
* PasswordHash
* Role
* IsActive
* CreatedAtUtc



# Setup

## 1. Clone the repository

```bash
git clone https://github.com/abunnazeer/auth-service.git
cd AuthService
```



## 2. Configure PostgreSQL

Create database:

```sql
CREATE DATABASE auth_service_db;
```



## 3. Update connection string

Edit:

```
AuthService.API/appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=auth_service_db;Username=postgres;Password=postgres"
}
```



## 4. Apply database migrations

```bash
dotnet ef database update \
--project AuthService.Infrastructure \
--startup-project AuthService.API
```



## 5. Run the API

```bash
cd AuthService.API
dotnet run
```

API will start on:

```
http://localhost:5219
```



# API Endpoints

## Register User

```
POST /auth/register
```

Example request:

```json
{
  "firstName": "Abdullahi",
  "lastName": "Ahmad",
  "email": "test@example.com",
  "password": "Password123!",
  "role": 1
}
```



## Login

```
POST /auth/login
```

Request:

```json
{
  "email": "test@example.com",
  "password": "Password123!"
}
```

Response:

```json
{
  "accessToken": "JWT_TOKEN"
}
```



## Get Current User

```
GET /me
```

Requires Authorization header:

```
Authorization: Bearer JWT_TOKEN
```

Example response:

```json
{
  "id": "680e3b79-474d-4eeb-970a-c6585270fb12",
  "email": "dbtest@example.com",
  "role": "User"
}
```



# Security

This project implements:

* BCrypt password hashing
* JWT signing using HMAC SHA256
* Token expiration
* Issuer and audience validation
* Protected endpoints with `RequireAuthorization()`



# Example JWT Claims

```
sub
email
given_name
surname
role
exp
iss
aud
```



# Future Improvements

* Refresh tokens
* Role-based authorization policies
* Email verification
* Password reset
* Rate limiting
* Docker deployment
* OpenAPI/Swagger security integration
* Redis caching
* OAuth providers (Google, GitHub)



# Author

**Abdullahi Ahmad**

Software engineer focused on backend systems, distributed authentication, and scalable application architecture.



