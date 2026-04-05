# AcademicPlanner

AcademicPlanner is an Android mobile application built with .NET MAUI for students to track academic terms, courses, and assessments. The app allows users to create, edit, and delete terms, manage courses within each term, manage assessments within each course, view detailed course information, share course notes, and receive alerts for important dates.

## Overview

This project was developed as a term, course, and assessment planner for students. It supports:

- term management
- course management
- assessment management
- course notes sharing
- local notifications for selected course and assessment dates
- seeded evaluation data for testing

The app uses a local SQLite database through `sqlite-net-pcl` and is currently configured for Android.

## Features

### Terms
- Add a term
- Edit a term
- Delete a term
- Prevent overlapping term dates
- Display terms on the home page
- Show `(Current)` next to a term when the current date falls within that term’s date range

### Courses
- Add courses to a term
- Edit course information
- Delete a course
- View detailed course information
- Store:
  - course title
  - start date
  - end date
  - course status
  - instructor name
  - instructor phone
  - instructor email
  - notes
  - alert setting
- Share course notes using the device share sheet
- Open phone and email actions from the course overview page

### Assessments
- Add assessments to a course
- Edit assessment information
- Delete assessments
- Store:
  - assessment title
  - assessment type
  - start date
  - end date
  - alert setting
- Limit assessments so a course can only have:
  - one Objective assessment
  - one Performance assessment

### Notifications
- Course alerts can be set for:
  - start date
  - end date
  - both
- Assessment alerts can be set for:
  - start date
  - end date
  - both

## Tech Stack

- .NET MAUI
- C#
- XAML
- SQLite
- `sqlite-net-pcl`

## Project Structure

```text
AcademicPlanner/
│
├── Models/
│   ├── Term.cs
│   ├── Course.cs
│   └── Assessment.cs
│
├── Data/
│   └── AcademicPlannerDatabase.cs
│
├── Services/
│   ├── SeedDataService.cs
│   └── INotificationManagerService.cs
│
├── Views/
│   ├── TermsPage.xaml
│   ├── TermEditPage.xaml
│   ├── TermOverviewPage.xaml
│   ├── CourseEditPage.xaml
│   ├── CourseOverviewPage.xaml
│   ├── AssessmentEditPage.xaml
│   └── AppShell.xaml
│
├── Platforms/
│   └── Android/
│
└── Resources/
    ├── Images/
    └── AppIcon/
