using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Driver.Services.Api.Controllers;

[ApiController]
[Route("/uploads")]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FilesController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet("{filename}")]
    public IActionResult GetFile(string filename)
    {
        // Security check: prevent path traversal
        if (
            string.IsNullOrEmpty(filename)
            || filename.Contains("..")
            || filename.Contains("/")
            || filename.Contains("\\")
        )
        {
            return BadRequest("Invalid filename");
        }

        var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads");
        var filePath = Path.Combine(uploadsPath, filename);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found");
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var fileStream = System.IO.File.OpenRead(filePath);
        return File(fileStream, contentType, filename);
    }
}

