# VetLink API

A comprehensive e-commerce RESTful API for veterinary products built with .NET 10.0. VetLink provides a complete platform for managing veterinary product sales, orders, reviews, and user interactions with support for multiple user roles (Admin, Buyer, Seller).

## 🚀 Features

### Core Functionality
- **User Management**: Multi-role authentication system (Admin, Buyer, Seller)
- **Product Management**: Full CRUD operations for products with image support
- **Order Management**: Complete order processing with shipment tracking
- **Review System**: Product reviews and ratings
- **Brand & Category Management**: Organized product catalog
- **Coupon System**: Discount codes and promotional offers
- **Wishlist**: Save favorite products for later
- **Real-time Notifications**: SignalR-based notification system
- **Messaging**: Real-time chat functionality
- **Image Storage**: Cloudinary integration for product images
- **Email Service**: Transactional emails for orders and notifications
- **Audit Logging**: Track system changes and user activities

### Technical Features
- **JWT Authentication**: Secure token-based authentication
- **API Versioning**: RESTful API with versioning support
- **Rate Limiting**: Protection against API abuse
- **Caching**: Redis-based caching for improved performance
- **Swagger Documentation**: Interactive API documentation
- **Exception Handling**: Centralized error handling middleware
- **Logging**: Serilog integration for structured logging
- **Validation**: FluentValidation for request validation

## 🏗️ Architecture

The project follows Clean Architecture principles with clear separation of concerns:

```
VetLink-API/
├── VetLink.Data/              # Data layer
│   ├── Entities/              # Domain entities
│   ├── Contexts/              # DbContext and factory
│   ├── Configurations/        # Entity Framework configurations
│   └── Migrations/            # Database migrations
│
├── VetLink.Repository/        # Repository layer
│   ├── Repositories/          # Repository implementations
│   ├── Interfaces/            # Repository contracts
│   ├── Specifications/        # Query specifications
│   └── SeedData/              # Database seed data
│
├── VetLink.Services/          # Business logic layer
│   ├── Services/              # Service implementations
│   ├── Validators/            # FluentValidation validators
│   └── Hubs/                  # SignalR hubs
│
└── VetLink.WebAPI/            # Presentation layer
    ├── Controllers/           # API controllers
    ├── Extensions/            # Service extensions
    ├── Middleware/            # Custom middleware
    └── Helpers/               # Helper classes
```

## 🛠️ Tech Stack

- **.NET 10.0** - Framework
- **ASP.NET Core Web API** - Web framework
- **Entity Framework Core 10.0** - ORM
- **SQL Server** - Database
- **Redis** - Caching
- **JWT Bearer** - Authentication
- **Cloudinary** - Image storage
- **MailKit** - Email service
- **SignalR** - Real-time communication
- **AutoMapper** - Object mapping
- **FluentValidation** - Request validation
- **Serilog** - Logging
- **Swagger/OpenAPI** - API documentation
- **NSwag** - OpenAPI/Swagger tooling

## 📋 Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or SQL Server Express)
- [Redis](https://redis.io/download) (for caching)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) (recommended)

## ⚙️ Configuration

### 1. Database Setup

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "VetLinkDB": "server=.; database=VetlinkDb; Trusted_Connection=true; TrustServerCertificate=True;",
  "redis": "localhost"
}
```

### 2. JWT Configuration

Configure JWT settings in `appsettings.json`:

```json
"Token": {
  "Key": "Your-Secret-Key-Here-Must-Be-At-Least-32-Characters",
  "Issuer": "https://localhost:7087",
  "Audience": "https://localhost:4200"
}
```

### 3. Email Settings

Configure email service (Gmail example):

```json
"EmailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSSL": true,
  "SenderEmail": "your-email@gmail.com",
  "AppPassword": "your-app-password"
}
```

### 4. Cloudinary Configuration

Set up Cloudinary for image storage:

```json
"Cloudinary": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
}
```

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd VetLink-API
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Update Database

Apply migrations to create the database:

```bash
cd VetLink.WebAPI
dotnet ef database update --project ../VetLink.Data
```

Or use the Package Manager Console in Visual Studio:

```powershell
Update-Database
```

### 4. Run the Application

```bash
dotnet run --project VetLink.WebAPI
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:7087`

### 5. Access Swagger Documentation

Navigate to:
- Swagger UI: `https://localhost:7087/swagger` (Development only)
- OpenAPI: `https://localhost:7087/swagger/v1/swagger.json`

## 📚 API Endpoints

### Authentication
- `POST /api/v1/account/buyer/register` - Register as buyer
- `POST /api/v1/account/seller/register` - Register as seller
- `POST /api/v1/account/login` - User login
- `POST /api/v1/account/logout` - User logout
- `POST /api/v1/account/forgot-password` - Request password reset
- `POST /api/v1/account/reset-password` - Reset password

### Products
- `GET /api/v1/products` - Get all products (with pagination and search)
- `GET /api/v1/products/{id}` - Get product details
- `POST /api/v1/products` - Create product (Seller/Admin)
- `PUT /api/v1/products` - Update product (Seller/Admin)
- `DELETE /api/v1/products/{id}` - Delete product (Seller/Admin)
- `GET /api/v1/products/{id}/reviews` - Get product reviews
- `POST /api/v1/products/reviews` - Add review (Buyer)

### Orders
- `GET /api/v1/orders` - Get user orders
- `GET /api/v1/orders/{id}` - Get order details
- `POST /api/v1/orders` - Create order
- `PUT /api/v1/orders/{id}/cancel` - Cancel order

### Categories & Brands
- `GET /api/v1/categories` - Get all categories
- `GET /api/v1/brands` - Get all brands

### Admin
- Admin-specific endpoints for user management, order management, and system administration

## 🔐 Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-token>
```

## 🧪 Testing

Use the provided `VetLink.WebAPI.http` file for testing endpoints, or use Swagger UI for interactive testing.

Example request:

```http
POST https://localhost:7087/api/v1/account/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

## 📦 Project Structure

### VetLink.Data
Contains all data-related code:
- Entity models
- DbContext configuration
- Entity Framework configurations
- Database migrations

### VetLink.Repository
Implements the Repository pattern:
- Generic repository
- Unit of Work pattern
- Query specifications
- Database seeding

### VetLink.Services
Business logic layer:
- Service implementations
- DTOs (Data Transfer Objects)
- Validation logic
- SignalR hubs
- External service integrations (Email, Cloudinary)

### VetLink.WebAPI
API presentation layer:
- Controllers
- Middleware
- Service registration
- API configuration

## 🔧 Development

### Adding a New Migration

```bash
dotnet ef migrations add MigrationName --project VetLink.Data --startup-project VetLink.WebAPI
```

### Removing Last Migration

```bash
dotnet ef migrations remove --project VetLink.Data --startup-project VetLink.WebAPI
```

## 📝 Notes

- The API uses API versioning (currently v1.0)
- Rate limiting is enabled to prevent abuse
- All sensitive data should be stored in environment variables or Azure Key Vault in production
- The database is automatically seeded with initial data on first run
- Swagger documentation is only available in Development environment

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Authors

- **VetLink Team**

## 🙏 Acknowledgments

- Entity Framework Core team
- ASP.NET Core team
- All open-source contributors

---

For more information or support, please open an issue in the repository.