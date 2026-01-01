namespace MotionSimulator;

public record class TrackPoint(
    double Latitude,
    double Longitude,
    decimal? Elevation,
    DateTimeOffset? Time
);
