using System;

namespace MotionSimulator;

public class Activity
{
    private readonly List<TrackPoint> _trackPoints = new();

    public DateTimeOffset? CreatedAt { get; }
    public string? Name { get; }
    public string? ActivityType { get; }

    public IReadOnlyList<TrackPoint> TrackPoints => _trackPoints;

    public Activity(DateTimeOffset createdAt, string? name, string? activityType)
    {
        CreatedAt = createdAt;
        Name = name;
        ActivityType = activityType;
    }

    public void AddTrackpoints(TrackPoint trackpoint)
    {
        _trackPoints.Add(trackpoint);
    }

}