# 🏇 Horse Racing Database System - Backend API

A comprehensive .NET 8 Web API for managing horse racing data, including stables, horses, owners, trainers, tracks, races, and race results.

## 📋 Table of Contents
- [Project Setup](#project-setup)
- [API Documentation](#api-documentation)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Troubleshooting](#troubleshooting)

---

## 🚀 Project Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (8.0 or higher)
- MySQL client or MySQL Workbench

### Step 1: Clone the Repository
```bash
git clone <repository-url>
cd backend
```

### Step 2: Create the Database

1. **Start MySQL Server** and log in:
```bash
mysql -u root -p
```

2. **Create the database** (if not exists):
```sql
CREATE DATABASE HorseRacingDB;
EXIT;
```

3. **Run the database.sql script** to create all tables, triggers, stored procedures, and insert data:
```bash
mysql -u root -p HorseRacingDB < database.sql
```

Alternatively, you can copy the contents of `database.sql` and execute it in MySQL Workbench.

### Step 3: Update Connection String

1. Open `backend/appsettings.json`
2. Update the `DefaultConnection` string with your MySQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HorseRacingDB;User=root;Password=YOUR_MYSQL_PASSWORD;"
  }
}
```

**Important:** Replace `YOUR_MYSQL_PASSWORD` with your actual MySQL root password.

### Step 4: Restore Dependencies

Navigate to the backend project folder and restore NuGet packages:
```bash
cd backend
dotnet restore
```

### Step 5: Build the Project

```bash
dotnet build
```

If the build is successful, you're ready to run!

### Step 6: Run the Application

```bash
dotnet run
```

The API will start on:
- **Local HTTPS:** `https://localhost:5001`
- **Local HTTP:** `http://localhost:5000`
- **Azure Production:** `https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net`

### Step 7: Test the API

**For Local Development:**
Open your browser and navigate to the Swagger UI:
```
https://localhost:5001/swagger
```

**For Azure Production:**
Access the live API at:
```
https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/swagger
```

You can now test all endpoints interactively!

---

## 📚 API Documentation

**Base URLs:**
- **Local Development:** `https://localhost:5001`
- **Azure Production:** `https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net`

### 🔧 Admin Endpoints

#### 1. Add New Race with Results
**POST** `/api/races`

Creates a new race and optionally adds race results in a single transaction.

**Request Body:**
```json
{
  "raceId": "race37",
  "raceName": "Champions Cup",
  "trackName": "Riyadh",
  "raceDate": "2007-06-15",
  "raceTime": "15:30:00",
  "results": [
    {
      "horseId": "horse1",
      "results": "first",
      "prize": 100000
    },
    {
      "horseId": "horse5",
      "results": "second",
      "prize": 50000
    }
  ]
}
```

**Success Response (201 Created):**
```json
{
  "raceId": "race37",
  "raceName": "Champions Cup",
  "trackName": "Riyadh",
  "raceDate": "2007-06-15T00:00:00",
  "raceTime": "15:30:00"
}
```

---

#### 2. Delete Owner (Stored Procedure)
**DELETE** `/api/owners/{id}`

Executes the `sp_DeleteOwner` stored procedure to delete an owner and all associated ownership records.

**Example Request:**
```
DELETE https://localhost:5001/api/owners/owner5
# Or for production:
DELETE https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/owners/owner5
```

**Success Response (200 OK):**
```json
{
  "message": "Owner 'owner5' and associated ownership records deleted successfully"
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Owner with ID 'owner5' not found"
}
```

---

#### 3. Move Horse to New Stable
**PUT** `/api/horses/{horseId}/stable/{newStableId}`

Transfers a horse from its current stable to a new stable.

**Example Request:**
```
PUT https://localhost:5001/api/horses/horse8/stable/stable2
# Or for production:
PUT https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/horses/horse8/stable/stable2
```

