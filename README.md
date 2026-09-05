# LostFoundPetReporter 🐾

**LostFoundPetReporter** is a full-stack application designed to help people report lost and found pets and automatically identify potential matches between reports.

The system consists of a **.NET Web API backend** and a **.NET MAUI mobile application**. Users can create lost or found pet reports, upload pet images, use AI-assisted animal description, view reports on a map, calculate routes to reported locations, and receive push notifications when a potential match is found.

---

## 📱 Overview

The main goal of LostFoundPetReporter is to simplify the process of reconnecting lost pets with their owners.

A typical workflow looks like this:

```text
                 ┌─────────────────────┐
                 │   Mobile Application │
                 │      .NET MAUI       │
                 └──────────┬──────────┘
                            │
                            │ HTTP / JWT
                            ▼
                 ┌─────────────────────┐
                 │      ASP.NET API     │
                 │                     │
                 │  Reports            │
                 │  Users              │
                 │  Matching            │
                 │  Authentication     │
                 │  Notifications      │
                 └──────────┬──────────┘
                            │
              ┌─────────────┼─────────────┐
              │             │             │
              ▼             ▼             ▼
          SQL Server      Gemini AI       FCM
           Database      Image Analysis   Push
                            │           Notifications
                            │
                            ▼
                         Matching
```

---

# ✨ Features

## 🐕 Lost Pet Reports

Users can create reports for pets that have gone missing.

A report can contain information such as:

* Pet type
* Breed
* Colors
* Sex
* Age
* Size
* Weight
* Coat characteristics
* Distinctive markings
* Collar information
* Location
* Images

Reports are associated with the user who created them.

---

## 🐾 Found Pet Reports

Users can also report pets they have found.

Found reports contain:

* Found location
* Animal description
* Images
* Additional information about the animal

Found reports can subsequently be compared against existing lost reports.

---

# 🤖 AI-Assisted Animal Identification

The project integrates **Google Gemini** to analyze uploaded pet images and automatically generate an animal description.

Instead of requiring the user to manually enter every characteristic, an image can be analyzed and used to populate fields such as:

```json
{
  "name": "",
  "type": "dog",
  "breed": "Labrador Retriever",
  "colors": "cream,yellow",
  "sex": "Male",
  "age": null,
  "size": "Large",
  "weightKg": null,
  "coatLength": "Short",
  "coatType": "Straight",
  "pattern": "Solid",
  "distinctiveMarkings": ""
}
```

The AI functionality is separated from the report creation logic through dedicated DTOs and services.

This allows the application to use AI as an **assistive feature** rather than making the report creation process dependent on AI.

---

# 🔎 Lost & Found Matching

One of the main features of the backend is automatic matching between lost and found reports.

When a new report is created, the API can queue a matching operation.

For example:

```text
New Found Report
       │
       ▼
Create report
       │
       ▼
Queue matching operation
       │
       ▼
Compare against Lost Reports
       │
       ▼
Potential matches
       │
       ▼
Notify owner
```

The matching process is handled asynchronously so that report creation does not have to wait for potentially expensive matching operations.

The project uses background processing and queues for this functionality.

---

# 🔔 Push Notifications

The application uses **Firebase Cloud Messaging (FCM)** for push notifications.

Android devices register their FCM device token with the API.

The backend stores device information through a `UserDevice` entity containing information such as:

* User ID
* FCM token
* Platform
* Last updated time

When a potential match is discovered, the system can send a notification to the relevant pet owner.

Example flow:

```text
Lost Report
     │
     ▼
Found Report Created
     │
     ▼
Matching Service
     │
     ▼
Potential Match
     │
     ▼
Find Lost Report Owner
     │
     ▼
Find Registered Device
     │
     ▼
Firebase Cloud Messaging
     │
     ▼
📱 Owner receives notification
```

---

# 🔐 Authentication

The API uses **JWT Bearer Authentication**.

After successful login, the mobile application stores the authenticated user's session and includes the JWT token in subsequent API requests.

