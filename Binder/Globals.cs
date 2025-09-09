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
}
