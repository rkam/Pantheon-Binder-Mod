#if UNIT_TESTS
namespace Binder;
public class MelonMod { }
#else
using Il2CppPantheonPersist;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;

namespace Binder;
#endif

/// <summary>
/// Main mod class that handles remembering the location of a player's last
///   bind location. and (optionally) displays it below the compass.
/// It will also optionally echo to Chat upon login and when using a bindstone.
/// It will display a terse or more informative message, based on an option and
///   the actual bindpoint information.
/// It saves the information (per player) to a config file for player re-logging.
///
/// It is compatible with multiple accounts and simultaneous game instances.
///
/// This mod works well with the mods by onyxius which also show
///   information below the compass.
/// </summary>
public class ModMain : MelonMod
{
    public const string ModVersion = "1.0.0";

#if !UNIT_TESTS
    // HUD display
    private TextMeshProUGUI? _textMeshPro;  // The TextMeshPro component for rendering text
    private GameObject? _textObject;        // The GameObject that holds our text display
#endif

    // Config Options
    private bool _atLogin = false;       // Show the current bind at login.
    private bool _inHUD = false;         // Show the current bind below compass.
    private bool _toChat = true;         // Echo the new bind in Chat
    private bool _terse = true;          // Show terse or verbose information.

    // Easy access to us for Hooks.
    internal static ModMain Instance { get; private set; }

    public ModMain()
    {
        Instance = this;
        LoadGlobalPrefs();
    }

    /// <summary>
    /// Called whenever the bind message is seen.
    ///   Parameters are the same for all incoming Chat messages.
    /// </summary>
    internal string HandleBindAtStone(string name, string msg, ChatChannelType channel)
    {

        if (string.IsNullOrWhiteSpace(msg))
            return msg;

        var loc = Utils.GetPlayerMapPoint();

        if (loc == null)
        {
            // Since they just bound, what we had is invalid
            Globals.Warn($"Cannot find current location.");
            Utils.EchoToChat("BinderMod: Cannot find current location - clearing last bind location");

            Globals.PPrefs!.SetBind(new Bindpoint()); // This will also set current zone, if it can.
            return SaveAndDisplayBindPoint(true);
        }

        var bs = Array.Find(LocBuilder.Bindstones, e => e.IsNear(loc));
        if (bs != null)
        {
            Globals.PPrefs!.SetBind(new Bindpoint(bs));
            return SaveAndDisplayBindPoint(true);
        }

        // This is a bindstone we don't know of.

        var x = (float)Math.Round(loc.X, 2);
        var y = (float)Math.Round(loc.Y, 2);

        Globals._cya("Unrecognized bindstone for current location.");
        Globals.PPrefs!.SetBind(new Bindpoint(loc, false));

        return SaveAndDisplayBindPoint(true);
    }

    /// <summary>
    /// Called when the user types xxbindinfo - will show the current bind.
    /// </summary>
    internal string HandleBindInfoCommand(string cmd, ChatChannelType channel)
    {
        var s = Globals.PPrefs!.bindpoint.ChatText(false);
        Utils.EchoToChat(s);
        return "";
    }

    /// <summary>
    /// Called when the user types xxbindload - will load from config file.
    /// </summary>
    internal void HandleBindLoadCommand(string cmd, ChatChannelType channel)
    {
        LoadPlayerPrefs();
    }

    /// <summary>
    /// Called when the user types 'xxbindinfo-XX', or 'xxbindinfo-XX-yy'
    ///
    /// It will set the current bind to "xx" (in "yy").
    ///   If "xx" matches an existing bindstone, it will act as if that bindstone
    ///     was used.
    ///   If "xx" is a common zone abbreviation (e.g. WE, SP, AVP, HC, etc.):
    ///     - It will set to a bindstone within that zone if there is only one
    ///         bindstone in it.
    ///     - It will set the name to 'somewhere' if "yy" is not present.
    ///   If "xx" is 'trainer', it will treat it as "bound at trainer" (i.e.
    ///     the same as if a new character was created.
    ///   Otherwise, just sets to "xx" (in "yy")
    /// </summary>
    internal string? HandleBindSetCommand(string cmd, ChatChannelType channel)
    {

        // [xxbind]set-name-zone
        String[] a = cmd.Split("-", StringSplitOptions.RemoveEmptyEntries);

        var bp = LocHelper.BindpointFor(a);
        if (bp == null)
        {
            return null;
        }
        Globals.PPrefs!.SetBind(bp!);

        var s = SaveAndDisplayBindPoint(false, true);
        Utils.EchoToChat(s);

        return "";      // valid command and we showed message, so return ""
    }

