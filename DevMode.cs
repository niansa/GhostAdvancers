using System;
using System.Reflection;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Lidgren.Network;
using Donteco;
using GameToolkit.Localization;
using static MelonLoader.MelonLogger;
using GhostAdvancers;
using Donteco.Menu;

namespace GhostAdvancers
{
    [HarmonyPatch(typeof(GameInit), "Awake", new Type[] {  })]
    public class DevModePatch
    {
        private static void Prefix(GameInit __instance)
        {
            Melon<Mod>.Logger.Msg("GameInit is about to awake, enabling TestAchievements");
            //__instance.DummyAccount = true;
            //__instance.DevUtils = true;
            __instance.TestAchievements = true;
            //__instance.DontCheckEnd = true;
        }
    }

    [HarmonyPatch(typeof(WarningScreen), "Awake", new Type[] { })]
    public class WarningScreenPatch
    {
        private static bool Prefix(WarningScreen __instance)
        {
            Melon<Mod>.Logger.Msg("Skipping WarningScreen");
            __instance.ConfirmShow();
            return false;
        }
    }

    [HarmonyPatch(typeof(GameLevelManager), nameof(GameLevelManager.LoadLevel), new Type[] { typeof(Levels) })]
    public class SchoolToTestLevelPatch
    {
        private static void Prefix(ref Levels level)
        {
            if (level == Levels.SchoolAbandoned)
            {
                Melon<Mod>.Logger.Msg("Replacing school with test level");
                level = Levels.Test;
            }
        }
    }
    [HarmonyPatch(typeof(PublicOrPrivateGameState), nameof(PublicOrPrivateGameState.StartGame), new Type[] {  })]
    public class MapLoadNoSingleplayerPatch
    {
        private static bool Prefix()
        {
            if (!NetworkManager.SinglePlayer)
            {
                Melon<Mod>.Logger.Msg("Force-disabling auto-singleplayer for easier network debugging");
                LobbyManager.Instance.StartGame();
                return false;
            }
            return true;
        }
    }
}