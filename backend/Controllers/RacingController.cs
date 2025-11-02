using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;
using MySqlConnector;

namespace backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class RacingController : ControllerBase
    {
        private readonly RacingDbContext _context;
        private readonly IConfiguration _configuration;

        public RacingController(RacingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region Admin Endpoints

        /// <summary>
        /// Admin: Add a new race with results
        /// POST /api/races
        /// </summary>
        [HttpPost("races")]
        public async Task<ActionResult<Race>> AddRace([FromBody] AddRaceDto addRaceDto)
        {
            try
            {
                // Verify the track exists using raw SQL
                var trackExists = await _context.Database
                    .SqlQuery<int>($"SELECT COUNT(*) as Value FROM Track WHERE TrackName = {addRaceDto.TrackName}")
                    .FirstOrDefaultAsync() > 0;
                    
                if (!trackExists)
                {
                    return BadRequest(new { message = $"Track '{addRaceDto.TrackName}' does not exist. Please use an existing track or create the track first." });
                }

                // Verify the race doesn't already exist using raw SQL
                var raceExists = await _context.Database
                    .SqlQuery<int>($"SELECT COUNT(*) as Value FROM Race WHERE RaceId = {addRaceDto.RaceId}")
                    .FirstOrDefaultAsync() > 0;
                    
                if (raceExists)
                {
                    return BadRequest(new { message = $"Race with ID '{addRaceDto.RaceId}' already exists." });
                }

                // Insert the race using raw SQL
                await _context.Database.ExecuteSqlAsync(
                    $"INSERT INTO Race (raceId, raceName, trackName, raceDate, raceTime) VALUES ({addRaceDto.RaceId}, {addRaceDto.RaceName}, {addRaceDto.TrackName}, {addRaceDto.RaceDate}, {addRaceDto.RaceTime})"
                );

                // Add race results if provided
                if (addRaceDto.Results != null && addRaceDto.Results.Any())
                {
                    // Verify all horses exist using raw SQL
                    foreach (var resultDto in addRaceDto.Results)
                    {
                        var horseExists = await _context.Database
                            .SqlQuery<int>($"SELECT COUNT(*) as Value FROM Horse WHERE HorseId = {resultDto.HorseId}")
                            .FirstOrDefaultAsync() > 0;
                            
                        if (!horseExists)
                        {
                            return BadRequest(new { message = $"Horse with ID '{resultDto.HorseId}' does not exist." });
                        }

                        // Insert race result using raw SQL
                        await _context.Database.ExecuteSqlAsync(
                            $"INSERT INTO RaceResults (raceId, horseId, results, prize) VALUES ({addRaceDto.RaceId}, {resultDto.HorseId}, {resultDto.Results}, {resultDto.Prize})"
                        );
                    }
                }

                // Fetch the created race using raw SQL
                var race = await _context.Races
                    .FromSql($"SELECT * FROM Race WHERE raceId = {addRaceDto.RaceId}")
                    .FirstOrDefaultAsync();

                if (race == null)
                {
                    return BadRequest(new { message = "Race was created but could not be retrieved." });
                }

                return CreatedAtAction(nameof(AddRace), new { id = race.RaceId }, race);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to add race", error = ex.Message });
            }
        }

        /// <summary>
        /// Admin: Delete an owner using the sp_DeleteOwner stored procedure
        /// DELETE /api/owners/{id}
        /// </summary>
        [HttpDelete("owners/{id}")]
        public async Task<IActionResult> DeleteOwner(string id)
        {
            try
            {
                // Check if owner exists using raw SQL
                var ownerExists = await _context.Database
                    .SqlQuery<int>($"SELECT COUNT(*) as Value FROM Owner WHERE OwnerId = {id}")
                    .FirstOrDefaultAsync() > 0;
                    
                if (!ownerExists)
                {
                    return NotFound(new { message = $"Owner with ID '{id}' not found" });
                }

                // Execute the stored procedure using raw SQL
                await _context.Database.ExecuteSqlAsync($"CALL sp_DeleteOwner({id})");

                return Ok(new { message = $"Owner '{id}' and associated ownership records deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to delete owner", error = ex.Message });
            }
        }

        /// <summary>
        /// Admin: Move a horse to a new stable
        /// PUT /api/horses/{horseId}/stable/{newStableId}
        /// </summary>
        [HttpPut("horses/{horseId}/stable/{newStableId}")]
        public async Task<IActionResult> MoveHorseToStable(string horseId, string newStableId)
        {
            try
            {
                // Find the horse using raw SQL
                var horse = await _context.Horses
                    .FromSql($"SELECT * FROM Horse WHERE horseId = {horseId}")
                    .FirstOrDefaultAsync();
                    
                if (horse == null)
                {
                    return NotFound(new { message = $"Horse with ID '{horseId}' not found" });
                }

                // Verify the stable exists using raw SQL
                var stable = await _context.Stables
                    .FromSql($"SELECT * FROM Stable WHERE stableId = {newStableId}")
                    .FirstOrDefaultAsync();
                    
                if (stable == null)
                {
                    return NotFound(new { message = $"Stable with ID '{newStableId}' not found" });
                }

                var oldStableId = horse.StableId;
                
                // Update horse stable using raw SQL
                await _context.Database.ExecuteSqlAsync(
                    $"UPDATE Horse SET stableId = {newStableId} WHERE horseId = {horseId}"
                );

                return Ok(new 
                { 
                    message = $"Horse '{horse.HorseName}' moved successfully",
                    horseId = horse.HorseId,
                    horseName = horse.HorseName,
                    oldStableId = oldStableId,
                    newStableId = newStableId,
                    newStableName = stable.StableName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to move horse", error = ex.Message });
            }
        }

        /// <summary>
        /// Admin: Approve a trainer to join a stable
        /// PUT /api/trainers/{trainerId}/approve/{stableId}
        /// </summary>
        [HttpPut("trainers/{trainerId}/approve/{stableId}")]
        public async Task<IActionResult> ApproveTrainerToStable(string trainerId, string stableId)
        {
            try
            {
                // Find the trainer using raw SQL
                var trainer = await _context.Trainers
                    .FromSql($"SELECT * FROM Trainer WHERE trainerId = {trainerId}")
                    .FirstOrDefaultAsync();
                    
                if (trainer == null)
                {
                    return NotFound(new { message = $"Trainer with ID '{trainerId}' not found" });
                }

                // Verify the stable exists using raw SQL
                var stable = await _context.Stables
                    .FromSql($"SELECT * FROM Stable WHERE stableId = {stableId}")
                    .FirstOrDefaultAsync();
                    
                if (stable == null)
                {
                    return NotFound(new { message = $"Stable with ID '{stableId}' not found" });
                }

                var oldStableId = trainer.StableId;
                
                // Update trainer stable using raw SQL
                await _context.Database.ExecuteSqlAsync(
                    $"UPDATE Trainer SET stableId = {stableId} WHERE trainerId = {trainerId}"
                );

                return Ok(new 
                { 
                    message = $"Trainer '{trainer.FirstName} {trainer.LastName}' approved to join stable",
                    trainerId = trainer.TrainerId,
                    trainerName = $"{trainer.FirstName} {trainer.LastName}",
                    oldStableId = oldStableId,
                    newStableId = stableId,
                    stableName = stable.StableName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to approve trainer", error = ex.Message });
            }
        }

        #endregion

        #region Helper Endpoints

        /// <summary>
        /// Get all available tracks (useful for validation before adding races)
        /// GET /api/tracks
        /// </summary>
        [HttpGet("tracks")]
        public async Task<ActionResult<IEnumerable<Track>>> GetAllTracks()
        {
            try
            {
                var tracks = await _context.Tracks
                    .FromSqlRaw("SELECT * FROM Track")
                    .ToListAsync();
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve tracks", error = ex.Message });
            }
        }

        #endregion

        #region Guest Endpoints

        /// <summary>
        /// Guest: Get horses by owner last name
        /// GET /api/horses/by-owner-lastname?lastName=Smith
        /// </summary>
        [HttpGet("horses/by-owner-lastname")]
        public async Task<ActionResult<IEnumerable<HorsesByOwnerDto>>> GetHorsesByOwnerLastName([FromQuery] string lastName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    return BadRequest(new { message = "lastName query parameter is required" });
                }

                // Use raw SQL to get owners and their horses
                var sql = @"
                    SELECT 
                        o.ownerId,
                        o.lname as OwnerLastName,
                        o.fname as OwnerFirstName,
                        h.horseId,
                        h.horseName,
                        h.age,
                        h.gender,
                        h.registration,
                        h.stableId,
                        s.stableName
                    FROM Owner o
                    INNER JOIN Owns ow ON o.ownerId = ow.ownerId
                    INNER JOIN Horse h ON ow.horseId = h.horseId
                    LEFT JOIN Stable s ON h.stableId = s.stableId
                    WHERE LOWER(o.lname) = LOWER({0})
                    ORDER BY o.ownerId, h.horseId";

                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = sql.Replace("{0}", "@lastName");
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@lastName";
                parameter.Value = lastName;
                command.Parameters.Add(parameter);

                var result = new List<HorsesByOwnerDto>();
                using var reader = await command.ExecuteReaderAsync();
                
                HorsesByOwnerDto? currentOwner = null;
                
                while (await reader.ReadAsync())
                {
                    var ownerId = reader.GetString(0);
                    
                    if (currentOwner == null || currentOwner.OwnerId != ownerId)
                    {
                        currentOwner = new HorsesByOwnerDto
                        {
                            OwnerId = ownerId,
                            OwnerLastName = reader.GetString(1),
                            OwnerFirstName = reader.GetString(2),
                            Horses = new List<HorseInfoDto>()
                        };
                        result.Add(currentOwner);
                    }
                    
                    currentOwner.Horses.Add(new HorseInfoDto
                    {
                        HorseId = reader.GetString(3),
                        HorseName = reader.GetString(4),
                        Age = reader.GetInt32(5),
                        Gender = reader.GetString(6),
                        Registration = reader.GetString(7),
                        StableId = reader.IsDBNull(8) ? null : reader.GetString(8),
                        StableName = reader.IsDBNull(9) ? null : reader.GetString(9)
                    });
                }

                if (!result.Any())
                {
                    return NotFound(new { message = $"No owners found with last name '{lastName}'" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve horses", error = ex.Message });
            }
        }

        /// <summary>
        /// Guest: Get trainers with first place winners
        /// GET /api/trainers/winners
        /// </summary>
        [HttpGet("trainers/winners")]
        public async Task<ActionResult<IEnumerable<TrainerWinnersDto>>> GetTrainersWithWinners()
        {
            try
            {
                // Use raw SQL to get trainers with first place winners
                var sql = @"
                    SELECT 
                        t.trainerId,
                        t.lname as TrainerLastName,
                        t.fname as TrainerFirstName,
                        t.stableId,
                        s.stableName,
                        h.horseId,
                        h.horseName,
                        r.raceId,
                        r.raceName,
                        rr.prize,
                        COUNT(*) OVER (PARTITION BY t.trainerId) as FirstPlaceWins
                    FROM Trainer t
                    INNER JOIN Stable s ON t.stableId = s.stableId
                    INNER JOIN Horse h ON h.stableId = t.stableId
                    INNER JOIN RaceResults rr ON h.horseId = rr.horseId
                    INNER JOIN Race r ON rr.raceId = r.raceId
                    WHERE LOWER(rr.results) = 'first'
                    ORDER BY FirstPlaceWins DESC, t.trainerId, r.raceId";

                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = sql;

                var trainerDict = new Dictionary<string, TrainerWinnersDto>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var trainerId = reader.GetString(0);
                    
                    if (!trainerDict.ContainsKey(trainerId))
                    {
                        trainerDict[trainerId] = new TrainerWinnersDto
                        {
                            TrainerId = trainerId,
                            TrainerLastName = reader.GetString(1),
                            TrainerFirstName = reader.GetString(2),
                            StableId = reader.GetString(3),
                            StableName = reader.GetString(4),
                            FirstPlaceWins = reader.GetInt32(10),
                            WinningHorses = new List<WinningHorseDto>()
                        };
                    }
                    
                    trainerDict[trainerId].WinningHorses.Add(new WinningHorseDto
                    {
                        HorseId = reader.GetString(5),
                        HorseName = reader.GetString(6),
                        RaceId = reader.GetString(7),
                        RaceName = reader.GetString(8),
                        Prize = reader.IsDBNull(9) ? null : reader.GetDecimal(9)
                    });
                }

                var result = trainerDict.Values.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve trainers with winners", error = ex.Message });
            }
        }

        /// <summary>
        /// Guest: Get trainers sorted by total prize winnings
        /// GET /api/trainers/winnings
        /// </summary>
        [HttpGet("trainers/winnings")]
        public async Task<ActionResult<IEnumerable<TrainerWinningsDto>>> GetTrainersByWinnings()
        {
            try
            {
                // Use raw SQL to get trainers sorted by total winnings
                var sql = @"
                    SELECT 
                        t.trainerId,
                        t.lname,
                        t.fname,
                        t.stableId,
                        s.stableName,
                        COALESCE(SUM(rr.prize), 0) as TotalWinnings,
                        COUNT(CASE WHEN LOWER(rr.results) = 'first' THEN 1 END) as TotalWins
                    FROM Trainer t
                    INNER JOIN Stable s ON t.stableId = s.stableId
                    LEFT JOIN Horse h ON h.stableId = t.stableId
                    LEFT JOIN RaceResults rr ON h.horseId = rr.horseId
                    GROUP BY t.trainerId, t.lname, t.fname, t.stableId, s.stableName
                    ORDER BY TotalWinnings DESC";

                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = sql;

                var result = new List<TrainerWinningsDto>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    result.Add(new TrainerWinningsDto
                    {
                        TrainerId = reader.GetString(0),
                        TrainerLastName = reader.GetString(1),
                        TrainerFirstName = reader.GetString(2),
                        StableId = reader.GetString(3),
                        StableName = reader.GetString(4),
                        TotalWinnings = reader.GetDecimal(5),
                        TotalWins = reader.GetInt32(6)
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve trainer winnings", error = ex.Message });
            }
        }

        /// <summary>
        /// Guest: Get track statistics (race counts and participant counts)
        /// GET /api/tracks/stats
        /// </summary>
        [HttpGet("tracks/stats")]
        public async Task<ActionResult<IEnumerable<TrackStatsDto>>> GetTrackStatistics()
        {
            try
            {
                // Use raw SQL to get track statistics
                var sql = @"
                    SELECT 
                        t.trackName,
                        t.location,
                        t.length,
                        COUNT(DISTINCT r.raceId) as TotalRaces,
                        COUNT(rr.horseId) as TotalHorseParticipants,
                        COUNT(DISTINCT rr.horseId) as UniqueHorses
                    FROM Track t
                    LEFT JOIN Race r ON t.trackName = r.trackName
                    LEFT JOIN RaceResults rr ON r.raceId = rr.raceId
                    GROUP BY t.trackName, t.location, t.length
                    ORDER BY TotalRaces DESC";

                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = sql;

                var result = new List<TrackStatsDto>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    result.Add(new TrackStatsDto
                    {
                        TrackName = reader.GetString(0),
                        Location = reader.GetString(1),
                        Length = reader.GetDecimal(2),
                        TotalRaces = reader.GetInt32(3),
                        TotalHorseParticipants = reader.GetInt32(4),
                        UniqueHorses = reader.GetInt32(5)
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve track statistics", error = ex.Message });
            }
        }

        #endregion
    }
}
