#if !UNIT_TESTS
using HarmonyLib;
using Il2Cpp;

namespace Binder.Hooks;

// Code almost identical to the same file in Pantheon-ShowServerTime-Mod by onyxius

/// <summary>
/// Hooks for UI panel events, specifically for creating the BoundAt display
/// (Code taken from Pantheon-ShowServerTime-Mod by onyxius)
/// </summary>
[HarmonyPatch(typeof(UICompass), nameof(UICompass.Start))]
public class UICompassHooks
{
    /// <summary>
    /// Called when the Compass UI panel starts
    /// Creates the BoundAt display when the compass panel is initialized
    /// </summary>
    /// <param name="__instance">The UI panel that started</param>
    private static void Postfix(UICompass __instance)
    {
        ModMain.Instance?.CreateBoundAtDisplay(__instance.transform);
    }
}
#endif
