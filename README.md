# Sports Academy Pro

A comprehensive sports academy management web application built with ASP.NET Core 8 MVC, Entity Framework Core, and SQLite.

## Features

| # | Module | Description |
|---|--------|-------------|
| 1 | **Students** | Profile, photo, DOB, blood group, jersey size, batch assignment |
| 2 | **Attendance** | Daily attendance with batch filter, bulk mark, monthly reports |
| 3 | **Fees** | Monthly/Quarterly/Yearly fee records, student ledger |
| 4 | **Fitness Tests** | Admin-configurable tests (20m Sprint, Shuttle), multi-date results |
| 5 | **Movement Screening** | Deep Squat, Hurdle Step etc. with score tracking |
| 6 | **Skill Development** | Groups (Batting/Bowling) → Skills (Stance, Grip) with assessments |
| 7 | **Strength & Conditioning** | Groups (Warm-up/Strength) → Exercises with measurements |
| 8 | **Mental Toughness** | Confidence, Concentration, Decision Making scoring |
| 9 | **Teamwork & Leadership** | Communication, Leadership, Cooperation scoring |
| 10 | **Injury Tracking** | Log/track injuries with severity, treatment, return date |
| 11 | **Nutrition Tracking** | Daily meal logs with macros, nutrition goals per student |
| 12 | **AI Performance Scoring** | Composite 0-10 score with radar chart, athlete rankings |

## Tech Stack

- **Backend**: ASP.NET Core 8 MVC, C#
- **Database**: SQLite via Entity Framework Core 8
- **Frontend**: Bootstrap 5, Font Awesome 6, Chart.js, DataTables
- **Auto-seed**: Batches, fitness tests, skill groups, and more

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Run Instructions

```bash
cd SportsAcademy
dotnet restore
dotnet run
```

Then open: **http://localhost:5000**

The SQLite database (`sportsacademy.db`) is created automatically on first run with seed data.

## Project Structure

```
SportsAcademy/
├── Controllers/          # One controller per module
├── Data/
│   └── ApplicationDbContext.cs   # EF Core context + seed data
├── Models/               # Entity models
│   ├── Student.cs
│   ├── AttendanceRecord.cs
│   ├── FeePayment.cs
│   ├── FitnessTest.cs
│   ├── MovementScreening.cs
│   ├── SkillDevelopment.cs
│   ├── StrengthConditioning.cs
│   ├── MentalToughness.cs
│   ├── Teamwork.cs
│   ├── InjuryRecord.cs
│   └── Nutrition.cs
├── Views/                # Razor views per module
├── wwwroot/
│   ├── css/site.css      # Custom styles
│   └── js/site.js
└── Program.cs
```

## Seed Data Included

- **Batches**: Morning, Evening, Weekend
- **Fitness Tests**: 20m Sprint, 5-10-5 Shuttle, Vertical Jump
- **Movement Tests**: Deep Squat, Hurdle Step, Inline Lunge
- **Skill Groups**: Batting, Bowling, Fielding (with sample skills)
- **S&C Groups**: Warm-up, Strength, Conditioning (with exercises)
- **Mental Tests**: Confidence, Concentration, Decision Making, Resilience
- **Teamwork Tests**: Communication, Leadership, Cooperation, Sportsmanship

## AI Performance Score

Each athlete gets a composite score (0-10) computed as:

| Component | Weight |
|-----------|--------|
| Fitness Tests | 20% |
| Movement Screening | 15% |
| Skill Development | 20% |
| Strength & Conditioning | 15% |
| Mental Toughness | 15% |
| Teamwork & Leadership | 15% |
| Attendance Rate | 10% modifier |

Tiers: **Elite** (8.5+) | **Advanced** (7+) | **Developing** (5+) | **Beginner** (3+)
