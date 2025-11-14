using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Polarite.Multiplayer;

using Steamworks;

using UnityEngine;

namespace Polarite.Patches
{
    [HarmonyPatch(typeof(EnemyIdentifier))]
    internal class EnemyNetworkPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Spawn(EnemyIdentifier __instance)
        {
            if (__instance.GetComponent<NetworkEnemy>() == null && __instance.gameObject.scene.name != null && NetworkManager.InLobby)
            {
                string newPath = SceneObjectCache.GetOrCreatePath(__instance.gameObject);
                if (!SceneObjectCache.Contains(__instance.gameObject))
                {
                    SceneObjectCache.Add(newPath, __instance.gameObject);
                }
                NetworkEnemy.Create(newPath, __instance, NetworkManager.Id);
            }
        }
        [HarmonyPatch(nameof(EnemyIdentifier.DeliverDamage))]
        [HarmonyPrefix]
        static void Damage(EnemyIdentifier __instance, ref float multiplier, ref Vector3 force, ref GameObject target, ref Vector3 hitPoint)
        {
            if(__instance.TryGetComponent<NetworkPlayer>(out var netP) && multiplier > 0f && NetworkManager.Instance.CurrentLobby.GetData("pvp") == "1")
            {
                netP.HandleFriendlyFire(NetworkManager.Id, Mathf.RoundToInt(multiplier));
                return;
            }
            if(force == Vector3.zero)
            {
                return;
            }
            NetworkEnemy netE = __instance.GetComponent<NetworkEnemy>();
            if (netE != null)
            {
                netE.BroadcastDamage(multiplier, __instance.hitter, target == __instance.weakPoint, hitPoint);
            }
        }
    }
}
