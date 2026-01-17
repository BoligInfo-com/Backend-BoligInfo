# BoligInfo API

BoligInfo is an **ASP.NET Core Minimal API** backend for managing financial data related to equity, loans, and cash. The project provides RESTful endpoints for creating, reading, updating, and deleting financial entities. The API uses **PostgreSQL** as its database and leverages **Entity Framework Core** for ORM.  

---

## Features

- CRUD operations for:
  - **Loans**
  - **Equities**
  - **Cash**
- One-to-one and one-to-many relationships enforced in the database
- Validation on creation and updates
- Logging for error handling
- Swagger/OpenAPI documentation fully integrated
- XML documentation for enhanced API descriptions

---

## Technologies

- **Backend:** ASP.NET Core 9.0 Minimal API  
- **Database:** PostgreSQL (via Npgsql)  
- **ORM:** Entity Framework Core  
- **Logging:** Microsoft.Extensions.Logging  
- **API Documentation:** Swagger / Swashbuckle  

---

## Project Structure


## Project Structure
```
BoligInfo/
├── src/
| ├─ BoligInfo.Api # API controllers and Program.cs
| ├─ BoligInfo.Core # DTOs, Models, Enums
| ├─ BoligInfo.Database # DbContext and Entity configurations
| ├─ BoligInfo.Repositories # Repositories for Loan, Equity, Cash
| ├─ BoligInfo.Services # Services implementing business logic
├── test/
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- Optional: Visual Studio Code or Visual Studio
### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/BoligInfo.git
   cd BoligInfo/src/BoligInfo.Api
   ```
   
2. Configure PostgreSQL connection string in appsettings.json:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=DBNAME;Username=YOUR_USER_NAME;Password=YOUR_PASSWORD"
    }
  }
  ```
  
3. Apply database migrations:
  ```bash
  dotnet ef database update
  ```

4. Run the API:
  ```bash
  dotnet run
  ```
  
5. Access Swagger UI:
  ```bash
 https://localhost:YOURPORT/swagger
  ```

---

# Usage

- **Loans**
  - GET /api/loans
  - GET /api/loans/{id}
  - POST /api/loans
  - PUT /api/loans/{id}
  - DELETE /api/loans/{id}

- **Equities**
  - GET /api/equities
  - GET /api/equities/{id}
  - GET /api/equities/{id}/with-loans
  - POST /api/equities
  - PUT /api/equities/{id}
  - DELETE /api/equities/{id}

- **Cash**
  - GET /api/allcash
  - GET /api/allcash/{id}
  - GET /api/allcash/equity/{equityId}
  - POST /api/allcash
  - PUT /api/allcash/{id}
  - DELETE /api/allcash/{id}

---

# Logging
- Errors in Service classes are logged using ILogger.
- You can extend logging for other services or controllers as needed

---

# Notes
- The API uses XML documentation comments (`<summary>, <param>, <returns>`) to enhance Swagger UI.
- One-to-one relationships (Equity → Cash) and one-to-many relationships (Equity → Loans) are enforced both in C# and PostgreSQL.

---

# Contributing
1. Fork the repositor
2. Create a branch for your feature (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m 'Add feature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

---
