using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class AddRegionDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code must have 3 characteres")]
        [MaxLength(3, ErrorMessage =  "Code must have 3 characteres")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characteres")]
        public string Name { get; set; }

        public string? RegionImageUrl { get; set; }
    }
}