    /// <summary>
    /// Called when the user types xxbind{an-option} - toggles the option
    /// </summary>
    internal string? HandleBindToggleCommand(string option, ChatChannelType channel)
    {

        var isnow = false;
        switch (option)
        {
            case "login":
                _atLogin = !_atLogin;
#if UNIT_TESTS
                Globals.GPrefs!.atLogin = _atLogin;
#else
                Globals.GPrefs!.GetEntry<bool>("ShowAtLogin").Value = _atLogin;
#endif
                isnow = _atLogin;
                break;
            case "hud":
                _inHUD = !_inHUD;
#if UNIT_TESTS
                Globals.GPrefs!.inHUD = _inHUD;
#else
                Globals.GPrefs!.GetEntry<bool>("ShowInHUD").Value = _inHUD;
#endif
                _onHUDChanged();
                isnow = _inHUD;
                break;
            case "chat":
                _toChat = !_toChat;
#if UNIT_TESTS
                Globals.GPrefs!.toChat = _toChat;
#else
                Globals.GPrefs!.GetEntry<bool>("EchoToChat").Value = _toChat;
#endif
                isnow = _toChat;
                break;
            case "terse":
                _terse = !_terse;
#if UNIT_TESTS
                Globals.GPrefs!.terse = _terse;
#else
                Globals.GPrefs!.GetEntry<bool>("_terse").Value = _terse;
#endif
                _onTerseChanged();
                isnow = _terse;
                break;
            default:
                return null;
        }

        Globals.GPrefs.SaveToFile(false);

        Utils.EchoToChat($"{option} is now {isnow}");
        return "";
    }

    // --
    private void _onTerseChanged()
    {
        ShowInHUD(Globals.PPrefs?.bindpoint.HudText(_terse) ?? "");
    }

    // --
    private void _onHUDChanged()
    {
        ShowInHUD(Globals.PPrefs?.bindpoint.HudText(_terse) ?? "");
    }

    /// <summary>
    /// Updates the display with the current bind location
    /// </summary>
    internal string SaveAndDisplayBindPoint(bool atStone, bool justSet = false)
    {
        Globals.PPrefs!.SaveToFile(false);

        ShowInHUD(Globals.PPrefs!.bindpoint.HudText(_terse));

        if (!_toChat) { return ""; }

        // If we are at a bindstone, return the Terse version, since
        //    it appends to the "You are now bound to this stone!" message.
        if (atStone) { return Globals.PPrefs!.bindpoint.ChatText(true); }

        return Globals.PPrefs!.bindpoint.ChatText(_terse, justSet);
    }

    /// <summary>
    /// Updates the display with the current bind location
    /// </summary>
    void ShowInHUD(string displayText)
    {
#if UNIT_TESTS
        Globals._gre($"HUD: {displayText}");
#else
        try
        {
            if (_textMeshPro != null)
            {
                // We can't Show/Hide ourself because we're not a window,
                //  (we're part of CompassPanel), so just set text to "" to hide.
                _textMeshPro.text = (_inHUD) ? displayText : "";
            }
        }
        catch (Exception e)
        {
            // Silently handle any errors
            Globals.Warn("Cannot update bind window: " + e.ToString());
        }
#endif
    }

#if !UNIT_TESTS
    /// <summary>
    /// Creates the Bindpoint display UI element below the compass.
    ///   Accomodates known mods that also display below the compass.
    /// </summary>
    /// <param name="compassPanel">The compass panel transform to attach the text to</param>
    internal void CreateBoundAtDisplay(Transform compassPanel)
    {
        Globals.compassPanel = compassPanel;
        // Always create.  We will show/hide it based on _inHUD
        CreateTextualDisplay(Globals.PPrefs?.bindpoint.HudText(_terse) ?? "");
        _onHUDChanged();
    }
#endif

    void LoadGlobalPrefs()
    {
#if UNIT_TESTS
        Globals.GPrefs = new MelonPreferences_Category();

        _atLogin = Globals.GPrefs!.atLogin;
        _inHUD   = Globals.GPrefs!.inHUD;
        _toChat  = Globals.GPrefs!.toChat;
        _terse   = Globals.GPrefs!.terse;
#else
        // Global prefs (only actual bind-points are per-player)
        Globals.GPrefs = MelonPreferences.CreateCategory("Binder");

        _atLogin = Globals.GPrefs.GetEntry<bool>("ShowAtLogin")?.Value ??
                   Globals.GPrefs.CreateEntry("ShowAtLogin", false).Value;

        _inHUD = Globals.GPrefs.GetEntry<bool>("ShowInHUD")?.Value ??
                 Globals.GPrefs.CreateEntry("ShowInHUD", false).Value;

        _toChat = Globals.GPrefs.GetEntry<bool>("EchoToChat")?.Value ??
                  Globals.GPrefs.CreateEntry("EchoToChat", true).Value;

        _terse = Globals.GPrefs.GetEntry<bool>("_terse")?.Value ??
                 Globals.GPrefs.CreateEntry("_terse", true).Value;

        Globals.GPrefs.SaveToFile(false);
#endif
    }

