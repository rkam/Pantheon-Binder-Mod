namespace Binder;

/// <summary>
/// Class defining the information to create a Zone
/// </summary>
internal class ZoneEntry
{
    internal string ZoneName;
    internal string CompassName;
    internal string Key;

    internal ZoneEntry(string zoneName, string compassName, string key)
    {
        ZoneName = zoneName;
        CompassName = compassName;
        Key = key;
    }
}

/// <summary>
/// Class defining the information to create a Town/Settlement
/// </summary>
internal class TownEntry
{
    internal string Fullname;
    internal string Displayname;
    internal string CompassName;
    internal string Key;
    internal string ZoneKey;

    internal TownEntry(string fullName, string displayName, string? compassName, string key, string zoneKey)
    {
        Fullname = fullName;
        Displayname = displayName;
        CompassName = compassName ?? fullName;
        Key = key;
        ZoneKey = zoneKey;
    }
}

/// <summary>
/// Class defining the information to create a Bindstone.
/// </summary>
internal class BindEntry
{
    internal string Name;
    internal string ZoneKey;
    internal float X;
    internal float Y;
    internal float Z;

    internal BindEntry(string name, string zoneKey, float x, float z, float y)
    {
        Name = name;
        ZoneKey = zoneKey;
        X = x;
        Y = y;
        Z = z;
    }
}

/// <summary>
/// Run through the data in LocInfo.cs and create the relevant objects
///  (Zones, Towns, CompassArea and Bindstones).
/// Keeping the data in a separate file makes it easier to update when new
///   changes are launched.  (e.g. to add a new Zone/Town/Stone, just add an
///   entry to the appropriate array.
/// This code ensures that everything is created and connected properly.
///   e.g. There is no CompassArea info, since it can be created from the
///         Zone/Town info.
/// </summary>
class LocBuilder
{
    internal static Zone[] Zones;                   // List of known Zones
    internal static Town[] Towns;                   // List of known Towns
    internal static CompassArea[] CompassAreas;     // List of known CompassAreas
    internal static Bindstone[] Bindstones;         // List of known Bindstones

    static LocBuilder()
    {
        Dictionary<string, Zone> zoneHash = new Dictionary<string, Zone>();
        Dictionary<string, Town> townHash = new Dictionary<string, Town>();
        Dictionary<string, CompassArea> compHash = new Dictionary<string, CompassArea>();


        // We will need this later for sorting bindstones into zones.
        // For now, we'll just init it to empty values.
        Dictionary<string, List<Bindstone>> zoneBindHash = new Dictionary<string, List<Bindstone>>();

        // Zones and Towns match Shalazam - NOTE: Shalazam (game?) calls Towns Settlements

        List<Zone> zones = new List<Zone>();
        List<Town> towns = new List<Town>();
        List<CompassArea> compassAreas = new List<CompassArea>();
        List<Bindstone> bindstones = new List<Bindstone>();

        foreach (ZoneEntry z in LocInfo.ZoneInfo)   // zonename, compassname, key
        {
            var zone = new Zone(z.ZoneName, z.Key);
            zoneHash[z.Key] = zone;
            zones.Add(zone);

            var compass = new CompassArea(z.CompassName, zone);
            compHash[z.Key] = compass;
            compassAreas.Add(compass);

            zoneBindHash[z.Key] = new List<Bindstone>();
        }

        foreach (TownEntry t in LocInfo.TownInfo)    // fullname, displayname, compassname, key, zonekey
        {
            var zone = zoneHash[t.ZoneKey];
            var town = new Town(t.Fullname, t.Displayname, zone);
            townHash[t.Key] = town;
            towns.Add(town);

            var compass = new CompassArea(t.CompassName, town);
            compHash[t.Key] = compass;
            compassAreas.Add(compass);
        }

        Zones = zones.ToArray();
        Towns = towns.ToArray();
        CompassAreas = compassAreas.ToArray();

        foreach (BindEntry t in LocInfo.BindInfo)    // name, zonekey, x, z, y
        {
            // NOTE: zonekey == compkey
            var comp = compHash[t.ZoneKey];
            var loc = new MapPoint(t.X, t.Z, t.Y, compHash[t.ZoneKey]);
            var bind = new Bindstone(t.Name, loc);

            bindstones.Add(bind);

            zoneBindHash[t.ZoneKey].Add(bind);
        }

        Bindstones = bindstones.ToArray();

        foreach (string k in zoneBindHash.Keys)
        {
            zoneHash[k].Binds = zoneBindHash[k].ToArray();
        }
    }
}