The mobile application uses:

```text
IUserSession
      │
      └── CurrentUser
```

and an HTTP authorization handler is responsible for attaching the JWT token to API requests.

Protected API endpoints use:

```csharp
[Authorize]
```

to ensure that authenticated functionality cannot be accessed anonymously.

---

# 🗺️ Maps & Location

The mobile application uses **Mapsui** with OpenStreetMap data.

The map functionality includes:

* Displaying the user's current location
* Displaying report locations
* Selecting reports from the map
* Showing the user's location
* Tracking location changes
* Drawing routes
* Showing route progress
* Using the phone's movement/location to update the map

The application uses `MapPoint` objects to represent geographic coordinates.

---

# 🚗 Route Calculation

Routing is handled using the **OSRM (Open Source Routing Machine)** API.

A route request follows the OSRM format:

```text
https://router.project-osrm.org/route/v1/driving/
    {longitude1},{latitude1};
    {longitude2},{latitude2}
    ?overview=full&geometries=geojson
```

The returned GeoJSON route is converted into a Mapsui geometry and displayed on the map.

The current map implementation supports the general flow:

```text
User Location
      │
      ▼
Select Report
      │
      ▼
Get Report Coordinates
      │
      ▼
Request OSRM Route
      │
      ▼
Receive GeoJSON
      │
      ▼
Convert Route Geometry
      │
      ▼
Draw Route on Mapsui
```

---

# 📍 Location Tracking

The mobile application retrieves the device's current location and updates the map.

The location update process is separated from route calculation so that the user's movement can be reflected independently of the initial route request.

This allows the application to support features such as:

* Live user position
* Route progress
* Updating the map as the user moves
* Direction/orientation visualization

---

# 🖼️ Image & File Processing

Pet images are sent from the mobile application and processed by the API.

Images can be transferred as Base64 data.

The backend uses an asynchronous file-processing queue so that saving multiple images does not unnecessarily block the main report creation request.

The architecture contains components such as:

```text
ExtFileQueue
      │
      ▼
ExtFileBackgroundService
      │
      ▼
ExtFileService
      │
      ▼
IFileStorageService
      │
      ▼
Stored image
```

This separation keeps file processing independent from the main CRUD operations.

---

# 🏗️ Architecture

The project is divided into two primary applications.

```text
LostFoundPetReporter
│
├── LostFoundPetReporter.API
│
├── LostFoundPetReporter.CoreDb
│
└── LostFoundPetReporter.Mobile
```

## Backend

The backend is an ASP.NET Core Web API.

Its responsibilities include:

* Authentication
* User management
* Lost reports
* Found reports
* Animal descriptions
* Matching
* Image processing
* Push notifications
* Database access
* Background processing

The backend follows a layered approach using controllers, services, repositories, DTOs and database models.

---

## Mobile

The mobile application is built with **.NET MAUI**.

The application contains:

```text
Mobile
│
├── Models
├── Views
├── ViewModels
├── Services
│   ├── Api
│   ├── Session
│   ├── Maps
│   └── Notifications
└── Platforms
    └── Android
```

The application uses MVVM concepts to separate UI, state and application logic.

---

# 🧩 DTO Architecture

The API separates database models from API contracts using DTOs.

For example:

```text
User
 │
 ├── UserDto
 └── CreateUserDto
```

Similarly, report-related objects use separate DTOs for responses and creation requests.

This prevents the API from exposing database entities directly and allows request and response models to evolve independently.

---

# 🗄️ Database

The backend uses **Entity Framework Core** with **SQL Server**.

The database contains entities for concepts such as:

* Users
* Lost Reports
* Found Reports
* Animal Descriptions
* Report attachments
* User devices

Relationships include:

```text
User
 │
 └── LostReports

LostReport
 │
 ├── AnimalDescription
 ├── Attachments
 └── FoundReports / Matches

User
 │
 └── UserDevices
```

Entity Framework Core is responsible for database access and persistence.

---

# ⚙️ Background Processing

