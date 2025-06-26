using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:44372/api/v1/images
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        //POST: api/v1/images/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequestDto requestDto)
        {
            ValidateFileUpload(requestDto);

            if (ModelState.IsValid)
            {
                // convert dto to domain model
                var imageDomain = new Image
                {
                    File = requestDto.File,
                    FileExtension = Path.GetExtension(requestDto.File.FileName),
                    FileDescription = requestDto.FileDescription,
                    FileName = requestDto.FileName,
                    FileSizeInBytes = requestDto.File.Length,
                };

                await imageRepository.UploadAsync(imageDomain);

                return Ok(new
                {
                    message = "Image uploaded successfully", 
                    imageDomain
                });

            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtrensions = new string[] { ".jpg", ".jpeg", ".png", ".pdf" };

            if (!allowedExtrensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File is more than 10mb. Please upload a smaller size.");
            }
        }
    }
}
