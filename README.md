# Property Management System (PMS)

A comprehensive .NET-based property management solution for property owners and tenants. This system facilitates property management, tenant allocation, billing, and service requests.

## Overview

The Property Management System (PMS) is a web application built with ASP.NET Core that provides a platform for property owners to manage their properties, tenants, and billing. It also allows tenants to view and pay bills, request services, and access utility consumption reports.

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 9.0
- **Authentication**: ASP.NET Core Identity
- **Email Service**: Azure Communication Services Email
- **UI**: Bootstrap, HTML, CSS, JavaScript
- **Architecture**: MVC with Areas pattern

## Key Features

### For Property Owners

- **Dashboard**: Overview of properties, tenants, bills, and income
- **Property Management**: Add, modify, and delete properties
- **Tenant Management**: Add, modify, and delete tenants
- **Room Management**: Allocate tenants to rooms/units
- **Billing**: Create and manage bills for tenants
- **Reports**: View financial and utility reports

### For Tenants

- **Dashboard**: Overview of bills, services, and utility consumption
- **Bill Management**: View and pay bills
- **Service Requests**: Request maintenance and other services
- **Reports**: View electricity and water consumption reports
- **Profile Management**: Update personal information

## Database Structure

The system uses the following main entities:

- **Users**: Extended from ASP.NET Identity with additional properties
- **Properties**: Represents physical properties owned by property owners
- **Rooms**: Individual units within properties that can be rented
- **Bills**: Financial transactions related to rent and utilities
- **Services**: Maintenance and other service requests

## Project Structure

The application follows a structured approach with separate areas for different user roles:

- **Areas/Admin**: Controllers and views for property owner functionality
- **Areas/User**: Controllers and views for tenant functionality
- **Areas/Identity**: Authentication and user management

## Getting Started

### Prerequisites

- .NET SDK 8.0 or higher
- SQL Server
- Visual Studio 2022 or another compatible IDE

### Setup Instructions

1. Clone the repository
2. Update the connection string in `appsettings.json` to point to your SQL Server instance
3. Run the following commands in the terminal:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

4. Navigate to `https://localhost:5001` in your browser

## Configuration

The application uses the following configuration settings:

- **Database Connection**: Set in `appsettings.json` under `ConnectionStrings:DatabaseConnection`
- **Email Service**: Azure Communication Services configuration in `appsettings.json` under `AzureEmail`

## Architecture

The application follows the MVC (Model-View-Controller) pattern with the following components:

- **Models**: Data entities and view models
- **Views**: Razor views for rendering UI
- **Controllers**: Handle user requests and business logic
- **Services**: External service integrations (email, etc.)
- **Data**: Database context and migrations 