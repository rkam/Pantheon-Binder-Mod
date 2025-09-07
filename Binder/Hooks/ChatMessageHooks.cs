#if !UNIT_TESTS
using HarmonyLib;
using Il2Cpp;
using Il2CppPantheonPersist;

namespace Binder.Hooks;

// Code based on the same file in EnhancedExperienceBar by Roast
[HarmonyPatch(typeof(UIChatWindows), nameof(UIChatWindows.PassMessage), typeof(string), typeof(string), typeof(ChatChannelType))]
public class ChatMessageHooks
{
    private static bool Prefix(UIChatWindows __instance, string name, ref string message, ChatChannelType channel)
    {
        // name is a character name:
        //  For system messages (?) it's the current player.
        //  For tells, it's the source of the tell.
        //    (And yes, if it's a tell FROM you, it's your name).

        return Commands.CheckForCommands(name, ref message, channel);
    }
}
#endif
