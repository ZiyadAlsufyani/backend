# Racing Controller API Documentation

## Overview
The `RacingController` provides comprehensive endpoints for both Admin and Guest users to interact with the Horse Racing Database System.

**Base URLs:**
- **Local Development:** `https://localhost:5001`
- **Azure Production:** `https://ics321-racing-api-gdd6g6hvdcbch7bu.uaenorth-01.azurewebsites.net`

---

## 🔐 Admin Endpoints

### 1. Add New Race with Results
**Endpoint:** `POST /api/races`  
**Description:** Create a new race and optionally add race results in a single transaction.

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
    },
    {
      "horseId": "horse10",
      "results": "third",
      "prize": 25000
    }
  ]
}
```

**Response:** `201 Created`
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

### 2. Delete Owner (Using Stored Procedure)
**Endpoint:** `DELETE /api/owners/{id}`  
**Description:** Executes the `sp_DeleteOwner` stored procedure to delete an owner and all associated ownership records.

**Example:** `DELETE /api/owners/owner5`

**Response:** `200 OK`
```json
{
  "message": "Owner 'owner5' and associated ownership records deleted successfully"
}
```

**Error Response:** `404 Not Found`
```json
{
  "message": "Owner with ID 'owner99' not found"
}
```

---

### 3. Move Horse to New Stable
**Endpoint:** `PUT /api/horses/{horseId}/stable/{newStableId}`  
**Description:** Transfer a horse from its current stable to a new stable.

**Example:** `PUT /api/horses/horse3/stable/stable5`

**Response:** `200 OK`
```json
{
  "message": "Horse 'Dove of Peace' moved successfully",
  "horseId": "horse3",
  "horseName": "Dove of Peace",
  "oldStableId": "stablel",
  "newStableId": "stable5",
  "newStableName": "Ajman Stables"
}
```

---

### 4. Approve Trainer to Join Stable
**Endpoint:** `PUT /api/trainers/{trainerId}/approve/{stableId}`  
**Description:** Approve and assign a trainer to a specific stable.

**Example:** `PUT /api/trainers/trainer3/approve/stable2`

**Response:** `200 OK`
```json
{
  "message": "Trainer 'Ali Raad' approved to join stable",
  "trainerId": "trainer3",
  "trainerName": "Ali Raad",
  "oldStableId": "stable4",
  "newStableId": "stable2",
  "stableName": "Zayed Farm"
}
```

---

## 👥 Guest Endpoints

### 1. Get Horses by Owner Last Name
**Endpoint:** `GET /api/horses/by-owner-lastname?lastName={lastName}`  
**Description:** Retrieve all horses owned by owners with a specific last name, including stable information.

**Example:** `GET /api/horses/by-owner-lastname?lastName=Mohammed`

**Response:** `200 OK`
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

---

### 2. Get Trainers with First Place Winners
**Endpoint:** `GET /api/trainers/winners`  
**Description:** Retrieve all trainers whose stable's horses have won first place, including details of winning horses.

**Response:** `200 OK`
```json
[
  {
    "trainerId": "trainer2",
    "trainerLastName": "Saleh",
    "trainerFirstName": "Saeed",
    "stableId": "stablel",
    "stableName": "Zobair Farm",
    "firstPlaceWins": 5,
    "winningHorses": [
      {
        "horseId": "horse3",
        "horseName": "Dove of Peace",
        "raceId": "race1",
        "raceName": "Kings Cup",
        "prize": 500000
      },
      {
        "horseId": "horse15",
        "horseName": "FastOffMyFeet",
        "raceId": "race23",
        "raceName": "3-year-old fillies",
        "prize": 150000
      }
    ]
  },
  {
    "trainerId": "trainer1",
    "trainerLastName": "Mohammed",
    "trainerFirstName": "Fahd",
    "stableId": "stable2",
    "stableName": "Zayed Farm",
    "firstPlaceWins": 3,
    "winningHorses": [
      {
        "horseId": "horse6",
        "horseName": "Windrunner",
        "raceId": "race2",
        "raceName": "2-year-old fillies",
        "prize": 100000
      }
    ]
  }
]
```

---

### 3. Get Trainers Sorted by Total Winnings
**Endpoint:** `GET /api/trainers/winnings`  
**Description:** Retrieve all trainers sorted by total prize money won by horses in their stable.

**Response:** `200 OK`
```json
[
  {
    "trainerId": "trainer7",
    "trainerLastName": "Hamid",
    "trainerFirstName": "Ahmed",
    "stableId": "stable6",
    "stableName": "Dubai Stables",
    "totalWinnings": 1245000.00,
    "totalWins": 8
  },
  {
    "trainerId": "trainer2",
    "trainerLastName": "Saleh",
    "trainerFirstName": "Saeed",
    "stableId": "stablel",
    "stableName": "Zobair Farm",
    "totalWinnings": 980000.00,
    "totalWins": 6
  },
  {
    "trainerId": "trainer1",
    "trainerLastName": "Mohammed",
    "trainerFirstName": "Fahd",
    "stableId": "stable2",
    "stableName": "Zayed Farm",
    "totalWinnings": 750000.00,
    "totalWins": 5
  }
]
```

---

### 4. Get Track Statistics
**Endpoint:** `GET /api/tracks/stats`  
**Description:** Retrieve comprehensive statistics for all tracks including race counts and horse participation.

**Response:** `200 OK`
```json
[
  {
    "trackName": "Riyadh",
    "location": "SA",
    "length": 22.00,
    "totalRaces": 3,
    "totalHorseParticipants": 9,
    "uniqueHorses": 7
  },
  {
    "trackName": "Doha",
    "location": "QT",
    "totalRaces": 4,
    "length": 20.00,
    "totalHorseParticipants": 12,
    "uniqueHorses": 10
  },
  {
    "trackName": "Dubai",
    "location": "UE",
    "length": 17.00,
    "totalRaces": 4,
    "totalHorseParticipants": 14,
    "uniqueHorses": 8
  },
  {
    "trackName": "Jeddah",
    "location": "SA",
    "length": 19.00,
    "totalRaces": 3,
    "totalHorseParticipants": 8,
    "uniqueHorses": 6
  }
]
```

---

## 📊 DTO (Data Transfer Object) Classes

All DTOs are defined in `DTOs/RacingDtos.cs`:

1. **AddRaceDto** - For creating races with results
2. **RaceResultDto** - Individual race result
3. **HorsesByOwnerDto** - Grouped horses by owner
4. **HorseInfoDto** - Detailed horse information
5. **TrainerWinnersDto** - Trainers with winning horses
6. **WinningHorseDto** - Details of winning horses
7. **TrainerWinningsDto** - Trainer winnings summary
8. **TrackStatsDto** - Track statistics

---

## 🔧 Key Features

### Admin Endpoints Features:
- ✅ Transaction-based race creation with results
- ✅ Stored procedure execution for owner deletion
- ✅ Safe horse and trainer transfers with validation
- ✅ Detailed response messages with old/new values

### Guest Endpoints Features:
- ✅ Case-insensitive last name search
- ✅ Aggregated statistics and counts
- ✅ Sorted results (by wins, winnings, races)
- ✅ Clean JSON responses with nested objects
- ✅ Includes navigation property data (stable names, etc.)

---

## 🚀 Testing with Swagger

1. Start the application: `dotnet run`
2. Navigate to: `https://localhost:5001/swagger`
3. Test all endpoints directly from the Swagger UI

---

## 📝 Notes

- All endpoints include proper error handling
- Returns appropriate HTTP status codes (200, 201, 404, 400)
- Admin endpoints validate existence of related entities
- Guest endpoints use efficient LINQ queries with EF Core
- The `sp_DeleteOwner` stored procedure is executed via raw ADO.NET for compatibility