**Success Response (200 OK):**
```json
{
  "message": "Horse 'Flying Force' moved successfully",
  "horseId": "horse8",
  "horseName": "Flying Force",
  "oldStableId": "stable4",
  "newStableId": "stable2",
  "newStableName": "Zayed Farm"
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Horse with ID 'horse99' not found"
}
```

---

#### 4. Approve Trainer to Join Stable
**PUT** `/api/trainers/{trainerId}/approve/{stableId}`

Approves and assigns a trainer to a specific stable.

**Example Request:**
```
PUT https://localhost:5001/api/trainers/trainer3/approve/stable6
# Or for production:
PUT https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/trainers/trainer3/approve/stable6
```

**Success Response (200 OK):**
```json
{
  "message": "Trainer 'Ali Raad' approved to join stable",
  "trainerId": "trainer3",
  "trainerName": "Ali Raad",
  "oldStableId": "stable4",
  "newStableId": "stable6",
  "stableName": "Dubai Stables"
}
```

---

### 👥 Guest Endpoints

#### 5. Get Horses by Owner Last Name
**GET** `/api/horses/by-owner-lastname?lastName={lastName}`

Retrieves all horses owned by owners with a specific last name, including stable information.

**Example Request:**
```
GET https://localhost:5001/api/horses/by-owner-lastname?lastName=Mohammed
# Or for production:
GET https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/horses/by-owner-lastname?lastName=Mohammed
```

**Success Response (200 OK):**
```json
[
  {
    "ownerId": "owner2",
    "ownerLastName": "Mohammed",
    "ownerFirstName": "Khalid",
    "horses": [
      {
        "horseId": "horse3",
        "horseName": "Dove of Peace",
        "age": 3,
        "gender": "C",
        "registration": "33333",
        "stableId": "stablel",
        "stableName": "Zobair Farm"
      },
      {
        "horseId": "horse4",
        "horseName": "Ever Faster",
        "age": 3,
        "gender": "F",
        "registration": "44444",
        "stableId": "stable3",
        "stableName": "Zahra Farm"
      },
      {
        "horseId": "horse9",
        "horseName": "Laggard",
        "age": 2,
        "gender": "F",
        "registration": "99999",
        "stableId": "stable4",
        "stableName": "Sunny Stables"
      },
      {
        "horseId": "horse25",
        "horseName": "Beautiful Brown ",
        "age": 3,
        "gender": "F",
        "registration": "25250",
        "stableId": "stable6",
        "stableName": "Dubai Stables"
      }
    ]
  },
  {
    "ownerId": "owner3",
    "ownerLastName": "Mohammed",
    "ownerFirstName": "Faisal",
    "horses": [
      {
        "horseId": "horse2",
        "horseName": "Conquerer",
        "age": 2,
        "gender": "F",
        "registration": "22222",
        "stableId": "stable6",
        "stableName": "Dubai Stables"
      },
      {
        "horseId": "horse10",
        "horseName": "Formula One",
        "age": 6,
        "gender": "G",
        "registration": "10101",
        "stableId": "stable2",
        "stableName": "Zayed Farm"
      }
    ]
  }
]
```

**Error Response (404 Not Found):**
```json
{
  "message": "No owners found with last name 'Smith'"
}
```

---

#### 6. Get Trainers with First Place Winners
**GET** `/api/trainers/winners`

Retrieves all trainers whose stable's horses have won first place, sorted by number of wins.

**Example Request:**
```
GET https://localhost:5001/api/trainers/winners
# Or for production:
GET https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/trainers/winners
```

