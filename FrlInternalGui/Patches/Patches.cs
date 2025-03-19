using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Numerics;
using Fusion;
using GorillaTagScripts;
using Photon.Realtime;
using FrlGUITemplate.MainGUI;

namespace FrlGUITemplate.Patches
{
    public class Patches : MonoBehaviour
    {
        [HarmonyPatch(typeof(VRRig), "OnDisable")]
        internal class RigPatch : MonoBehaviour
        {
            public static bool Prefix(VRRig __instance)
            {
                return !(__instance == GorillaTagger.Instance.offlineVRRig);
            }
        }



        [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
        public static class RigPatch2
        {
            public static bool Prefix(VRRigJobManager __instance, VRRig rig)
            {
                return !(__instance == GorillaTagger.Instance.offlineVRRig);
            }
        }

        [HarmonyPatch(typeof(LoadBalancingClient), "OnDisconnectMessageReceived")]
        public static class AntiRPCKick
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNot), "SendReport")]
        internal class AntiCheat : MonoBehaviour
        {
            private static bool Prefix(string susReason, string susId, string susNick)
            {
                if (susId == PhotonNetwork.LocalPlayer.UserId && !susReason.Contains("empty rig"))
                {
                    NotifiLib.SendNotification("[<color=red>REPORT</color>] Reason: " + susReason);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
        public class DisableLogErrorCount : MonoBehaviour
        {
            private static bool Prefix(string logString, string stackTrace, UnityEngine.LogType type)
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNot), "CheckReports", MethodType.Enumerator)]
        public class DisableCheckReports : MonoBehaviour
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(FriendshipGroupDetection), "IsInMyGroup")]
        public class AutoInParty : MonoBehaviour
        {
            private static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }
        }
    }
}