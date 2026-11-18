namespace UavApi.Models;
public class FlightRecord {
    public Guid Id { get; set; }
    public Guid DroneId { get; set; }
    public Drone? Drone { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime FlightDate { get; set; }
    public int DurationMinutes { get; set; }
    public double DistanceKm { get; set; }
}