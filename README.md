# HonestAuto ASP.NET Core Web Application

This is a sample ASP.NET Core web application for managing a marketplace of cars, mechanics, and user interactions. It includes features for user registration, car evaluations, messaging, and more.

## Getting Started

Follow the instructions below to set up and run the application on your local development environment.

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (optional)

### Installation

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/DotaJack/HonestAuto

   Open the appsettings.json file in the project and configure your database connection string:

json

{
  "ConnectionStrings": {
    "MarketplaceContext": "your-connection-string-here"
  },
  // ...
}
E.G mine is {
  "ConnectionStrings": {
    "MarketplaceContext": "Data Source=localhost\\MSSQLSERVER03;Initial Catalog=HONESTAUTODB;Integrated Security=True;Persist Security Info=True;User ID=FYPADMIN;Password=test;Trust Server Certificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
User FYPADMIN is the user I created in SQL Server Management Studio, you can create your own user and password and use that instead.
  Sources: https://stackoverflow.com/questions/22368726/how-to-combine-two-models-into-a-single-model-and-pass-it-to-view-using-asp-net
  https://stackoverflow.com/questions/50820275/how-to-use-idesigntimedbcontextfactory-implementation-in-asp-net-core-2-1