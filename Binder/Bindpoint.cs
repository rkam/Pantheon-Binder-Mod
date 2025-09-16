using System.Text.RegularExpressions;
#if !UNIT_TESTS
using MelonLoader;
#endif

namespace Binder;

using DictSS = Dictionary<string, string>;

/// <summary>
/// Class that represents a bindpoint.
/// </summary>
class Bindpoint
{
    internal string Name { get; set; } = "??";
    internal string Zonename { get; set; } = "?";
    internal bool IsTrainer { get; set; } = false;
    internal bool IsUserName { get; set; } = false;
    internal bool IsRealZone { get; set; } = false;

    // Could restrict bindset names to not include ', ?, @ and <> and keep those for file IO.
    // Note: spaces can't be used for bindset, due to the way the Unknown Command works.
    //    ' needed for "Wild's End".
    //    @ , and >< needed for "Unknown stone @ <x, y>".
    //    ? and ?? represent unknown bind.
    //      (TODO: writing Unknown BP to file reduces messages (not bound, etc) -- desired?)
    private static readonly string _NM_REPAT = "([0-9a-zA-Z_ ?'@<,>]+)";

    /// <summary>
    /// Create a unknown bindpoint.
    /// </summary>
    internal Bindpoint()
    {
#if UNIT_TESTS
        if (Globals.GPrefs == null) {
            return;
        }
#else
        if (Globals.CompassPanel == null)       // This happens early on before we've setup
        {
            return;
        }
#endif

        Zonename = LocHelper.GetCompassArea().AreaName;
        IsRealZone = Zonename != "?";
    }

    /// <summary>
    /// Most common case. Clicked a bindstone.
    /// </summary>
    internal Bindpoint(Bindstone stone)
    {
        Name = stone.Name;
        Zonename = stone.Loc.AreaName;
        IsRealZone = true;
    }

    /// <summary>
    /// If at a bindstone we don't know about, we'll have only a location.
    /// If a user is created, they'll be bound at the trainer.
    /// </summary>
    internal Bindpoint(MapPoint loc, bool isTrainer)
    {
        if (isTrainer)
        {
            Name = "Trainer";
            IsTrainer = true;
        }
        else
        {
            var x = (int)Math.Round(loc.X, 2);
            var y = (int)Math.Round(loc.Y, 2);

            Name = $"Unknown Stone @ <{x}, {y}>";
        }

        // Loc is either created with a Zone/Area or is '?'
        Zonename = loc.AreaName;
        IsRealZone = Zonename != "?";
    }

    /// <summary>
    /// Create a bindpoint from the user command to "Trainer" with a possible zone.
    /// Note, isTrainer parameter distinguishes ctor signatures and is ignored.
    /// </summary>
    internal Bindpoint(bool isTrainer, Zone? zone)
    {
        Name = "Trainer";
        IsTrainer = true;

        if (zone != null)
        {
            Zonename = zone.Name;
            IsRealZone = true;
        }
        else
        {
            Zonename = LocHelper.GetCompassArea().AreaName;
            IsRealZone = Zonename != "?";
        }
    }

    /// <summary>
    /// Create a bindpoint from the user command given a zone.
    ///  e.g. /xxbindset-tf
    /// </summary>
    internal Bindpoint(string name, Zone zone)
    {
        Name = name;
        Zonename = zone.Name;
        IsRealZone = true;
        IsUserName = true;
    }

    /// <summary>
    /// Create a bindpoint from the user command in a zone with a known stone.
    ///  e.g. "upnorth" in WE
    /// </summary>
    internal Bindpoint(string name, Bindstone stone)
    {
        if (!string.IsNullOrEmpty(Name)) { Name = name; }
        Zonename = stone.Loc.AreaName;
        IsRealZone = true;
        IsUserName = true;
    }

