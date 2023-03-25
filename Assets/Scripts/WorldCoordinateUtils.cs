using System;
using System.Globalization;

public class WorldCoordinate
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public WorldCoordinate(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }
    
    public WorldCoordinate(string latLongString)
    {
        string[] latLong = latLongString.Split(',');
        float latitude;
        float longitude;
        GradeStatisticsView.TryParseFloat(latLong[0], out latitude);
        GradeStatisticsView.TryParseFloat(latLong[1], out longitude);
        Longitude = longitude;
        Latitude = latitude;
    }
    
    public override string ToString()
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        return $"{Latitude.ToString(nfi)}, {Longitude.ToString(nfi)}";
    }
}

public static class WorldCoordinateUtils
{
    public static WorldCoordinate GetAbsoluteCenter(WorldCoordinate coordinate1, WorldCoordinate coordinate2)
    {
        double centerLongitude = (coordinate1.Longitude + coordinate2.Longitude) / 2.0;
        double centerLatitude = (coordinate1.Latitude + coordinate2.Latitude) / 2.0;

        return new WorldCoordinate(centerLongitude, centerLatitude);
    }
}