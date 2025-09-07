# pantheon-binder-mod
A Pantheon Mod to track a player's bind points.

# Binder

**BinderMod**: This mod tracks the bind message and stores it away.

It will (optionally) display it below the compass.
- It plays nicely with the *ShowServerAndShard* and *ShowServerTime* mods by **onyxius** (which also display information below the compass)..

It also will (optionally) display it in chat, on demand, and at login.

## Overview:

The mod works in three ways:

Firstly, whenever a bindstone is clicked, it will note that bindstone and
        the zone. This will be displayed in the ways described above.

This is the normal operation. So, almost all of the time, the player will use the mod as above.

- The rest of the mod tries to help with not knowing the bind point.

Second, if the bind is not known (i.e. the mod is running and a player
        previously created without the mod running logs in),
        the mod will attempt to provide a guess at the bindpoint.

*Note that anytime the bindpoint is not known (or is incorrect - see below),
        this can be corrected by simply clicking on a bindstone.*

Finally, a command exists to manually set the bindpoint. This is to provide for
        the case where the player knows (at least roughly) where they are bound
        and does not yet want to re-bind.

### Options:

- There are options for showing the current bind point in game
    - below the compass
            (**default** *false* - Do not show).
    - at login or not
            (**default** *false* - Do not show).
    - when the bind is performed (name added to bind message).
            (**default** *true* - Show in the bind message).

- And a option for being terse in displaying the bind.
    e.g.  **Sorhiryth** vs. **Sorhiryth in Wild's End**
            (**default** *true* - Show terse information).

### Commands:

There are two main commands, the second having two forms:
- /xxbindinfo                 - displays the current bind to chat.

- /xxbindset-name             - sets the bind point to *name*
- /xxbindset-name-zone        -  (as above +) "in *zone*"

#### xxbindset details:

##### **/xxbindset**-*name*
- it will see if the name matches any *Bindstone*, and if so will
    act as if that bindstone was used.
- it will see if the name matches a *Zone*. If so, then if there is only
    one bindstone in that zone, it will use that bindstone. If there
    is more than one bindstone, it will use **'somewhere'** as the bindpoint
    and the specified zone.
- otherwise, it will just use the name as is and the current zone.

Examples:
- /xxbindset-halnir       -> Halnir Cave bindstone
- /xxbindset-sp           -> Silent Plains zone -> Skydurbin bindstone
- /xxbindset-trainer      -> Trainer
- /xxbindset-town         -> 'town' in <current zone>
- /xxbindset-tf           -> 'somewhere' in Thronefast

##### **/xxbindset**-*name-zone*
- it will do the above **-name** case, except that, if nothing matches
    it will also try to match *zone* to a zone. If so, it will use
    that as the bind zone (and use name as the bindpoint name).

Examples:
- /xxbindset-upnorth-we   - 'upnorth' in Wild's End
- /xxbindset-town-north   - 'town' in 'north'
NOTE: that if you specify a bindstone and an incorrect zone, it
        will ignore the zone.  e.g. **//xxbindset-availia-we** will show
        the same as if just **//xxbindset-availia** was used.

- You can distinguish user set names because they are all lower case and in single quotes.

- Finally, using a bindstone, will of course, overwrite user-set binds.

### Other Commands

There are four other commands which toggle the Options described above.
- /xxbindchat
- /xxbindhud
- /xxbindlogin
- /xxbindterse

## Notes

Firstly and obviously, the bind can only be tracked while the mod is active.
    So, if the mod is not running and you bind, it will be incorrect the next time
    the mod is running.

If a new character is created, it will set the bind to *Trainer*.

For characters created prior to running the mod:
- If the character is level 1, it assumes that it's bound at the trainer.
- Otherwise, it will show *Unknown* until an actual bind is set.

Again, if there are any issues, they can be corrected in one of two ways:
- Bind at a bindstone.
- Use the command **/xxbindset-name-zone** command.

The mod uses *known* bindstones. If they add another, it will not be recognized
    until the mod is updated.  In this case, it will display
- "**Unknown @** *X*, *Y* **in** *Zone*"

where *X* & *Y* are the player's coordinates at the time of the bind (which
    will be extremely close to the bindstone) and the current area the player is in.

Each player's bindpoint is stored on disk and independent of the game installation.
The benefit of this is that if you have multiple games installed, the bindpoint
will not be incorrect regardless of which game is used to login (even if playing
multiple instances of the game simulataneously).

A final note is that the term Zone as used by the mod is not always an actual zone. It is copied from the name that appears below the compass. So, for example, **The Village of Availia** or **Demith** is considered a Zone by the mod (even though you don't zone when entering/leaving that area).

## Caveats

- If you play the game without running the mod and you bind, this mod will show the old bind location until you bind again.
- The mod supports multiple installed games (multiple accounts).- If you have multiple accounts and log into VR with one, but then log into the game itself with a different account (second login from Desktop), then the the bind will be correct.

- The mod commands take advantage of the **Invalid Command** behaviour of the game.
    A side effect of this is that the sometimes the chat-mode gets changed (e.g. if you had it set to /group, it might be set to /whisper). This will only happen when commands are used, so it's not intrusive and easily fixed.
    - Also, this limits the characters that can be used in the bindpoint.  Additionally, for simplicity, valid chars are further limited to alphanumeric and a few others, including **_**, **@**, **?**, and **,** .
    - **Hint:** A good way to manually set a bind, if you might know, but unsure is to do **/xxsetbind-?demith**.  The **?** ensures it won't match the bindstone itself, while conveying the info.

## Issues

- Sometimes when a mod issues a message of certain types (e.g. Whisper), the default chat channel is set to that.
(When you hit Enter, it shows the prompt **To: ** indicating it's in whisper-mode).
You cannot issue commands in this mode. So, you must type **/say**, then can issue commands again.

- Only ASCII chars are allowed.
(Since user-set bindpoints should be rare, are temporary, and mostly abbreviations of bindstones/zones, it's just a minor issue.)


## TODO

- All known Bindstones are hard-coded. They shouldn't be.
    - If they were in a separate config file, they could be updated easily and by the user.
        - This would require a **lot** more work.
    - Best would be to be able to query Shalazam for them.

- The actual bindpoints are stored in separate config files.
This has the following benefits: It
    - keeps the clutter down in the main config file because each player had a bindpoint entry
    - allows support for multiple accounts (simultaneous or not),
    - automatically keeps the bindpoints around sfter a patch (only applies to the standalone version of the game).
- **Note** the *Options* are global preferences and so stored in the main config file and so per installed game.
