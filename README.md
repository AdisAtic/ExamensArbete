<!-- PROJECT SHIELDS -->
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# PriceTrackingAPI

A robust .NET 9.0 RESTful API for tracking and analyzing product price data from various e-commerce platforms. Monitor price changes, analyze trends, and receive notifications when prices drop or increase.

---

## 📖 Table of Contents
1. [About the Project](#about-the-project)
2. [Features](#features)
3. [Tech Stack](#tech-stack)
4. [Prerequisites](#prerequisites)
5. [Installation](#installation)
6. [Usage](#usage)
7. [API Endpoints](#api-endpoints)
8. [Docker Configuration](#docker-configuration)
9. [Configuration](#configuration)
10. [Logging](#logging)
11. [Contributing](#contributing)
12. [License](#license)
13. [Contact](#contact)

---

## 📋 About the Project

**PriceTrackingAPI** helps you automate price data collection and monitor price changes over time. Use it for:
- Tracking product prices across e-commerce platforms
- Data analysis and trend spotting
- Notifications when prices drop or increase

The primary function of this API is to track product prices and display the lowest price from the last 30 days, making it valuable for price comparison and buying decision support.

## 🌟 Features

- **Product Management**: Add, update, and manage products from multiple sources
- **Price Tracking**: Collect and store historical price data automatically
- **Data Analysis**: Access comprehensive price history for trend analysis
- **Notifications**: Configure alerts for price changes based on custom thresholds
- **RESTful API**: Simple and powerful endpoints for seamless integration
- **Price Comparison**: Displays the lowest price from the last 30 days

## 💻 Tech Stack

- **.NET 9.0**: Built on the latest .NET framework
- **Entity Framework Core 9.0.1**: ORM for database interactions
- **PostgreSQL**: Robust and scalable database backend via Npgsql
- **Swagger/OpenAPI**: Comprehensive API documentation
- **Serilog**: Advanced structured logging
- **Litium Framework**: Integration with Litium e-commerce platform

## 🔨 Prerequisites

Before beginning installation, ensure you have the following prerequisites installed on your system:

1. [**.NET SDK 9.0**](https://dotnet.microsoft.com/download/dotnet/9.0) - Required to build and run the application
   ```bash
   # Verify installation
   dotnet --version
   # Should show 9.0.x or higher
   ```

2. **PostgreSQL** - Required for the database (either local installation or Docker)
   - For local installation: [Download PostgreSQL](https://www.postgresql.org/download/)
   - For Docker: See the Docker Configuration section

3. **Git** - Required to clone the repository
   ```bash
   # Verify installation
   git --version
   ```

4. **Entity Framework Core CLI Tools** - Required for migrations
   ```bash
   # Install the tools
   dotnet tool install --global dotnet-ef
   
   # Verify installation
   dotnet ef --version
   ```

## 🚀 Installation

### Option 1: Local Setup

Follow these detailed steps to set up the project on your local machine:

1. **Clone the repository**
   ```bash
   git clone https://github.com/AdisAtic/ExamensArbete.git
   cd ExamensArbete
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Create or modify the configuration**
   
   Create a new `appsettings.Development.json` file in the project root directory (if it doesn't exist) or modify the existing one:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=prices;Username=admin;Password=nimda;"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft": "Warning",
         "Microsoft.Hosting.Lifetime": "Information"
       }
     },
     "AllowedHosts": "*"
   }
   ```

4. **Set up the PostgreSQL database**
   
   If using a local PostgreSQL installation:
   - Create a new database named `prices`
   - Create a user with username `admin` and password `nimda` (or use your own credentials and update the connection string accordingly)
   - Grant all privileges on the `prices` database to the user

5. **Apply database migrations**
   ```bash
   # Create a new migration if none exists
   dotnet ef migrations add InitialCreate
   
   # Apply migrations to the database
   dotnet ef database update
   ```

6. **Build the project**
   ```bash
   dotnet build
   ```

7. **Run the application**
   ```bash
   # Run in development mode
   dotnet run --environment Development
   # Or simply
   dotnet run
   ```

8. **Access the API**
   
   The API will be available at:
   - API: `https://localhost:5001/api` (HTTPS) or `http://localhost:5000/api` (HTTP)
   - Swagger UI: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

9. **Troubleshooting common issues**
   
   - **Port conflict**: If ports 5000/5001 are already in use, you can specify a different port:
     ```bash
     dotnet run --urls="https://localhost:5005"
     ```
   
   - **Database connection issues**: Verify your PostgreSQL service is running and credentials are correct
     ```bash
     # Check if PostgreSQL is running (Linux/macOS)
     sudo service postgresql status
     # or
     pg_isready
     ```
   
   - **Missing migrations**: If you see an error about missing migrations, ensure you've run the migrations commands correctly
     ```bash
     # Remove any broken migrations
     dotnet ef migrations remove
     # Add a fresh migration
     dotnet ef migrations add InitialCreate
     # Update the database
     dotnet ef database update
     ```

### Option 2: Docker Deployment

1. **Clone** the repository
   ```bash
   git clone https://github.com/AdisAtic/ExamensArbete.git
   cd ExamensArbete
   ```

2. **Start** the Docker containers using Docker Compose
   ```bash
   docker-compose up -d
   ```

This will start:
- PostgreSQL database accessible at `localhost:5432`
  - Database: `prices`
  - Username: `admin`
  - Password: `nimda`
- pgAdmin interface accessible at `localhost:8080`
  - Email: `admin@example.com`
  - Password: `admin`

3. **Configure the application** to connect to the Docker PostgreSQL instance by updating `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=prices;Username=admin;Password=nimda;"
     }
   }
   ```

4. **Build and run** the application
   ```bash
   dotnet build
   dotnet run
   ```

5. **Verify the deployment** by accessing the Swagger UI at `https://localhost:5001/swagger`

## 📊 Usage

After installation, you can use the API to:

1. Add products for price tracking
2. Retrieve historical price data
3. Set up price change notifications
4. Generate price trend reports

For detailed API documentation, refer to the Swagger UI when running the application.

## 🔌 API Endpoints

The API provides the following main endpoints:

- **Products**: `/api/products` - Manage product information
- **Prices**: `/api/prices` - Access historical price data
- **Alerts**: `/api/alerts` - Configure price change notifications
- **Sources**: `/api/sources` - Manage price data sources

## 🐳 Docker Configuration

The project includes a `docker-compose.yml` file with the following configuration:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:latest
    container_name: postgres-db
    restart: always
    environment:
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: nimda
      POSTGRES_DB: prices
    ports:
      - "5432:5432"
    networks:
      - backend

  pgadmin:
    image: dpage/pgadmin4
    container_name: pgadmin
    restart: always
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "8080:80"
    networks:
      - backend

networks:
  backend:
```

## ⚙️ Configuration

The application uses standard .NET configuration patterns. Key settings can be found in:

- `appsettings.json`: General application settings
- `appsettings.Development.json`: Development-specific settings

### Key Configuration Options

- **ConnectionStrings**: Database connection information
- **Logging**: Log levels and outputs
- **Serilog**: Structured logging configuration
- **API Keys**: External service authentication (if applicable)

<!-- ## 🧪 Testing

Testing is planned but not yet implemented. This section will be updated when tests are available.

```bash
dotnet test
```
-->

## 📝 Logging

Logging is implemented using Serilog with support for:

- Console logging
- File logging (logs stored in `/logs` directory)

Configure log levels and output destinations in `appsettings.json`.

## 👥 Contributing

This project is maintained by [AdisAtic](https://github.com/AdisAtic).

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📬 Contact

Adis Atic - [GitHub Profile](https://github.com/AdisAtic)

Project Link: [https://github.com/AdisAtic/ExamensArbete](https://github.com/AdisAtic/ExamensArbete)
