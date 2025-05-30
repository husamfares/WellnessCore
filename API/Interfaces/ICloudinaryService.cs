using System;

namespace API.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> AnalyzeImageAsync(string imageUrl);

}
