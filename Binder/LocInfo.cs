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

        // -------------------------------------------------------------------------
        //              zonename                compassname          key
        ZoneInfo = new ZoneEntry[] {
         new ZoneEntry("Thronefast",            "Thronefast",        "TF"),
         new ZoneEntry("Avendyr's Pass",        "AvendyrsPass",      "AVP"),
         new ZoneEntry("Eastern Plains",        "EasternPlains",     "EP"),
         new ZoneEntry("Silent Plains",         "SilentPlains",      "SP"),
         new ZoneEntry("Wild's End",            "WildsEnd",          "WE"),

         new ZoneEntry("Halnir Cave",           "HalnirCave",        "HC"),
         new ZoneEntry("Hangore",               "Hangore",           "HG"),      // Compass: ???  ##TODO: Zone?
         new ZoneEntry("Mad Run",               "MadRun",            "MR"),      // Compass: ???  ##TODO: Zone? na-Area?
         new ZoneEntry("Black Rose Keep",       "BlackRoseKeep",     "BRK"),     // Compass: ???  ##TODO: Zone? na-Area?

//       new ZoneEntry("Faerthale",             "Faerthale",         "FT"),
//       new ZoneEntry("Badia de Cara",         "BadiadeCara",       "BC"),
         new ZoneEntry("Ashbreather Enclave",   "AshbreatherEnclave","ASH"),     // Compass: ???  ##TODO: Zone? na-Area?
//       new ZoneEntry("Knightwolf Knoll",      "KnightwolfKnoll",   "KWK"),     // Compass: ???  ##TODO: Zone? na-Area?
    };

        // -------------------------------------------------------------------------
        //      fullname                   displayname        compassname,         key    zonekey
        TownInfo = new TownEntry[] {
new TownEntry("The Village of Availia",     "Availia",           null,             "VOA",  "TF"),
new TownEntry("The Barrowdark",             "Barrowdark",        null,             "BAR",  "TF"),
new TownEntry("Demith",                     "Demith",            null,             "DEM",  "AVP"),
new TownEntry("Sorhiryth",                  "Sorhiryth",         null,             "SOR",  "WE"),  // ##TODO: verify

new TownEntry("Arcanary",                   "Arcanary",          null,             "ARC",  "TF"),  // NOTE: does not show up below Compass (shows Thronefast)
new TownEntry("Thronefast City",            "Thronefast City",   "ThronefastCity", "TFC",  "TF"),  // ##TODO: verify exist?

new TownEntry("Skydurbin",                  "Skydurbin",         null,             "SKYD", "SP"),  // ##TODO: verify
new TownEntry("Kingswatch",                 "Kingswatch",        null,             "KING", "EP"),  // ##TODO: verify
new TownEntry("Port of Ru'lun",             "Port of Ru'lun",    "PortofRulun",    "PORT", "SP"),  // ##TODO: verify

/*
      // Inaccessible
new TownEntry("Lo'thale",                   "Lo'thale",          "Lothale",        "LOTH", "FT"),
new TownEntry("Faerthale City",             "Faerthale City",    "FaerthaleCity",  "FAER", "FT"),

      // Inaccessible - before player fully loaded. Not seen in actual game.
new TownEntry("The Even Darker Barrowdark", "Darker Barrowdark", null,             "EDBD", "TF"),
*/
    };

        // -------------------------------------------------------------------------
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
