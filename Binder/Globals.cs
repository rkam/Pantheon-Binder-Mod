#if !UNIT_TESTS
using Il2Cpp;
using MelonLoader;
using UnityEngine;
#endif

namespace Binder;

static class Globals
{
    internal static EntityPlayerGameObject? LocalPlayer = null;

    internal static MelonPreferences_Category? GPrefs = null;    // Global prefs

    internal static PlayerPrefs? PPrefs = null;                  // Per-player prefs

    internal static Transform? compassPanel = null;              // for zone name

    // -------------------------------------------------------------------------
    internal static bool TRACE = false;
#if UNIT_TESTS
    internal static bool FATAL = false;
#endif

    internal static void _bla(string s) { _Color(false, s, ConsoleColor.Black); }

    internal static void _blu(string s) { _Color(false, s, ConsoleColor.Blue); }
    internal static void _gre(string s) { _Color(false, s, ConsoleColor.Green); }
    internal static void _cya(string s) { _Color(false, s, ConsoleColor.Cyan); }
    internal static void _red(string s) { _Color(false, s, ConsoleColor.Red); }
    internal static void _mag(string s) { _Color(false, s, ConsoleColor.Magenta); }
    internal static void _yel(string s) { _Color(false, s, ConsoleColor.Yellow); }
    internal static void _whi(string s) { _Color(false, s, ConsoleColor.White); }
    internal static void _gra(string s) { _Color(false, s, ConsoleColor.Gray); }

    internal static void _dblu(string s) { _Color(false, s, ConsoleColor.DarkBlue); }
    internal static void _dgre(string s) { _Color(false, s, ConsoleColor.DarkGreen); }
    internal static void _dcya(string s) { _Color(false, s, ConsoleColor.DarkCyan); }
    internal static void _dred(string s) { _Color(false, s, ConsoleColor.DarkRed); }
    internal static void _dmag(string s) { _Color(false, s, ConsoleColor.DarkMagenta); }
    internal static void _dyel(string s) { _Color(false, s, ConsoleColor.DarkYellow); }
    internal static void _dgra(string s) { _Color(false, s, ConsoleColor.DarkGray); }

    internal static void _tblu(string s) { _Color(true, s, ConsoleColor.Blue); }
    internal static void _tgre(string s) { _Color(true, s, ConsoleColor.Green); }
    internal static void _tcya(string s) { _Color(true, s, ConsoleColor.Cyan); }
    internal static void _tred(string s) { _Color(true, s, ConsoleColor.Red); }
    internal static void _tmag(string s) { _Color(true, s, ConsoleColor.Magenta); }
    internal static void _tyel(string s) { _Color(true, s, ConsoleColor.Yellow); }
    internal static void _twhi(string s) { _Color(true, s, ConsoleColor.White); }
    internal static void _tgra(string s) { _Color(true, s, ConsoleColor.Gray); }

    internal static void _tdblu(string s) { _Color(true, s, ConsoleColor.DarkBlue); }
    internal static void _tdgre(string s) { _Color(true, s, ConsoleColor.DarkGreen); }
    internal static void _tdcya(string s) { _Color(true, s, ConsoleColor.DarkCyan); }
    internal static void _tdred(string s) { _Color(true, s, ConsoleColor.DarkRed); }
    internal static void _tdmag(string s) { _Color(true, s, ConsoleColor.DarkMagenta); }
    internal static void _tdyel(string s) { _Color(true, s, ConsoleColor.DarkYellow); }
    internal static void _tdgra(string s) { _Color(true, s, ConsoleColor.DarkGray); }

#if UNIT_TESTS
    internal static void Warn(string s) { _Color(false, "WARNING: " + s, ConsoleColor.Yellow); }
    internal static void Error(string s) { _Color(false, "ERROR: " + s, ConsoleColor.Red); }

    private static void _Color(bool isTrace, string s, ConsoleColor cc)
    {
      if (isTrace && !TRACE) { return; }
      UnitTests._Color(s, cc);
    }

#else
    internal static void Warn(string s)  { MelonLogger.Warning(s); }
    internal static void Error(string s) { MelonLogger.Error(s); }

    private static void _Color(bool isTrace, string s, ConsoleColor cc)
    {
        //    Console.BackgroundColor = ConsoleColor.White;
        if (isTrace)
        {
            if (!TRACE) { return; }
            var o = Console.ForegroundColor;
            Console.ForegroundColor = cc;
            Console.WriteLine(s);
            Console.ForegroundColor = o;
            return;
        }

        MelonLogger.Msg(cc, s);
    }
#endif
}
