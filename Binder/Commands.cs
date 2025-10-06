#if !UNIT_TESTS
using Il2CppPantheonPersist;
#endif
using System.Text.RegularExpressions;

namespace Binder;

class Commands
{
    internal static bool CheckForBound(string name, ref string message, ChatChannelType channel)
    {

        // "You are now bound to this stone!"
        //  (NOTE: Game also prints: "You now feel an affinity with this location")
        if (message.Contains("bound to this stone", StringComparison.InvariantCultureIgnoreCase))
        {
            var suffix = ModMain.Instance?.HandleBindAtStone(name, message, channel);
            if (suffix != string.Empty)
                message += $" ({suffix})";
            return true;
        }

        return true;
    }

    internal static bool CheckForCommands(string message, ChatChannelType channel)
    {
        var cmd = Regex.Replace(message, "^/xxbind(.*)", "$1");

        var msg = message;
        switch (cmd)
        {
            case "info":
                ModMain.Instance?.HandleBindInfoCommand(cmd, channel);
                break;

            case "load":
                ModMain.Instance?.HandleBindLoadCommand(cmd, channel);
                break;

            case "terse":
            case "hud":
            case "chat":
            case "login":
                ModMain.Instance?.HandleBindToggleCommand(cmd, channel);
                break;

            default:
                if (!cmd.StartsWith("set-"))      // "set-.*'."
                {
                    return true;                     // not recognized
                }

                ModMain.Instance?.HandleBindSetCommand(cmd, channel);
                break;
        }

        // We reach here only if we handled the message.
        return false;
    }
}
