# SodaAlert
This program saves and continuously monitors SoQL queries in the Socrata Open Data API (SODA).

Before you run SodaAlret install these dependencies:
    - dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    - dotnet add package Microsoft.EntityFrameworkCore.Design
    - dotnet add package Microsoft.EntityFrameworkCore.Tools

To start SodaAlert, run the command "dotnet run" in the directory SodaAlertService.



Some example queries for your JSON files:
$limit=5
$select=permit_,work_description,issue_date&$limit=5