Several operations are designed to run asynchronously.

Examples include:

### Matching

```text
Report Created
     │
     ▼
Matching Queue
     │
     ▼
Background Service
     │
     ▼
Matching Engine
```

### Image Processing

```text
Report Created
     │
     ▼
Image Queue
     │
     ▼
Background Service
     │
     ▼
File Storage
```

This architecture prevents expensive work from unnecessarily delaying API requests.

---

# 📡 API

The backend exposes REST-style endpoints for functionality such as:

```text
/users
/lostreports
/foundreports
/...
```

Authentication-protected endpoints use JWT authorization.

The mobile application communicates with the API using `HttpClient` services.

---

# 📱 Mobile Navigation

The mobile application contains several primary sections, including:

* 🏠 Home
* 📷 Camera / Create Report
* 🗺️ Map
* 📋 My Reports
* 👤 Profile

Authentication pages such as login and registration are handled separately from the main application navigation.

---

# 🛠️ Technologies

## Backend

| Technology               | Purpose                   |
| ------------------------ | ------------------------- |
| C#                       | Main programming language |
| ASP.NET Core             | REST API                  |
| Entity Framework Core    | ORM / database access     |
| SQL Server               | Database                  |
| JWT                      | Authentication            |
| Firebase Cloud Messaging | Push notifications        |
| Google Gemini            | AI image analysis         |
| OSRM                     | Route calculation         |

## Mobile

| Technology               | Purpose                           |
| ------------------------ | --------------------------------- |
| .NET MAUI                | Cross-platform mobile application |
| C#                       | Application logic                 |
| XAML                     | UI                                |
| MVVM                     | Application architecture          |
| Mapsui                   | Map rendering                     |
| OpenStreetMap            | Map data                          |
| Firebase Cloud Messaging | Push notifications                |

---

# 🔧 Development Environment

The project has been developed using:

* Visual Studio
* .NET 10
* .NET MAUI
* Android SDK
* Android Emulator
* SQL Server
* SQL Server Management Studio
* Postman

The mobile project targets platforms supported by .NET MAUI, including Android and Windows, with additional MAUI targets configured in the project.

---

# 🚀 Getting Started

## 1. Clone the repository

```bash
git clone https://github.com/NaftaliProjects/LostFoundPetReporter.git
```

Then:

```bash
cd LostFoundPetReporter
```

---

## 2. Configure the API

Configure the SQL Server connection string in the API configuration.

For local development, the API can be configured to run on HTTPS, for example:

```text
https://localhost:7074
```

and HTTP:

```text
http://localhost:5081
```

The exact URLs depend on the current launch configuration.

---

## 3. Configure the Mobile API URL

The MAUI application needs to point to the running API.

The API base address is configured through the mobile application's service registration.

For example:

```csharp
builder.Services.AddHttpClient(...)
```

For Android emulator development, make sure the API address is reachable from the emulator rather than assuming that `localhost` refers to the development machine.

---

## 4. Database

Create/configure the SQL Server database and apply the Entity Framework Core migrations.

The API's `DbContext` is configured for SQL Server.

---

## 5. Firebase

Push notifications require Firebase configuration for Android.

The Android application uses Firebase Cloud Messaging to obtain a device token.

The token is registered with the backend after authentication.

Firebase credentials and API keys should **not** be committed to the repository.

Sensitive configuration should be kept outside source control.

---

## 6. Gemini

The AI image-analysis functionality requires a Google Gemini API key.

The key should be provided through secure configuration rather than committed to Git.

Do not place API keys directly in source code or tracked configuration files.

---

# 🔒 Security

The project uses several security mechanisms:

* JWT authentication
* `[Authorize]` protected API endpoints
* User identity obtained from JWT claims
* Device-token ownership associated with authenticated users
* Secrets kept outside source control

Sensitive files and credentials should not be committed to GitHub.

In particular, Firebase configuration and Gemini/API keys should be treated as secrets.

---

# 🧪 Development & Testing

The API can be tested independently using tools such as **Postman**.