**Success Response (200 OK):**
```json
[
  {
    "trainerId": "trainer7",
    "trainerLastName": "Hamid",
    "trainerFirstName": "Ahmed",
    "stableId": "stable6",
    "stableName": "Dubai Stables",
    "firstPlaceWins": 4,
    "winningHorses": [
      {
        "horseId": "horse22",
        "horseName": "Lightening",
        "raceId": "race3",
        "raceName": "2-year-old colts",
        "prize": 70000.00
      },
      {
        "horseId": "horse22",
        "horseName": "Lightening",
        "raceId": "race17",
        "raceName": "Handicap",
        "prize": 1000000.00
      },
      {
        "horseId": "horse7",
        "horseName": "Catapult",
        "raceId": "race16",
        "raceName": "Claiming Stake",
        "prize": 15000.00
      },
      {
        "horseId": "horse18",
        "horseName": "Sublime",
        "raceId": "race24",
        "raceName": "3-year-old colts",
        "prize": 90000.00
      }
    ]
  },
  {
    "trainerId": "trainer2",
    "trainerLastName": "Saleh",
    "trainerFirstName": "Saeed",
    "stableId": "stablel",
    "stableName": "Zobair Farm",
    "firstPlaceWins": 3,
    "winningHorses": [
      {
        "horseId": "horse3",
        "horseName": "Dove of Peace",
        "raceId": "race1",
        "raceName": "Kings Cup",
        "prize": 500000.00
      },
      {
        "horseId": "horse23",
        "horseName": "Lazy Loser",
        "raceId": "race21",
        "raceName": "3-year-old colts",
        "prize": 70000.00
      },
      {
        "horseId": "horse24",
        "horseName": "Leaping Lizard",
        "raceId": "race23",
        "raceName": "3-year-old fillies",
        "prize": 150000.00
      }
    ]
  }
]
```

---

#### 7. Get Trainers Sorted by Total Winnings
**GET** `/api/trainers/winnings`

Retrieves all trainers sorted by total prize money won by horses in their stable.

**Example Request:**
```
GET https://localhost:5001/api/trainers/winnings
# Or for production:
GET https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/trainers/winnings
```

**Success Response (200 OK):**
```json
[
  {
    "trainerId": "trainer7",
    "trainerLastName": "Hamid",
    "trainerFirstName": "Ahmed",
    "stableId": "stable6",
    "stableName": "Dubai Stables",
    "totalWinnings": 1360000.00,
    "totalWins": 4
  },
  {
    "trainerId": "trainer2",
    "trainerLastName": "Saleh",
    "trainerFirstName": "Saeed",
    "stableId": "stablel",
    "stableName": "Zobair Farm",
    "totalWinnings": 1302800.00,
    "totalWins": 3
  },
  {
    "trainerId": "trainer1",
    "trainerLastName": "Mohammed",
    "trainerFirstName": "Fahd",
    "stableId": "stable2",
    "stableName": "Zayed Farm",
    "totalWinnings": 519700.00,
    "totalWins": 1
  },
  {
    "trainerId": "trainer3",
    "trainerLastName": "Ali",
    "trainerFirstName": "Raad",
    "stableId": "stable4",
    "stableName": "Sunny Stables",
    "totalWinnings": 234000.00,
    "totalWins": 2
  },
  {
    "trainerId": "trainer4",
    "trainerLastName": "Sayed",
    "trainerFirstName": "Wasim",
    "stableId": "stable3",
    "stableName": "Zahra Farm",
    "totalWinnings": 205000.00,
    "totalWins": 0
  }
]
```

---

#### 8. Get Track Statistics
**GET** `/api/tracks/stats`

Retrieves comprehensive statistics for all tracks, sorted by total races.

**Example Request:**
```
GET https://localhost:5001/api/tracks/stats
# Or for production:
GET https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/api/tracks/stats
```

