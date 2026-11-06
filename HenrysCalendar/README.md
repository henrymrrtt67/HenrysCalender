# Henry's Calendar

ASP.NET Core 8 (Razor Pages) + EF Core + SQLite. Melbourne time.

## Run
1. Install the .NET 8 SDK (https://dotnet.microsoft.com/download)
2. In this folder: `dotnet run`
3. Open the https://localhost URL shown in the console.

Open the folder in Visual Studio 2022 or VS Code (C# Dev Kit) to edit.

## Rules (in Services/BookingService.cs)
- Stored in UTC, entered/displayed in Australia/Melbourne
- Any duration, but must end by midnight (ending exactly at 12:00 AM is OK)
- One booking per slot: overlapping bookings are rejected
- Past times rejected; cancelled bookings free their slot

## Next steps
- [ ] Admin page (login) to see names/emails and cancel bookings
- [ ] Confirmation emails (MailKit or Azure Communication Services)
- [ ] Working hours / days off / buffer time
- [ ] EF migrations + SQL Server / Azure SQL
- [ ] Deploy to Azure App Service
- [ ] Unit tests for BookingService rules (xUnit)
