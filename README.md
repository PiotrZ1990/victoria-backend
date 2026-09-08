# Victoria — Integrated Education & Migration Management System

> A full-stack business management system designed to support education and migration consultancy processes, including CRM, case management, document workflows, education services, payments, CMS and a dedicated mobile client application.

---

## 📌 Overview

**Victoria** is an integrated IT system designed to support the daily operations of an education and migration consultancy.

The system consists of:

- 🌐 ASP.NET Core MVC web application for administrators and staff
- 📱 .NET MAUI mobile application for clients/students
- 🔌 ASP.NET Core REST API
- 🗄️ SQL Server relational database

Both the web and mobile applications communicate with the same backend API and database, providing a consistent source of business data.

The system reflects real-world business processes such as:

- customer and lead management
- case management
- education and visa applications
- document collection and verification
- automatic document checklists
- language course management
- course enrollments
- exams and results
- invoices and payments
- CMS content management
- reporting
- customer self-service through the mobile application

---

## 🎯 Project Goals

The main goals of the project were to create a centralized platform capable of:

- managing customers and leads
- converting leads into active cases
- managing education and visa applications
- tracking case stages and progress
- automatically generating required document checklists
- managing uploaded documents and their approval status
- managing language courses and student enrollments
- managing exams and results
- handling invoices and payments
- providing CMS functionality
- generating reports
- providing clients with access to their own cases, documents, courses and payments through a mobile application
- enforcing secure role-based access to business data

---

## 🏗️ Architecture

The application follows a layered client-server architecture.

```text
┌──────────────────────────────────────────────┐
│                  CLIENTS                     │
│                                              │
│   ASP.NET Core MVC        .NET MAUI          │
│       Web App           Mobile App           │
└───────────────┬──────────────┬───────────────┘
                │              │
                │ HTTP/HTTPS   │ JSON + JWT
                │              │
                ▼              ▼
┌──────────────────────────────────────────────┐
│              ASP.NET CORE API                │
│                                              │
│ Controllers                                  │
│ DTOs                                         │
│ Business Logic                               │
│ Authentication & Authorization               │
│ Validation                                   │
│ Services                                     │
└──────────────────────┬───────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────┐
│              INFRASTRUCTURE                  │
│                                              │
│ Entity Framework Core                       │
│ ASP.NET Identity                             │
│ Database Configuration                       │
│ Infrastructure Services                     │
└──────────────────────┬───────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────┐
│                 SQL SERVER                   │
│                                              │
│ CRM │ Cases │ Documents │ Education          │
│ Finance │ CMS │ Users │ Reports              │
└──────────────────────────────────────────────┘
```

### Architectural layers

| Layer | Responsibility |
|---|---|
| Presentation | Web and mobile user interfaces |
| API | REST endpoints and communication layer |
| Business / Domain | Domain entities and business rules |
| Infrastructure | EF Core, Identity and infrastructure services |
| Database | Persistent relational data storage |

---

## 📂 Solution Structure

```text
Victoria
│
├── Victoria.Backend
│   ├── Controllers
│   ├── DTOs
│   ├── Services
│   └── API configuration
│
├── Victoria.Domain
│   ├── Entities
│   ├── Enums
│   └── Domain models
│
├── Victoria.Infrastructure
│   ├── Entity Framework Core
│   ├── DbContext
│   ├── Entity configurations
│   ├── ASP.NET Identity
│   └── Infrastructure services
│
├── Victoria.Web
│   ├── MVC Controllers
│   ├── Views
│   ├── Razor
│   └── Web UI
│
└── Victoria.Mobile
    ├── .NET MAUI
    ├── Pages
    ├── Views
    └── API communication
```

---

# 🚀 Features

## 👥 CRM

The CRM module manages the initial relationship with potential customers.

### Features

- Lead creation and management
- Lead editing
- Lead status management
- Assignment of accounts/staff members
- Conversion of leads into case files
- Case ownership and responsibility management

---

## 📁 Case Management

Case Management is one of the core parts of the system.

### Features

- Create and edit case files
- Track case status
- Manage case stages
- Connect cases with education applications
- Connect cases with visa applications
- Automatically initialize required document checklists
- Verify case readiness
- Prevent invalid stage transitions

### Case lifecycle

```text
New
  │
  ▼
Planning
  │
  ▼
Application
  │
  ▼
Visa
  │
  ▼
Completed
```

A case cannot progress to a stage when required checklist conditions have not been satisfied.

