# RaceDay - South African Event Management System

## Project Overview

RaceDay is a full-stack web application built to help manage road running, walking, and cycling events across South Africa. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day.

This repository contains the complete submission for **Part 1, Part 2, and Part 3** of the project.

---

## Project Breakdown

### Part 1: System Planning and Database
For Part 1, I focused on the foundational planning and database design:
- **Entity Relationship Diagram (ERD):** Designed a relational database with 7 core entities (Roles, Users, Events, EventRoutes, Categories, Enrolments, Results), including primary keys, foreign keys, and cardinality.
- **API Endpoint Plan:** Planned over 25 RESTful endpoints covering authentication, users, events, categories, enrolments, and results.
- **SQL Database Script:** Wrote a complete SQL script in SSMS to create the database, define all constraints, and seed it with realistic South African sample data.

### Part 2: RESTful API Development
For Part 2, I built the backend API that powers the system:
- **ASP.NET Core Web API:** Built using C# and .NET 10.
- **Entity Framework Core:** Connected the API to SQL Server using a Code-First approach.
- **Authentication & Authorization:** Implemented JWT (JSON Web Tokens) for secure login and BCrypt for password hashing. Enforced role-based access (Organiser vs. Participant).
- **Swagger Integration:** Documented and tested all endpoints.
- **Unit Testing:** Created an xUnit test project covering authentication and role validation.

### Part 3: Web Application & Deployment
For Part 3, I built the frontend and deployed the application:
- **ASP.NET Core MVC:** Built a responsive web interface consuming the Part 2 API.
- **Session Management:** Maintained user state and role-based navigation across the app.
- **Azure Blob Storage:** Integrated cloud storage so Organisers can upload event banner images.
- **Docker:** Containerized the API, MVC app, and SQL Database using `docker-compose`.

---

## System Roles

### Event Organiser
- Create, update, and manage events
- Upload event banner images (Azure Blob Storage)
- Add different race categories (5km, 10km, Half Marathon, etc.)
- View all participants who have signed up for events
- Record finishing times and positions

### Participant
- Browse and search for upcoming events
- Register for events and choose their category
- View personal enrolment history and generated Bib Numbers
- Track race results and personal bests

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend API | C# / .NET 10 Web API |
| Frontend | ASP.NET Core MVC |
| Database | SQL Server (Entity Framework Core) |
| Authentication | JWT & Session Management |
| Password Hashing | BCrypt |
| Cloud Storage | Azure Blob Storage |
| Testing | xUnit & Swagger UI |
| CI/CD | GitHub Actions |
| Containerization | Docker & Docker Compose |

---

## Database Schema

The system uses 7 core tables:

| Table | What It Stores |
|-------|----------------|
| Roles | User types (Admin, Organiser, Participant) |
| Users | All system user accounts (hashed passwords) |
| Events | Race and event information (including banner URL) |
| EventRoutes | Route details like distance, start/end points |
| Categories | Different race categories within an event |
| Enrolments | Who is registered for which event |
| Results | Finishing times and positions for participants |

---

## Setup Instructions (Local Development)

1. **Clone the Repositories:**
   ```bash
   git clone https://github.com/Rolimukz/RaceDay.API.git
   git clone https://github.com/Rolimukz/RaceDay1.git
