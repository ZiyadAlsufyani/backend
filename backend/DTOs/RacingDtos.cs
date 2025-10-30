namespace backend.DTOs
{
    // DTO for adding a new race with results
    public class AddRaceDto
    {
        public string RaceId { get; set; } = string.Empty;
        public string RaceName { get; set; } = string.Empty;
        public string? TrackName { get; set; }
        public DateTime? RaceDate { get; set; }
        public TimeSpan? RaceTime { get; set; }
        public List<RaceResultDto> Results { get; set; } = new List<RaceResultDto>();
    }

    public class RaceResultDto
    {
        public string HorseId { get; set; } = string.Empty;
        public string? Results { get; set; }
        public decimal? Prize { get; set; }
    }

    // DTO for horses grouped by owner last name
    public class HorsesByOwnerDto
    {
        public string OwnerLastName { get; set; } = string.Empty;
        public string OwnerFirstName { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public List<HorseInfoDto> Horses { get; set; } = new List<HorseInfoDto>();
    }

    public class HorseInfoDto
    {
        public string HorseId { get; set; } = string.Empty;
        public string HorseName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Registration { get; set; }
        public string? StableId { get; set; }
        public string? StableName { get; set; }
    }

    // DTO for trainers with first place winners
    public class TrainerWinnersDto
    {
        public string TrainerId { get; set; } = string.Empty;
        public string TrainerLastName { get; set; } = string.Empty;
        public string TrainerFirstName { get; set; } = string.Empty;
        public string? StableId { get; set; }
        public string? StableName { get; set; }
        public int FirstPlaceWins { get; set; }
        public List<WinningHorseDto> WinningHorses { get; set; } = new List<WinningHorseDto>();
    }

    public class WinningHorseDto
    {
        public string HorseId { get; set; } = string.Empty;
        public string HorseName { get; set; } = string.Empty;
        public string RaceId { get; set; } = string.Empty;
        public string RaceName { get; set; } = string.Empty;
        public decimal? Prize { get; set; }
    }

    // DTO for trainers sorted by total winnings
    public class TrainerWinningsDto
    {
        public string TrainerId { get; set; } = string.Empty;
        public string TrainerLastName { get; set; } = string.Empty;
        public string TrainerFirstName { get; set; } = string.Empty;
        public string? StableId { get; set; }
        public string? StableName { get; set; }
        public decimal TotalWinnings { get; set; }
        public int TotalWins { get; set; }
    }

    // DTO for track statistics
    public class TrackStatsDto
    {
        public string TrackName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public decimal? Length { get; set; }
        public int TotalRaces { get; set; }
        public int TotalHorseParticipants { get; set; }
        public int UniqueHorses { get; set; }
    }
}
