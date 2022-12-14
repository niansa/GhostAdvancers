/*
    GhostAdvancers (Useful mods for Ghost Watchers)
    Copyright (C) 2022  niansa/Tuxifan

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#if DEBUG
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
#endif