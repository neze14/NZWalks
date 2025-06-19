using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    // https://localhost:44372/api/regions
    [Route("api/[controller]")]
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
        [HttpGet]
        public IActionResult GetAll()
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
            var regionsDomain = dbContext.Regions.ToList();

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
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id); // this only takes the primary key
            // Get region domain model from db
            var regionDomain = dbContext.Regions.FirstOrDefault(r => r.Id == id); // this can work for any other column

            if (regionDomain == null)
            {
                return NotFound();
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
        [HttpPost]
        public IActionResult CreateRegion([FromBody] AddRegionDto addRegionRequestDto)
        {
            // Check if region with the same code already exists
            var existingRegion = dbContext.Regions.FirstOrDefault(r => r.Code == addRegionRequestDto.Code);
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
            dbContext.Regions.Add(regionDomainModel);
            dbContext.SaveChanges();

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
        [Route("{id:Guid}")]
        public IActionResult UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
        {
            // Check if region exists
            var regionDomain = dbContext.Regions.FirstOrDefault(r => r.Id == id);
            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            // Check if any other region already has the same code or name
            var duplicateRegionCode = dbContext.Regions.FirstOrDefault(r =>
                r.Id != id &&
                r.Code == updateRegionDto.Code 
            );
            if (duplicateRegionCode != null)
            {
                return BadRequest(new { message = "Another region with the same code already exists." });
            }

            var duplicateRegionName = dbContext.Regions.FirstOrDefault(r =>
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

            dbContext.SaveChanges();

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
        [Route("{id:Guid}")]
        public IActionResult DeleteRegion([FromRoute] Guid id)
        {
            // Check if region exists
            var regionDomain = dbContext.Regions.FirstOrDefault(r => r.Id == id);
            if (regionDomain == null)
            {
                return NotFound(new { message = "Region not found." });
            }

            dbContext.Regions.Remove(regionDomain);
            dbContext.SaveChanges();

            return Ok(new
            {
                message = "Region deleted successfully."
            });
        }
    }
}
