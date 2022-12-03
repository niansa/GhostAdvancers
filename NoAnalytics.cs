using System;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Donteco;
using GhostAdvancers;

namespace GhostAdvancers
{
    [HarmonyPatch(typeof(CustomAnalytics), nameof(CustomAnalytics.SendCollectedEvents), new Type[] {  })]
    public class NoAnalyticsPatch
    {
        private static bool Prefix()
        {
            Melon<Mod>.Logger.Msg("Not sending analytics data");
            return false;
        }
    }
}