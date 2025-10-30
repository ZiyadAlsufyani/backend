# Quick Testing Guide for Racing API

## Setup

1. **Start MySQL and create the database:**
   ```bash
   mysql -u root -p < database.sql
   ```

2. **Update connection string in `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=HorseRacingDB;User=root;Password=YOUR_PASSWORD;"
   }
   ```

3. **Run the API:**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI:**
   ```
   https://localhost:5001/swagger
   ```

---

## Quick Test Commands (PowerShell)

### Admin Endpoints

#### 1. Add a New Race
```powershell
$body = @{
    raceId = "race37"
    raceName = "Test Championship"
    trackName = "Riyadh"
    raceDate = "2007-06-20"
    raceTime = "16:00:00"
    results = @(
        @{
            horseId = "horse1"
            results = "first"
            prize = 150000
        },
        @{
            horseId = "horse5"
            results = "second"
            prize = 75000
        }
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/races" -Method POST -Body $body -ContentType "application/json" -SkipCertificateCheck
```

#### 2. Delete an Owner
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/owners/owner5" -Method DELETE -SkipCertificateCheck
```

#### 3. Move Horse to New Stable
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/horses/horse8/stable/stable2" -Method PUT -SkipCertificateCheck
```

#### 4. Approve Trainer to Stable
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/trainers/trainer3/approve/stable6" -Method PUT -SkipCertificateCheck
```

---

### Guest Endpoints

#### 1. Get Horses by Owner Last Name
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/horses/by-owner-lastname?lastName=Mohammed" -Method GET -SkipCertificateCheck
```

#### 2. Get Trainers with Winners
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/trainers/winners" -Method GET -SkipCertificateCheck
```

#### 3. Get Trainers by Winnings
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/trainers/winnings" -Method GET -SkipCertificateCheck
```

#### 4. Get Track Statistics
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/tracks/stats" -Method GET -SkipCertificateCheck
```

---

## Using curl (Cross-platform)

### Admin Endpoints

#### Add Race
```bash
curl -X POST "https://localhost:5001/api/races" \
  -H "Content-Type: application/json" \
  -d '{
    "raceId": "race37",
    "raceName": "Test Championship",
    "trackName": "Riyadh",
    "raceDate": "2007-06-20",
    "raceTime": "16:00:00",
    "results": [
      {"horseId": "horse1", "results": "first", "prize": 150000},
      {"horseId": "horse5", "results": "second", "prize": 75000}
    ]
  }' \
  -k
```

#### Delete Owner
```bash
curl -X DELETE "https://localhost:5001/api/owners/owner5" -k
```

#### Move Horse
```bash
curl -X PUT "https://localhost:5001/api/horses/horse8/stable/stable2" -k
```

#### Approve Trainer
```bash
curl -X PUT "https://localhost:5001/api/trainers/trainer3/approve/stable6" -k
```

### Guest Endpoints

```bash
# Get horses by owner last name
curl -X GET "https://localhost:5001/api/horses/by-owner-lastname?lastName=Mohammed" -k

# Get trainers with winners
curl -X GET "https://localhost:5001/api/trainers/winners" -k

# Get trainers by winnings
curl -X GET "https://localhost:5001/api/trainers/winnings" -k

# Get track statistics
curl -X GET "https://localhost:5001/api/tracks/stats" -k
```

---

## Expected Results Summary

### Admin Endpoints
- ✅ **POST /api/races**: Creates race and returns `201 Created` with race object
- ✅ **DELETE /api/owners/{id}**: Executes stored procedure, returns success message
- ✅ **PUT /api/horses/{id}/stable/{id}**: Updates horse stable, returns transfer details
- ✅ **PUT /api/trainers/{id}/approve/{id}**: Updates trainer stable, returns approval details

### Guest Endpoints
- ✅ **GET /api/horses/by-owner-lastname**: Returns array of owners with their horses
- ✅ **GET /api/trainers/winners**: Returns trainers sorted by first-place wins
- ✅ **GET /api/trainers/winnings**: Returns trainers sorted by total prize money
- ✅ **GET /api/tracks/stats**: Returns tracks with race and participation statistics

---

## Troubleshooting

### Connection String Issues
If you get database connection errors:
1. Verify MySQL is running: `mysql -u root -p`
2. Check database exists: `SHOW DATABASES;`
3. Update password in `appsettings.json`

### Port Already in Use
If port 5001 is busy:
```bash
# Check what's using the port
netstat -ano | findstr :5001

# Kill the process (replace PID)
taskkill /PID <PID> /F
```

### CORS Issues
If testing from a browser/frontend:
- The API has CORS enabled for all origins
- Check browser console for specific CORS errors

---

## Sample Test Sequence

1. **Check existing data:**
   ```bash
   GET /api/tracks/stats
   GET /api/trainers/winnings
   ```

2. **Test owner deletion:**
   ```bash
   GET /api/horses/by-owner-lastname?lastName=Nasr
   DELETE /api/owners/owner5
   GET /api/horses/by-owner-lastname?lastName=Nasr  # Should show owner5 is gone
   ```

3. **Move a horse:**
   ```bash
   PUT /api/horses/horse8/stable/stable6
   GET /api/horses/by-owner-lastname?lastName=Fahd  # Verify new stable
   ```

4. **Add a new race:**
   ```bash
   POST /api/races  # With test data
   GET /api/tracks/stats  # Should show increased race count
   ```

---

## API Response Status Codes

- **200 OK**: Successful GET, PUT, DELETE
- **201 Created**: Successful POST
- **400 Bad Request**: Invalid input data
- **404 Not Found**: Resource doesn't exist
- **500 Internal Server Error**: Server/database error

Check response bodies for detailed error messages!
