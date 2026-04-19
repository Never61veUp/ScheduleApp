namespace ScheduleApp.Core.Model;

public class Location
{
    public string Name { get; }
    public string MapUrl { get; }

    private Location() { }

    public Location(string name, string mapUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty");

        if (string.IsNullOrWhiteSpace(mapUrl))
            throw new ArgumentException("Map URL cannot be empty");

        Name = name;
        MapUrl = mapUrl;
    }
}