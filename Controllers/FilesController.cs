using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UavApi.Data;
using UavApi.Models;

namespace UavApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public FilesController(ApplicationDbContext context) => _context = context;

    [HttpGet("flight/{flightRecordId}")]
    public async Task<IActionResult> GetFilesByFlight(Guid flightRecordId)
    {
        var files = await _context.FlightDocuments
            .Where(d => d.FlightRecordId == flightRecordId)
            .Select(d => new { d.Id, d.FileName, d.UploadDate })
            .ToListAsync();

        return Ok(files);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] Guid flightRecordId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран");

        var flightExists = await _context.FlightRecords.AnyAsync(f => f.Id == flightRecordId);
        if (!flightExists)
            return NotFound("Запись о полете не найдена");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var safeFileName = Path.GetFileName(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var document = new FlightDocument
        {
            Id = Guid.NewGuid(),
            FlightRecordId = flightRecordId,
            FileName = safeFileName,
            FilePath = filePath,
            UploadDate = DateTime.UtcNow
        };

        _context.FlightDocuments.Add(document);
        await _context.SaveChangesAsync();

        return Ok(new { document.Id, document.FileName, document.UploadDate });
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var document = await _context.FlightDocuments.FindAsync(id);
        if (document == null) return NotFound("Файл не найден в БД");

        if (!System.IO.File.Exists(document.FilePath))
            return NotFound("Физический файл не найден на сервере");

        var bytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);
        return File(bytes, "application/octet-stream", document.FileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(Guid id)
    {
        var document = await _context.FlightDocuments.FindAsync(id);
        if (document == null) return NotFound("Файл не найден в БД");

        try
        {
            if (System.IO.File.Exists(document.FilePath))
            {
                System.IO.File.Delete(document.FilePath);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при удалении файла с диска: {ex.Message}");
        }

        _context.FlightDocuments.Remove(document);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
