# RaceDay - South African Event Management System

## Project Overview

RaceDay is a full-stack web application built to help manage road running, walking, and cycling events across South Africa. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day.

This is a combined submission for Part 1 (System Planning and Database) and Part 2 (RESTful API Development) of a three-part project.

---

## System Roles

### Event Organiser
- Create, update, and manage events
- Add different race categories (5km, 10km, Half Marathon, Full Marathon)
- View all participants who have signed up for events
- Record finishing times and positions
- Manage route information and course details
- View participant lists and generate reports

### Participant
- Browse and search for upcoming events
- Register for events and choose your category
- View your personal enrolment history
- Track your race results and personal bests
- View event details like routes, start times, and entry fees
- Withdraw from events if needed

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend API | C# / .NET 10 Web API |
| Database | SQL Server |
| ORM | Entity Framework Core |
| Authentication | JWT (JSON Web Tokens) |
| Password Hashing | BCrypt |
| API Testing | Swagger UI |
| Unit Testing | xUnit |
| CI/CD | GitHub Actions |
| Containerization | Docker (Part 3) |
| Cloud | Azure or AWS (Part 3) |

---

## Database Schema

The system uses 7 core tables:

| Table | What It Stores |
|-------|----------------|
| Roles | User types (Admin, Organiser, Participant) |
| Users | All system user accounts |
| Events | Race and event information |
| EventRoutes | Route details like distance, start/end points, and maps |
| Categories | Different race categories within an event |
| Enrolments | Who is registered for which event |
| Results | Finishing times and positions for participants |

---

## API Endpoints

### Authentication

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | /api/auth/register | Register a new user | Public |
| POST | /api/auth/login | Login and get JWT token | Public |

### Users

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | /api/users/profile | Get current user's profile | Logged in |
| PUT | /api/users/profile | Update current user's profile | Logged in |
| GET | /api/users/{id} | Get user by ID | Organiser, Admin |

### Events

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | /api/events | Get all events | Public |
| GET | /api/events/upcoming | Get upcoming events | Public |
| GET | /api/events/past | Get past events | Public |
| GET | /api/events/search | Search events | Public |
| GET | /api/events/{id} | Get event details | Public |
| POST | /api/events | Create a new event | Organiser |
| PUT | /api/events/{id} | Update an event | Organiser (owner) |
| DELETE | /api/events/{id} | Delete an event | Organiser (owner) |

### Categories

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | /api/categories/{id} | Get category by ID | Public |
| PUT | /api/categories/{id} | Update a category | Organiser (owner) |
| DELETE | /api/categories/{id} | Delete a category | Organiser (owner) |
| GET | /api/categories/{id}/results | Get results for a category | Public |

### Enrolments

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | /api/enrolments/me | Get my enrolments | Participant |
| POST | /api/events/{eventId}/enrolments | Enrol in an event | Participant |
| PUT | /api/enrolments/{id} | Update enrolment | Participant |
| DELETE | /api/enrolments/{id} | Withdraw from event | Participant |
| GET | /api/events/{eventId}/enrolments | Get event enrolments | Organiser (owner) |

### Results

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | /api/results/me | Get my results | Participant |
| GET | /api/events/{eventId}/results | Get event results | Public |
| POST | /api/enrolments/{enrolmentId}/results | Create a result | Organiser (owner) |
| PUT | /api/results/{id} | Update a result | Organiser (owner) |

### Routes

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | /api/events/{id}/route | Get route information | Public |
| PUT | /api/events/{id}/route | Update route information | Organiser (owner) |

---

## Setup Instructions

### 1. Clone the Repository
