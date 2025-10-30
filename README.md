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

### ✅ API Controllers
- **`StablesController`**: Full CRUD operations for Stables (sample/demo)
- **`RacingController`**: Complete Admin and Guest endpoints
  - Admin: Add races, delete owners (via stored procedure), move horses, approve trainers
  - Guest: Query horses by owner, get trainer statistics, track statistics
- Uses async/await pattern for database operations
- Proper HTTP status codes and RESTful conventions
- DTOs for clean JSON responses

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

### Racing Controller (Main API)

#### Admin Endpoints
- `POST /api/races` - Add a new race with results
- `DELETE /api/owners/{id}` - Delete owner using sp_DeleteOwner stored procedure
- `PUT /api/horses/{horseId}/stable/{newStableId}` - Move horse to new stable
- `PUT /api/trainers/{trainerId}/approve/{stableId}` - Approve trainer to join stable

#### Guest Endpoints
- `GET /api/horses/by-owner-lastname?lastName={name}` - Get horses by owner last name
- `GET /api/trainers/winners` - Get trainers with first place winners
- `GET /api/trainers/winnings` - Get trainers sorted by total prize winnings
- `GET /api/tracks/stats` - Get track statistics with race and participant counts

### Stables Controller (Sample CRUD)
- `GET /api/Stables` - Get all stables
- `GET /api/Stables/{id}` - Get a specific stable
- `POST /api/Stables` - Create a new stable
- `PUT /api/Stables/{id}` - Update a stable
- `DELETE /api/Stables/{id}` - Delete a stable

📖 **See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for detailed endpoint documentation**  
🧪 **See [TESTING_GUIDE.md](TESTING_GUIDE.md) for testing examples**

### Swagger UI
When running in Development mode, access Swagger UI at:
```
https://localhost:{port}/swagger
```

## API Features

### Admin Operations
✅ **Race Management**: Create races with multiple results in one transaction  
✅ **Owner Management**: Delete owners using database stored procedure  
✅ **Horse Management**: Transfer horses between stables with validation  
✅ **Trainer Management**: Approve and assign trainers to stables  

### Guest Queries
✅ **Owner-based Search**: Find all horses owned by specific last name  
✅ **Trainer Statistics**: View trainers ranked by wins and prize money  
✅ **Track Analytics**: Comprehensive statistics on races and participants  
✅ **Performance Metrics**: Aggregate data on winnings and achievements  

### Technical Features
- **DTOs**: Clean data transfer objects for structured JSON responses
- **Stored Procedures**: Direct execution of MySQL stored procedures
- **LINQ Queries**: Efficient database queries with Entity Framework
- **Error Handling**: Comprehensive error messages and status codes
- **API Documentation**: Swagger/OpenAPI integration for testing

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