**Success Response (200 OK):**
```json
[
  {
    "trackName": "Dhahran",
    "location": "SA",
    "length": 20.00,
    "totalRaces": 5,
    "totalHorseParticipants": 15,
    "uniqueHorses": 10
  },
  {
    "trackName": "Jeddah",
    "location": "SA",
    "length": 19.00,
    "totalRaces": 4,
    "totalHorseParticipants": 10,
    "uniqueHorses": 9
  },
  {
    "trackName": "Doha",
    "location": "QT",
    "length": 20.00,
    "totalRaces": 4,
    "totalHorseParticipants": 12,
    "uniqueHorses": 10
  },
  {
    "trackName": "Dubai",
    "location": "UE",
    "length": 17.00,
    "totalRaces": 4,
    "totalHorseParticipants": 11,
    "uniqueHorses": 7
  },
  {
    "trackName": "Riyadh",
    "location": "SA",
    "length": 22.00,
    "totalRaces": 3,
    "totalHorseParticipants": 9,
    "uniqueHorses": 7
  },
  {
    "trackName": "Yanbu",
    "location": "SA",
    "length": 18.00,
    "totalRaces": 3,
    "totalHorseParticipants": 9,
    "uniqueHorses": 8
  },
  {
    "trackName": "Jubail",
    "location": "SA",
    "length": 15.00,
    "totalRaces": 3,
    "totalHorseParticipants": 9,
    "uniqueHorses": 8
  },
  {
    "trackName": "Sharjah",
    "location": "UE",
    "length": 20.00,
    "totalRaces": 3,
    "totalHorseParticipants": 5,
    "uniqueHorses": 5
  },
  {
    "trackName": "Bahrain",
    "location": "BH",
    "length": 18.00,
    "totalRaces": 2,
    "totalHorseParticipants": 5,
    "uniqueHorses": 4
  }
]
```

---

### 📦 Bonus: Stables CRUD Endpoints

#### 9. Get All Stables
**GET** `/api/Stables`

**Success Response (200 OK):**
```json
[
  {
    "stableId": "stablel",
    "stableName": "Zobair Farm",
    "location": "Riyadh",
    "colors": "orange"
  },
  {
    "stableId": "stable2",
    "stableName": "Zayed Farm",
    "location": "Dubai",
    "colors": "kiwi"
  },
  {
    "stableId": "stable3",
    "stableName": "Zahra Farm",
    "location": "Jeddah",
    "colors": "cinnamon"
  }
]
```

#### 10. Get Stable by ID
**GET** `/api/Stables/{id}`

**Example:** `GET https://localhost:5001/api/Stables/stablel`

**Success Response (200 OK):**
```json
{
  "stableId": "stablel",
  "stableName": "Zobair Farm",
  "location": "Riyadh",
  "colors": "orange"
}
```

#### 11. Create New Stable
**POST** `/api/Stables`

**Request Body:**
```json
{
  "stableId": "stable7",
  "stableName": "New Racing Stables",
  "location": "Abu Dhabi",
  "colors": "gold"
}
```

**Success Response (201 Created):**
```json
{
  "stableId": "stable7",
  "stableName": "New Racing Stables",
  "location": "Abu Dhabi",
  "colors": "gold"
}
```

#### 12. Update Stable
**PUT** `/api/Stables/{id}`

**Example:** `PUT https://localhost:5001/api/Stables/stable7`

**Request Body:**
```json
{
  "stableId": "stable7",
  "stableName": "Updated Racing Stables",
  "location": "Abu Dhabi",
  "colors": "silver"
}
```

**Success Response (204 No Content)**

#### 13. Delete Stable
**DELETE** `/api/Stables/{id}`

**Example:** `DELETE https://localhost:5001/api/Stables/stable7`

**Success Response (204 No Content)**

---

## 🛠️ Technologies Used

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 8.0** - ORM for database operations
- **Pomelo.EntityFrameworkCore.MySql 8.0** - MySQL provider for EF Core
- **MySqlConnector 2.3.5** - For stored procedure execution
- **MySQL 8.0+** - Database system
- **Swashbuckle.AspNetCore 6.5.0** - Swagger/OpenAPI documentation

---

## 📁 Project Structure

