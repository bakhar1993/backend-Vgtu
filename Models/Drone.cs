namespace UavApi.Models;
public class Drone {
    public Guid Id { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public double MaxTakeoffWeight { get; set; }
    public int MaxFlightTime { get; set; }
    public ICollection<FlightRecord> FlightRecords { get; set; } = new List<FlightRecord>();
}