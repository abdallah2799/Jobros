
# 💼 Jobros – Job Portal System

## 📖 Overview
**Jobros** is a job portal web application developed as part of the ITI Full Stack .NET track.  
It aims to connect **job seekers** with **employers**, allowing them to create profiles, post and apply for jobs, and manage applications in a structured, scalable system.

The project follows **Onion Architecture**, ensuring clean separation between business logic, data access, and UI.

---

## 🧱 Architecture Overview

```
Jobros.sln
│
├── Core
│   ├── Entities
│   ├── DTOs
│   ├── Enums
│   ├── Interfaces
│
├── Infrastructure
│   ├── Data
│   ├── Configurations
│   ├── Repositories
│   ├── Logging
│
├── Application
│   ├── Services
│   ├── DTO Mappers
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
| **Abdullah Ragab** | Team Lead | Architecture, Database Design (ERD), Interfaces |
| **Karim Reda** | Developer | Entity Framework Implementation (Code First), Seed Data |
| **Sohila Salah** | Developer | Repositories, Admin Controller, Query Optimization |
| **Mohamed Mekkawi** | Developer | Services, Job Seeker & Employer Controllers |
| **Arwa Yehia** | UI & Docs | Front-End Views, Documentation |

---

## 🚀 Features (MVP Scope)

- User Registration & Authentication (Job Seeker / Employer / Admin)
- Job Posting by Employers
- Job Search & Apply for Job Seekers
- Application Management (Admin + Employer)
- Email Notifications (optional)
- Reporting and Export (optional future enhancement)

---

## 🧩 Design Patterns Used
- **Repository Pattern** – For clean data access.  
- **Unit of Work Pattern** – For transaction management.  
- **Dependency Injection** – For loose coupling.  
- **DTO Pattern** – For separating domain and presentation models.

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
   ```

2. Open the solution in **Visual Studio 2022**.

3. Add `appsettings.json` with your SQL Server connection string.

4. Apply migrations:
   ```bash
   Update-Database
   ```

5. Run the project:
   ```bash
   dotnet run
   ```

---

## 📅 Project Duration
> **10 Days** (ITI Mini Project – Full Stack .NET)

---

## 🧑‍💻 License
This project is developed for educational purposes as part of the **Information Technology Institute (ITI)** program.  
Not intended for commercial use.

---
**Developed with by ITI Full Stack .NET Team ( Abdullah Ragab , Karim Reda , Mohammed Mekkawy , Sohila Salah , Arwa Yahia )**
