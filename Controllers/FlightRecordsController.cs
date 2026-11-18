using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UavApi.Data;
using UavApi.DTOs;
using UavApi.Models;

namespace UavApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightRecordsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public FlightRecordsController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlightRecordResponseDto>>> GetMyRecords()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr == null) return Unauthorized();

        var userId = Guid.Parse(userIdStr);
        var isAdmin = User.IsInRole("Admin");

        var query = _context.FlightRecords.Include(f => f.Drone).AsQueryable();
        if (!isAdmin)
        {
            query = query.Where(f => f.UserId == userId);
        }

        var records = await query
            .Select(f => new FlightRecordResponseDto
            {
                Id = f.Id,
                DroneId = f.DroneId,
                DroneModelName = f.Drone != null ? f.Drone.ModelName : null,
                UserId = f.UserId,
                FlightDate = f.FlightDate,
                DurationMinutes = f.DurationMinutes,
                DistanceKm = f.DistanceKm
            })
            .ToListAsync();

        return Ok(records);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FlightRecordResponseDto>> GetRecord(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        var record = await _context.FlightRecords
            .Include(f => f.Drone)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (record == null) return NotFound("Запись о полете не найдена");
        if (!isAdmin && record.UserId != userId) return Forbid();

        return Ok(new FlightRecordResponseDto
        {
            Id = record.Id,
            DroneId = record.DroneId,
            DroneModelName = record.Drone?.ModelName,
            UserId = record.UserId,
            FlightDate = record.FlightDate,
            DurationMinutes = record.DurationMinutes,
            DistanceKm = record.DistanceKm
        });
    }

    [HttpPost]
    public async Task<ActionResult<FlightRecordResponseDto>> CreateRecord(FlightRecordCreateDto dto)
    {
        var drone = await _context.Drones.FindAsync(dto.DroneId);
        if (drone == null) return NotFound("Указанный БПЛА не найден");

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var record = new FlightRecord
        {
            Id = Guid.NewGuid(),
            DroneId = dto.DroneId,
            UserId = userId,
            FlightDate = dto.FlightDate,
            DurationMinutes = dto.DurationMinutes,
            DistanceKm = dto.DistanceKm
        };

        _context.FlightRecords.Add(record);
        await _context.SaveChangesAsync();

        var response = new FlightRecordResponseDto
        {
            Id = record.Id,
            DroneId = record.DroneId,
            DroneModelName = drone.ModelName,
            UserId = record.UserId,
            FlightDate = record.FlightDate,
            DurationMinutes = record.DurationMinutes,
            DistanceKm = record.DistanceKm
        };

        return CreatedAtAction(nameof(GetRecord), new { id = record.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FlightRecordResponseDto>> UpdateRecord(Guid id, FlightRecordCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        var record = await _context.FlightRecords.FindAsync(id);
        if (record == null) return NotFound("Запись о полете не найдена");
        if (!isAdmin && record.UserId != userId) return Forbid();

        var drone = await _context.Drones.FindAsync(dto.DroneId);
        if (drone == null) return NotFound("Указанный БПЛА не найден");

        record.DroneId = dto.DroneId;
        record.FlightDate = dto.FlightDate;
        record.DurationMinutes = dto.DurationMinutes;
        record.DistanceKm = dto.DistanceKm;

        await _context.SaveChangesAsync();

        return Ok(new FlightRecordResponseDto
        {
            Id = record.Id,
            DroneId = record.DroneId,
            DroneModelName = drone.ModelName,
            UserId = record.UserId,
            FlightDate = record.FlightDate,
            DurationMinutes = record.DurationMinutes,
            DistanceKm = record.DistanceKm
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecord(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        var record = await _context.FlightRecords.FindAsync(id);
        if (record == null) return NotFound("Запись о полете не найдена");
        if (!isAdmin && record.UserId != userId) return Forbid();

        _context.FlightRecords.Remove(record);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
