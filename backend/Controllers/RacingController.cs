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
                // Verify the track exists
                var trackExists = await _context.Tracks.AnyAsync(t => t.TrackName == addRaceDto.TrackName);
                if (!trackExists)
                {
                    return BadRequest(new { message = $"Track '{addRaceDto.TrackName}' does not exist. Please use an existing track or create the track first." });
                }

                // Verify the race doesn't already exist
                var raceExists = await _context.Races.AnyAsync(r => r.RaceId == addRaceDto.RaceId);
                if (raceExists)
                {
                    return BadRequest(new { message = $"Race with ID '{addRaceDto.RaceId}' already exists." });
                }

                // Create the race
                var race = new Race
                {
                    RaceId = addRaceDto.RaceId,
                    RaceName = addRaceDto.RaceName,
                    TrackName = addRaceDto.TrackName,
                    RaceDate = addRaceDto.RaceDate,
                    RaceTime = addRaceDto.RaceTime
                };

                _context.Races.Add(race);
                await _context.SaveChangesAsync();

                // Add race results if provided
                if (addRaceDto.Results != null && addRaceDto.Results.Any())
                {
                    // Verify all horses exist
                    var horseIds = addRaceDto.Results.Select(r => r.HorseId).Distinct().ToList();
                    var existingHorseIds = await _context.Horses
                        .Where(h => horseIds.Contains(h.HorseId))
                        .Select(h => h.HorseId)
                        .ToListAsync();
                    
                    var missingHorses = horseIds.Except(existingHorseIds).ToList();
                    if (missingHorses.Any())
                    {
                        return BadRequest(new { message = $"The following horse IDs do not exist: {string.Join(", ", missingHorses)}" });
                    }

                    foreach (var resultDto in addRaceDto.Results)
                    {
                        var raceResult = new RaceResults
                        {
                            RaceId = race.RaceId,
                            HorseId = resultDto.HorseId,
                            Results = resultDto.Results,
                            Prize = resultDto.Prize
                        };
                        _context.RaceResults.Add(raceResult);
                    }
                    await _context.SaveChangesAsync();
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
                // Check if owner exists
                var owner = await _context.Owners.FindAsync(id);
                if (owner == null)
                {
                    return NotFound(new { message = $"Owner with ID '{id}' not found" });
                }

                // Execute the stored procedure
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("sp_DeleteOwner", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_ownerId", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }

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
                // Find the horse
                var horse = await _context.Horses.FindAsync(horseId);
                if (horse == null)
                {
                    return NotFound(new { message = $"Horse with ID '{horseId}' not found" });
                }

                // Verify the stable exists
                var stable = await _context.Stables.FindAsync(newStableId);
                if (stable == null)
                {
                    return NotFound(new { message = $"Stable with ID '{newStableId}' not found" });
                }

                var oldStableId = horse.StableId;
                horse.StableId = newStableId;
                await _context.SaveChangesAsync();

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
                // Find the trainer
                var trainer = await _context.Trainers.FindAsync(trainerId);
                if (trainer == null)
                {
                    return NotFound(new { message = $"Trainer with ID '{trainerId}' not found" });
                }

                // Verify the stable exists
                var stable = await _context.Stables.FindAsync(stableId);
                if (stable == null)
                {
                    return NotFound(new { message = $"Stable with ID '{stableId}' not found" });
                }

                var oldStableId = trainer.StableId;
                trainer.StableId = stableId;
                await _context.SaveChangesAsync();

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
                var tracks = await _context.Tracks.ToListAsync();
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

                var result = await _context.Owners
                    .Where(o => o.LastName.ToLower() == lastName.ToLower())
                    .Select(o => new HorsesByOwnerDto
                    {
                        OwnerId = o.OwnerId,
                        OwnerLastName = o.LastName,
                        OwnerFirstName = o.FirstName,
                        Horses = o.Owns.Select(own => new HorseInfoDto
                        {
                            HorseId = own.Horse.HorseId,
                            HorseName = own.Horse.HorseName,
                            Age = own.Horse.Age,
                            Gender = own.Horse.Gender,
                            Registration = own.Horse.Registration,
                            StableId = own.Horse.StableId,
                            StableName = own.Horse.Stable != null ? own.Horse.Stable.StableName : null
                        }).ToList()
                    })
                    .ToListAsync();

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
                // Get all trainers with their stables and horses that won first place
                var trainersWithWinners = await _context.Trainers
                    .Include(t => t.Stable)
                    .Select(t => new
                    {
                        Trainer = t,
                        WinningHorses = _context.RaceResults
                            .Where(rr => rr.Results != null && rr.Results.ToLower() == "first" && 
                                   rr.Horse.StableId == t.StableId)
                            .Include(rr => rr.Horse)
                            .Include(rr => rr.Race)
                            .Select(rr => new WinningHorseDto
                            {
                                HorseId = rr.Horse.HorseId,
                                HorseName = rr.Horse.HorseName,
                                RaceId = rr.Race.RaceId,
                                RaceName = rr.Race.RaceName,
                                Prize = rr.Prize
                            })
                            .ToList()
                    })
                    .ToListAsync();

                // Filter trainers who have at least one winning horse
                var result = trainersWithWinners
                    .Where(t => t.WinningHorses.Any())
                    .Select(t => new TrainerWinnersDto
                    {
                        TrainerId = t.Trainer.TrainerId,
                        TrainerLastName = t.Trainer.LastName,
                        TrainerFirstName = t.Trainer.FirstName,
                        StableId = t.Trainer.StableId,
                        StableName = t.Trainer.Stable != null ? t.Trainer.Stable.StableName : null,
                        FirstPlaceWins = t.WinningHorses.Count,
                        WinningHorses = t.WinningHorses
                    })
                    .OrderByDescending(t => t.FirstPlaceWins)
                    .ToList();

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
                // Get trainers with their total winnings from horses in their stable
                var trainersWithWinnings = await _context.Trainers
                    .Include(t => t.Stable)
                    .Select(t => new
                    {
                        Trainer = t,
                        TotalWinnings = _context.RaceResults
                            .Where(rr => rr.Horse.StableId == t.StableId && rr.Prize.HasValue)
                            .Sum(rr => (decimal?)rr.Prize) ?? 0,
                        TotalWins = _context.RaceResults
                            .Count(rr => rr.Horse.StableId == t.StableId && 
                                   rr.Results != null && rr.Results.ToLower() == "first")
                    })
                    .ToListAsync();

                var result = trainersWithWinnings
                    .Select(t => new TrainerWinningsDto
                    {
                        TrainerId = t.Trainer.TrainerId,
                        TrainerLastName = t.Trainer.LastName,
                        TrainerFirstName = t.Trainer.FirstName,
                        StableId = t.Trainer.StableId,
                        StableName = t.Trainer.Stable != null ? t.Trainer.Stable.StableName : null,
                        TotalWinnings = t.TotalWinnings,
                        TotalWins = t.TotalWins
                    })
                    .OrderByDescending(t => t.TotalWinnings)
                    .ToList();

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
                var trackStats = await _context.Tracks
                    .Select(t => new TrackStatsDto
                    {
                        TrackName = t.TrackName,
                        Location = t.Location,
                        Length = t.Length,
                        TotalRaces = t.Races.Count,
                        TotalHorseParticipants = t.Races
                            .SelectMany(r => r.RaceResults)
                            .Count(),
                        UniqueHorses = t.Races
                            .SelectMany(r => r.RaceResults)
                            .Select(rr => rr.HorseId)
                            .Distinct()
                            .Count()
                    })
                    .OrderByDescending(t => t.TotalRaces)
                    .ToListAsync();

                return Ok(trackStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve track statistics", error = ex.Message });
            }
        }

        #endregion
    }
}
