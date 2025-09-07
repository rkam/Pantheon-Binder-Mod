#if !UNIT_TESTS
using Il2CppPantheonPersist;
#endif
using System.Text.RegularExpressions;

namespace Binder;

class Commands
{
    internal static bool CheckForCommands(string name, ref string message, ChatChannelType channel)
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

        if (message.StartsWith("Unknown command 'xxbind"))
        {
            var cmd = Regex.Replace(message, "^.*xxbind(.*)'.*", "$1");

            var msg = message;
            switch (cmd)
            {
                case "info":
                    msg = ModMain.Instance?.HandleBindInfoCommand(cmd, channel);
                    break;

                case "load":
                    ModMain.Instance?.HandleBindLoadCommand(cmd, channel);
                    break;

                case "terse":
                case "hud":
                case "chat":
                case "login":
                    msg = ModMain.Instance?.HandleBindToggleCommand(cmd, channel);
                    if (msg == null)    // not recognized
                    {
                        return true;                     // not recognized
                    }
                    break;

                default:
                    if (!cmd.StartsWith("set-"))      // "set-.*'."
                    {
                        return true;                     // not recognized
                    }

                    msg = ModMain.Instance?.HandleBindSetCommand(cmd, channel);
                    if (msg == null)    // not recognized
                    {
                        return true;
                    }
                    break;
            }

            // We reach here only if we handled the message.
            return false;
        }

        return true;
    }
}
