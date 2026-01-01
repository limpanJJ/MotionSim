namespace MotionSimulator.Activities;

public sealed class ActivityAnalyzer
{
    public ActivityMetrics Analyze(Activity activity)
    {
        double totalDistance = CalculateTotalDistance(activity);

        return new ActivityMetrics
        {
            DistanceKm = totalDistance
        };
    }
    public double CalculateTotalDistance(Activity activity)
    {
        var points = activity.TrackPoints;

        if (points.Count < 2)
            return 0;

        double total = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            total += Haversine(points[i], points[i + 1]);
        }
        return total;
    }
    private static double Haversine(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        // distance between latitudes and longitudes
        double dLat = Math.PI / 180 * (lat2 - lat1);
        double dLon = Math.PI / 180 * (lon2 - lon1);

        // convert to radians
        lat1 = Math.PI / 180 * lat1;
        lat2 = Math.PI / 180 * lat2;

        // apply formulae
        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                   Math.Pow(Math.Sin(dLon / 2), 2) *
                   Math.Cos(lat1) * Math.Cos(lat2);
        double rad = 6371;
        double c = 2 * Math.Asin(Math.Sqrt(a));
        return rad * c;
    }
    public static double Haversine(TrackPoint p1, TrackPoint p2)
    {
        return Haversine(
            p1.Latitude,
            p1.Longitude,
            p2.Latitude,
            p2.Longitude
        );
    }
}
