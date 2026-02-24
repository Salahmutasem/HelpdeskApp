Helpdesk Ticketing System
A simple helpdesk ticketing web application where employees can create support tickets, organize them by category, add comments, and track their status.

Tech Stack
LayerTechnologyBackendASP.NET Core MVC (.NET 8)DatabaseSQL Server (Azure SQL Edge via Docker)Data AccessADO.NET (SqlConnection, SqlCommand, SqlDataReader)FrontendRazor Views with Bootstrap 5ContainerizationDocker Compose

Features

Login and logout with session-based authentication
Manage users (create, list, activate, deactivate)
Manage categories (create, list, activate, deactivate)
Manage tickets (create, list, view details, soft delete)
Search tickets by title or description
Filter tickets by status (Open, In Progress, Closed)
Pagination on the tickets list
Add comments to tickets (blocked on closed tickets)
Change ticket status (Open, In Progress, Closed)
Strong password validation
Unique email and category name validation
Inactive users cannot log in
Soft delete for tickets
Parameterized SQL queries to prevent SQL injection

Prerequisites
Make sure you have the following installed:

.NET 8 SDK
Docker Desktop
sqlcmd -- install on Mac with: brew install sqlcmd

Setup Instructions
Step 1: Clone the repository
git clone https://github.com/Salahmutasem/HelpdeskApp.git
cd HelpdeskApp
Step 2: Start the database
Make sure Docker Desktop is running, then:
docker compose up -d
Wait about 30 seconds for SQL Server to fully start.
Step 3: Create the database and tables
Run the SQL script to create the database, tables, and seed data:
sqlcmd -S localhost -U sa -P 'YOUR_PASSWORD_HERE' -N disable -i Docker/init-db.sql
Replace YOUR_PASSWORD_HERE with the SA password set in your docker-compose.yml.
You only need to run this command once. The data is saved in a Docker volume and survives restarts.

I also created a bash script that automates steps 2 and 3 together. The script starts Docker, waits for SQL Server, and runs the SQL script automatically. The commands from the script are what you see in steps 2 and 3 above.

Step 4: Configure the connection string
Copy appsettings.json to a new file called appsettings.Development.json:
cp appsettings.json appsettings.Development.json
Open appsettings.Development.json and replace YOUR_PASSWORD_HERE with your actual SQL Server SA password:
{
"ConnectionStrings": {
"DefaultConnection": "Server=localhost;Database=HelpdeskDB;User Id=sa;Password=YOUR_ACTUAL_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
}
}
Step 5: Run the application
dotnet restore
dotnet run
Open your browser at the URL shown in the terminal (e.g. http://localhost:5282).

Default Login Credentials
EmailPasswordadmin@helpdesk.comAdmin@123john@helpdesk.comTest@1234

Project Structure
HelpdeskApp/
├── Controllers/
│ ├── BaseController.cs - Session check for all protected pages
│ ├── AccountController.cs - Login and logout
│ ├── UsersController.cs - User list, create, activate/deactivate
│ ├── CategoriesController.cs - Category list, create, activate/deactivate
│ └── TicketsController.cs - Ticket list, create, details, delete, comments
│
├── Data/
│ ├── BaseDb.cs - Shared database connection logic
│ ├── UserDb.cs - User-related SQL queries
│ ├── CategoryDb.cs - Category-related SQL queries
│ └── TicketDb.cs - Ticket and comment SQL queries
│
├── Models/
│ ├── User.cs - User model with validation
│ ├── Category.cs - Category model with validation
│ ├── Ticket.cs - Ticket model with validation
│ ├── TicketComment.cs - Comment model with validation
│ └── LoginViewModel.cs - Login form model
│
├── Views/
│ ├── Shared/\_Layout.cshtml - Main layout with navbar
│ ├── Account/Login.cshtml - Login page
│ ├── Users/ - User list and create pages
│ ├── Categories/ - Category list and create pages
│ ├── Tickets/ - Ticket list, create, and details pages
│ ├── \_ViewImports.cshtml - Shared imports for all views
│ └── \_ViewStart.cshtml - Sets the default layout
│
├── Docker/
│ ├── init-db.sql - SQL script to create database and seed data
│ └── init.py - Python script for auto database init
│
├── database-script.sql - Standalone SQL script (deliverable)
├── docker-compose.yml - Docker Compose config for SQL Server
├── appsettings.json - App configuration (placeholder password)
├── Program.cs - App entry point and service registration
└── HelpdeskApp.csproj - Project file with dependencies

Architecture
MVC Pattern
The application follows the Model-View-Controller pattern. Models define the data structure and validation rules using DataAnnotations. Views are Razor pages that render HTML with Bootstrap styling. Controllers handle HTTP requests, call the data layer, and return views.
Data Layer (Separation of Concerns)
Database operations are split into separate classes by feature. BaseDb provides the shared connection logic and all other Db classes inherit from it. UserDb, CategoryDb, and TicketDb each handle queries for their own tables. All queries use parameterized SQL to prevent SQL injection. ADO.NET is used directly with no Entity Framework.
Authentication
Session-based authentication is used. On login, the user ID and name are stored in the server session. BaseController checks for a valid session before every action. Controllers that need login inherit from BaseController. AccountController inherits from Controller directly since the login page must be accessible without a session.
Dependency Injection
All Db classes are registered as scoped services in Program.cs and injected into controllers through their constructors. This follows the standard ASP.NET Core dependency injection pattern.

Daily Workflow
Start the database and run the app:
docker compose up -d
dotnet run
Stop everything (press Ctrl+C to stop the app first):
docker compose down
Reset the database from scratch:
docker compose down -v
docker compose up -d

# Wait 30 seconds

sqlcmd -S localhost -U sa -P 'YOUR_PASSWORD_HERE' -N disable -i Docker/init-db.sql
