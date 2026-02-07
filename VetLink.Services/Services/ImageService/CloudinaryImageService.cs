using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
//using VetLink.Services.Configurations;
using VetLink.Services.Services.ImageService;
using VetLink.Services.Services.ImageService.Dtos;

public class CloudinaryImageStorageService : IImageStorageService
{
	private readonly Cloudinary _cloudinary;

	public CloudinaryImageStorageService(
		IOptions<CloudinarySettings> config)
	{
		var acc = new Account(
			config.Value.CloudName,
			config.Value.ApiKey,
			config.Value.ApiSecret
		);

		_cloudinary = new Cloudinary(acc);
	}

	public async Task<List<ImageUploadResultDto>> UploadImagesAsync(
		  List<IFormFile> files,
		  string folder)
	{
		var results = new List<ImageUploadResultDto>();

		foreach (var file in files)
		{
			if (file.Length == 0) continue;

			using var stream = file.OpenReadStream();

			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, stream),
				Folder = folder,
				Transformation = new Transformation()
					.Quality("auto")
					.FetchFormat("auto")
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);

			results.Add(new ImageUploadResultDto
			{
				Url = uploadResult.SecureUrl.ToString(),
				PublicId = uploadResult.PublicId
			});
		}

		return results;
	}

	public async Task<string> UploadImageAsync(
		IFormFile file,
		string folder,
		bool isPrimary = false)
	{
		if (file.Length == 0)
			throw new Exception("Empty file");

		await using var stream = file.OpenReadStream();

		var uploadParams = new ImageUploadParams
		{
			File = new FileDescription(file.FileName, stream),
			Folder = folder,
			UseFilename = true,
			UniqueFilename = true,
			Overwrite = false
		};

		var result = await _cloudinary.UploadAsync(uploadParams);

		if (result.StatusCode != System.Net.HttpStatusCode.OK)
			throw new Exception("Cloudinary upload failed");

		return result.SecureUrl.ToString();
	}

	public async Task DeleteImageAsync(List<string> publicIds)
	{
		foreach (var id in publicIds)
		{
			await _cloudinary.DestroyAsync(
				new DeletionParams(id)
			);
		}
	}
}
