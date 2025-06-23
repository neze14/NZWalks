using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:44372/api/regions
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper )
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        // GET all regions
        // GET: https://localhost:44372/api/regions
        [HttpGet("/get-all-regions")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            // Get Data from db - domain models
            var regionsDomain = await regionRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            // Map Domain models to DTOs
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            return Ok(regionsDto);
        }

        // GET region by id
        // GET: https://localhost:44372/api/regions/{id}
        [HttpGet]
        [Route("/get-region/{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Get region domain model from db
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            // map/convert region domain model to region dto
            var regionDto = mapper.Map<RegionDto>(regionDomain);

            // retunr dto to client
            return Ok(regionDto);
        }

        // POST create new region
        // POST: https://localhost:44372/api/regions
        [HttpPost("/create-region")]
        [ValidateModel]
        public async Task<IActionResult> CreateRegion([FromBody] AddRegionDto addRegionRequestDto)
        {
            // Check if region with the same code already exists
            var existingRegion = await dbContext.Regions.FirstOrDefaultAsync(r => r.Code == addRegionRequestDto.Code);
            if (existingRegion != null)
            {
                return BadRequest("Region already exists.");
            }

            // map/convert region dto to domain model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            // use domain model to create region
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            // Map domain model back to dto
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            //return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, new
            {
                message = "Region created successfully.",
                data = regionDto
            });
        }

        // PUT update region
        // PUT: https://localhost:44372/api/regions/{id}
        [HttpPut]
        [Route("/update-region/{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
        {
            // map dtp tp domain model
            var regionDomainModel = mapper.Map<Region>(updateRegionDto);

            // Check if region exists
            var response = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (!string.IsNullOrWhiteSpace(response.message))
            {
                return response.message.ToLower().Contains("not found")
                    ? NotFound(new { message = response.message })
                    : BadRequest(new { message = response.message });
            }

            await dbContext.SaveChangesAsync();

            // Map domain model to dto
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(new
            {
                message = "Region updated successfully.",
                data = regionDto
            });
        }

        // DELETE deleta region
        // DELETE: https://localhost:44372/api/regions/{id}
        [HttpDelete]
        [Route("/delete-region/{id:Guid}")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            // Check if region exists
            var regionDomain = await regionRepository.DeleteAsync(id);

            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            return Ok(new
            {
                message = "Region deleted successfully."
            });
        }
    }
}
