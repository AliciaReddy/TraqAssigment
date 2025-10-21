# Human Ledger - Financial Management System

A **modern, full-featured financial management system** built with **ASP.NET Core MVC**.  
Human Ledger provides comprehensive tools for managing people, accounts, and transactions with a **beautiful, professional, and user-friendly interface**.

---

## Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
- [Database Setup](#database-setup)
- [Business Rules](#business-rules)
- [Developer](#developer)
- [License](#license)

---

## Features

### Core Functionality
- **Person Management** – Create, edit, view, and delete person records  
- **Account Management** – Manage multiple accounts per person with real-time balance tracking  
- **Transaction Management** – Complete audit trail of all financial activities  
- **Secure Authentication** – Cookie-based authentication with password hashing  
- **Advanced Search** – Search by ID number, surname, or account number  
- **Account Status Management** – Open/Close accounts 

### User Experience
- Beautiful gradient backgrounds with overlay effects  
- Modern card-based layout with **navy, yellow, and white** theme  
- Smooth animations and transitions  
- Intuitive navigation flow  
- Real-time form validation  
- Professional typography using **Poppins** font  

---

## Tech Stack

### Backend
- **Framework:** ASP.NET Core 9.0 MVC  
- **Language:** C# 13.0  
- **ORM:** Entity Framework Core 9.0  
- **Database:** SQL Server  
- **Authentication:** Cookie-based authentication with ASP.NET Core Identity  

### Frontend
- **UI Framework:** Bootstrap 5  
- **Icons:** Bootstrap Icons 1.11.3  
- **Template Engine:** Razor Views  
- **Styling:** Custom CSS with modern design system  
- **Fonts:** Google Fonts (Poppins)  

### Development Tools
- **IDE:** Visual Studio 2022  
- **Version Control:** GitHub  
- **Package Manager:** NuGet  

---

## Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/HumanLedger.git
   cd HumanLedger
2. **Open the project**
   - Open HumanLedger.sln in Visual Studio 2022
3. **Restore NuGet Packages**
   - dotnet restore
4. **Build project**
   - dotnet build
  
## Database Setup

1. **Update connection string in appsettings.json **
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HumanLedgerDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
2. **Run Migrations**
   - dotnet ef database update
3. Verify the tables in SQL Server Management Studio (SSMS).

## Business Rules

- Each person can own multiple accounts
- Account numbers are unique per person
- Transactions automatically adjust account balances
- Accounts must be Open before transactions can be performed
- Deleting a person removes all associated accounts and transactions (with confirmation)
- Login credentials are hashed using bcrypt

## Developer 
Name: Alicia Reddy 
Email: aliciareddy1911@icloud.com
LinkedIn: linkedin.com/in/alicia-reddy

## License
This project is licensed under the MIT License.

