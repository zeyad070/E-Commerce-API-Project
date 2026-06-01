# ASP.NET Core 3-Layer E-Commerce API

A production-grade RESTful Web API for an e-commerce platform built with **.NET 10**, following a Clean 3-Layer Architecture with the Repository and Unit of Work patterns. Features JWT-authenticated role-based authorization, full CRUD operations for products and categories, a shopping cart system, order processing with transactional integrity, and image upload support.

---

## Features

- **JWT Authentication & Role-Based Authorization** — Register/Login with token-based auth. Two roles: `Admin` and `User` (Customer).
- **Product Management** — Full CRUD for products with pagination, filtering by category, and search by name.
- **Category Management** — Full CRUD for categories with product count tracking.
- **Shopping Cart** — Authenticated users can add, update, and remove items from their cart.
- **Order Processing** — Create orders from cart contents with stock validation and database transactions.
- **Image Upload** — Admin upload and update images for products and categories with file type and size validation.
- **FluentValidation** — Request validation with detailed error messages.
- **Generic Repository + Unit of Work** — Clean data access abstraction with transactional support.
- **Auto-Audit Fields** — All entities automatically track `CreatedAt` and `UpdatedAt`.
- **API Documentation** — Interactive API reference via Scalar UI.
- **Pagination** — Consistent paginated responses for list endpoints.
- **Seed Data** — Pre-loaded categories and products for immediate testing.

---

## Technologies Used