---

## 📄 Document Management

The document module supports the complete document collection and verification process.

### Features

- Upload documents
- Store document metadata
- Associate documents with applications and cases
- Review uploaded documents
- Approve documents
- Reject documents
- Maintain document checklists
- Automatically update checklist status
- Track document completion

The system supports different document relationships, including:

```text
Case
 ├── Documents
 ├── Application Checklist
 └── Visa Checklist
```

---

## 🎓 Education Management

The education module manages language courses and students.

### Features

- Create and manage language courses
- Create course groups
- Manage students
- Manage enrollments
- Check group capacity
- Prevent duplicate enrollments
- Allow students to unenroll
- Manage exams
- Manage exam sessions
- Store exam results

### Enrollment workflow

```text
Student Login
     │
     ▼
Browse Courses
     │
     ▼
Select Group
     │
     ▼
Check Capacity
     │
     ▼
Create Enrollment
     │
     ▼
Enrollment Confirmed
```

---

## 💰 Finance

The finance module handles invoices and payments.

### Features

- Create invoices
- Manage invoice status
- Register payments
- Associate payments with invoices
- View financial information
- Provide students with access to their invoices
- Support financial reporting

---

## 📰 CMS

The built-in CMS allows administrators to manage public-facing content.

### Features

- News posts
- Informational pages
- Page sections
- Teachers
- Testimonials
- System settings

This allows content to be managed without changing the application code.

---

# 👤 User Roles

The system implements role-based authorization.

| Role | Access |
|---|---|
| **Admin** | Full system access |
| **Staff** | Operational access to customers, cases, documents, education and finance |
| **Student** | Access only to their own data |

### Administrator

Administrators can:

- manage users
- manage roles
- manage system settings
- manage CMS content
- manage courses and exams
- access reports
- access all business modules

### Staff

Staff members can:

- manage leads
- manage cases
- manage documents
- handle education and visa applications
- manage courses
- manage enrollments
- manage invoices and payments

### Student

Students can:

- view their own cases
- view document checklists
- view their own documents
- view course enrollments
- access course information
- view invoices and payments
- access news/content

Students are restricted from accessing data belonging to other users.

---

# 🔐 Authentication & Authorization

Authentication is implemented using:

- ASP.NET Identity
- JWT tokens
- role-based authorization
- `[Authorize]` attributes

### Authentication flow

```text
User
 │
 │ Login credentials
 ▼
ASP.NET Core API
 │
 │ Validate credentials
 ▼
ASP.NET Identity
 │
 │ Generate JWT
 ▼
JWT Token
 │
 ▼
Client Application
 │
 │ Authorization: Bearer <token>
 ▼
Protected API Endpoint
 │
 ▼
Role-based authorization
 │
 ▼
Business operation
```

The API returns appropriate HTTP status codes for unauthorized and forbidden requests, including:

- `401 Unauthorized`
- `403 Forbidden`

---

# 🗄️ Database

The system uses **Microsoft SQL Server** with **Entity Framework Core**.

Database development and management were performed using:

- Entity Framework Core migrations
- SQL Server
- SQL Server Management Studio
- LINQ

---

## Database Modules

### CRM

```text
Leads
CaseFiles
StudyApplications
VisaApplications
```

### Documents

```text
Documents
FileResources
ApplicationDocuments
VisaDocuments
ApplicationDocumentChecklists
VisaDocumentChecklists
CaseApplicationChecklistItems
CaseVisaChecklistItems
```

### Education

```text
LanguageCourses
CourseGroups
Students
Enrollments
Exams
ExamSessions
ExamResults
```

### Finance

```text
Invoices
Payments
```

### CMS

```text
NewsPosts
Pages
PageSections
Teachers
Testimonials
Settings
```

### Users

```text
AspNetUsers
AspNetRoles
AspNetUserRoles
```

---

## 🔗 Database Relationships

The database uses relational integrity through:

- primary keys
- foreign keys
- one-to-many relationships
- many-to-many relationships where required
- unique constraints
- indexes
- validation rules

Example:

```text
Lead
 │
 └── CaseFile
       │
       ├── StudyApplication
       │
       ├── VisaApplication
       │
       ├── Documents
       │
       └── Checklists
```

---

## 📐 Database Design

The database follows relational normalization principles including:

- First Normal Form (1NF)
- Second Normal Form (2NF)
- Third Normal Form (3NF)

Indexes are used for:

