using System;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Donteco;
using GhostAdvancers;
using Lidgren.Network;

[assembly: MelonInfo(typeof(Mod), "Ghost Advancers", "0.1.0", "Tuxifan <tuxifan@posteo.de>")]



namespace GhostAdvancers
{
    public class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Debug.Log("Ghost Advancers mod is running!!!");
            LoggerInstance.Msg("Logged mod usage in Unity.");
            GUI.enabled = true;
        }

        public override void OnGUI()
        {
            GUI.Window(0, new Rect(20, 20, 400, 600), Environment.window, "Environment");
        }

        public override void OnFixedUpdate()
        {
            AntiCheat.RunChecks();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // This is split up into 2 parts since this is the only place we can access the buildIndex, see MapLoadStacktracePatch for the other part
            if (buildIndex > 2)
            {
                LoggerInstance.Msg($"Map {sceneName} has been loaded by game!");
                new Environment(sceneName);
            }
            else if (buildIndex == 1 && Environment.instance != null)
            {
                Environment.instance = null;
                LoggerInstance.Msg("Map has been unloaded by game!");
            }
        }
    }

    namespace Logging
    {
        [HarmonyPatch(typeof(GameLevelManager), nameof(GameLevelManager.LoadLevel), new Type[] { typeof(string) })]
        public class MapLoadStacktracePatch
        {
            // This is split up into 2 parts since this is the only place we can get a proper stack trace, see Mod.OnSceneWasLoaded for the other part
            private static void Prefix()
            {
                var trace = new System.Diagnostics.StackTrace();
                Melon<Mod>.Logger.Msg($"Stack trace of map load:\n{trace}");
            }
        }
    }
}