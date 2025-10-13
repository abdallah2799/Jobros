
# 🧭 Project Guidelines – Jobros MVC Solution

## 🎯 Goal
Ensure the project remains organized, understandable, and easy to maintain by all team members. Everyone follows the same **naming conventions**, **folder structure**, and **coding standards**.

---

## 🧱 1. Solution Structure Overview
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

## ⚙️ 2. Responsibilities of Each Project

### 🧩 Core Project
> Contains all business contracts and domain logic abstractions.

**Folders:**
- **Entities:** Table models (e.g., User, Job, Application, Company).
- **DTOs:** Data Transfer Objects for communication between layers.
- **Enums:** Constant values (e.g., UserRole, JobType, ApplicationStatus).
- **Interfaces:** Definitions only (`IRepository`, `IUnitOfWork`, `IService`).

---

### 🧰 Infrastructure Project
> Handles data access and external services.

**Folders:**
- **Data:** Contains `ApplicationDbContext` (inherits from DbContext).
- **Configurations:** Entity configurations using `IEntityTypeConfiguration<T>`.
- **Repositories:** Implementations for interfaces in Core (e.g., `GenericRepository<T>`).
- **Logging:** Code related to logging and exception handling.

---

### 🧠 Application Project
> Contains business logic and service layer implementations.

**Folders:**
- **Services:** Business logic classes between UI and Infrastructure.
- **DTO Mappers:** Entity-to-DTO mapping (manual or AutoMapper).

---

### 🌐 UI Project (MVC)
> The user-facing layer.

**Folders:**
- **Controllers:** Handle requests and call services.
- **Models:** ViewModels only (used to bind data in views).
- **Views:** Razor pages (`.cshtml`).
- **wwwroot:** Frontend assets (CSS, JS, Bootstrap, etc.).

---

## 🧾 3. Coding Rules

| Type | Convention | Example |
|------|-------------|----------|
| Classes | PascalCase | JobService, UserRepository |
| Methods | PascalCase | GetAllJobs(), AddEmployer() |
| Variables | camelCase | jobTitle, applicationList |
| Interfaces | Start with I | IRepository, IService |
| Enums | PascalCase | JobStatus.Open, Role.Admin |
| Files | Match class name | JobRepository.cs, JobService.cs |

---

## 🧩 4. Folder Rules

| Location 	 | Should Contain 		         | Must NOT Contain 	  |
|--------------- |----------------------------------- |---------------------------|
| Core 		 | Interfaces, Entities, Enums, DTOs  | Implementation logic 	  |
| Infrastructure | Data, Repositories, Configurations | Views, Controllers 	  |
| Application 	 | Services, DTO Mappers		         | EF or UI code 		  |
| UI 		 | Controllers, Views, Models 	         | Business Logic or EF code |

---

## 🪄 5. Branching & Version Control

- **Main Branch:** Stable, no direct pushes.  
- **Develop Branch:** Base for new features.  
- Each member creates their branch:  
  ```
  feature/karim-ef
  feature/sohila-repo
  feature/mekkawi-services
  feature/arwa-ui
  ```

**Commit Format:**
```
feat: add job entity and configuration
fix: update JobService logic
refactor: simplify repository pattern
```

---

## 🧩 6. Error Handling & Validation

- Use `try/catch` for potential errors.
- Services handle logging.
- Controllers must validate `ModelState` before calling services.

---

## 💬 7. Communication Rules

- Any structure or naming change must be discussed first.
- Everyone pushes work to their feature branch daily.
- No merge to `Master` without review approval.

---

**End of Guidelines**
