# HR Leave Management System

A comprehensive Human Resources Leave Management application that streamlines leave request workflows, tracks employee leave balances, and provides real-time dashboard analytics for HR teams.

## Overview

The HR Leave Management System is a full-stack application designed to centralize and automate leave management processes. It enables employees to submit leave requests, managers to approve/reject requests, and HR teams to maintain comprehensive leave policies and reporting.

**Key Features:**
- Employee leave request submission and status tracking
- Department and leave type management
- Real-time leave balance tracking per employee
- Public holiday integration (API-based)
- Role-based access control (Admin, Manager, Employee)
- JWT-based authentication with refresh token support
- Comprehensive dashboard with leave analytics
- Email notifications for leave actions
- Rate limiting and request timing middleware

## Tech Stack

### Backend
- **.NET 10** – Modern ASP.NET Core web API framework
- **Entity Framework Core 10** – ORM for SQL Server data access
- **ASP.NET Core Identity** – User authentication and role management
- **JWT Bearer Authentication** – Stateless token-based auth
- **AutoMapper** – Object-to-object mapping
- **Serilog** – Structured logging to console and rolling file logs
- **Swagger/Swashbuckle** – OpenAPI documentation
- **MailKit** – SMTP-based email services
- **BCrypt.Net** – Password hashing

### Frontend
- **React 18+** – UI framework
- **TypeScript** – Type-safe JavaScript
- **Vite** – Modern build tool
- **Radix UI + Tailwind CSS** – Component library and styling
- **React Hook Form** – Form state management
- **TanStack Query** – Data fetching and caching

### Database
- **SQL Server** – Primary data store

## Installation & Quick Start

### Prerequisites
- **.NET 10 SDK**
- **SQL Server** 
- **Node.js 18+** & npm (for frontend)
- **Git** – Version control

### Backend Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/aya3012003/Project-3.git
   cd Project-3/Project-3
   ```

2. **Configure secrets:**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=HRLeaveDB;Trusted_Connection=true;"
   dotnet user-secrets set "JWT:Secret" "your-super-secret-jwt-key-min-32-chars"
   dotnet user-secrets set "EmailSettings:Email" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:Password" "your-app-password"
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the backend:**
   ```bash
   dotnet run
   ```
   API will be available at `http://localhost:7000` with Swagger documentation at `http://localhost:7000/swagger`

### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd Project-3/frontend
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Create environment configuration:**
   ```bash
   cp .env.local.example .env.local
   # Update .env.local with your API URL (typically http://localhost:7000)
   ```

4. **Run development server:**
   ```bash
   npm run dev
   ```
   Frontend will be available at `http://localhost:5173`

## Project Architecture

```
Project-3/
├── Project-3/                          # ASP.NET Core backend
│   ├── Program.cs                      # Application entry point & DI configuration
│   ├── src/
│   │   ├── API/
│   │   │   ├── Controllers/            # REST API endpoints
│   │   │   ├── Extensions/             # Service & authentication extensions
│   │   │   └── Middleware/             # Custom middleware (exception handling, rate limiting)
│   │   ├── Application/
│   │   │   ├── Models/                 # Domain entities (User, LeaveRequest, Department, etc.)
│   │   │   ├── Services/               # Business logic interfaces & implementations
│   │   │   ├── DTOs/                   # Data transfer objects for API requests/responses
│   │   │   ├── Mapping/                # AutoMapper profiles
│   │   │   └── ExceptionHandling/      # Custom exception classes
│   │   └── Infrastructure/
│   │       ├── Data/                   # DbContext, migrations, seed data
│   │       ├── Repositories/           # Generic repository pattern & UoW
│   │       ├── identity/               # JWT options, token service
│   │       └── Shared/                 # Enums (LeaveStatus, EmployeeType)
│   ├── Properties/
│   │   └── launchSettings.json         # Debug profiles
│   └── appsettings.json                # Configuration & secrets reference
│
└── frontend/                           # React + TypeScript frontend
	├── src/
	│   ├── components/                 # Reusable UI components
	│   ├── pages/                      # Page-level components
	│   ├── contexts/                   # React context (auth state)
	│   ├── hooks/                      # Custom React hooks
	│   ├── lib/                        # Utilities (API client, helpers)
	│   └── types/                      # TypeScript interface definitions
	├── vite.config.ts                  # Vite build configuration
	├── package.json                    # Dependencies & scripts
	└── .env.local                      # Runtime environment variables
```

### Architecture Highlights

- **Layered Architecture:** API → Services → Repository → Data
- **Repository Pattern:** Centralized data access with generic `Repository<T>` and `UnitOfWork`
- **Dependency Injection:** All services registered in `Program.cs` for loose coupling
- **DTOs:** Separation of domain models from API contracts
- **Authentication:** JWT-based stateless auth with refresh tokens
- **Logging:** Serilog structured logging to file (`logs/hrms-log.txt`) and console
- **Role-Based Access Control:** Admin, Manager, Employee roles via Identity

## Core Entities & Workflows

### Key Models
- **User** – Employee/Manager/Admin with department assignment
- **LeaveType** – Configurable leave categories (Annual, Sick, Casual, etc.)
- **LeaveRequest** – Employee leave submission with status (Pending, Approved, Rejected)
- **EmployeeLeaveBalance** – Tracks available/used leave per employee per type
- **Department** – Organizational divisions
- **Holiday** – Public holidays (fetched from external API or manual entry)

### Main Workflows
1. **Leave Request Submission:** Employee submits request → Pending status → Manager review
2. **Leave Approval:** Manager approves/rejects → Balance updated → Email notification
3. **Leave Balance:** System auto-tracks balance based on leave type allocation and usage
4. **Dashboard Analytics:** HR views department-level usage, employee history, trends


### Logging
Production logs are written to `logs/hrms-log-YYYYMMDD.txt` with daily rolling.
Log levels:
- **Information** – General app events
- **Warning** – Recoverable issues (Microsoft framework warnings suppressed)
- **Error** – Exceptions caught by global handler

## Contributors

- **Aya**
- **Moamen** 
- **Gihad** 

