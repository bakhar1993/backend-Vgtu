using System.ComponentModel.DataAnnotations;

namespace UavApi.DTOs;

public class FlightRecordCreateDto
{
    [Required]
    public Guid DroneId { get; set; }

    [Required]
    public DateTime FlightDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Продолжительность полета должна быть больше 0")]
    public int DurationMinutes { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Дистанция не может быть отрицательной")]
    public double DistanceKm { get; set; }
}

public class FlightRecordResponseDto
{
    public Guid Id { get; set; }
    public Guid DroneId { get; set; }
    public string? DroneModelName { get; set; }
    public Guid UserId { get; set; }
    public DateTime FlightDate { get; set; }
    public int DurationMinutes { get; set; }
    public double DistanceKm { get; set; }
}
