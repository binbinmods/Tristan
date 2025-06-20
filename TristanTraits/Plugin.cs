using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using static Obeliskial_Essentials.CardDescriptionNew;
using System.IO;
using UnityEngine;
using System;
using static Tristan.Traits;
using BepInEx.Configuration;
using Obeliskial_Essentials;

namespace Tristan
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials")]
    [BepInDependency("com.stiffmeds.obeliskialcontent")]
    [BepInProcess("AcrossTheObelisk.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal int ModDate = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;

        public static ConfigEntry<bool> EnableDebugging { get; set; }


        public static string characterName = "Tristan";
        public static string heroName = characterName;

        public static string subclassName = "Owlknight"; // needs caps

        public static string subclassname = subclassName.ToLower();
        public static string itemStem = subclassname;
        public static string debugBase = "Binbin - Testing " + characterName + " ";

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");

            EnableDebugging = Config.Bind(new ConfigDefinition(subclassName, "Enable Debugging"), true, new ConfigDescription("Enables debugging logs."));

            // register with Obeliskial Essentials
            RegisterMod(
                _name: PluginInfo.PLUGIN_NAME,
                _author: "binbin",
                _description: "Tristan, the Owl Knight.",
                _version: PluginInfo.PLUGIN_VERSION,
                _date: ModDate,
                _link: @"https://github.com/binbinmods/Tristan",
                _contentFolder: "Tristan",
                _type: ["content", "hero", "trait"]
            );
            // apply patches
            string card = "owlknightcognitivecalm";
            string text = $"{medsSpriteText("crack")} on monsters does not increase Damage, but Heals for 1 per charge when the target is hit.";
            AddTextToCardDescription(text, TextLocation.ItemBeginning, card, includeAB: true);

            card = "owlknightmindbreaker";
            text = $"{medsSpriteText("crack")} on monsters increases Mind Damage by received 1 per charge.";
            AddTextToCardDescription(text, TextLocation.ItemBeginning, card, includeAB: true);

            card = "owlknightlowerguard";
            text = $"{medsSpriteText("insane")} on monsters reduces Blunt Resistance by received 0.3% per charge.";
            AddTextToCardDescription(text, TextLocation.ItemEnchantBeforeCast, card, includeAB: true);


            harmony.PatchAll();
        }

        internal static void LogDebug(string msg)
        {
            if (EnableDebugging.Value)
            {
                Log.LogDebug(debugBase + msg);
            }

        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }
    }
}
