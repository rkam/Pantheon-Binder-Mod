#if !UNIT_TESTS
using Binder;
using MelonLoader;

[assembly: MelonInfo(typeof(ModMain), nameof(Binder), ModMain.ModVersion, "ioanna")]
[assembly: MelonGame("Visionary Realms", "Pantheon")]
#endif
