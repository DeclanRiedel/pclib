using Microsoft.AspNetCore.Mvc;
using pclib.Services;
using pclib.Models;

namespace pclib.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileService _fileService;

        public FilesController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FileMetadata>>> GetAllFiles()
        {
            return await _fileService.GetAllFileMetadataAsync();
        }

        [HttpGet("preview/{id}")]
        public async Task<ActionResult<byte[]>> GetFilePreview(int id)
        {
            var metadata = await _fileService.GetFileMetadataAsync(id);
            if (metadata == null)
            {
                return NotFound();
            }

            var preview = await _fileService.GetFilePreviewAsync(metadata.FilePath);
            return File(preview, "image/png");
        }
    }
}
