\# RaceDay Website (Part 3)



RaceDay is a race-entry system for runs, walks and charity cycles. This repository holds the \*\*ASP.NET Core MVC website\*\*. It talks to the RaceDay API, which is in a separate repository: https://github.com/ST10488539/RaceDay-API-poe



\## What it does



\*\*Participants\*\* can register, log in, browse and filter events (with banner images), view event details, enrol by choosing a category, see their enrolments and results, and edit their profile with a profile picture.



\*\*Organisers\*\* can see a dashboard with enrolment counts, create, edit and delete their own events, upload an event banner, manage categories, view enrolments, and enter or edit results.



\## How it fits together



Browser -> RaceDay.Web (MVC website) -> RaceDay API -> SQL Server



Images go from the website to the API, which stores them in Azure Blob Storage. The website never talks to the database or to Azure directly.



\## Roles and login



A user picks a role (Participant or Organiser) when registering, and the API stores it. When someone logs in, the API returns their role and the website keeps it in the session. Every organiser page checks the role, so a participant who types an organiser address is sent back to the login page. The API enforces the same rules with 401 (not logged in) and 403 (wrong role) responses.



\## Running it locally



You need the .NET 10 SDK, Visual Studio, SQL Server Express and an Azure Storage account (for images).



1\. \*\*API:\*\* clone the API repository and open its solution. In `RaceDay/appsettings.json`, change the `Server=` part of the connection string to your own SQL Server instance. In the `RaceDay` project folder, create the database with:



```

dotnet ef database update

```



&#x20;  Store the Azure Storage connection string in user secrets, so it never goes into a file:



```

dotnet user-secrets set "AzureStorage:ConnectionString" "your-connection-string"

```



&#x20;  Start the API. It listens on `https://localhost:7056`, and Swagger is on the same address.



2\. \*\*Website:\*\* open `RaceDay.Web.slnx`, check that the API address in `RaceDay.Web/appsettings.json` matches the one above, and start the project. It opens on `https://localhost:7237`.



3\. Register one participant and one organiser to try both sides.



\## Tests



The API repository contains 56 automated integration tests covering authentication, sessions, roles, events, categories, enrolments and results. Run them with `dotnet test` in the API solution folder.



\## Design decisions



\- The role is chosen at registration. Anyone can register as an organiser, which suits this assignment. A real system would approve organisers first.

\- Organisers can only change their own events, categories and results.

\- Enrolment checks the participant's age on the event day against the category, and allows one entry per event.

\- The API calculates pace from the chip time (or the finish time if there is no chip time).

\- Images (JPG, PNG or WEBP, up to 5 MB) are validated by the API and stored with public read access, so the website can display them.

\- Secrets such as the Azure connection string are kept in user secrets and are never committed.



\## Differences from the Part 1 design



The final system adds `Distance` to events and `ProfilePictureURL` to users. It also keeps the extra columns from my original database (`Gender`, `ImageURL`, `ChipTime`, `Humidity` and others), and adds participant, route and weather endpoints.

\## youtube video link

https://youtu.be/O1kUTXkw6tA


\## Credits



The welcome page image is a free-to-use stock photo.

