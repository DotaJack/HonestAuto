Honest Auto ASP.NET MVC Web Application

Welcome to the Honest Auto ASP.NET MVC Web Application! This README file will guide you through the installation and configuration process for this project. Before you begin, please make sure you have the following software installed:

    Visual Studio 2022
    Microsoft SQL Server 2022
    SQL Server Management Studio (SSMS) 19

Getting Started

    Clone the Repository: Start by cloning this repository to your local machine using Git:

    bash

git clone https://github.com/DotaJack/HonestAuto

Open the Project: Launch Visual Studio 2022 and open the HonestAuto.sln solution file located in the root folder of the cloned repository.

Restore NuGet Packages: Right-click on the solution in Solution Explorer and select "Restore NuGet Packages" to ensure all required packages are downloaded and installed.

Database Setup:

a. Open SQL Server Management Studio (SSMS).

b. Connect to your SQL Server instance.

c. Create a new database for the Honest Auto application. You can use the following SQL script as an example:

sql

CREATE DATABASE HonestAutoDB;

d. Make sure you have the appropriate permissions to create and modify databases.

Configure Connection String:

a. Open the appsettings.json file located in the HonestAuto project folder within your ASP.NET MVC project.

b. Inside the appsettings.json file, find the "ConnectionStrings" section and replace the "MarketplaceContext" connection string with your specific configuration. Here's an example:

json

"ConnectionStrings": {
    "MarketplaceContext": "Data Source=localhost\\MSSQLSERVER03;Initial Catalog=HONESTAUTODB;Integrated Security=True;Persist Security Info=True;User ID=FYPADMIN;Password=test;Trust Server Certificate=True"
}

Make sure to replace the connection string value with your actual connection details if they differ.

Database Migration:

a. In Visual Studio, open the Package Manager Console by going to Tools > NuGet Package Manager > Package Manager Console.

b. Set the "Default Project" in the Package Manager Console to HonestAuto (the name of your main project).

c. Run the following command to generate a new migration based on the changes made to the application models:

bash

Add-Migration InitialMigration

Replace "InitialMigration" with a meaningful name for your migration if needed.

d. Apply the migration to the database using the following command:

bash

Update-Database

This command will create the necessary tables in your SQL Server database.

Run the Application:

a. Set the HonestAuto project as the startup project by right-clicking on it in Solution Explorer and selecting "Set as StartUp Project."

b. Press F5 or click the "Start" button to build and run the application.

Access the Application:

The application should now be running locally. You can access it in your web browser at http://localhost:port/, where port is the port number configured in your Visual Studio project settings.
