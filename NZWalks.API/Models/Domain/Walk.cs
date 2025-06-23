namespace NZWalks.API.Models.Domain
{
    public class Walk
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double LengthInKm { get; set; } // Length in kilometers
        public string? WalkImageUrl { get; set; }
        public Guid RegionId { get; set; } // Foreign key to Region
        public Guid DifficultyId { get; set; } // Foreign key to Difficulty
        
        // Navigation properties
        public Region Region { get; set; }
        public Difficulty Difficulty { get; set; }
    }
}