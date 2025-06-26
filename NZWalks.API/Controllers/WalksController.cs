using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:44372/api/walks
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]

    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        // GET: get all walks
        [HttpGet("get-all-walks")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 5)
        {
            var walksDomain = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            // Map domain model to dto
            return Ok(mapper.Map<List<WalkDto>>(walksDomain));

        }

        // GET: get all walks
        [HttpGet]
        [Route("get-walk/{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walksDomain = await walkRepository.GetByIdAsync(id);

            if (walksDomain == null) return NotFound(new { message = "Walk not found." });

            // Map domain model to dto
            return Ok(mapper.Map<WalkDto>(walksDomain));
        }

        // POST: create walk
        [HttpPost("create-walk")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddWalkDto addWalkDto)
        {
             // Map dto to domain model
            var walksDomain = mapper.Map<Walk>(addWalkDto);

            await walkRepository.CreateAsync(walksDomain);

            // Map domain model to dto
            return Ok(new
            {
                message = "Walk registered successfully",
                data = mapper.Map<WalkDto>(walksDomain)
            });
        }

        // PUT: update walk
        [HttpPut]
        [Route("update-walk/{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkDto updateWalkDto)
        {
            // Map dto to domain model
            var walkDomain = mapper.Map<Walk>(updateWalkDto);

            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            if (walkDomain == null) return NotFound(new { message = "Walk not found." });

            // Map domain model to dto
            return Ok(new
            {
                message = "Walk updated successfully",
                data = mapper.Map<WalkDto>(walkDomain)
            });
        }

        // DELETE: delete walk
        [HttpDelete]
        [Route("delete-walk/{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walksDomain = await walkRepository.DeleteAsync(id);

            if (walksDomain == null) return NotFound(new { message = "Walk not found." });

            // Map domain model to dto
            //return Ok(mapper.Map<WalkDto>(walksDomain));
            return Ok(new { message = "Walk deleted successfully."});
        }
    }
}
