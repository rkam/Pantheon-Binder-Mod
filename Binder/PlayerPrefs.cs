#if !UNIT_TESTS
using MelonLoader;
#endif

namespace Binder;

internal class PlayerPrefs
{
    internal Bindpoint bindpoint { get; private set; }

#if UNIT_TESTS
    internal static string _DirPath;
    internal readonly string _Path;
#else
    private static string _DirPath;
    private readonly string _Path;
#endif

    // -------------------------------------------------------------------------
    internal PlayerPrefs()
    {
        bindpoint = new Bindpoint();

        if (Globals.LocalPlayer == null)
        {
            _Path = "";
            return;
        }

        string fn = $"{Globals.LocalPlayer!.info.DisplayName.ToLower()}_{Globals.LocalPlayer!.info.CharacterId}.cfg";

        _Path = Path.Combine(_DirPath, fn);
    }

    internal Bindpoint? GetBind()
    {
        // This text is added only once to the file.
        if (!File.Exists(_Path))
        {
            return null;
        }

        string text = File.ReadAllText(_Path);
        if (text == "")
        {
            return null;
        }

        string[] lines = text.Split("\n");
        if (lines.Length == 0)
        {
            return null;
        }

        var bp = Bindpoint.FromPrefs(lines);
        if (bp == null) {
            return null;
        }

        Globals._cya($"GetBind({bp.Name}, {bp.Zonename} [ t: {bp.IsTrainer}, u: {bp.IsUserName}, r: {bp.IsRealZone}) ]");
        bindpoint = bp;
        return bindpoint;
    }

    internal void SetBind(Bindpoint bp)
    {
        Globals._cya($"SetBind({bp.Name}, {bp.Zonename} [ t: {bp.IsTrainer}, u: {bp.IsUserName}, r: {bp.IsRealZone}) ]");
        bindpoint = bp;
    }

    internal void SaveToFile(bool showMessage)
    {
        var prefs = bindpoint.ToPrefs();
        if (showMessage) { Globals._gra($"Player Bindpoints Saved to {_Path}"); }
        File.WriteAllText(_Path, prefs);
    }

    // --
    static PlayerPrefs()
    {
        // Put the binds config file in a VR-independent location, so that if you
        // have multiple accounts and login (to VR) with one and then login
        // to the game with another, you'll still have the binds.
        // - This includes having a Steam and a Standalone game.
        // We do our own file IO, since we may be used by multiple game processes.

        // Use "AppData/Roaming"
        var appData = Environment.GetEnvironmentVariable("APPDATA");
        if (appData == null)
        {
            appData = "";
#if !UNIT_TESTS
            // No idea where this will be or if writeable. Can't really happen though.
            Globals.Warn("Environment['APPDATA'] does not exist!? Config dir located in: .");
#endif
        }

        _DirPath = Path.Combine(appData, "PantheonBinderMod");
        Directory.CreateDirectory(_DirPath);     // NOP if already exists
    }
}