    /// <summary>
    /// Create a bindpoint from the user command.
    /// </summary>
    internal Bindpoint(string name, string? zonename = null)
    {
        if (!string.IsNullOrEmpty(Name)) { Name = name; }

        // We already checked if zone is a real zone and it wasn't.
        if (zonename != null)
        {
            Zonename = zonename;
            IsRealZone = false;
        }
        else
        {
            Zonename = LocHelper.GetCompassArea().AreaName;
            IsRealZone = Zonename != "?";
        }
        IsUserName = true;
    }

    /// <summary>
    /// Is this bindstone valid.
    ///  - When mod first launched with an existing character.
    ///  Note: Starting (Level 1) chars will have their bind set to the trainer.
    /// </summary>
    internal bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && Name != "??";
    }

    /// <summary>
    /// Provide the display text for the HUD (in-game window below the compass).
    /// </summary>
    internal string HudText(bool terse)
    {
        if (!IsValid())
        {
            return "<Unknown Bind>";
        }

        var name = GetDisplayName(terse);
        var zone = GetDisplayZone();

        if (terse && !IsUserName)
        {
            return "Bind: " + name;
        }

        return $"Bind: {name}{zone}";
    }

    /// <summary>
    /// Provide the display text for the Chat window
    /// </summary>
    internal string ChatText(bool terse, bool justSet = false)
    {
        if (!IsValid())
            return "No known bind - please bind again somewhere or use /xxbindset-name-zone.";

        var name = GetDisplayName(terse);

        if (terse && !justSet)
            return name;

        var zone = GetDisplayZone();
        return $"You are bound at {name}{zone}.";
    }

    /// <summary>
    /// Format the bindpoint name according to the internal flags.
    /// </summary>
    string GetDisplayName(bool terse)
    {
        if (IsUserName)
        {
            return $"'{Name}'";
        }

        if (IsTrainer && !terse)
        {
            var pc = Globals.LocalPlayer!.info.Class.ToString();
            if (pc != null)
                return $"{pc} Trainer";
        }

        return Name;
    }

    /// <summary>
    /// Format the zone-name suffix according to the internal flags.
    /// </summary>
    string GetDisplayZone()
    {
        if (Zonename == "?")
        {
            // Cannot be a real zone and regardless of IsUserName, don't quote it.
            return " in " + Zonename;
        }

        // Single quote user specified zones.
        return " in " + ((IsRealZone) ? Zonename : $"'{Zonename}'");
    }

    /// <summary>
    /// Convert this object to a string containing the persistent properties.
    /// </summary>
    internal string ToPrefs()
    {
        var s = "";
        s += $"Name =      \"{Name}\"\n";
        s += $"Zonename =  \"{Zonename}\"\n";
        return s;
    }

    // --
    private static string? _ParsePrefLine(string input, string pat)
    {
        Regex r = new Regex(pat, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(2));
        Match m = r.Match(input);
        if (!m.Success)
        {
            return null;
        }

        string s = m.Groups[1].Captures[0] + "";
        return s;
    }

    //  name_uniqID.cfg:
    //    Name = "Availia"
    //    Zonename = "Thronefast"

    private static readonly DictSS[] _PREFS = new DictSS[] {
        //           pref-name         pref-regex-pattern
new DictSS { { "n", "Name" },        { "p", $"^Name *= *\"({_NM_REPAT})\"$"     }},
new DictSS { { "n", "Zonename" },    { "p", $"^Zonename *= *\"({_NM_REPAT})\"$" }},
        // NOTE: anything more complicated than above might need longer timeout
    };

    /// <summary>
    /// Convert the (lines of the) persistent properties file to an instance.
    /// </summary>
    internal static Bindpoint? FromPrefs(string[] prefLines)
    {
        string? bindname = null;
        string? zonename = null;

        foreach (string s in prefLines)
        {
            if (s == "" || s == "[Binds]" || s.StartsWith("#")) { continue; }

            foreach (Dictionary<string, string> h in _PREFS)
            {
                string name = h["n"];
                string pat = h["p"];
                if (s.StartsWith(name))
                {
                    try
                    {
                        var v = _ParsePrefLine(s, pat);
                        switch (name)
                        {
                            case "Name": bindname = v; break;
                            case "Zonename": zonename = v; break;
                        }
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Warning("Regex timed out (invalid bind config file?): " + e.ToString());
                        Utils.EchoToChat("BinderMod: Invalid bindpoint in config file. Clearing it. You will need to re-bind or use /xxbindset-name-zone.");
                        return null;
                    }
                    break;
                }
            }
        }

        Bindpoint? bp = FromPrefs(bindname, zonename);
        if (bp == null)
        {
            MelonLogger.Warning($"File parse failed (bind: '{bindname}', zone: '{zonename}')");
            Utils.EchoToChat("BinderMod: Cannot parse bindpoint in config file. Clearing it. You will need to re-bind or use /xxbindset-name-zone.");
        }

        return bp;
    }

    /// <summary>
    /// Convert the verified persistent properties to an instance.
    /// Called when reading the prefs and also from the bindset command.
    /// NOTE: names should be already verified
    /// </summary>
    internal static Bindpoint? FromPrefs(string? bindname, string? zonename)
    {
        // NOTE: names should be already verified (VerifyName)
        if (string.IsNullOrEmpty(bindname) || string.IsNullOrEmpty(zonename)) { return null; }

        bool isTrainer = false;
        bool isUserName = false;
        bool isRealZone = false;
        _SetInternalFlags(ref bindname, ref zonename, ref isTrainer, ref isUserName, ref isRealZone);

        Bindpoint b = new Bindpoint(bindname, zonename, isTrainer, isUserName, isRealZone);
        return b;
    }

    // --
    private static void _SetInternalFlags(ref string bindname, ref string zonename, ref bool isTrainer, ref bool isUserName, ref bool isRealZone)
    {
        zonename = zonename.Trim();
        bindname = bindname.Trim();

        // Sort out zone
        string zn = zonename.ToLower();
        string bn = bindname.ToLower();

        Zone? z = Array.Find(LocBuilder.Zones, z => z.Abbrev.ToLower() == zn || z.Name.ToLower() == zn);
        if (z == null)
        {
            isRealZone = false;
        }
        else
        {
            zonename = z.Name;
            isRealZone = true;
        }

        // Sort out name
        if (bn == "trainer")
        {
            isTrainer = true;
            isUserName = false;
            bindname = "Trainer";
            return;
        }

        isTrainer = false;
        if (bn.StartsWith("unknown stone"))
        {
            isUserName = false;
            return;
        }

        Bindstone? b = Array.Find(LocBuilder.Bindstones, e => e.Name.ToLower() == bn);
        if (b != null)
        {
            bindname = b.Name;
            isUserName = false;
            return;
        }

        isUserName = true;
    }

    /// <summary>
    /// Verify the name - i.e. restrict to a small set of chars.
    /// </summary>
    internal static bool VerifyName(string bindname)
    {
        try
        {
            Regex r = new Regex($"^{_NM_REPAT}$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1));
            Match m = r.Match(bindname);
            return m.Success;
        }
        catch (Exception e)
        {
            MelonLogger.Warning("Regex timed out: " + e.ToString());
            Utils.EchoToChat("BinderMod: Internal Error (regex timeout).");
        }
        return false;
    }

#if UNIT_TESTS
    internal static readonly Bindpoint EMPTY_BINDPOINT = new Bindpoint("??", "?", false, false, false);

    // --
    internal
#else
    private
#endif
    Bindpoint(string bindname, string zonename, bool isTrainer, bool isUserName, bool isRealZone)
    {
        Name = bindname;
        Zonename = zonename;
        IsTrainer = isTrainer;
        IsUserName = isUserName;
        IsRealZone = isRealZone;
    }

    /// <summary>
    /// Convert this object to a string.
    /// </summary>
    override public string ToString()
    {
        var s = "";
        s += $"Name:       \"{Name}\"\n";
        s += $"Zonename:   \"{Zonename}\"\n";
        s += $"IsTrainer:  {IsTrainer}\n";
        s += $"IsUserName: {IsUserName}\n";
        s += $"IsRealZone: {IsRealZone}";
        return s;
    }
}
