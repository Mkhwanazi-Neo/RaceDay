RaceDay - Full-Stack Event Management SystemRaceDay is a full-stack web-based event management platform built for the South African road running, walking, and cycling community. The platform enables Event Organisers to publish and manage events, categories, and participant results, while Participants can register, view events, and keep track of their performance history. 

Repository StructurePlaintextRaceDay/
├── .github/
│   └── workflows/
│       └── build-and-test.yml     # GitHub Actions CI/CD Pipeline
├── docs/                          # Project Documentation (Part 1)[cite: 2, 6]
│   ├── ERD_Diagram.png            # Entity-Relationship Diagram
│   ├── API_Endpoint_Plan.md       # API Specification Plan[cite: 2]
│   └── RaceDay_Database.sql       # SQL Database Script for SSMS[cite: 6]
├── src/
│   └── RaceDay.API/               # ASP.NET Core 8 Web API Project[cite: 7]
│       ├── Attributes/            # Session Authorization Attributes
│       ├── Controllers/           # RESTful API Controllers[cite: 9]
│       │   ├── AuthController.cs
│       │   ├── UsersController.cs
│       │   ├── EventsController.cs
│       │   ├── CategoriesController.cs
│       │   ├── EnrolmentsController.cs
│       │   └── ResultsController.cs
│       ├── Data/                  # Entity Framework Core DbContext
│       ├── DTOs/                  # Request & Response Data Models
│       ├── Models/                # Domain Entities matching ERD
│       ├── Services/              # Password Hashing Utilities
│       └── Program.cs             # Application Pipeline & Dependency Injection[cite: 8, 10]
└── tests/
    └── RaceDay.Tests/             # xUnit Unit Test Project
Part 1: Architecture & Database Design1. Database Schema & ERDThe system schema is built around 6 core entities:   ORGANISER: Stores event organiser account details.   PARTICIPANT: Stores athlete profile data.EVENT: Holds event names, dates, locations, and route descriptions.   CATEGORY: Defines event distances and age divisions (e.g., 21km, 10km Fun Walk).   REGISTRATION: Links participants, events, and selected categories.   RESULT: Records finish times, race numbers, and rankings.   

2. API Endpoint PlanA complete specification was designed covering route naming, HTTP verbs, request payloads, response structures, and role permissions.   Part 2: ASP.NET Core Web API & TestingKey Features ImplementedCode-First Entity Framework Core: Managed via EF Core migrations (InitialCreate) with constraints.   Authentication & Role-Based Access:Secure password hashing using BCrypt.Net.Session-based authentication (UserId, UserRole) to maintain state.Role restriction via custom [SessionAuthorize] attributes (Organiser vs Participant)
3. .Swagger UI Integration: Built-in endpoint documentation and browser testing environment.   Unit Testing: Unit tests built with xUnit and EF Core In-Memory database covering authentication, event management, and access controls.
4. CI/CD Integration: Automated GitHub Actions workflow building and executing tests on every push.   Getting StartedPrerequisites.NET 8.0 SDKSQL Server (SQLEXPRESS or MSSQLSERVER)SQL Server Management Studio (SSMS)Local Setup InstructionsClone the Repository:Bashgit clone https://github.com/your-username/RaceDay.git
cd RaceDay
Configure Connection String:
Open src/RaceDay.API/appsettings.json and update the DefaultConnection to point to your local SQL Server instance[cite: 8]:JSON{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=RaceDayDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
  }
}
Apply Database Migrations:
Navigate into the API project folder and update the database:   Bashcd src/RaceDay.API
dotnet ef database update
Run the API:Bashdotnet run
Access Swagger UI in your browser at https://localhost:7083/swagger/index.html.   Run Unit Tests:
Execute tests locally via terminal:   Bashdotnet test
Continuous Integration StatusThe repository utilizes GitHub Actions to automatically build the solution and execute unit test suites on every push or pull_request to the main branch
