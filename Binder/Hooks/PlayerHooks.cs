#if !UNIT_TESTS
using HarmonyLib;
using Il2Cpp;

namespace Binder.Hooks;

// Code almost identical to the same file in ShalazamGPS by ModsOfPantheon
[HarmonyPatch(typeof(EntityPlayerGameObject))]
[HarmonyPatch(nameof(EntityPlayerGameObject.NetworkStart))]
public class PlayerNetworkStart
{
    private static void Postfix(EntityPlayerGameObject __instance)
    {
        // Fired in character select
        if (__instance.NetworkId.Value == 1)
        {
            return;
        }

        if (__instance.NetworkId.Value == EntityPlayerGameObject.LocalPlayerId.Value)
        {
            Globals.LocalPlayer = __instance;
            Globals.PPrefs = new PlayerPrefs();
            ModMain.Instance?.LoadPlayerPrefs();
        }
    }
}

[HarmonyPatch(typeof(EntityPlayerGameObject), nameof(EntityPlayerGameObject.NetworkStop))]
public class PlayerNetworkStop
{
    private static void Prefix(EntityPlayerGameObject __instance)
    {
        if (__instance.NetworkId.Value == EntityPlayerGameObject.LocalPlayerId.Value)
        {
            Globals.GPrefs?.SaveToFile(true);
            Globals.PPrefs?.SaveToFile(true);
            Globals.PPrefs = null;
            Globals.LocalPlayer = null;
        }
    }
}
#endif
