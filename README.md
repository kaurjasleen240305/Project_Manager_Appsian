# 🧩 Mini Project Manager — Minimal .NET 8 Web API

A lightweight and efficient **Project Management REST API** built with **.NET 8**, designed as part of the **Appsian Logical Assignment**.  
It provides a clean architecture, JWT authentication, and a simple SQLite database setup — ready to scale or deploy anywhere.

---

## 🚀 Features

- 🧠 **Project & Task Management** — Create and track projects with ease.  
- 🔐 **JWT Authentication** — Secure endpoints with token-based access.  
- 💾 **SQLite Integration** — Lightweight and easy-to-use database for local development.  
- ⚙️ **Minimal API Architecture** — Built on modern .NET 8 patterns.  
- 🌍 **Cross-platform** — Runs seamlessly on Windows, macOS, or Linux.  

---

## 🧰 Tech Stack

| Layer | Technology |
|:------|:------------|
| Backend | .NET 8 (C#) |
| Database | SQLite |
| Authentication | JWT (JSON Web Tokens) |
| ORM | Entity Framework Core |
| Deployment | Render |

---

## 🧑‍💻 Getting Started

### Prerequisites
Make sure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Git
- SQLite (optional; included by default)

---

### *Bonus Section - **
 - 👉 **[View Deployment on Render](https://project-manager-appsian.onrender.com/swagger/index.html)**

 - Implemented **SMART SCHEDULE API** by taking dependencies of each task and schedule them based on topological sort in Graph data structure algorithms.

---

### ⚙️ Setup Instructions

```bash
# 1️⃣ Clone the repository
git clone https://github.com/<your-username>/project-manager-app.git
cd project-manager-app

# 2️⃣ Restore dependencies
dotnet restore

# 3️⃣ Run the application
dotnet run
