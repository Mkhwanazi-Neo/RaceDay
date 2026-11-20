RaceDay - Full-Stack Event Management SystemRaceDay is a full-stack web-based event management platform built for the South African road running, walking, and cycling community. The platform enables Event Organisers to publish and manage events, categories, and participant results, while Participants can register, view events, and keep track of their performance history. 

YouTube Link : https://youtu.be/0qW0lwgwYuI
YouTube Link Part 3 POE:  https://youtu.be/NWGrM19qXPM

Key Features

Event Organisers

Organiser Dashboard: View real-time event counts, total participant enrolments, and quick actions for every managed event.

Event Management (CRUD + Cloud Storage): Create, edit, and delete events with banner images uploaded directly to Azure Blob Storage (event-banners container).

Race Categories: Configure multiple race categories per event (discipline, distance in km, and entry fee in ZAR).

Enrolments & Race Timing: Track participant registrations and publish official race results (bib number, finish time, overall position, and category standing).

Participants (Athletes)

Event Discovery: Browse and filter upcoming running, walking, and cycling events by name or location.

Category Enrolment: Inspect event routes and enter specific race categories.

Performance History: View personal finish times, overall standings, and category placements.

Athlete Profile: Update personal details and upload a profile picture to Azure Blob Storage (profile-pictures container).

Prerequisites

.NET 8.0 SDK

SQL Server / SQL Server Express

Azurite (Local Azure Storage Emulator)

Docker Desktop (for containerized deployment)

Running Locally (Visual Studio)

1. Start Azurite Blob Storage

Create a local workspace folder and start the Azurite emulator on port 10000:

mkdir C:\azurite
azurite --location C:\azurite


2. Apply Database Migrations

Ensure SQL Server is running and update the database from the RaceDay (API) project folder:

cd RaceDay
dotnet ef database update


3. Launch Both Projects

Open RaceDay.sln in Visual Studio.

Right-click the Solution > Configure Startup Projects... > Multiple startup projects.

Set both RaceDay.API and RaceDay.MVC to Start, then press F5.

Running with Docker

Ensure Docker Desktop, Azurite, and RaceDay.API (listening on HTTPS port 7083) are running on your host machine.

From the root solution folder, build and launch the containerized MVC web application in a single command:

docker build -t raceday-mvc -f RaceDay.MVC/Dockerfile . && docker run -d -p 8080:8080 -e ApiSettings__BaseUrl="https://host.docker.internal:7083/" --name raceday-web raceday-mvc


Once started, access the web application at:

Web Frontend: http://localhost:8080

To stop and remove the container:

docker stop raceday-web && docker rm raceday-web


CI/CD Pipeline

Every push and pull request triggers the GitHub Actions workflow (.github/workflows/build-and-test.yml), which restores dependencies, compiles both RaceDay.API and RaceDay.MVC in Release configuration, and executes the automated test suite.
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
