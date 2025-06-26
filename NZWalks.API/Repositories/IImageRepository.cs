using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IImageRepository
    {
        Task<Image> UploadAsync(Image image);
    }
}
