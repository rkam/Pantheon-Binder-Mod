namespace Binder;

/// <summary>
/// Class that represents an (in-game) location.
/// </summary>
class MapPoint
{
    internal readonly float X;
    internal readonly float Y;
    internal readonly float Z;

    internal readonly CompassArea Area;

    /// <summary>
    /// Note the order of the parameters.
    ///  - Game engine does this and hence most code also does it this way.
    /// </summary>
    internal MapPoint(float x, float z, float y, CompassArea? area)
    {
        X = (float)Math.Round(x, 2);
        Y = (float)Math.Round(y, 2);
        Z = (float)Math.Round(z, 2);

        Area = area ?? new CompassArea();
    }

    internal MapPoint() { X = Y = Z = 0f; Area = new CompassArea(); }

    internal string ZoneName => Area.ZoneName;
    internal string AreaName => Area.AreaName;
}

/// <summary>
/// Class that represents a zone.
/// </summary>
class Zone
{
    internal readonly string Name;
    internal readonly string Abbrev;

    internal Bindstone[] Binds { get; set; }
//  internal Town[]      Towns { get; set; }   // Unnecessary

    internal Zone(string name, string abbrev)
    {
        Name = name;
        Abbrev = abbrev;
        Binds = new Bindstone[0];
    }
}

/// <summary>
/// Class that represents a town (aka city, village, settlement).
/// Note this only has a zone, not a location (since the location is a Polygon).
/// </summary>
class Town
{
    internal readonly string CompassName;
    internal readonly string DisplayName;
    internal readonly Zone Zone;

    internal Town(string compassName, string displayName, Zone zone)
    {
        CompassName = compassName;
        DisplayName = displayName;
        Zone = zone;
    }
}

/// <summary>
/// Class that represents the area the Compass shows - resolves to either a Zone or a Settlement.
/// </summary>
class CompassArea
{
    internal readonly string Name;
    private readonly Zone? _Zone;     // Zone OR
    private readonly Town? _Town;     // Town, never both

    internal CompassArea() { Name = "?"; _Zone = null; _Town = null; }

    internal CompassArea(string name, Zone zone)
    {
        Name = name;
        _Zone = zone;
    }

    internal CompassArea(string name, Town town)
    {
        Name = name;
        _Town = town;
    }

    // ##TODO: Can probably get rid of this..
    // ##TODO:  [ currently searching for Zones, should search Areas (except for Abbrev, hmm). ]
    // ##TODO: Regardless, if search for Zones & Areas everywhere too, then can likely simplify Bindpoint ctor.
    // ##TODO: Also, once that's done, can likely remove some calls to getAreaPlayerIsIn
    internal string ZoneName =>
        (_Zone?.Name ?? _Town?.Zone.Name ?? "?");

    // Thronefast : Availia | Silent Plains
    internal string AreaName => (_Zone?.Name ?? _Town?.DisplayName ?? "?");

    // Thronefast : The Village of Availia | SilentPlains
    internal string CompassName => (_Zone?.Name ?? _Town?.CompassName ?? "?");
}

/// <summary>
/// Class that represents a bindstone.
/// </summary>
class Bindstone
{
    internal readonly string Name;
    internal readonly MapPoint Loc;

    internal Bindstone(string name, MapPoint loc)
    {
        Name = name;
        Loc = loc;
    }

    // To see what bindstone is closest to a player when the bind,
    //  we create a bounding box around the player and see which
    //  bindstone is within it.
    //
    // Data:
    // Wild's End:   OK: /jumploc 3839.11 516.45 -3044.47 272
    // Wild's End:  BAD: /jumploc 3839.17 516.45 -3044.47 272
    //                      So, just 0.06 -- wow, tight!
    private static readonly float _BIND_STONE_RANGE = 10f;     // Bigger is fine

    internal bool IsNear(MapPoint loc) { return IsNear(loc.X, loc.Y); }

    /// <summary>
    /// Is the (player's) location near this bindstone.
    /// </summary>
    internal bool IsNear(float pos_x, float pos_y)
    {

        var left = Loc.X - _BIND_STONE_RANGE;
        var right = Loc.X + _BIND_STONE_RANGE;
        var top = Loc.Y + _BIND_STONE_RANGE;
        var bottom = Loc.Y - _BIND_STONE_RANGE;


        return (pos_x >= left && pos_x <= right) && (pos_y >= bottom && pos_y <= top);
    }
}