    /// <summary>
    // Called each time a player logs in - Read or create a default bindpoint and display it.
    /// </summary>
    internal void LoadPlayerPrefs()
    {

        // [PlayerName_b52597d]
        var entryName = $"{Globals.LocalPlayer!.info.DisplayName}_{Globals.LocalPlayer.info.CharacterId}";

        Bindpoint? bp = Globals.PPrefs!.GetBind();

        if (bp == null)
        {
            bp = Utils.GetStartingBindpoint();
            Globals.PPrefs!.SetBind(bp);
        }

        if (_atLogin) {
            // Hmm, could always show this if Unknown bind. Might be helpful.
            //      but contradicts the flag, so just honor the flag always.
            Utils.EchoToChat(Globals.PPrefs!.bindpoint.ChatText(false));
        }

        SaveAndDisplayBindPoint(false);
    }

#if !UNIT_TESTS
    // Code originally from Pantheon-ShowServerTime-Mod by onyxius
    internal void CreateTextualDisplay(string displayText)
    {
        // Remove the old BoundAt display if it exists
        if (_textObject != null)
        {
            UnityEngine.Object.Destroy(_textObject);
        }

        // Default parent is the compass panel
        var parentTransform = Globals.compassPanel;
        // Default position: 5 units below the compass (matches server name mod default)
        var anchoredPosition = new Vector2(0, -7);

        // Check if either the ShowServerAndShard or ShowServerTime mod is loaded
        // ServerTime positions itself based on ServerAndShard, so try that first.
        var hasTimeMod = MelonMod.RegisteredMelons.Any(m => m.Info.Name == "ShowServerTime");
        var timePanelName = "TimeDisplay";
        var hasServerMod = MelonMod.RegisteredMelons.Any(m => m.Info.Name == "ShowServerAndShard");
        var serverPanelName = "InfoPanelText";

        // ##TODO: Finding the mods, but cannot find TimeDisplay in either compass nor InfoPanelText
        // ##TODO: WORKAROUND - just count the mods and offset by appropriate amount and keep compass as parent
        // ##TODO:                 -- ServerTimeMod changes parent to InfoPanelText if present.
        if (hasTimeMod || hasServerMod)
        {
            var yoff = 0;
            var parentToBe = Globals.compassPanel!;

            // Try to find the server name text object as a child of the compass panel

            if (hasServerMod)
            {
                var serverObj = Globals.compassPanel!.Find(serverPanelName);
                yoff -= 18;
                if (serverObj != null)
                {
                    //                yoff = 5;
                    parentToBe = serverObj;
                }
            }

            if (hasTimeMod)
            {
                var timeObj = parentToBe.Find(timePanelName);
                yoff -= 18;
                if (timeObj != null)
                {
                    //                yoff = 5;
                    parentToBe = timeObj;
                }
            }

            if (parentToBe != null)
            {
                // If found, parent the BoundAt display to the server name text
                //              parentTransform = parentToBe;

                // Place the BoundAt display right below the server name (0 units offset)
                anchoredPosition = new Vector2(0, yoff);
            }
            else
            {
                anchoredPosition = new Vector2(0, yoff);
            }
        }

        // Create a new GameObject for the BoundAt display
        _textObject = new GameObject("BoundAtDisplay");
        // Set its parent to either the compass panel or the server name text
        _textObject.transform.SetParent(parentTransform, false);

        // Add and configure the TextMeshPro component for rendering the BoundAt
        _textMeshPro = _textObject.AddComponent<TextMeshProUGUI>();
        _textMeshPro.text = displayText;
        _textMeshPro.fontSize = 12;
        _textMeshPro.color = Color.yellow;
        _textMeshPro.alignment = TextAlignmentOptions.Center;

        // Set up the RectTransform to position the text correctly
        var rectTransform = _textMeshPro.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0f);  // Anchor to bottom center
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 1f);    // Pivot at top center
        rectTransform.anchoredPosition = anchoredPosition;  // Vertical offset
        rectTransform.sizeDelta = new Vector2(400, 20);   // Set width and height
    }
#endif
}
