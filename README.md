# Horse Racing Database System - .NET 8 Web API

## Project Structure

```
backend/
├── Controllers/
│   └── StablesController.cs       # Sample API controller for Stables
├── Data/
│   └── RacingDbContext.cs         # Entity Framework DbContext
├── Models/
│   ├── Stable.cs                  # Stable entity
│   ├── Horse.cs                   # Horse entity
│   ├── Owner.cs                   # Owner entity
│   ├── Owns.cs                    # Owns relationship (composite key)
│   ├── Trainer.cs                 # Trainer entity
│   ├── Track.cs                   # Track entity
│   ├── Race.cs                    # Race entity
│   └── RaceResults.cs             # RaceResults entity (composite key)
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration including connection string
└── backend.csproj                # Project file with dependencies
```

## Features Implemented

### ✅ Models (POCOs)
- **Stable**: StableId (PK), StableName, Location, Colors
- **Horse**: HorseId (PK), HorseName, Age, Gender, Registration, StableId (FK)
- **Owner**: OwnerId (PK), LastName, FirstName
- **Owns**: OwnerId + HorseId (Composite PK)
- **Trainer**: TrainerId (PK), LastName, FirstName, StableId (FK)
- **Track**: TrackName (PK), Location, Length
- **Race**: RaceId (PK), RaceName, TrackName (FK), RaceDate, RaceTime
- **RaceResults**: RaceId + HorseId (Composite PK), Results, Prize

### ✅ RacingDbContext
- DbSet properties for all 8 entities
- Composite key configuration for `Owns` and `RaceResults`
- Foreign key relationships configured with proper cascade/set null behaviors
- Navigation properties set up for all relationships

### ✅ Program.cs Configuration
- **Entity Framework Core**: Configured with Pomelo MySQL provider
- **Connection String**: Read from appsettings.json
- **CORS Policy**: "AllowMyFrontend" allows all origins, methods (GET, POST, PUT, DELETE), and headers
- **Swagger/OpenAPI**: Enabled for API documentation and testing

### ✅ Sample Controller
- `StablesController` demonstrates full CRUD operations
- Uses async/await pattern for database operations
- Proper HTTP status codes and RESTful conventions

## NuGet Packages Installed

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

## Configuration

### Connection String (appsettings.json)
Update the connection string with your MySQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HorseRacingDB;User=root;Password=yourpassword;"
}
```

### CORS Policy
The API is configured with a permissive CORS policy named "AllowMyFrontend":
- Allows any origin (*)
- Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
- Allows any headers

## Database Setup

1. **Create the database** using the provided `database.sql` file:
   ```bash
   mysql -u root -p < database.sql
   ```

2. **Update connection string** in `appsettings.json` with your MySQL credentials

3. **Run the application**:
   ```bash
   dotnet run
   ```

## API Endpoints

### Sample Endpoints (Stables)
- `GET /api/Stables` - Get all stables
- `GET /api/Stables/{id}` - Get a specific stable
- `POST /api/Stables` - Create a new stable
- `PUT /api/Stables/{id}` - Update a stable
- `DELETE /api/Stables/{id}` - Delete a stable

### Swagger UI
When running in Development mode, access Swagger UI at:
```
https://localhost:{port}/swagger
```

## Next Steps

To create controllers for other entities, follow the pattern in `StablesController.cs`:

1. Create a new controller class in the `Controllers` folder
2. Inherit from `ControllerBase`
3. Add the `[Route("api/[controller]")]` and `[ApiController]` attributes
4. Inject `RacingDbContext` via constructor
5. Implement CRUD operations using Entity Framework Core

## Running the Application

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

The API will start on:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Technologies Used

- **.NET 8.0**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core 8.0**: ORM for database operations
- **Pomelo.EntityFrameworkCore.MySql**: MySQL provider for EF Core
- **MySQL**: Database system
- **Swagger/OpenAPI**: API documentation and testing

## Database Schema Highlights

- **Composite Keys**: Owns and RaceResults tables use composite primary keys
- **Foreign Keys**: Proper relationships between entities
- **Triggers**: `before_horse_delete` trigger backs up deleted horses to `old_info` table
- **Stored Procedure**: `sp_DeleteOwner` safely deletes owners and their relationships

---

**Author**: Horse Racing Database System  
**Course**: ICS321 - Database Systems  
**Term**: 251
