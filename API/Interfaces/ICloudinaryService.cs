using System;
using API.Services;

namespace API.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task DeleteImageAsync(string publicId);
}
