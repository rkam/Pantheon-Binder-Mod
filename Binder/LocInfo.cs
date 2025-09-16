namespace Binder;

/// <summary>
/// Data class for Zones, Towns, and Bindstones.
///  LocBuilder.cs uses the data here to construct the above objects.
///  This just provides an easy to modify set of data.
/// </summary>
class LocInfo
{
    internal static ZoneEntry[] ZoneInfo;
    internal static TownEntry[] TownInfo;
    internal static BindEntry[] BindInfo;

    static LocInfo()
    {
        // ZONES ---------------------------------------------------------------
        //                 zonename                key
        ZoneInfo = new ZoneEntry[] {
            new ZoneEntry("Thronefast",            "TF"),
            new ZoneEntry("Avendyr's Pass",        "AVP"),
            new ZoneEntry("Eastern Plains",        "EP"),
            new ZoneEntry("Silent Plains",         "SP"),
            new ZoneEntry("Wilds End",             "WE"),

            new ZoneEntry("Halnir Cave",           "HC"),
            new ZoneEntry("Black Rose Keep",       "BRK"),

         // Announced ("as glimpse into next major zone") 250909 (Black Rose Keep release post)
//          new ZoneEntry("Badia de Cara",         "BC"),

         // City/Zone on Shalazam map - announced sometime pre-EA
//          new ZoneEntry("Faerthale",             "FT"),
    };

        // TOWNS / AREAS -------------------------------------------------------
        //      fullname              displayname       key     zonekey
        TownInfo = new TownEntry[] {
new TownEntry("The Village of Availia", "Availia",      "VOA",    "TF"),
new TownEntry("The Barrowdark",         "Barrowdark",   "BAR",    "TF"),
new TownEntry("The Crossroads",         "Crossroads",   "XRD",    "TF"),
new TownEntry("Demith",                 null,           "DEM",    "AVP"),

        // NOTE: these do NOT show up below Compass (shows Thronefast, etc.)
        //       (could remove, but there's a chance they could add them, so leave for now)
new TownEntry("Sorhiryth",              null,           "SOR",    "WE"),
new TownEntry("Arcanary",               null,           "ARC",    "TF"),
new TownEntry("Skydurbin",              null,           "SKYD",   "SP"),
new TownEntry("Kingswatch",             null,           "KING",   "EP"),
new TownEntry("Port of Ru'lun",         null,           "PORT",   "SP"),
new TownEntry("Ashbreather Enclave",    null,           "ASH",    "SP"),

/*
      // Inaccessible
new TownEntry("Kosa Ull",               null,           "KOSA",   "BC"),   // POST 1st mention: 250909

new TownEntry("Thronefast City",        null,           "TFC",    "TF"),   // on MAP, pre-EA
new TownEntry("Lo'thale",               null,           "LOTH",   "FT"),   // on MAP, pre-EA
new TownEntry("Faerthale City",         null,           "FAER",   "FT"),   // on MAP, pre-EA

      // Inaccessible -- before player fully loaded. Not seen in actual game.
new TownEntry("The Even Darker Barrowdark", "Darker Barrowdark", null, "EDBD", "TF"),
*/
    };

        // BINDSTONES -------------------------------------------------------------
        //                  name,         zonekey
        BindInfo = new BindEntry[] {
            new BindEntry("Availia",          "TF",  x: 3357f, z: 497f, y:  3784f),
            new BindEntry("AvP Gate",         "TF",  x: 3546f, z: 591f, y:  2940f),
            new BindEntry("Demith",           "AVP", x: 3396f, z: 595f, y:  2030f),
            new BindEntry("Ghaldassii Ruins", "WE",  x: 3383f, z: 471f, y: -3532f),
            new BindEntry("Halnir Cave",      "AVP", x: 2388f, z: 565f, y:  2044f),
            new BindEntry("Lower Bridge",     "TF",  x: 4092f, z: 470f, y:  3002f),
            new BindEntry("Kingswatch",       "EP",  x: 2615f, z: 886f, y:   433f),
            new BindEntry("Skydurbin",        "SP",  x: -112f, z: 696f, y:  -759f),
            new BindEntry("Sorhiryth",        "WE",  x: 3830f, z: 514f, y: -3042f),
            new BindEntry("Wellpond",         "WE",  x: 3261f, z: 486f, y: -1991f),
        };
    }
}
