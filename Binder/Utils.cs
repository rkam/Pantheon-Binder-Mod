#if !UNIT_TESTS
using Il2Cpp;
using Il2CppPantheonPersist;
#endif

namespace Binder;

class Utils
{
    /// <summary>
    /// Get the loc and compass area that the player is at/in.
    /// </summary>
    internal static MapPoint? GetPlayerMapPoint()
    {
        var playerPos = Globals.LocalPlayer?.transform.position;

        if (playerPos == null) { return null; }

        CompassArea area = LocHelper.GetCompassArea();

        var loc = new MapPoint(
          MathF.Round(playerPos.Value.x, 2),
          MathF.Round(playerPos.Value.y, 2),      // y & z are swapped in MelonLoader vs game (see also ShalazamGPS).
          MathF.Round(playerPos.Value.z, 2),
          area
        );

        return loc;
    }

    /// <summary>
    /// Echos the current bind location to chat
    /// </summary>
    internal static void EchoToChat(string displayText, ChatChannelType channel = ChatChannelType.Whisper)
    {
#if UNIT_TESTS
        UnitTests.lastChat = displayText;
        Globals._gre($"Chat[{channel}]: {displayText}");
#else
        if (UIChatWindows.Instance == null ||
            UIChatWindows.Instance.mainWindow == null ||
            UIChatWindows.Instance.mainWindow.chats == null)
            return;

        foreach (UIChatWindow.ChatAndTab chatAndTab in UIChatWindows.Instance.mainWindow.chats)
        {
            UIChat chat = chatAndTab.Chat;

            chat?.AddMessage("",
                displayText, channel,
                CombatLogDirectionalFilter.All,
                CombatLogFilter.Both,
                CombatLogPlayerFilter.All,
                false,
                false);
        }
#endif
    }

    /// <summary>
    /// Guesses at the Player's StartingBindpoint. Returns "Trainer" or Unknown
    /// </summary>
    internal static Bindpoint GetStartingBindpoint()
    {
        var loc = Utils.GetPlayerMapPoint();
        if (loc == null)
        {
            return new Bindpoint();
        }

        // Two cases:
        // 1) Player created while mod installed.
        //   Easy (Level is 1 && Exp is 0), so can set as Bound-at-Trainer.

        // 2) Player created before (or while) mod not installed.
        //    No idea where they are bound. But, if low level, probably at Trainer.
        //
        // Now,
        //  Some players (esp. twinks) will bind at a stone right away.
        //  Others will bind after some leveling, say level 5.
        //
        // So, if level is >= 5 or so, then no idea, so don't even bother.
        // if level is 1-5, then could be either really, so won't bother either.
        // => if level 1 with some exp, probably still Bound-at-Trainer, but guessing.

        var exp = Globals.LocalPlayer?.Experience;
        if (exp == null)
        {
            return new Bindpoint();
        }

        var playerLevel = exp.Level;
        var playerExp = exp.CalculateCurrentExperienceIntoLevel();

        if (playerLevel != 1)
        {
            return new Bindpoint();
        }

        if (playerExp > 0)
        {
            Utils.EchoToChat("BinderMod: Assuming a starting player is bound at Trainer. If incorrect, rebind at a bindstone or use /xxbindset-somename-zone.");
        }

        return new Bindpoint(loc, true);              // L1 -> Trainer
    }
}
