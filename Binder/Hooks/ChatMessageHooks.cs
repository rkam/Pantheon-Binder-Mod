#if !UNIT_TESTS
using HarmonyLib;
using Il2Cpp;
using Il2CppPantheonPersist;

namespace Binder.Hooks;

// Intercept You-are-bound message.
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

        return Commands.CheckForBound(name, ref message, channel);
    }
}

//
// Code from ShalazamGPS by Roast
[HarmonyPatch(typeof(EntityClientMessaging.Logic), nameof(EntityClientMessaging.Logic.SendChatMessage), typeof(string), typeof(ChatChannelType))]
public class SendChatMessageHook
{
    private static bool Prefix(EntityClientMessaging.Logic __instance, string message, ChatChannelType channel)
    {
        if (message.StartsWith("/xxbind"))
        {
            return Commands.CheckForCommands(message, channel);
        }

        return true;
    }
}

// Code from ShalazamGPS by Roast
[HarmonyPatch(typeof(EntityClientMessaging.Logic), nameof(EntityClientMessaging.Logic.RequestWhisper))]
public class RequestWhisperHook
{
    private static bool Prefix(UIChatInput __instance, string targetPlayerName, string message)
    {
        if (message.StartsWith("/xxbind"))
        {
            return Commands.CheckForCommands(message, ChatChannelType.Whisper);
        }

        return true;
    }
}
#endif