- primary keys
- foreign keys
- frequently searched fields
- commonly queried business data

The database also uses:

- views for selected financial, case and enrollment reporting
- stored procedures for reporting, bulk operations and selected business validations
- database functions for financial calculations, aggregation and statistics

---

# 🔄 Entity Framework Core

Entity Framework Core is responsible for the application's ORM layer.

It provides:

- object-relational mapping
- entity configuration
- migrations
- LINQ queries
- change tracking
- database access

The application uses EF Core to keep the domain model and SQL Server database synchronized through migrations and configuration.

---

# 🔌 REST API

The backend exposes RESTful endpoints consumed by both client applications.

Communication uses:

```text
HTTP / HTTPS
JSON
JWT Bearer Authentication
```

Example controller areas include:

```text
CaseFilesController
DocumentsController
EnrollmentsController
PaymentsController
```

The API supports standard CRUD operations where applicable:

```text
GET
POST
PUT
DELETE
```

Example API flow:

```text
Mobile/Web Client
      │
      │ HTTP Request
      ▼
ASP.NET Core Controller
      │
      ▼
Business Logic
      │
      ▼
Entity Framework Core
      │
      ▼
SQL Server
```

---

# 📱 Mobile Application

The mobile application is built with **.NET MAUI** and provides customers with access to their information.

### Main functionality

- Login
- Case overview
- Case status
- Document checklists
- Documents
- Language courses
- Course enrollments
- Invoices
- Payments
- News

The mobile application communicates with the backend exclusively through the REST API.

---

# 🌐 Web Application

The web application is built using:

- ASP.NET Core MVC
- Razor
- Bootstrap
- HttpClient

It provides the administrative and staff interface for managing the system.

### Main functionality

- CRM management
- Case management
- Document management
- Education management
- Finance
- CMS
- User management
- Reports
- CRUD interfaces
- Business workflow management

---

# 🧠 Business Logic

The application contains business rules responsible for maintaining process consistency.

Examples include:

### Case management

- case stage validation
- checklist requirements
- readiness checks
- controlled stage transitions

### Documents

- document approval/rejection
- checklist synchronization
- document status management

### Education

- group capacity validation
- duplicate enrollment prevention
- enrollment management

### Finance

- invoice and payment relationships
- financial status handling

---

# 🔁 Main Business Workflows

## Lead → Case

```text
New Lead
   │
   ▼
Lead Management
   │
   ▼
Lead Conversion
   │
   ▼
Case File Created
   │
   ▼
Application Checklists Initialized
```

---

## Document Review

```text
Student Uploads Document
          │
          ▼
      Document Stored
          │
          ▼
       Staff Review
        /       \
       /         \
  Approved      Rejected
      │             │
      ▼             ▼
Checklist       Requires
Updated         Correction
```

---

## Case Progression

```text
New
 │
 ▼
Planning
 │
 ├── Required checklist incomplete
 │        │
 │        └── Stage transition blocked
 │
 ▼
Application
 │
 ▼
Visa
 │
 ▼
Completed
```

---

## Course Enrollment

```text
Student
   │
   ▼
Login
   │
   ▼
Browse Courses
   │
   ▼
Select Group
   │
   ▼
Capacity Validation
   │
   ▼
Enrollment
```

---

## Payments

```text
Invoice Created
      │
      ▼
Payment Registered
      │
      ▼
Financial Status Updated
      │
      ▼
Student Can View Invoice
```

---

# 🧪 Testing

Testing covered the main functional, security and business requirements.

## Authentication Tests

Tested scenarios included:

- valid login
- invalid login
- access without authentication
- access with incorrect role
- JWT authorization
- `401 Unauthorized`
- `403 Forbidden`

---

## CRUD Testing

CRUD functionality was tested across the major modules, including:

- leads
- cases
- documents
- courses
- enrollments
- exams
- payments
- CMS content
- users

---

## Business Logic Testing

Test scenarios included:

- automatic checklist updates
- case stage restrictions
- document approval/rejection
- course capacity validation
- duplicate enrollment prevention
- business rule validation

---

## Mobile Testing

The mobile application was tested for:

- authentication
- case access
- checklist access
- course browsing
- enrollments
- invoice access
- REST API communication

---

## Database Testing

Database testing included:

- foreign key integrity
- unique constraints
- data consistency
- relational integrity
- concurrent operations

---

## Performance Testing

Basic performance tests were performed for:

- list operations
- API requests
- document uploads
- typical application load

