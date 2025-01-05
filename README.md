# Family Finance App

A web application designed to help families manage their finances. The app allows users to track their income and expenses, set savings goals, view financial reports, and export data to Excel. It also includes features for managing family groups, where the head of the family can control who is part of the group and oversee their financial activities.

## Features

- **Track Transactions**: Add, edit, and delete transactions (income and expenses).
- **Manage Family Groups**: The head of the family can create, manage, and invite members to the family group.
- **Set Financial Goals**: Define and monitor savings goals for the family.
- **Budgets & Limits**: Set spending limits and track progress.
- **Generate Reports**: View financial summaries and transaction histories.
- **Excel Export**: Export transaction data based on date filters and family groups.
- **Responsive Design**: Optimized for both desktop and mobile devices.

## Technologies Used

- **ASP.NET 8**: The backend framework for building the application.
- **Entity Framework Core**: For interacting with the SQL Server database.
- **Bootstrap**: For responsive and modern front-end design.
- **ExcelPackage (EPPlus)**: To export transaction data to Excel.
- **SQL Server**: The relational database for storing transaction and family data.

## Installation

To get started with the project, follow these steps:

### Prerequisites

Make sure you have the following installed on your machine:

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or a compatible relational database)
- [Visual Studio](https://visualstudio.microsoft.com/) or any other preferred IDE

### Clone the repository

```
git clone https://github.com/michalkuzdowicz/Family_Finance.git
cd Family_Finance
```

### Set up the database
1. Configure your appsettings.json to connect to your local SQL Server database.
2. Run the migrations to set up the database schema:
```
dotnet ef database update
```

### Running the Application
After setting up the database, you can run the application:

```
dotnet run
```

Visit https://localhost:7265 to start using the app.

## Usage
### Managing Transactions
- **Add Transaction**: Add income or expense transactions with categories, amounts, and descriptions.
- **View Transactions**: Filter transactions by date range and family group.
- **Edit/Delete Transactions**: Modify or remove existing transactions.
### Managing Family Groups
- **Create a Family Group**: The head of the family can create a group.
- **Invite Family Members**: Send invitations to other users to join the family group.
- **Assign Roles**: Assign roles such as 'Head' to a user within the family group.
### Reports
- **Generate Financial Reports**: Get summaries of income, expenses, and balances.
- **Export to Excel**: Export transaction data to Excel, with filters for date and family group.

## Contributing
Contributions are welcome! If you would like to improve the project, feel free to fork the repository, make your changes, and submit a pull request.
Please follow the coding standards and include tests for new features or fixes.

## License
This project is licensed under the MIT License - see the LICENSE file for details.
