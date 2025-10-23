# 💼 Jobros – Job Portal System

## 📖 Overview
**Jobros** is a job portal web application developed as part of the ITI Full Stack .NET track.  
It connects **job seekers** with **employers**, allowing them to create profiles, post and apply for jobs, and manage applications in a structured, scalable system.

The project follows **Onion Architecture**, ensuring clean separation between business logic, data access, and UI for maintainability and scalability.

---

## 🧱 Architecture Overview

```
Jobros.sln
│
├── Core
│ ├── Entities
│ ├── DTOs
│ ├── Enums
│ ├── Interfaces
│
├── Infrastructure
│ ├── Data
│ ├── Configurations
│ ├── Repositories
│ ├── Logging
│
├── Application
│ ├── Services
│ ├── DTO Mappers
│
└── UI (ASP.NET MVC)
├── Controllers
├── Models
├── Views
├── wwwroot
├── appsettings.json
```

---

## ⚙️ Technologies Used

| Layer | Technology |
|--------|-------------|
| Core | .NET 9.0, C#, DTOs, Interfaces |
| Infrastructure | Entity Framework Core, SQL Server |
| Application | AutoMapper, Dependency Injection |
| UI | ASP.NET Core MVC, Bootstrap 5, jQuery |

---

## 👥 Team Members

| Member | Role | Responsibilities |
|--------|------|------------------|
| **Abdullah Ragab** | **Team Lead** | Project planning and task assignment, setting up architecture and repository on GitHub, database design (ERD), defining repository and UnitOfWork interfaces, implementing authentication module (service, controller, and integration with views for login, register, and password recovery), configuring email service using **SendGrid API**, implementing reporting and export service, integrating **Serilog** for structured logging, guiding the team in debugging and resolving repository conflicts. |
| **Karim Mohammed Reda** | **Backend Developer** | Implemented database using **Entity Framework Core (Code First)**, integrated Identity tables, developed the **Admin Module** (DTOs, services, controllers), and ensured role management and data consistency. |
| **Sohila Salah** | **Backend Developer** | Implemented the **IRepository** and **IUnitOfWork** interfaces prepared earlier, developed the **Employer Module** (DTOs, services, controllers), and contributed to backend logic refinement. |
| **Mohammed Essam Mekkawi** | **Full Stack Developer** | Developed the **Job Seeker Module** (controllers, services, and views), connecting front-end components with application services. |
| **Arwa Yahia** | **Frontend Developer** | Designed and implemented front-end **views** for Home, Landing Page, Static Pages, Admin Pages, and Employer Pages, ensuring responsive layout and consistent UI design. |

---

## 🚀 Features (MVP Scope)

- User Registration & Authentication (Job Seeker / Employer / Admin)
- Job Posting and Management by Employers
- Job Search and Apply for Job Seekers
- Application Management (Admin + Employer)
- Email Notifications (SendGrid)
- Reporting and Export (Excel/PDF)
- Logging and Health Checks with Serilog

---

## 🧩 Design Patterns Used
- **Repository Pattern** – For clean data access abstraction.  
- **Unit of Work Pattern** – For consistent transaction management.  
- **Dependency Injection** – For loose coupling between layers.  
- **DTO Pattern** – For clear separation between domain and presentation models.

---

## 🪄 Coding Standards
Refer to [`ProjectGuidelines.md`](./ProjectGuidelines.md) for:
- Folder structure responsibilities  
- Naming conventions  
- Branching and communication rules  

---

## 🧰 Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/abdallah2799/Jobros.git
2. Open the solution in Visual Studio 2022.

3. Add appsettings.json with your SQL Server connection string.

4. Apply migrations:
   `Update-Database`
   `dotnet run`

## 📅 Project Duration

10 Days (October 13 – October 23, 2025)
Developed as part of the ITI Full Stack .NET Mini Project

## 🧑‍💻 License

This project was developed for educational purposes under the Information Technology Institute (ITI) program.
Not intended for commercial use.

## Developed by the ITI Full Stack .NET Team (Abdullah Ragab, Karim Mohammed Reda, Sohila Salah, Mohammed Essam Mekkawi, Arwa Yahia)

---