| Technology | Version |
|---|---|
| [.NET](https://dotnet.microsoft.com/) | 10.0 |
| [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api) | 10.0 |
| [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) | 10.0 |
| [SQL Server](https://www.microsoft.com/en-us/sql-server/) | — |
| [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) | 10.0 |
| [JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt) | 10.0 |
| [FluentValidation](https://fluentvalidation.net/) | 12.1 |
| [Scalar.AspNetCore](https://scalar.com/) | 2.14 |
| [OpenAPI](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi) | Built-in |

---

## Project Architecture

The solution follows a strict **3-Layer Architecture** with a shared common layer, ensuring separation of concerns, testability, and maintainability.

```
ProductSystem.APIs           ─ Presentation Layer (Controllers, Middleware)
      │
ProductApp.BLL              ─ Business Logic Layer (Managers, DTOs, Validators)
      │
ProductApp.DAL              ─ Data Access Layer (DbContext, Repositories, Unit of Work, Entities)
      │
ProductApp.Common           ─ Shared Layer (Utilities, Enums, Settings Models)
```

| Layer | Responsibility |
|---|---|
| **ProductSystem.APIs** | HTTP request/response handling, authentication, authorization, static files, CORS. |
| **ProductApp.BLL** | Business logic, DTOs, validation rules (FluentValidation), JWT token generation. |
| **ProductApp.DAL** | Database context, entity models, generic & specific repositories, Unit of Work, EF Core migrations. |
| **ProductApp.Common** | Shared utility classes (`ApiResponse<T>`, `PaginationResponse<T>`), enums (`OrderStatus`), settings models (`JwtSettings`, `ImageSettings`, `AdminSettings`). |

### Key Design Patterns

- **Repository Pattern** — Generic `IGenericRepository<T>` with specific repositories (`IProductRepositories`, `ICategoryRepositories`, etc.) for data access abstraction.
- **Unit of Work Pattern** — `IUnitOfWork` aggregates all repositories and provides transactional support via `IDbContextTransaction`.
- **Dependency Injection** — All services, managers, and repositories are registered via built-in DI container.
- **DTO Pattern** — Clean separation between API contracts and domain models. Auto-mapping via manual mapping in managers.
- **Fluent Validation** — Validation logic separated from controllers into dedicated validator classes.

---

## Installation & Setup

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Developer, or Express edition)
- An IDE (Visual Studio 2022, JetBrains Rider, or VS Code)

### Clone the Repository

```bash
git clone https://github.com/yourusername/your-repo-name.git
cd your-repo-name
```

### Configure the Database

1. Open `ProductSystem.APIs/appsettings.json` and update the `ConnectionStrings.ProductSystem` value to point to your SQL Server instance:

```json
"ConnectionStrings": {
  "ProductSystem": "Server=.\\sqlexpress;Database=ASPNETCoreE-Commerce;Trusted_Connection=true;TrustServerCertificate=true"
}
```

2. Apply the EF Core migrations to create the database schema:

```bash
dotnet ef database update --project ProductApp.DAL --startup-project ProductSystem.APIs
```

> The migration will create all tables (Categories, Products, Carts, CartItems, Orders, OrderItems, and ASP.NET Core Identity tables) and seed initial data.

### Configure JWT & Settings

The default `appsettings.json` includes development-only values. For production, replace the following:

```json
"Jwt": {
  "Key": "YourSuperSecretKey_MustBe32CharsLong!!",
  "Issuer": "ProductSystemAPI",
  "Audience": "ProductSystemClient"
},
"AdminSettings": {
  "SecretCode": "Admin@Secret123"
}
```

> **Security Note:** Never commit real secrets to version control. Use environment variables, Azure Key Vault, or User Secrets for sensitive values.

### Run the Application

```bash
dotnet run --project ProductSystem.APIs
```

The API launches at:
- **HTTP:** `http://localhost:5032`
- **HTTPS:** `https://localhost:7232`

Browse to `https://localhost:7232/scalar` to explore the interactive API documentation (development mode only).

---

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Auth | Role | Description |
|---|---|---|---|---|
| POST | `/api/auth/register` | ❌ | — | Register a new user. Supply `AdminCode` to register as Admin. |
| POST | `/api/auth/login` | ❌ | — | Authenticate and receive a JWT token. |

### Products (`/api/product`)

| Method | Endpoint | Auth | Role | Description |
|---|---|---|---|---|
| GET | `/api/product` | ❌ | — | List products. Supports `categoryId`, `name`, `pageNumber`, `pageSize` query params. |
| GET | `/api/product/{id}` | ❌ | — | Get a single product by ID. |
| POST | `/api/product` | ✅ | Admin | Create a new product. |
| PUT | `/api/product/{id}` | ✅ | Admin | Update an existing product. |
| DELETE | `/api/product/{id}` | ✅ | Admin | Delete a product. |

### Categories (`/api/category`)

| Method | Endpoint | Auth | Role | Description |
|---|---|---|---|---|
| GET | `/api/category` | ❌ | — | List all categories with product counts. |
| GET | `/api/category/{id}` | ❌ | — | Get a single category by ID. |
| POST | `/api/category` | ✅ | Admin | Create a new category. |
| PUT | `/api/category/{id}` | ✅ | Admin | Update a category. |
| DELETE | `/api/category/{id}` | ✅ | Admin | Delete a category. |

### Shopping Cart (`/api/cart`)

| Method | Endpoint | Auth | Role | Description |
|---|---|---|---|---|
| GET | `/api/cart` | ✅ | User | Retrieve the current user's cart with items. |
| POST | `/api/cart` | ✅ | User | Add an item to the cart. |
| PUT | `/api/cart` | ✅ | User | Update item quantity in the cart. |
| DELETE | `/api/cart/{productId}` | ✅ | User | Remove an item from the cart. |

### Orders (`/api/orders`)

| Method | Endpoint | Auth | Role | Description |
|---|---|---|---|---|
| POST | `/api/orders` | ✅ | User | Create an order from the current cart (transactional). |
| GET | `/api/orders` | ✅ | User | List the current user's orders. |
| GET | `/api/orders/{id}` | ✅ | User | Get detailed order information. |

### Images (`/api/image`, `/api/products`, `/api/categories`)

| Method | Endpoint | Auth | Role | Description |
|---|---|---|---|---|
| POST | `/api/image/upload` | ✅ | Admin | Upload a general image. |
| POST | `/api/products/{id}/image` | ✅ | Admin | Upload or update a product image. |
| POST | `/api/categories/{id}/image` | ✅ | Admin | Upload or update a category image. |

### Response Format

All responses follow a consistent envelope:

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { },
  "errors": null
}
```

Paginated endpoints additionally include pagination metadata.

---

## Database

**Provider:** SQL Server (via `Microsoft.EntityFrameworkCore.SqlServer`)
**Database Name:** `ASPNETCoreE-Commerce`

### Entity-Relationship Summary

```
Category ──1:N──> Product
User ──1:N──> Cart ──1:N──> CartItem ──N:1──> Product
User ──1:N──> Order ──1:N──> OrderItem ──N:1──> Product
```

| Entity | Description |
|---|---|
| `Category` | Product categories (e.g., Laptops, Accessories). |
| `Product` | Sellable items with price, stock, and category association. |
| `Cart` | Per-user shopping cart (one cart per user). |
| `CartItem` | Individual line items within a cart. |
| `Order` | Completed purchase with status tracking. |
| `OrderItem` | Line items within an order (snapshot of product at purchase time). |
| `AspNetUsers` | ASP.NET Core Identity users (extensible). |
| `AspNetRoles` | Role definitions (`Admin`, `User`). |

### Seed Data

The application seeds the database on first run with:
- **4 Categories:** Laptops, PCs, Phones & Tablets, Accessories
- **6 Products:** Laptop, Smartphone, Tablet, Headphones, Smartwatch, PC (each assigned to a category)

---

## Screenshots

> Screenshots will be added here as the project evolves. Consider including images of your Scalar API documentation page, a sample API response in Postman, and the database diagram.

---

## Future Improvements

- **Refresh Token support** — Improve authentication security with refresh token rotation.
- **Email confirmation & password reset** — Complete the ASP.NET Core Identity workflow.
- **Product search with full-text indexing** — Better search performance for large catalogs.
- **Order status management** — Admin endpoints to update order status (Processing, Shipped, Delivered).
- **Payment gateway integration** — Stripe or PayPal integration for real payment processing.
- **Caching layer** — Distributed caching (Redis) for frequently accessed product/category data.
- **API rate limiting** — Protect endpoints from abuse.
- **Health checks & monitoring** — Endpoint health monitoring with structured logging.
- **Unit & integration tests** — Comprehensive test coverage for managers and controllers.
- **Docker support** — Containerized deployment with `docker-compose` (API + SQL Server).

---

## Author

**Senior .NET Developer** — Built with a focus on clean architecture, separation of concerns, and industry best practices.

[![LinkedIn](https://img.shields.io/badge/-LinkedIn-blue?style=flat-square&logo=linkedin)](https://www.linkedin.com/in/zeyad-rabeea-901244241/)
[![GitHub](https://img.shields.io/badge/-GitHub-gray?style=flat-square&logo=github)](https://github.com/zeyad070)

---

> **License:** This project is for demonstration and portfolio purposes.