```
backend/
├── Controllers/
│   ├── RacingController.cs        # Main API (8 endpoints)
│   └── StablesController.cs       # CRUD operations (5 endpoints)
├── Data/
│   └── RacingDbContext.cs         # Entity Framework DbContext
├── DTOs/
│   └── RacingDtos.cs              # Data Transfer Objects
├── Models/
│   ├── Stable.cs                  # Stable entity
│   ├── Horse.cs                   # Horse entity
│   ├── Owner.cs                   # Owner entity
│   ├── Owns.cs                    # Ownership relationship (composite key)
│   ├── Trainer.cs                 # Trainer entity
│   ├── Track.cs                   # Track entity
│   ├── Race.cs                    # Race entity
│   └── RaceResults.cs             # Race results (composite key)
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration & connection string
├── backend.csproj                # Project dependencies
└── database.sql                  # Complete database schema & data
```

---

## 🔒 CORS Configuration

The API is configured with a permissive CORS policy for development:
- **Allows:** All origins (*)
- **Methods:** GET, POST, PUT, DELETE, and all other HTTP methods
- **Headers:** All headers allowed

This is perfect for frontend development. For production, update the CORS policy in `Program.cs` to specify exact origins.

---

## 🐛 Troubleshooting

### Database Connection Issues
- Verify MySQL is running: `mysql -u root -p`
- Check database exists: `SHOW DATABASES;` in MySQL
- Ensure password in `appsettings.json` is correct
- Check MySQL is listening on port 3306

### Port Already in Use
If port 5001 is busy:
```powershell
# Windows PowerShell
netstat -ano | findstr :5001
taskkill /PID <PID> /F
```

### Build Errors
```bash
dotnet clean
dotnet restore
dotnet build
```

---

## 📞 API Quick Reference

| # | Method | Endpoint | Description |
|---|--------|----------|-------------|
| **Admin Endpoints** |
| 1 | POST | `/api/races` | Add race with results |
| 2 | DELETE | `/api/owners/{id}` | Delete owner (stored procedure) |
| 3 | PUT | `/api/horses/{id}/stable/{id}` | Move horse to stable |
| 4 | PUT | `/api/trainers/{id}/approve/{id}` | Approve trainer |
| **Guest Endpoints** |
| 5 | GET | `/api/horses/by-owner-lastname?lastName=X` | Get horses by owner |
| 6 | GET | `/api/trainers/winners` | Trainers with winners |
| 7 | GET | `/api/trainers/winnings` | Trainers by winnings |
| 8 | GET | `/api/tracks/stats` | Track statistics |
| **Stables CRUD** |
| 9 | GET | `/api/Stables` | Get all stables |
| 10 | GET | `/api/Stables/{id}` | Get stable by ID |
| 11 | POST | `/api/Stables` | Create stable |
| 12 | PUT | `/api/Stables/{id}` | Update stable |
| 13 | DELETE | `/api/Stables/{id}` | Delete stable |

---

## 📝 Notes for Frontend Developers

1. **All endpoints return JSON** with proper HTTP status codes
2. **Error responses** include a `message` field explaining the error
3. **CORS is enabled** - no issues calling from any origin during development
4. **Date/Time format**: Dates are in ISO 8601 format (`YYYY-MM-DD`)
5. **Query parameters** are case-insensitive for searching
6. **Test with Swagger**:
   - Local: `https://localhost:5001/swagger`
   - Production: `https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net/swagger`
7. **Base URLs**:
   - Local Development: `https://localhost:5001`
   - Azure Production: `https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net`
8. **Content-Type**: All POST/PUT requests require `Content-Type: application/json` header

### Common HTTP Status Codes
- `200 OK` - Successful GET, PUT, DELETE
- `201 Created` - Successful POST
- `204 No Content` - Successful update/delete with no response body
- `400 Bad Request` - Invalid input data
- `404 Not Found` - Resource doesn't exist
- `500 Internal Server Error` - Server/database error

---

## 👨‍💻 Author

**Horse Racing Database System**  
ICS321 - Database Systems  
Term 251

---

## 📄 License

This project is created for educational purposes as part of ICS321 coursework.
