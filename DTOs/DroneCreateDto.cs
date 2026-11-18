using System.ComponentModel.DataAnnotations;

namespace UavApi.DTOs;

public class DroneCreateDto
{
    [Required]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    public string Manufacturer { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Максимальный взлетный вес должен быть больше 0")]
    public double MaxTakeoffWeight { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Максимальное время полета должно быть больше 0")]
    public int MaxFlightTime { get; set; }
}

public class DroneResponseDto
{
    public Guid Id { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public double MaxTakeoffWeight { get; set; }
    public int MaxFlightTime { get; set; }
}
