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
using System;
using System.Diagnostics;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Donteco;
using GhostAdvancers;
using Lidgren.Network;

using Debug = UnityEngine.Debug;

[assembly: MelonInfo(typeof(Mod), "Ghost Advancers", "1.0.0", "Tuxifan <tuxifan@posteo.de>")]



namespace GhostAdvancers
{
    public class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Debug.Log("Ghost Advancers mod is running!!!");
            LoggerInstance.Msg("Logged mod usage in Unity.");
            GUI.enabled = true;

            Popup.Show("License note", "GhostAdvancers  Copyright (C) 2022  niansa/Tuxifan\n" +
                                       "This program comes with ABSOLUTELY NO WARRANTY; for details press <b>Ctrl+F10</b>.\n" +
                                       "This is free software, and you are welcome to redistribute it\n" +
                                       "under certain conditions; press <b>Ctrl+F11</b> for details.", 30);
            Popup.SetSizePresetLongMultiLine(4);
        }

        public override void OnGUI()
        {
            Environment.RunGUI();
        }

        public override void OnFixedUpdate()
        {
            AntiCheat.RunChecks();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.F10))
                {
                    Process.Start("https://github.com/niansa/GhostAdvancers/blob/master/LICENSE.txt#L589");
                }
                if (Input.GetKeyDown(KeyCode.F11))
                {
                    Process.Start("https://github.com/niansa/GhostAdvancers/blob/master/LICENSE.txt#L71");
                }
            }
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