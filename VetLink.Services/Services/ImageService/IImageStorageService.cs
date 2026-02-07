using Microsoft.AspNetCore.Http;
using VetLink.Services.Services.ImageService.Dtos;

namespace VetLink.Services.Services.ImageService
{
    public interface IImageStorageService
    {
		Task<List<ImageUploadResultDto>> UploadImagesAsync(List<IFormFile> files,string folder);

		Task<string> UploadImageAsync(IFormFile file,string folder,bool isPrimary = false);

		Task DeleteImageAsync(List<string> publicIds);
	}
}
