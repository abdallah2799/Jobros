
# 🧭 Jobros Database Design (Story + ERD + Entities)

## 📖 Database Story

The **Jobros** system connects **job seekers**, **employers**, and **admins** under one platform.  
It starts with a single **User** table that represents all accounts.  
Depending on their role, users can be:

- **Admin** – Manages the whole system.
- **Employer** – Creates company profiles and posts job offers.
- **Job Seeker** – Builds a professional profile and applies for jobs.

Each user may have one and only one specialized profile:

- **EmployerProfile**: Holds company details such as name, industry, and location.
- **JobSeekerProfile**: Holds resume information, skills, and experience.

Employers create **Jobs**, which belong to a specific **Category**.  
Job seekers apply for those jobs using the **Application** table, which tracks status and timestamps.

---

## 🧩 ERD Relationships Overview

```
 User
 ├── EmployerProfile (1:1)
 │     └── Job (1:N)
 │           └── Application (1:N)
 │
 └── JobSeekerProfile (1:1)
       └── Application (1:N)

 Category (1:N) → Job
```

---

## 🧱 Entities Breakdown

### 1. User
**Purpose:** Represents the account of any system user.

| Property | Type | Description |
|-----------|------|-------------|
| ID | int | Primary Key |
| FullName | nvarchar(150) | User's full name |
| Email | nvarchar(200) | Unique email for login |
| PasswordHash | nvarchar(max) | Hashed password |
| Role | enum(Admin, Employer, JobSeeker) | Defines the user type |
| IsActive | bit | Account activation flag |
| CreatedAt | datetime2 | Account creation date |

**Relations:**
- 1:1 → EmployerProfile
- 1:1 → JobSeekerProfile

---

### 2. EmployerProfile
**Purpose:** Stores company or employer details.

| Property | Type | Description |
|-----------|------|-------------|
| ID | int | Primary Key |
| UserID | int | FK → User.ID |
| CompanyName | nvarchar(200) | Company’s official name |
| Industry | nvarchar(100) | Business industry |
| Website | nvarchar(200) | Company website |
| Location | nvarchar(150) | Company location |
| Description | nvarchar(max) | Summary about the company |
| CreatedAt | datetime2 | Creation timestamp |

**Relations:**
- 1:N → Job
- 1:1 → User

---

### 3. JobSeekerProfile
**Purpose:** Represents job seekers’ personal profiles.

| Property | Type | Description |
|-----------|------|-------------|
| ID | int | Primary Key |
| UserID | int | FK → User.ID |
| Bio | nvarchar(max) | About section |
| Skills | nvarchar(max) | List of skills |
| ExperienceYears | int | Number of experience years |
| ResumeUrl | nvarchar(400) | Resume file link |
| CreatedAt | datetime2 | Creation timestamp |

**Relations:**
- 1:N → Application
- 1:1 → User

---

### 4. Category
**Purpose:** Defines job fields such as IT, Sales, or Marketing.

| Property | Type | Description |
|-----------|------|-------------|
| ID | int | Primary Key |
| Name | nvarchar(100) | Unique category name |
| Description | nvarchar(max) | Optional details |

**Relations:**
- 1:N → Job

---

### 5. Job
**Purpose:** Represents an available job posted by an employer.

| Property | Type | Description |
|-----------|------|-------------|
| ID | int | Primary Key |
| EmployerID | int | FK → EmployerProfile.ID |
| CategoryID | int | FK → Category.ID |
| Title | nvarchar(250) | Job title |
| Description | nvarchar(max) | Job description |
| Requirements | nvarchar(max) | Job requirements |
| SalaryRange | nvarchar(100) | Optional salary info |
| JobType | enum(FullTime, PartTime, Internship, Remote) | Work type |
| Location | nvarchar(200) | Job location |
| IsActive | bit | Whether job is open or closed |
| CreatedAt | datetime2 | Creation timestamp |
| ExpirationDate | datetime2 | Job expiry date |

**Relations:**
- N:1 → EmployerProfile  
- N:1 → Category  
- 1:N → Application  

---

### 6. Application
**Purpose:** Tracks job applications submitted by job seekers.

| Property | Type | Description |
|-----------|------|-------------|
| ID | int | Primary Key |
| JobID | int | FK → Job.ID |
| JobSeekerID | int | FK → JobSeekerProfile.ID |
| Status | enum(Pending, Reviewed, Accepted, Rejected) | Application status |
| AppliedAt | datetime2 | Application timestamp |
| CoverLetter | nvarchar(max) | Optional cover letter |

**Relations:**
- N:1 → Job  
- N:1 → JobSeekerProfile  

---

## 🔑 Keys Summary

| Type | Table | Columns | Description |
|------|--------|----------|-------------|
| PK | User | ID | Primary key for all users |
| FK | EmployerProfile | UserID → User.ID | 1:1 user-company mapping |
| FK | JobSeekerProfile | UserID → User.ID | 1:1 user-jobseeker mapping |
| PK | Category | ID | Primary key for category |
| FK | Job | EmployerID → EmployerProfile.ID | Employer’s job link |
| FK | Job | CategoryID → Category.ID | Job’s category link |
| FK | Application | JobID → Job.ID | Job link |
| FK | Application | JobSeekerID → JobSeekerProfile.ID | Job seeker link |

---

## 🧩 Concept Summary

- **User** is the root entity.  
- **EmployerProfile** and **JobSeekerProfile** are branches that extend user roles.  
- **Job** belongs to **EmployerProfile** and **Category**.  
- **Application** connects **Job** and **JobSeekerProfile**.  
- **Category** groups multiple jobs logically.

---

**This structure ensures scalability:**  
You can later add tables for `Notifications`, `SavedJobs`, or `Reviews` without breaking existing relationships.
