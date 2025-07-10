using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using System.Runtime.InteropServices;

namespace NZWalks.API.Repositories
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetAllAsync(
            string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 5
            );

        Task<Region?> GetByIdAsync(Guid id);

        Task<Region> CreateAsync(Region region);

        Task<(Region? region, string message)> UpdateAsync(Guid id, UpdateRegionDto region);

        Task<Region?> DeleteAsync(Guid id);
    }
}
