using System.Globalization;
using System.Xml.Linq;

namespace MotionSimulator;

public class GpxParser
{
    public static Activity CreateActivity()
    {
        var filename = "Night_Walk.gpx";
        var currentDirectory = Directory.GetCurrentDirectory();
        var RunningFilepath = Path.Combine(currentDirectory, filename);
        XDocument gpxDoc = XDocument.Load(RunningFilepath);

        XNamespace gpx = "http://www.topografix.com/GPX/1/1";


        var firstTrkPt = gpxDoc
            .Descendants(gpx + "trkpt")
            .FirstOrDefault();

        // Console.WriteLine(firstTrkPt);


        var parseData = new ParsedData();

        foreach (XElement element in gpxDoc.Descendants())
        {
            // Get Activity activity creation time
            if (element.Name.LocalName == "time" && element.Parent?.Name.LocalName == "metadata")
            {
                DateTimeOffset timestamp = DateTimeOffset.Parse(element.Value);
                parseData.CreatedAt = timestamp;
            }

            // Get Name of the activity
            if (element.Name.LocalName == "name" && element.Parent?.Name.LocalName == "trk")
            {
                parseData.Name = element.Value;
            }
            // Get Type of activity
            if (element.Name.LocalName == "type" && element.Parent?.Name.LocalName == "trk")
            {
                parseData.ActivityType = element.Value;
            }

            // Get Latitude and Longitude
            if (element.Name.LocalName == "trkpt" && element.Parent?.Name.LocalName == "trkseg")
            {
                var latAttr = element.Attribute("lat");
                var lonAttr = element.Attribute("lon");

                if (latAttr is null || lonAttr is null)
                {
                    throw new InvalidOperationException("Missing lat/lon attributes.");
                }

                if (!double.TryParse(latAttr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) ||
                    !double.TryParse(lonAttr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
                {
                    throw new FormatException("Invalid lat/lon format.");
                }
                // Get Elevation

                decimal? elevation = null;
                var eleElement = element.Elements()
                    .FirstOrDefault(e => e.Name.LocalName == "ele");


                if (eleElement != null)
                {
                    var raw = eleElement.Value.Trim();
                    if (!decimal.TryParse(raw,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var parsedElevation))
                    {
                        throw new FormatException("Invalid elevation format.");
                    }

                    elevation = parsedElevation;
                }

                // Get timestamp at location
                var dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";
                DateTimeOffset? trackTime = null;

                var timeElement = element.Elements()
                    .FirstOrDefault(e => e.Name.LocalName == "time");

                if (timeElement is null)
                {
                    // missing time is OK!
                    continue;
                }
                var rawTime = timeElement.Value.Trim();

                if (!DateTimeOffset.TryParseExact
                    (rawTime,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var parsedTime))
                {
                    throw new FormatException("Invalid time format.");
                }
                trackTime = parsedTime;

                var track = new TrackPoint(latitude, longitude, elevation, trackTime);
                // Add Trackdetails to the ParseData
                parseData.TrackPoints.Add(track);

            }

        }

        var activity = new Activity(
            parseData.CreatedAt,
            parseData.Name,
            parseData.ActivityType
        );

        foreach (var tp in parseData.TrackPoints)
        {
            activity.AddTrackpoints(tp);
        }


        // foreach (var element in activity.TrackPoints)
        // {
        //     Console.WriteLine(element);
        // }

        return activity;


        // IEnumerable<XElement> c1 = from el in gpxDoc.Elements(gpx + "ele") select el;
        // foreach (XElement el in c1)
        //     Console.WriteLine(el);
    }
    internal sealed class ParsedData
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string? Name { get; set; }
        public string? ActivityType { get; set; }

        public List<TrackPoint> TrackPoints { get; } = new();
    }

}