The system performed smoothly under typical expected usage scenarios.

---

# 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| **C#** | Main programming language |
| **ASP.NET Core Web API** | Backend REST API |
| **ASP.NET Core MVC** | Web application |
| **.NET MAUI** | Cross-platform mobile application |
| **Entity Framework Core** | ORM / database access |
| **Microsoft SQL Server** | Relational database |
| **ASP.NET Identity** | Authentication and user management |
| **JWT** | API authentication |
| **Razor** | Web UI |
| **Bootstrap** | UI styling |
| **HttpClient** | HTTP communication |
| **Swagger** | API testing and documentation |
| **Visual Studio** | Development environment |
| **SQL Server Management Studio** | Database management |
| **Git** | Version control |

---

# 🧩 Design Patterns & Principles

The project uses several established software engineering approaches.

### Layered Architecture

Responsibilities are separated between:

```text
Presentation
API
Business / Domain
Infrastructure
Database
```

### MVC

The web application follows the Model-View-Controller pattern.

### Dependency Injection

ASP.NET Core Dependency Injection is used to provide application services and dependencies.

### Repository-like Data Access

Entity Framework Core provides the application's data access abstraction through `DbContext`, LINQ and entity configurations.

### Separation of Concerns

The project separates:

- UI
- API communication
- business logic
- domain models
- infrastructure
- persistence

This improves maintainability and allows individual components to evolve independently.

---

# 📈 Scalability & Extensibility

The architecture was designed to allow future development without restructuring the entire application.

Possible extensions include:

- new business modules
- additional REST endpoints
- external service integrations
- additional mobile functionality
- new reporting features
- infrastructure scaling
- caching
- monitoring

The shared API architecture also allows additional client applications to consume the same backend.

---

# 🧗 Engineering Challenges

Some of the main technical challenges addressed during development included:

### Module synchronization

Keeping CRM, cases, documents, education and finance synchronized required careful entity relationships and business rules.

### Business workflows

Case progression and document checklists required validation beyond simple CRUD operations.

### Authorization

Different roles require different levels of access, while students must only be able to access their own data.

### Mobile ↔ Backend integration

The mobile application required reliable communication with the REST API while maintaining authentication and consistent business data.

### Data integrity

The relational database needed to maintain consistent relationships between customers, cases, applications, documents, courses and financial records.

---

# 🔮 Future Improvements

The project can be further developed with additional automation and integrations.

## Business Features

- automated workflows
- external payment provider integration
- email notifications
- push notifications
- advanced analytics
- additional reporting

## Performance & Infrastructure

- caching
- query optimization
- additional database optimization
- containerization
- monitoring
- improved deployment infrastructure

## Mobile Application

- push notifications
- improved offline support
- enhanced document functionality
- additional customer self-service features

---

# 📊 Project Highlights

Victoria demonstrates practical experience with:

- full-stack .NET development
- ASP.NET Core Web API
- ASP.NET Core MVC
- .NET MAUI
- REST API design
- Entity Framework Core
- SQL Server
- relational database design
- authentication and authorization
- JWT
- ASP.NET Identity
- role-based access control
- business process implementation
- document workflows
- CRUD operations
- mobile/backend integration
- testing
- layered architecture
- dependency injection
- scalable application design

---

# 📌 Project Status

The core system has been implemented with:

- ✅ Backend REST API
- ✅ Web application
- ✅ Mobile application
- ✅ SQL Server database
- ✅ Authentication & authorization
- ✅ CRM
- ✅ Case Management
- ✅ Document Management
- ✅ Education Management
- ✅ Finance
- ✅ CMS
- ✅ Business workflows
- ✅ Role-based permissions
- ✅ API integration
- ✅ Testing of core functionality

The implementation covers the main functional and architectural requirements defined for the project.

---

# 📄 Documentation

The project was designed and implemented according to a detailed functional and technical specification covering:

- system goals
- functional requirements
- non-functional requirements
- architecture
- database design
- business processes
- implementation
- testing
- security
- scalability
- future development

---

# 👨‍💻 About the Project

Victoria was developed as a practical full-stack software engineering project focused on designing and implementing a complete business application rather than a simple CRUD system.

The project combines multiple technologies and application layers into one integrated platform, with a shared backend API serving both web and mobile clients.

It demonstrates the ability to design a system around real business processes, model complex relational data, implement authorization and business rules, and expose the functionality through multiple client applications.

---

# 📜 License

This project is provided for educational and portfolio purposes.
