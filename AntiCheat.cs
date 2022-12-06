using System;
using MelonLoader;
using HarmonyLib;
using Donteco;
using UnityEngine;
using Lidgren.Network;
using System.Diagnostics;
using System.Collections.Generic;
using GhostAdvancers;
using MhNetworking.Models;
using System.Linq;

namespace GhostAdvancers
{
    class AntiCheat
    {
        public static double interval = 1.0;
        public static double overspeedTolerance = 1.3;
        private static Stopwatch timer;
        private static Dictionary<string, Vector3> lastKnownPositions;

        static AntiCheat() {
            timer = new Stopwatch();
            timer.Start();
            lastKnownPositions = new Dictionary<string, Vector3>();
        }
        public static void RunChecks()
        {
            if (Environment.lobby == null) return;
            if (Environment.instance == null)
            {
                if (lastKnownPositions.Count != 0) Melon<Mod>.Logger.Msg("Reset last known player positions");
                // Clean up last known positions if there is no instance
                lastKnownPositions.Clear();
                return;
            }
            // Check if it's time to detect speedhacks
            double delta = timer.Elapsed.TotalMilliseconds / 1000.0; 
            if (delta >= interval)
            {
                // Interate over all players
                foreach (var player in Environment.instance.players)
                {
                    var playerId = player.GetComponent<PlayerSetup>().SteamId.ToString();
                    if (playerId.Length == 0) continue; // Shouldn't ever happen, but for some reason it does
                    // Add player to last known positions if needed
                    if (!lastKnownPositions.ContainsKey(playerId))
                    {
                        lastKnownPositions[playerId] = player.transform.position;
                        Melon<Mod>.Logger.Msg($"Registered player {playerId}");
                    } else
                    {
                        // Get distance between last known and current player positions
                        var lastPos = lastKnownPositions[playerId];
                        var newPos = player.transform.position;
                        lastKnownPositions[playerId] = newPos;
                        var distance = Vector2.Distance(new Vector2(lastPos.x, lastPos.z), new Vector2(newPos.x, newPos.z)); // Conversion to Vector2 since we don't want to account for the y axis
                        Melon<Mod>.Logger.Msg($"Distance travelled by {playerId} in {delta} seconds: {distance}");
                        // Get max distance possible for delta
                        var maxDistancePossible = PlayerMovementController.RunSpeed * delta;
                        if (distance > maxDistancePossible * overspeedTolerance)
                        {
                            var playerInfo = Environment.lobby.Players[playerId];
                            Melon<Mod>.Logger.Msg($"Speedhack detected (Player: {playerId}/{playerInfo.Nickname})");
                            Popup.Show("Speedhack detected", $"<b><color=red>{playerInfo.Nickname} is likely using a speedhack.</color></b>\nConsider taking action if this keeps occurring.", 20.0);
                            Popup.SetSizePresetLongMultiLine(2);
                        }
                    }
                }
                // Reset timer
                timer.Restart();
            }
        }
    }
    [HarmonyPatch(typeof(Tool), nameof(Tool.Take), new Type[] { typeof(Transform) })]
    public class ToolPickRangeCheckPatch
    {
        public static readonly float maxGrabDistance = 3.2f;
        public static float overgrabTolerance = 1.1f;

        private static bool Prefix(InventoryTools __instance, Transform player)
        {
            var playerId = player.gameObject.GetComponent<PlayerSetup>().SteamId.ToString();
            var playerPosition = player.gameObject.transform.position;
            var toolPosition = __instance.gameObject.transform.position;
            var distance = Vector3.Distance(playerPosition, toolPosition);
            Melon<Mod>.Logger.Msg($"Player {playerId} is trying to pick up an item at a distance of {distance} meters");
            if (distance > 3.2f * overgrabTolerance)
            {
                var playerInfo = Environment.lobby.Players[playerId];
                Melon<Mod>.Logger.Msg($"Grabbing hack detected (Player: {playerId}/{playerInfo.Nickname})");
                Popup.Show("Grabbing hack detected", $"<b><color=red>{playerInfo.Nickname} is using a grabbing hack..</color></b>\nConsider taking action.", 20.0);
                Popup.SetSizePresetLongMultiLine(2);
                return false;
            }
            return true;
        }
    }
}