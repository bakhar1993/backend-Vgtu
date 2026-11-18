using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UavApi.Data;
using UavApi.DTOs;
using UavApi.Models;

namespace UavApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DronesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public DronesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<DroneResponseDto>>> GetDrones()
    {
        var drones = await _context.Drones
            .Select(d => new DroneResponseDto
            {
                Id = d.Id,
                ModelName = d.ModelName,
                Manufacturer = d.Manufacturer,
                MaxTakeoffWeight = d.MaxTakeoffWeight,
                MaxFlightTime = d.MaxFlightTime
            })
            .ToListAsync();

        return Ok(drones);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<DroneResponseDto>> GetDrone(Guid id)
    {
        var drone = await _context.Drones.FindAsync(id);
        if (drone == null) return NotFound("БПЛА не найден");

        return Ok(new DroneResponseDto
        {
            Id = drone.Id,
            ModelName = drone.ModelName,
            Manufacturer = drone.Manufacturer,
            MaxTakeoffWeight = drone.MaxTakeoffWeight,
            MaxFlightTime = drone.MaxFlightTime
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DroneResponseDto>> CreateDrone(DroneCreateDto dto)
    {
        var drone = new Drone
        {
            Id = Guid.NewGuid(),
            ModelName = dto.ModelName,
            Manufacturer = dto.Manufacturer,
            MaxTakeoffWeight = dto.MaxTakeoffWeight,
            MaxFlightTime = dto.MaxFlightTime
        };

        _context.Drones.Add(drone);
        await _context.SaveChangesAsync();

        var response = new DroneResponseDto
        {
            Id = drone.Id,
            ModelName = drone.ModelName,
            Manufacturer = drone.Manufacturer,
            MaxTakeoffWeight = drone.MaxTakeoffWeight,
            MaxFlightTime = drone.MaxFlightTime
        };

        return CreatedAtAction(nameof(GetDrone), new { id = drone.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DroneResponseDto>> UpdateDrone(Guid id, DroneCreateDto dto)
    {
        var drone = await _context.Drones.FindAsync(id);
        if (drone == null) return NotFound("БПЛА не найден");

        drone.ModelName = dto.ModelName;
        drone.Manufacturer = dto.Manufacturer;
        drone.MaxTakeoffWeight = dto.MaxTakeoffWeight;
        drone.MaxFlightTime = dto.MaxFlightTime;

        await _context.SaveChangesAsync();

        return Ok(new DroneResponseDto
        {
            Id = drone.Id,
            ModelName = drone.ModelName,
            Manufacturer = drone.Manufacturer,
            MaxTakeoffWeight = drone.MaxTakeoffWeight,
            MaxFlightTime = drone.MaxFlightTime
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDrone(Guid id)
    {
        var drone = await _context.Drones.FindAsync(id);
        if (drone == null) return NotFound("БПЛА не найден");

        _context.Drones.Remove(drone);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
