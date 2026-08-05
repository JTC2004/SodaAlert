# SodaAlert
This continuously monitors Socrata Open Data API (SODA) endpoints by reading SoQL queries and endpoint URLs from JSON files in the JSONs directory.

Before you run SodaAlert, make sure you have .NET installed, and install the following dependencies in the root of the project:
    - dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    - dotnet add package Microsoft.EntityFrameworkCore.Design
    - dotnet add package Microsoft.EntityFrameworkCore.Tools

How to use:
    - Some sample JSON files are in the JSONs folder, but you can add your own JSON files with the same format. Just be sure to specify and endpoint, SoQL query, and amount of seconds between when SodaAlert checks.

    - To start SodaAlert, run the command "dotnet run" in the directory SodaAlertService.

FYI: SodaAlert currently doesn't monitor each endpoint on separate time intervals. It instead uses the amount of seconds from the first file read for all JSONS.

Some example queries for your JSON files:
    - $limit=5
    - $select=permit_,work_description,issue_date&$limit=5