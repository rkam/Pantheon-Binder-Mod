#if !UNIT_TESTS
using UnityEngine;
#endif

namespace Binder;

/// <summary>
/// Utility class for locations - bindpoints, bindstones, zones, etc.
/// </summary>
class LocHelper : LocBuilder
{
    //----------------------------------------------------------------------
    /// <summary>
    /// Either match a bindstone to the user-supplied name or just use the string as the bindname.
    ///  - cmdParts will be name-zone or just name  (from /xxbindset-name-zone)
    /// </summary>
    internal static Bindpoint? BindpointFor(string[] cmdParts)
    {
        // [ set, name, zone ]  ( from /"[xxbind]set-name-zone" )

        if (cmdParts.Length <= 1)
        {
            // Shouldn't happen.
            return null;
        }

        var bindname = cmdParts[1];

        if (!Bindpoint.VerifyName(bindname))
        {
            Globals._gre($"Invalid bind name '{bindname}'");
            Utils.EchoToChat("BinderMod: Invalid name - must be alpha-numeric.");
            return null;
        }

        if (bindname == "trainer")
        {
            return _FindBindpointForTrainer(cmdParts);
        }

        // See if we can resolve this with just the name part.
        var t = _FindBindstoneForUser(bindname);
        if (t.Item1)
        {
            // success -- if we have a bindstone, then return that
            var bs = t.Item2;
            if (bs != null) { return new Bindpoint(bs); }

            // Otherwise, we have a zone and found multiple bindstones.
            var z = t.Item3!;
            return new Bindpoint("somewhere", z);
        }

        if (cmdParts.Length == 2)     // name, no zone
        {
            var b = new Bindpoint(bindname);
            return b;
        }

        var zonename = cmdParts[2];

        if (!Bindpoint.VerifyName(zonename))
        {
            Globals._gre($"Invalid zone name '{zonename}'");
            Utils.EchoToChat("BinderMod: Invalid zone - must be alpha-numeric.");
            return null;
        }

        // We have name-zone.  Since we've checked name thoroughly, really just
        // checking/resolving zone.
        var bp = _DeriveBindpoint(bindname, zonename);
        if (bp != null) { return bp; }

        // User specified name and zone do not match anything.
        bp = new Bindpoint(bindname, zonename);
        return bp;
    }

    //----------------------------------------------------------------------
    // See if the bind is an actual bindstone name, or if it's a zone name
    //  (and there's only one bindstone in that zone).
    // If both those fail, see if it's the start of a single bindstone name.
    private static Bindpoint _FindBindpointForTrainer(string[] cmdParts)
    {
        if (cmdParts.Length == 2)     // name ("trainer"), no zone
        {
            var b = new Bindpoint(true, null);
            return b;
        }

        var zn = cmdParts[2];

        var foundZ = Array.Find(LocBuilder.Zones, z => z.Abbrev.ToLower() == zn || z.Name.ToLower() == zn);
        return new Bindpoint(true, foundZ);
    }

    //----------------------------------------------------------------------
    // See if the bind is an actual bindstone name, or if it's a zone name
    //  (and there's only one bindstone in that zone).
    // If both those fail, see if it's the start of a single bindstone name.
    private static (bool, Bindstone?, Zone?) _FindBindstoneForUser(string bn)
    {

        // Because this is a hack using an invalid command, the system has
        // lowercased the entire command and thus all compares must be downcased.


        // First look for a Bindstone whose name matches.
        var found1 = Array.Find(LocBuilder.Bindstones, e => e.Name.ToLower() == bn);
        if (found1 != null)
        {
            return (true, found1, null);
        }

        // Then look for any zones that match.
        //    e.g. SP -> Skydurbin
        //    e.g. WE -> 3 matches

        var foundZ = Array.Find(Zones, z => z.Abbrev.ToLower() == bn || z.Name.ToLower() == bn);

        if (foundZ == null)
        {
            // Now try Bindstones which start with the name (e.g. ava -> availia)
            // NOTE: we match zones first, so 'we' -> "Wild's End" and not "Wellpond in Wild's End"

            var foundB = Array.FindAll(LocBuilder.Bindstones, e => e.Name.ToLower().StartsWith(bn));
            if (foundB.Length == 0)
            {
                foreach (Bindstone b in LocBuilder.Bindstones)
                {
                }
                return (false, null, null);
            }

            if (foundB.Length == 1)
            {
                return (true, foundB[0], null);
            }

            return (false, null, null);
        }

        // We have a zone, so find all bindstones within that zone.
        var foundS = Array.FindAll(Bindstones, e => e.Loc.ZoneName == foundZ.Name);


        switch (foundS.Length)
        {
            case 0: return (false, null, null);
            case 1: return (true, foundS[0], null);
            default: break;
        }

        Globals._gra($"Found {foundS.Length} bindstones for '{bn}'");
        return (true, null, foundZ);
    }

