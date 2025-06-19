using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    // https://localhost:44372/api/regions
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET all regions
        // GET: https://localhost:44372/api/regions
        [HttpGet("/get-all-regions")]
        public async Task<IActionResult> GetAll()
        {
            //var regions = new List<Region>
            //{
            //    new Region
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Auckland Region",
            //        Code = "AKL",
            //        RegionImageUrl = "",
            //    },
            //     new Region
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Wellington Region",
            //        Code = "WLG",
            //        RegionImageUrl = "",
            //    },
            //};

            // Get Data from db - domain models
            var regionsDomain = await dbContext.Regions.ToListAsync();

            // Map Domain models to DTOs
            var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl,
                });
            }

            return Ok(regionsDto);
        }

        // GET region by id
        // GET: https://localhost:44372/api/regions/{id}
        [HttpGet]
        [Route("/get-region/{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id); // this only takes the primary key
            // Get region domain model from db
            var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id); // this can work for any other column
            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            // map/convert region domain model to region dto
            var regionDto = new RegionDto
            {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };

            // retunr dto to client
            return Ok(regionDto);
        }

        // POST create new region
        // POST: https://localhost:44372/api/regions
        [HttpPost("/create-region")]
        public async Task<IActionResult> CreateRegion([FromBody] AddRegionDto addRegionRequestDto)
        {
            // Check if region with the same code already exists
            var existingRegion = await dbContext.Regions.FirstOrDefaultAsync(r => r.Code == addRegionRequestDto.Code);
            if (existingRegion != null)
            {
                return BadRequest("Region already exists.");
            }

            // map/convert region dto to domain model
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl,
            };

            // use domain model to create region
            await dbContext.Regions.AddAsync(regionDomainModel);
            await dbContext.SaveChangesAsync();

            // Map domain model back to dto
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl,
            };

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
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
        {
            // Check if region exists
            var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            // Check if any other region already has the same code or name
            var duplicateRegionCode = await dbContext.Regions.FirstOrDefaultAsync(r =>
                r.Id != id &&
                r.Code == updateRegionDto.Code 
            );
            if (duplicateRegionCode != null)
            {
                return BadRequest(new { message = "Another region with the same code already exists." });
            }

            var duplicateRegionName = await dbContext.Regions.FirstOrDefaultAsync(r =>
                r.Id != id &&
                r.Name == updateRegionDto.Name
            );
            if (duplicateRegionName != null)
            {
                return BadRequest(new { message = "Another region with the same name already exists." });
            }


            // Map dto to domain model
            regionDomain.Code = updateRegionDto.Code;
            regionDomain.Name = updateRegionDto.Name;
            regionDomain.RegionImageUrl = updateRegionDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            // Map domain model to dto
            var regionDto = new RegionDto
            {
                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl,
            };

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
            var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            dbContext.Regions.Remove(regionDomain);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Region deleted successfully."
            });
        }
    }
}
