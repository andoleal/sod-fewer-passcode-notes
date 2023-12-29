namespace FewerPasscodeNotes
{
    using System.Linq;
    using BepInEx;
    using BepInEx.Unity.IL2CPP;

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Shadows of Doubt.exe")]
    public class Plugin : BasePlugin
    {
        const string PluginGuid = "FewerPasscodeNotes";
        const string PluginName = "Fewer Passcode Notes";
        const string PluginVersion = "1.0.0";

        const double ReduceNotesCountByThisPergentage = 0.6;

        static System.Random _rng;
        static HarmonyLib.Harmony Harmony;

        // Increased before PickPassword() functions and decreased after
        // Before WriteNote(), if more than 0, the chance is rolled whether to spawn the note
        static int _notes = 0;

        public override void Load()
        {
            Harmony = new HarmonyLib.Harmony(PluginGuid);
            Log.LogInfo($"Plugin {PluginGuid} is loaded!");
            Harmony.PatchAll();
            Log.LogInfo($"Plugin {PluginGuid} is patched!");
            _rng = new System.Random(System.DateTime.Now.Millisecond);
        }

        public override bool Unload()
        {
            Harmony.UnpatchSelf();
            return base.Unload();
        }

        [HarmonyLib.HarmonyPatch]
        public class Human_WriteNote
        {
            static NewGameLocation _lastLocation = null;
            static bool _lastLocationDenied = true;

            public static bool Prefix(ref Interactable __result, NewGameLocation placement)
            {
                bool denied = _lastLocationDenied;
                if (_lastLocation != placement)
                {
                    _lastLocation = placement;
                    denied = _notes > 0 && _rng.NextDouble() < ReduceNotesCountByThisPergentage;
                }

                _lastLocation = placement;
                _lastLocationDenied = denied;

                if (denied)
                {
                    __result = null;
                    return false;
                }
                return true;
            }

            public static System.Reflection.MethodBase TargetMethod()
            {
                return HarmonyLib.AccessTools.GetTypesFromAssembly(typeof(Human).Assembly)
                    .SelectMany(type => type.GetMethods())
                    .Where(

                    method => method.DeclaringType == typeof(Human) 
                    && method.Name.Equals("WriteNote") 
                    && method.GetParameters()[0].ParameterType == typeof(Human.NoteObject))

                    .Cast<System.Reflection.MethodBase>().First();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Human), "PickPassword")]
        public class Human_PickPassword
        {
            [HarmonyLib.HarmonyPriority(10)]
            public static bool Prefix(ref Human __instance)
            {
                _notes++;
                return true;
            }

            public static void Postfix()
            {
                _notes--;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(NewRoom), "PickPassword")]
        public class NewRoom_PickPassword
        {
            [HarmonyLib.HarmonyPriority(10)]
            public static bool Prefix()
            {
                _notes++;
                return true;
            }

            public static void Postfix()
            {
                _notes--;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(NewAddress), "PickPassword")]
        public class NewAddress_PickPassword
        {
            [HarmonyLib.HarmonyPriority(10)]
            public static bool Prefix()
            {
                _notes++;
                return true;
            }

            public static void Postfix()
            {
                _notes--;
            }
        }
    }
}
