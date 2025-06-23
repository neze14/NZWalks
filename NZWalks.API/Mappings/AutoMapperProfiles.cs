using AutoMapper;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region, RegionDto>().ReverseMap(); 
            CreateMap<AddRegionDto, Region>().ReverseMap(); 
            CreateMap<UpdateRegionDto, Region>().ReverseMap(); 
            CreateMap<Walk, WalkDto>().ReverseMap(); 
            CreateMap<AddWalkDto, Walk>().ReverseMap(); 
            CreateMap<Difficulty, DifficultyDto>().ReverseMap(); 
            CreateMap<UpdateWalkDto, Walk>().ReverseMap(); 
        }
    }
}