A typical development workflow is:

```text
Start SQL Server
      │
      ▼
Start ASP.NET API
      │
      ▼
Verify API endpoints
      │
      ▼
Start Android Emulator
      │
      ▼
Start .NET MAUI application
      │
      ▼
Login / Register
      │
      ▼
Create Lost / Found Report
      │
      ▼
Test matching
      │
      ▼
Test notification
      │
      ▼
Test map / routing
```

---

# 📂 Project Structure

A simplified structure of the solution:

```text
LostFoundPetReporter/
│
├── LostFoundPetReporter.API/
│   ├── Controllers/
│   ├── DTO/
│   ├── Services/
│   ├── Repositories/
│   ├── BackgroundServices/
│   └── ...
│
├── LostFoundPetReporter.CoreDb/
│   ├── Models/
│   ├── DbContext/
│   └── ...
│
├── LostFoundPetReporter.Mobile/
│   ├── Models/
│   ├── Views/
│   ├── ViewModels/
│   ├── Services/
│   ├── Platforms/
│   └── ...
│
└── README.md
```

---

# 🔄 Example End-to-End Scenario

### A person loses their dog

The owner opens the mobile application and creates a Lost Report.

```text
📱 Create Lost Report
        │
        ├── Upload photo
        ├── Animal information
        └── Last known location
        │
        ▼
      API
        │
        ▼
    SQL Server
```

Later, another user finds a similar dog and creates a Found Report.

```text
📱 Create Found Report
        │
        ├── Upload photo
        ├── Animal information
        └── Found location
        │
        ▼
      API
        │
        ▼
 Matching Queue
        │
        ▼
 Matching Service
        │
        ▼
 Potential Match
        │
        ▼
 Firebase Cloud Messaging
        │
        ▼
📱 Original owner receives notification
```

The owner can then open the matching report, view its location on the map, and calculate a route to it.

---

# 🎯 Project Goals

The project is intended to demonstrate a complete real-world application involving:

* Full-stack development
* REST API development
* Database design
* Entity Framework Core
* Authentication and authorization
* Mobile development
* MVVM
* Background processing
* Queue-based architecture
* AI integration
* Image processing
* Push notifications
* Location services
* Maps
* Routing
* API integration

Rather than implementing everything as one large application layer, the project separates responsibilities between the mobile client, API, database and asynchronous services.

---

# 🧠 Architecture Principles

Some of the main architectural principles used throughout the project are:

### Separation of concerns

UI, API, database, authentication, matching, file processing and notifications are separated into their own components.

### Asynchronous processing

Potentially expensive operations such as matching and file processing are moved into background queues.

### DTO-based API contracts

Database entities are not directly exposed as API contracts.

### Service abstractions

Interfaces such as:

```csharp
IUserSession
IFileStorageService
IPushNotificationService
IMapService
```

allow implementation details to remain separated from the code that consumes them.

### Dependency Injection

Services and application components are registered through the .NET dependency injection container.

---

# 📌 Current Development Status

The project is actively developed and currently includes working implementations/prototypes for:

* User registration and login
* JWT authentication
* Lost reports
* Found reports
* Animal descriptions
* AI-assisted image analysis
* Image/file processing
* Lost/found matching
* Firebase push notifications
* User device registration
* Mapsui/OpenStreetMap maps
* Device location
* OSRM routing
* Route drawing
* Route progress tracking

Some parts of the application are still under active development and refinement.

---

# 🛣️ Future Improvements

Potential future development areas include:

* Improved matching accuracy
* More advanced image similarity
* Better geographic matching
* Match confidence scores
* Improved route/navigation UX
* More robust background job handling
* Image optimization
* Cloud-based file storage
* Production deployment
* Automated tests
* CI/CD pipeline
* Monitoring and logging
* Improved notification handling
* Additional supported platforms

---

# 👨‍💻 Author

**Naftali Davidov**

GitHub:

https://github.com/NaftaliProjects

---

# 📄 License

This project is currently under development.
