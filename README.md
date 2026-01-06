# BoligInfo

A house investment management API built with ASP.NET Core for tracking property equity and associated loans.

## Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Architecture**: Clean Architecture (API → Services → Repositories → Database)

## Project Structure
```
BoligInfo/
├── src/
│   ├── BoligInfo.Core/              # Domain models, DTOs, enums
│   ├── BoligInfo.Infrastructure/
│   │   ├── BoligInfo.Database/      # DbContext, EF configurations
│   │   ├── BoligInfo.Repositories/  # Data access layer
│   │   │   ├── BoligInfo.LoanRepository/
│   │   │   └── BoligInfo.EquityRepository/
│   │   └── BoligInfo.Services/      # Business logic
│   └── BoligInfo.Api/               # REST API endpoints
```

## Features

- **Equity Management**: Track property investments in multiple currencies
- **Loan Management**: Manage various loan types (Fixed, Adjustable, F1-F5)
- **RESTful API**: Full CRUD operations for equities and loans
- **Foreign Key Relations**: Loans linked to equity with cascade delete

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL

## API Endpoints

### Equities
- `GET /api/equities` - Get all equities
- `GET /api/equities/{id}` - Get equity by ID
- `GET /api/equities/{id}/with-loans` - Get equity with associated loans
- `POST /api/equities` - Create new equity
- `PUT /api/equities/{id}` - Update equity
- `DELETE /api/equities/{id}` - Delete equity

### Loans
- `GET /api/loans` - Get all loans
- `GET /api/loans/{id}` - Get loan by ID
- `GET /api/loans/equity/{equityId}` - Get loans by equity
- `POST /api/loans` - Create new loan
- `PUT /api/loans/{id}` - Update loan
- `DELETE /api/loans/{id}` - Delete loan

## Testing

HTTP test files available in `src/BoligInfo.Api/HttpTests/`:
- `loans.http` - Loan endpoint tests
- `equities.http` - Equity endpoint tests

Use REST Client extension in VS Code or run directly in Visual Studio/Rider.
