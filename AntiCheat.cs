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

namespace GhostAdvancers
{
    class AntiCheat
    {
        public static double interval = 1.0;
        public static double overspeedTolerance = 1.1;
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
                            Melon<Mod>.Logger.Msg($"Speedhack detected (Player: {playerId}/{Environment.lobby.Players[playerId].Nickname})");
                        }
                    }
                }
                // Reset timer
                timer.Restart();
            }
        }
    }
}