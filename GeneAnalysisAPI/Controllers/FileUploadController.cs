using GeneAnalysisAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeneAnalysisAPI.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly MinioService _minioService;

        public FileUploadController(MinioService minioService)
        {
            _minioService = minioService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // ✅ Required for Swagger
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files) // 🔹 Changed to List<IFormFile>
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            foreach (var file in files)
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;
                await _minioService.UploadFileAsync(stream, file.FileName);
            }

            return Ok(new { message = $"{files.Count} file(s) uploaded successfully" });
        }



        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var stream = await _minioService.GetFileAsync(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
    }
}
