# EasyClinic API
Medical clinic management and appointment booking RESTful API built with ASP.NET Core, evolving toward Clean Architecture and production-ready deployment.

> **Notice:** ⚠️ **Work in Progress (Under Active Development)**  
> This project is currently being developed as a practical, production-ready application alongside studying the course *"Mastering ASP.NET Core: Concepts to Production-Ready APIs"*.

---

## 📌 About The Project

**EasyClinic** is a RESTful Web API built with **.NET 9** designed to manage core clinical operations, including doctor schedules, patient profiles, medical services, and appointment bookings. 

The primary goal of this repository is to demonstrate modern backend practices, starting from foundational ASP.NET Core concepts to Clean Architecture, robust validation, containerization, and production readiness.

---

## 🛠 Tech Stack & Tools

- **Framework:** .NET 9 (ASP.NET Core Web API)
- **Database:** SQL Server
- **ORM:** Entity Framework Core 9 (Code-First)
- **Validation:** FluentValidation
- **Documentation:** OpenAPI / Swagger UI
- **Architecture:** Controller-based & Minimal APIs

---

## ✅ Progress & Implemented Features (Up to Module #18)

- [x] **Core Domain Models & Database Setup:**
  - Entities: `Doctor`, `Patient`, `Appointment`, `Specialization`, `MedicalService`, `AppointmentStatus`, `DoctorService`.
  - Configured `AppDbContext` and applied initial Entity Framework Core Migrations (including unique constraints).
- [x] **API Endpoints & Routing:**
  - Full CRUD operations implemented via **Controllers** (Doctors, Patients, Appointments, Medical Services).
  - Light-weight endpoints built using **Minimal APIs** (`SpecializationEndpoints`, `AppointmentStatusEndpoint`).
  - DTO Pattern applied for input/output isolation (`CreateDTOs`, `PatchDTOs`, `ResponseDTOs`).
- [x] **Request Pipeline & Middleware:**
  - Custom `ExceptionHandlingMiddleware` for standardized error responses.
  - Action Filters (`LoggingActionFilter`, `ValidationFilter`).
- [x] **Validation & Options Pattern:**
  - Strongly-typed `FluentValidation` integration for request payloads.
  - Configuration handling using `ClinicSettings` and `JwtSettings`.

---

## 🚧 Upcoming Roadmap (Future Modules)

- [ ] **Clean Architecture Refactoring:** Splitting the solution into `Domain`, `Application`, `Infrastructure`, and `API` projects.
- [ ] **Design Patterns:** Applying `Result Pattern` and `Domain Events`.
- [ ] **Authentication & Authorization:** JWT-based user roles and permissions.
- [ ] **Docker Integration:** Containerizing the API and SQL Server using `Dockerfile` and `docker-compose`.
- [ ] **CI Pipeline:** Automated build and test workflows using GitHub Actions.

---

## 🚀 How to Run Locally

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) installed.
- SQL Server instance (LocalDB or SQL Server Express).
