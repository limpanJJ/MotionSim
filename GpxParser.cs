using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace LoppSimulator;

public class GpxParser
{
    public record class TrackPoint(
        double Latitude,
        double Longitude,
        decimal? Elevation,
        DateTimeOffset? Time
    );
    public class Activity
    {
        public int Id { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? Name { get; set; }
        public string? ActivityType { get; set; }
        public List<TrackPoint> TrackPoints { get; set; } = new();

        public void AddDetails(TrackPoint trackpoint)
        {
            TrackPoints.Add(trackpoint);
        }

    }
    public static void ReadFile()
    {
        var filename = "Night_Walk.gpx";
        var currentDirectory = Directory.GetCurrentDirectory();
        var RunningFilepath = Path.Combine(currentDirectory, filename);
        XDocument gpxDoc = XDocument.Load(RunningFilepath);

        XNamespace gpx = "http://www.topografix.com/GPX/1/1";


        var firstTrkPt = gpxDoc
            .Descendants(gpx + "trkpt")
            .FirstOrDefault();

        Console.WriteLine(firstTrkPt);


        var newActivity = new Activity();

        foreach (XElement element in gpxDoc.Descendants())
        {
            // Get Activity activity creation time
            if (element.Name.LocalName == "time" && element.Parent.Name.LocalName == "metadata")
            {
                DateTimeOffset timestamp = DateTimeOffset.Parse(element.Value);
                newActivity.CreatedAt = timestamp;
            }

            // Get Name of the activity
            if (element.Name.LocalName == "name" && element.Parent.Name.LocalName == "trk")
            {
                newActivity.Name = element.Value;
            }
            // Get Type of activity
            if (element.Name.LocalName == "type" && element.Parent.Name.LocalName == "trk")
            {
                newActivity.ActivityType = element.Value;
            }

            // Get Latitude and Longitude
            if (element.Name.LocalName == "trkpt" && element.Parent.Name.LocalName == "trkseg")
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

                var raw = eleElement.Value.Trim();

                if (eleElement != null)
                {
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

                var rawTime = timeElement.Value.Trim();

                if (timeElement != null)
                {
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
                }
                var track = new TrackPoint(latitude, longitude, elevation, trackTime);
                // Add Trackdetails to the 
                newActivity.AddDetails(track);
            }


            // if (element.Name.LocalName == "trkpt")
            // {
            //     foreach (XAttribute attribute in element.Attributes())
            //     {
            //         Console.WriteLine($"{attribute.Name}: {attribute.Value}");
            //     }
            // }
            // Console.WriteLine($"{element.Name.LocalName}: {element.Value}");
        }
        foreach (var element in newActivity.TrackPoints)
        {
            Console.WriteLine(element);
        }

        // IEnumerable<XElement> c1 = from el in gpxDoc.Elements(gpx + "ele") select el;
        // foreach (XElement el in c1)
        //     Console.WriteLine(el);
    }
}
