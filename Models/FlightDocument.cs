namespace UavApi.Models;
public class FlightDocument {
    public Guid Id { get; set; }
    public Guid FlightRecordId { get; set; }
    public FlightRecord? FlightRecord { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
}