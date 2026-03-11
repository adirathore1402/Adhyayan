# Adhyayan (अध्ययन) — Curriculum Alignment System

A full-stack application for Indian school children (Classes 1–5) that provides AI-powered practice questions aligned with **CBSE** and **5 state board** curricula.

## Features

- **Multi-board support** — CBSE, Maharashtra, Karnataka, Tamil Nadu, Uttar Pradesh, Madhya Pradesh
- **NCERT-aligned curriculum** — Math, English, Environmental Studies for Classes 1–5
- **AI question generation** — OpenAI-powered MCQ creation, age-appropriate and original
- **Chapter-based practice** — Pick board → grade → subject → chapter and practice
- **Cross-board concept mapping** — Understand equivalent topics across different boards
- **Parent dashboard** — Track children's progress with per-chapter accuracy
- **JWT authentication** — Secure parent accounts with token-based auth

## Tech Stack

| Layer          | Technology                              |
|----------------|-----------------------------------------|
| Backend API    | ASP.NET Core (.NET 10), C#              |
| Database       | SQLite via Entity Framework Core        |
| Frontend       | React 18, TypeScript, Vite              |
| Authentication | JWT Bearer tokens                       |
| AI             | OpenAI API (gpt-4o-mini)               |

## Project Structure

```
Adhyayan/
├── curriculum/              # JSON curriculum config files
│   ├── cbse_class1–5.json   # NCERT-aligned chapters & topics
│   └── state_board_mappings.json
├── src/
│   ├── Adhyayan.Core/       # Domain models, interfaces, DTOs
│   ├── Adhyayan.Infrastructure/  # EF Core, repositories, services
│   ├── Adhyayan.Api/        # ASP.NET Core Web API controllers
│   └── Adhyayan.Web/        # React + Vite frontend
└── Adhyayan.slnx            # .NET solution file
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

### Setup

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd Adhyayan
   ```

2. **Configure secrets**

   Create `src/Adhyayan.Api/appsettings.Development.json`:
   ```json
   {
     "Jwt": {
       "Key": "YourSecretKeyAtLeast32Characters!!"
     },
     "OpenAI": {
       "ApiKey": "sk-your-openai-api-key"
     }
   }
   ```

3. **Run the API**
   ```bash
   cd src/Adhyayan.Api
   dotnet run
   ```
   The API starts at `http://localhost:5000`. On first run it auto-creates the SQLite database and seeds all curriculum data.

4. **Run the frontend**
   ```bash
   cd src/Adhyayan.Web
   npm install
   npm run dev
   ```
   Opens at `http://localhost:5173` and proxies API calls to the backend.

## API Endpoints

| Method | Endpoint                                    | Auth | Description                    |
|--------|---------------------------------------------|------|--------------------------------|
| POST   | `/api/auth/register`                        | No   | Register a parent account      |
| POST   | `/api/auth/login`                           | No   | Login and receive JWT token    |
| GET    | `/api/curriculum/boards`                    | No   | List all boards                |
| GET    | `/api/curriculum/boards/{id}/grades`        | No   | Grades for a board             |
| GET    | `/api/curriculum/subjects`                  | No   | List all subjects              |
| GET    | `/api/curriculum/chapters?boardId&gradeId&subjectId` | No | Chapters with topics |
| POST   | `/api/questions/generate`                   | Yes  | AI-generate questions          |
| POST   | `/api/practice/start`                       | Yes  | Start a practice session       |
| POST   | `/api/practice/{id}/answer`                 | Yes  | Submit an answer               |
| POST   | `/api/practice/{id}/complete`               | Yes  | Complete a session             |
| GET    | `/api/dashboard/children`                   | Yes  | List parent's children         |
| POST   | `/api/dashboard/children`                   | Yes  | Add a child                    |
| GET    | `/api/dashboard/children/{id}/progress`     | Yes  | Child's practice progress      |

## License

MIT