    //----------------------------------------------------------------------
    // Process the name-zone version of the command.
    // - We already checked the bind name, so now see if the specified zone is found.
    //      If so, and there's only one bindstone in that zone, return a bind
    //          using the user-specified name and the zone.
    //      e.g. "upnorth-we" --> "'upnorth' in Wild's End"
    //           "sp"         --> "Skydurbin in Silent Plains"
    private static Bindpoint? _DeriveBindpoint(string bn, string zn)
    {

        var foundZ = Array.Find(LocBuilder.Zones, z => z.Abbrev.ToLower() == zn || z.Name.ToLower() == zn);
        if (foundZ == null)
        {
            return null;
        }

        // We have a zone, so find all bindstones within that zone.
        var foundS = Array.FindAll(Bindstones, e => e.Loc.ZoneName == foundZ.Name);

        switch (foundS.Length)
        {
            case 0: return null;
            case 1:
                // Common case is: I don't remember the bindstone name, but it's in WE
                //    e.g. "upnorth-we" --> "'upnorth' in Wild's End"

                // Weird case for zones with just 1 bindstone:
                //   User specified "name.sp", which finds B_Skydurbin
                //      But, they could have just done "sp".
                //   - We could just set to Skydurbin bindstone, but since they
                //     specified an explicit name, maybe they want it.
                //   So, create a bind with that name and the zone of the
                //     found stone.
                //   e.g. "weird-sp" --> "'weird' in Silent Plains"

                var b = new Bindpoint(bn, foundS[0]);
                return b;

            default: break;
        }

        Globals._gra($"Found {foundS.Length} matches in '{foundZ.Name}' for '{bn}'");

        // Found the zone, but there are several bindstones within the zone.
        // So, create a bind for with that name and the zone.
        //    e.g. "upnorth-we" --> "'upnorth' in Wild's End"

        return new Bindpoint(bn, foundS[0]);
    }

    /// <summary>
    /// Returns the name of the area the user is in (i.e. what shows below the Compass) or Unknown ("?").
    /// </summary>
    internal static CompassArea GetCompassArea()
    {
#if UNIT_TESTS
        var areaPlayerIsIn = Globals.LocalPlayer?.info.compassArea;
#else
        // Compass->Child(LocationNameDisply)->Child("Text (TMP)")->Component("Il2CppTMPro.TextMeshProUGUI").m_text ==> "The Village of Availia"
        UnityEngine.Transform? locationNameDisplay = Globals.compassPanel!.Find("LocationNameDisplay");

        Transform? textChild = locationNameDisplay?.GetChild(1);        // "Text (TMP)" [ Child[0] == Background ]

        Il2CppTMPro.TextMeshProUGUI? textMesh = textChild?.GetComponent<Il2CppTMPro.TextMeshProUGUI>();

        var areaPlayerIsIn = textMesh?.m_text;
#endif

        if (areaPlayerIsIn == null)
        {
            return new CompassArea();
        }

        var area = Array.Find(LocBuilder.CompassAreas, a => a.Name == areaPlayerIsIn);
        if (area != null)
        {
            return area;
        }

        return new CompassArea();
    }
}
